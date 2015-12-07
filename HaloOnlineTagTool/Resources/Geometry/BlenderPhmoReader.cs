using System;
using System.IO;
using SimpleJSON;

namespace HaloOnlineTagTool.Resources.Geometry
{
	/// <summary>
	/// This class loads, reads, tokenises, and parses a simple file format
	/// designed to store data exported from the Blender modeling program. 
	/// </summary>
	class BlenderPhmoReader
	{

		public string filepath;

		public BlenderPhmoReader(string fpath)
		{
			filepath = fpath;
		}

		public JSONNode ReadFile()
		{
			string contents;
			try
			{
				// open the file as a text-stream
                StreamReader sr = null;

                try
                {
                    sr = new StreamReader(filepath);

                }catch(FileNotFoundException){
                    Console.WriteLine("The system cannot find the file specified.");
                    return null;
                }
				contents = sr.ReadToEnd();
				sr.Close();
			}
			catch (FileNotFoundException)
			{
				Console.WriteLine("File: {0} could not be found.", filepath);
				return null;
			};

			//parse the file as json
			var json = JSON.Parse(contents);

			return json;
		}

	}
}
