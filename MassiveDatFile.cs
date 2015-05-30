using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using CommonUtils;

namespace ExtractMassiveWavetables
{
	/// <summary>
	/// Extract the files from Native Instruments Massive tables.dat file.
	/// This is ported over from python to c#
	/// https://gist.github.com/lalinsky/8f2cd9e8f80e62c82af2
	/// all credits goes to Lukáš Lalinský
	/// </summary>
	public static class MassiveDatFile
	{
		private static Entry parse_nimd_file(BinaryFile bFile) {
			// ready 12 bytes
			string magic = bFile.ReadString(4);
			int empty = bFile.ReadInt32();
			int totalSize = bFile.ReadInt32();
			
			if (!magic.Equals("NIMD")) {
				throw new ArgumentException("Invalid NIMD header");
			}
			
			return parse_nimd_entry_list(bFile, "");
		}
		
		private static Entry parse_nimd_entry(BinaryFile bFile) {
			
			// read boolean (1 byte)
			bool isList = bFile.ReadBoolean();

			// ready 2 ints (8 bytes)
			int total_size = bFile.ReadInt32();
			int name_size = bFile.ReadInt32();
			
			string name = bFile.ReadString(name_size);

			if (isList) {
				return parse_nimd_entry_list(bFile, name);
			}
			
			// ready 2 ints (8 bytes)
			int data_offset = bFile.ReadInt32();
			int data_size = bFile.ReadInt32();
			
			return new Entry(name, data_offset, data_size);
		}
		
		private static Entry parse_nimd_entry_list(BinaryFile bFile, string name) {

			// ready 2 ints (8 bytes)
			int totalSize = bFile.ReadInt32();
			int numEntries = bFile.ReadInt32();
			
			var entries = new List<Entry>();
			for (int i=0; i<numEntries;i++) {
				entries.Add(parse_nimd_entry(bFile));
			}

			return new Entry(name, entries);
		}
		
		private static void extract_nimd_entry(BinaryFile bFile, Entry entry, string base_output_dir, string output_dir, Dictionary<string, MassiveMapElement> map) {

			string path = Path.Combine(output_dir, entry.Name);
			
			if (entry.IsList) {
				foreach (var sub_entry in entry.Entries) {
					extract_nimd_entry(bFile, sub_entry, base_output_dir, path, map);
				}
			} else {
				// get binary content
				bFile.Seek(entry.DataOffset);
				byte[] byteArray = bFile.ReadBytes(entry.DataSize);
				
				#region Output Original Filenames
				// use path after the base dir as map key
				string mapKey = IOUtils.GetRightPartOfPath(path, base_output_dir + Path.DirectorySeparatorChar);

				// add all the original files into an Original directory
				path = Path.Combine(base_output_dir, "Original", mapKey);

				// create directory
				string pathDirectory = Path.GetDirectoryName(path);
				if (!Directory.Exists(pathDirectory)) {
					Directory.CreateDirectory(pathDirectory);
				}
				
				// write original file
				System.Console.Out.WriteLine("Creating file {0}.", path);
				BinaryFile.Write(path, byteArray);
				#endregion
				
				#region Output Correct Filenames
				// determine correct filename
				if (map.ContainsKey(mapKey)) {
					var mapElement = map[mapKey];
					if (!mapElement.GroupName.Equals("")
					    && !mapElement.CorrectFileName.Equals("")) {
						
						// group directory
						string groupDirectory = Path.Combine(base_output_dir, "Correct Waveforms", StringUtils.MakeValidFileName(mapElement.GroupName));
						
						if (!Directory.Exists(groupDirectory)) {
							Directory.CreateDirectory(groupDirectory);
						}
						
						// output the corrected filenames
						string newFileName = string.Format("{0}_{1}.wav", mapElement.GroupIndex, mapElement.CorrectFileName);
						string newFilePath = Path.Combine(groupDirectory, newFileName);
						System.Console.Out.WriteLine("Creating file {0}.", newFilePath);
						BinaryFile.Write(newFilePath, byteArray);
					}
				}
				#endregion
			}
		}
		
		/// <summary>
		/// Extract NI Massive's tables.dat file into a specified output directory
		/// </summary>
		/// <param name="tablesDatFilePath">path to tables.dat</param>
		/// <param name="outputDirPath">path to output directory</param>
		/// <returns></returns>
		public static bool Extract(string tablesDatFilePath, string outputDirPath)
		{
			if (File.Exists(tablesDatFilePath)) {
				string fileName = Path.GetFileNameWithoutExtension(tablesDatFilePath);
				
				// read in the map to save as NI Massive's GUI filenames
				var map = MassiveMapping.ReadMassiveMapping("massive_map.csv");
				
				var bFile = new BinaryFile(tablesDatFilePath, BinaryFile.ByteOrder.LittleEndian);
				Entry nimd = parse_nimd_file(bFile);
				extract_nimd_entry(bFile, nimd, outputDirPath, outputDirPath, map);

				bFile.Close();
				
				return true;
			} else {
				return false;
			}
		}
	}

	#region Entry
	class Entry {

		public string Name {
			get;
			set;
		}

		public int DataOffset {
			get;
			set;
		}

		public int DataSize {
			get;
			set;
		}

		public List<Entry> Entries {
			get;
			set;
		}
		
		public void Add(Entry entry) {
			this.Entries.Add(entry);
		}

		public Entry(string name, int data_offset, int data_size) {
			this.Name = name;
			this.DataOffset = data_offset;
			this.DataSize = data_size;
			
			this.Entries = new List<Entry>();
		}

		public Entry(string name, List<Entry> entries) {
			this.Name = name;
			this.Entries = entries;
		}

		public bool IsList {
			get {
				return Entries.Count > 0;
			}
		}
		
		public override string ToString()
		{
			if (IsList) {
				return string.Format("Name: {0}. Children: {1}", Name, Entries.Count);
			} else {
				return string.Format("Name: {0}. Data Offset: {1}, Data Size {2}", Name, DataOffset, DataSize);
			}
		}
	}
	#endregion
}
