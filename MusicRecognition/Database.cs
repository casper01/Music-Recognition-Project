using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicRecognition
{
    /// <summary>
    /// Manages data from database
    /// </summary>
	public class Database : IDisposable
    {
        /// <summary>
        /// Path to database
        /// </summary>
        private const string DATABASE = "musicDatabase.s3db";
        private SQLiteConnection oSQLiteConnection;
        /// <summary>
        /// Identificator of test songs in database
        /// </summary>
        private string testType = "Test";

        /// <summary>
        /// Creates new instance of database connection
        /// </summary>
        public Database()
        {
            oSQLiteConnection = new SQLiteConnection("Data Source=" + DATABASE);
            oSQLiteConnection.Open();
        }

        /// <summary>
        /// Gets all songs saved in database
        /// </summary>
        /// <returns>List of songs</returns>
        public List<DataElement> GetElements()
        {
            List<DataElement> songs = new List<DataElement>();
            string sql = "SELECT * FROM MusicFiles";
            using (SQLiteCommand command = new SQLiteCommand(sql, oSQLiteConnection))
            {
                SQLiteDataReader reader = command.ExecuteReader();

	            while (reader.Read())
	            {
		            string path = (string) reader["PATH"];
				    songs.Add(new DataElement(path, DataElement.ToSongGenre((string) reader["GENRE"]),
			            Parser.ToDoubleArray((string) reader["MEAN"]), GetMFCCCoefficients(path), Parser.To2dDoubleArray((string) reader["COVARIANCE_MATRIX"])));
	            }
	            return songs;
            }
        }

        /// <summary>
        /// Updates database, adds not analysed yet songs from specified folders
        /// </summary>
        public void UpdateElements(string classicPath, string rockPath, string hiphopPath, string countryPath)
        {
            PythonHelper.runScriptSavingToDb(DATABASE, classicPath, rockPath, hiphopPath, countryPath);
        }

        /// <summary>
        /// Gets mean vector from database of specified song
        /// </summary>
        /// <param name="songPath">Path to song saved in database</param>
        /// <returns>Mean vector</returns>
        public double[] GetMeanVector(string songPath)
        {
            string sql = $"SELECT * FROM MusicFiles WHERE PATH LIKE \"%{songPath}\"";
            using (SQLiteCommand command = new SQLiteCommand(sql, oSQLiteConnection))
            {
                SQLiteDataReader reader = command.ExecuteReader();
                reader.Read();
                return Parser.ToDoubleArray(reader["MEAN"].ToString());
            }
        }

        /// <summary>
        /// Gets covariance matrix from database of specified song
        /// </summary>
        /// <param name="songPath">Path to song saved in database</param>
        /// <returns>Covariance matrix</returns>
        public double[,] GetCovarianceMatrix(string songPath)
        {
            string sql = $"SELECT * FROM MusicFiles WHERE PATH LIKE \"%{songPath}\"";
            using (SQLiteCommand command = new SQLiteCommand(sql, oSQLiteConnection))
            {
                SQLiteDataReader reader = command.ExecuteReader();
                reader.Read();
                return Parser.To2dDoubleArray(reader["COVARIANCE_MATRIX"].ToString());
            }
        }

        /// <summary>
        /// Gets MFCC coefficients from database of specified song
        /// </summary>
        /// <param name="songPath">Path to song saved in database</param>
        /// <returns>Array of ordered coefficients</returns>
        public double[][] GetMFCCCoefficients(string songPath)
        {
            int songId = GetSongId(songPath);
            string sql = $"SELECT * FROM MFCC WHERE SONGID = {songId} ORDER BY SONGPART";
            using (SQLiteCommand command = new SQLiteCommand(sql, oSQLiteConnection))
            {
                SQLiteDataReader reader = command.ExecuteReader();
                List<double[]> mfcc = new List<double[]>();
                while (reader.Read())
                {
                    double[] tempMfcc = Parser.ToDoubleArray(reader["MFCC"].ToString());
                    mfcc.Add(tempMfcc);
                }
                return mfcc.ToArray();
            }
        }

        /// <summary>
        /// Gets id of song saved in database
        /// </summary>
        /// <param name="songPath">Path of song saved in database</param>
        /// <returns>Song id</returns>
        private int GetSongId(string songPath)
        {
            string sql = $"SELECT * FROM MusicFiles WHERE PATH LIKE \"%{songPath}\"";
            using (SQLiteCommand command = new SQLiteCommand(sql, oSQLiteConnection))
            {
                SQLiteDataReader reader = command.ExecuteReader();
                reader.Read();
                return int.Parse(reader["ID"].ToString());
            }
        }

        /// <summary>
        /// Adds test song to database, computes its MFCC coefficients, covariance matrix and mean vector
        /// </summary>
        /// <param name="songPath">Path to song on disc</param>
        public void AddTestSong(string songPath)
        {
            PythonHelper.saveSongToDb(DATABASE, songPath, testType);
        }

        /// <summary>
        /// Deletes all test files saved to database using AddTestSong method
        /// </summary>
        public void DeleteTestFiles()
        {
            // delete from MFCC table
            string sql = $"DELETE FROM MFCC WHERE ID IN (SELECT ID FROM MusicFiles WHERE GENRE = \'" + testType + "\')";
            using (SQLiteCommand command = new SQLiteCommand(sql, oSQLiteConnection))
            {
                command.ExecuteNonQuery();
            }

            // delete from MusicFiles
            sql = $"DELETE FROM MusicFiles WHERE GENRE = \'" + testType + "\'";
            using (SQLiteCommand command = new SQLiteCommand(sql, oSQLiteConnection))
            {
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Checks if specified song exists in database
        /// </summary>
        /// <param name="songName">Path to song</param>
        /// <returns>True if song exists in database, otherwise false</returns>
        public bool Exists(string songName)
        {
            string sql = "SELECT 1 FROM MusicFiles WHERE PATH LIKE \"%" + songName + "\"";
            using (SQLiteCommand command = new SQLiteCommand(sql, oSQLiteConnection))
            {
                SQLiteDataReader reader = command.ExecuteReader();
                return reader.HasRows;
            }
        }

        public void Dispose()
        {
            if (null != oSQLiteConnection)
            {
                oSQLiteConnection.Close();
                oSQLiteConnection.Dispose();
                oSQLiteConnection = null;
            }
        }
    }
}
