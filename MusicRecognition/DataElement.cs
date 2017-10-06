using System;

namespace MusicRecognition
{
    /// <summary>
    /// Represents type of a song
    /// </summary>
	public enum SongGenre
	{
		Rock,
		Country,
		Hiphop,
		Classical
	}

    /// <summary>
    /// Represents a song
    /// </summary>
	public class DataElement
	{
        private static Extractor _extractor;
        private static Extractor extractor
        {
            get
            {
                if (_extractor == null)
                    _extractor = new Extractor();
                return _extractor;
            }
        }
		private SongGenre _label;

        /// <summary>
        /// Creates new song instance
        /// </summary>
		public DataElement()
		{
		}

        /// <summary>
        /// Creates new song instance
        /// </summary>
        /// <param name="filePath">Path to song file on drive</param>
		public DataElement(string filePath)
		{
			FilePath = filePath;
		}

        /// <summary>
        /// Creates new song instance
        /// </summary>
        /// <param name="label">Genre of the song</param>
        /// <param name="meanVector">Mean vector of song features</param>
        /// <param name="covarianceMatrix">Covariancematrix of song features</param>
		public DataElement(SongGenre label, double[] meanVector, double[,] covarianceMatrix)
		{
			_label = label;
			MeanVector = meanVector;
			CovarianceMatrix = covarianceMatrix;
		}

        /// <summary>
        /// Creates new song instance
        /// </summary>
        /// <param name="filePath">Genre of the song</param>
        /// <param name="label">Genre of the song</param>
        /// <param name="meanVector">Mean vector of song features</param>
        /// <param name="mfccCoefficients">MFCC coefficients of song</param>
        /// <param name="covarianceMatrix">Covariancematrix of song features</param>
		public DataElement(string filePath, SongGenre label, double[] meanVector, double[][] mfccCoefficients, double[,] covarianceMatrix)
        {
            FilePath = filePath;
            _label = label;
            MeanVector = meanVector;
	        MfccCoefficients = mfccCoefficients;
            CovarianceMatrix = covarianceMatrix;
        }

        /// <summary>
        /// Creates new song instance
        /// </summary>
        /// <param name="meanVector">Mean vector of song features</param>
		public DataElement(double[] meanVector)
		{
			MeanVector = meanVector;
		}

        /// <summary>
        /// Checks if features of this song which is represented by this instance are extracted
        /// </summary>
        /// <returns>False if mean vector, covariance matrix and mfcc coefficients are not extracted. Otherwise true</returns>
        public bool HasFeaturesExtracted()
        {
            if (MeanVector != null && CovarianceMatrix != null && MfccCoefficients != null)
            {
                foreach (var v in MeanVector)
                    if (Math.Abs(v) > double.Epsilon)
                        return true;
                foreach (var m in CovarianceMatrix)
                    if (Math.Abs(m) > double.Epsilon)
                        return true;
                for(int i=0; i<MfccCoefficients.Length;i++)
		            for (int j = 0; j < MfccCoefficients[i].Length; j++)
						if(Math.Abs(MfccCoefficients[i][j]) > double.Epsilon)
							return true;
            }
            return false;
        }

        /// <summary>
        /// Path to the song file on drive
        /// </summary>
		public string FilePath { get; set; }

        /// <summary>
        /// Covariance matrix of song extracted features
        /// </summary>
		public double[,] CovarianceMatrix { get; set; }

        /// <summary>
        /// Mean vector of song extracted features
        /// </summary>
		public double[] MeanVector { get; set; }

        /// <summary>
        /// MFCC coefficients of song extracted features
        /// </summary>
		public double[][] MfccCoefficients { get; set; }

		public double NeighbourDistance { get; set; }

		public double Probability { get; set; }

        /// <summary>
        /// Genre of the song
        /// </summary>
		public SongGenre Label
		{
			set { _label = value; }
            get { return _label; }
		}

        /// <summary>
        /// Extracts features of the song from database
        /// </summary>
		public void GetFeatures()
		{
            double[] meanVector;
			double[][] mfccCoefficients;
			double[,] covMatrix;
			extractor.Extract(this, out meanVector, out mfccCoefficients, out covMatrix);
            MeanVector = meanVector;
			MfccCoefficients = mfccCoefficients;
			CovarianceMatrix = covMatrix;
		}

        /// <summary>
        /// Gets genre of the song
        /// </summary>
        /// <returns>Genre of the song</returns>
		public SongGenre GetClass()
		{		
			return _label;
		}

        /// <summary>
        /// Computes KL distance to the other song
        /// </summary>
        /// <param name="data">Song to which distance will be computed</param>
		public void GetDistance(DataElement data)
		{
            NeighbourDistance = MathHelper.KLdistance(MeanVector, CovarianceMatrix, data.MeanVector, data.CovarianceMatrix);
		}

        /// <summary>
        /// Converts string to enum
        /// </summary>
        public static SongGenre ToSongGenre(string genre)
        {
            switch(genre)
            {
                case "Rock":
                    return SongGenre.Rock;
                case "Country":
                    return SongGenre.Country;
                case "Hiphop":
                    return SongGenre.Hiphop;
                case "Classical":
                    return SongGenre.Classical;
            }
            return SongGenre.Rock;
        }

        /// <summary>
        /// Converts enum to string
        public static string ToString(SongGenre genre)
        {
            switch(genre)
            {
                case SongGenre.Rock:
                    return "Rock";
                case SongGenre.Country:
                    return "Country";
                case SongGenre.Hiphop:
                    return "Hiphop";
                case SongGenre.Classical:
                    return "Classical";
            }
            return "Rock";
        }
	}
}
