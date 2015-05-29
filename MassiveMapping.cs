using System;
using System.IO;
using System.Collections.Generic;

namespace ExtractMassiveWavetables
{
	/// <summary>
	/// Description of MassiveMapping.
	/// </summary>
	public class MassiveMapping
	{
		public Dictionary<string, KeyValues> Tables {
			get;
			set;
		}
		
		public MassiveMapping(string filePath)
		{
			Tables = new Dictionary<string, KeyValues>();

			using (var reader = new StreamReader(filePath))
			{
				// First line contains column names.
				var columnNames = reader.ReadLine().Split(',');
				for(int i = 1; i < columnNames.Length; ++i)
				{
					var columnName = columnNames[i];
					Tables.Add(columnName, new KeyValues(columnName));
				}

				var line = reader.ReadLine();
				while (line != null)
				{
					var columns = line.Split(',');

					for (int i = 1; i < columns.Length; ++i)
					{
						var table = Tables[columnNames[i]];
						table.AddValue(columns[0], columns[i]);
					}

					line = reader.ReadLine();
				}
			}
		}
	}
	
	
	public class KeyValues
	{
		private Dictionary<string, string> _values = new Dictionary<string, string>();
		private String _key;

		public KeyValues(String key)
		{
			_key = key;
		}

		public void AddValue(string key, string value)
		{
			_values.Add(key, value);
		}
		
		public override string ToString()
		{
			return string.Format("Key: {0}. Count: {1}", _key, _values.Count);
		}
	}

}
