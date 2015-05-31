using System;

namespace ExtractMassiveWavetables
{
	class Program
	{
		const string _version = "0.3";
		
		public static void Main(string[] args)
		{
			if (args.Length < 2) {
				System.Console.Out.WriteLine("Exctract NI Massive Wavetables (and other content from tables.dat).");
				System.Console.Out.WriteLine("Version {0}", _version);
				System.Console.Out.WriteLine();
				System.Console.Out.WriteLine("Usage ExtractMassiveWavetables.exe <path-to-tables.dat> <path-to-output-directory>");
				return;
			}
			
			if (!MassiveDatFile.Extract(args[0], args[1])) {
				System.Console.WriteLine("Massive's tables.dat not found!");
			}
		}
	}
}