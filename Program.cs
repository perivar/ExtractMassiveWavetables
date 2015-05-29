using System;

namespace ExtractMassiveWavetables
{
	class Program
	{
		public static void Main(string[] args)
		{
			MassiveDatFile.Extract(@"C:\Users\perivar.nerseth\OneDrive\Audio\Audio Software\Native Instruments\Massive\tables.dat", @"c:\temp");

			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
			
		}
	}
}