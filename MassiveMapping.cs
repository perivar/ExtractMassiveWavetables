using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace ExtractMassiveWavetables
{
	/// <summary>
	/// Description of MassiveMapping.
	/// </summary>
	public static class MassiveMapping
	{
		/// <summary>
		/// Read in a map from the original filenames to the ones NI uses themselves in the GUI
		/// </summary>
		/// <param name="filePath">filepath to the mapping csv file</param>
		/// <returns>a map</returns>
		public static Dictionary<string, MassiveMapElement> ReadMassiveMapping(string filePath)
		{
			var collection = from line in File.ReadAllLines(filePath).Skip(1)
				let columns = line.Split(',')
				select new MassiveMapElement
			{
				Index = int.Parse(columns[0]),
				OrigFilePath = columns[1],
				OrigFileName = columns[2],
				CorrectFileName = columns[3],
				GroupName = columns[4],
				GroupIndex = CommonUtils.NumberUtils.IntTryParse(columns[5], -1),
				Comment = columns[6]
			};
			
			return collection.ToDictionary( t => t.OrigFilePath, t => t);
		}
	}
	
	public class MassiveMapElement {
		int index;
		string origFilePath;
		string origFileName;
		string correctFileName;
		string groupName;
		int groupIndex;
		string comment;

		public int Index {
			get {
				return index;
			}
			set {
				index = value;
			}
		}

		public string OrigFilePath {
			get {
				return origFilePath;
			}
			set {
				origFilePath = value;
			}
		}

		public string OrigFileName {
			get {
				return origFileName;
			}
			set {
				origFileName = value;
			}
		}

		public string CorrectFileName {
			get {
				return correctFileName;
			}
			set {
				correctFileName = value;
			}
		}

		public string GroupName {
			get {
				return groupName;
			}
			set {
				groupName = value;
			}
		}

		public int GroupIndex {
			get {
				return groupIndex;
			}
			set {
				groupIndex = value;
			}
		}

		public string Comment {
			get {
				return comment;
			}
			set {
				comment = value;
			}
		}
		
		public override string ToString()
		{
			return string.Format("[{0}] '{1}' => '{2}'", index, origFileName, correctFileName);
		}
		
	}
}
