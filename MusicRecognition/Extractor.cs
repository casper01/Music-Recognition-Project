using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicRecognition
{
    /// <summary>
    /// Extracts features of song
    /// </summary>
	public class Extractor
	{
        private const string FEATURES_EXTRACTION_SCRIPT = @"..\..\..\..\..\lib\FeaturesExtraction.py";

        /// <summary>
        /// Extracts features of the song. Saves it to database if the features do not exist there yet
        /// </summary>
        /// <param name="data">Song of which features will be extracted</param>
        /// <param name="meanVector">Extracted mean vector</param>
        /// <param name="mfccCoefficients">Extracted MFCC coefficients</param>
        /// <param name="covarianceMatrix">Extracted covariance matrix</param>
        public void Extract(DataElement data, out double[] meanVector, out double[][] mfccCoefficients, out double[,] covarianceMatrix)
		{
            string songName = data.FilePath.Split(new[] { '\\' }).Last();
            using (var db = new Database())
            {
                bool songInDatabase = db.Exists(songName);
                if (!songInDatabase)
                    db.AddTestSong(data.FilePath);

                meanVector = db.GetMeanVector(songName);
                covarianceMatrix = db.GetCovarianceMatrix(songName);
                mfccCoefficients = db.GetMFCCCoefficients(songName);

                if (!songInDatabase)
                    db.DeleteTestFiles();
            }
		} 
	}
}
