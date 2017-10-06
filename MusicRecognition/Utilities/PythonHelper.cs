using System;
using System.Diagnostics;
using System.IO;

namespace MusicRecognition
{
	public static class PythonHelper
	{
        private const string DBMANAGER_SCRIPT = @"..\..\..\..\..\lib\dbManager.py";
        private const string SONG2DBEXTRACTOR = @"..\..\..\..\..\lib\song2dbExtractor.py";
        /// <summary>
        /// Runs python script and captures standard output
        /// </summary>
        /// <param name="cmd">Name of script</param>
        /// <param name="args">Arguments</param>
        /// <returns>Whole text printed to stdout by script</returns>
        private static string run_cmd(string cmd, string args)
		{
			ProcessStartInfo start = new ProcessStartInfo();
			start.FileName = @"C:\Python27\python.exe";
            string script = string.Format("{0} {1}", cmd, args);
            start.Arguments = script;
			start.UseShellExecute = false;
			start.RedirectStandardOutput = true;
			start.WindowStyle = ProcessWindowStyle.Hidden;
			start.CreateNoWindow = true;
            using (Process process = Process.Start(start))
			{
				using (StreamReader reader = process.StandardOutput)
				{
					string result = reader.ReadToEnd();
                    if (!result.Contains("Success"))
                        throw new ExecutionEngineException($"Python script failed!\n Content: {script}");
                    return result;
				}
			}
		}

        /// <summary>
        /// Runs script which saves to database songs from specified paths
        /// </summary>
        /// <param name="dbPath">database path</param>
        /// <param name="classicPath">path to songs with classic music</param>
        /// <param name="rockPath">path to songs with rock music</param>
        /// <param name="hiphopPath">path to songs with hiphop music</param>
        /// <param name="countryPath">path to songs with country music</param>
        public static void runScriptSavingToDb(string dbPath, string classicPath, string rockPath, string hiphopPath, string countryPath)
        {
            if ((classicPath[classicPath.Length - 1] != '\\' && classicPath[classicPath.Length - 1] != '/') ||
                (rockPath[rockPath.Length - 1] != '\\' && rockPath[rockPath.Length - 1] != '/') ||
                (hiphopPath[hiphopPath.Length - 1] != '\\' && hiphopPath[hiphopPath.Length - 1] != '/') ||
                (countryPath[countryPath.Length - 1] != '\\' && countryPath[countryPath.Length - 1] != '/'))
                throw new ArgumentException("Path to folder must end with slash or backslash");

            string args = dbPath + " " + classicPath + " " + DataElement.ToString(SongGenre.Classical);
            run_cmd(DBMANAGER_SCRIPT, args);
            args = dbPath + " " + rockPath + " " + DataElement.ToString(SongGenre.Rock);
            run_cmd(DBMANAGER_SCRIPT, args);
            args = dbPath + " " + hiphopPath + " " + DataElement.ToString(SongGenre.Hiphop);
            run_cmd(DBMANAGER_SCRIPT, args);
            args = dbPath + " " + countryPath + " " + DataElement.ToString(SongGenre.Country);
            run_cmd(DBMANAGER_SCRIPT, args);
        }

        /// <summary>
        /// Extracts features of specified song and saves it do database
        /// </summary>
        /// <param name="dbPath">Path to database</param>
        /// <param name="songPath">Path to song on drive</param>
        /// <param name="genre">Genre of song</param>
        public static void saveSongToDb(string dbPath, string songPath, string genre)
        {
            string args = dbPath + " \"" + songPath + "\" " + genre;
            run_cmd(SONG2DBEXTRACTOR, args);
        }
	} 
}
