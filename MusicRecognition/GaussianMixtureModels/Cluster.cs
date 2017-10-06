using System;

namespace MusicRecognition.GaussianMixtureModels
{
    public class Cluster
    {
	    private const double minLogValue = -350;
        /// <summary>
        /// Mean vector
        /// </summary>
        public double[] Mean { get; set; }
        /// <summary>
        /// Covariance matrix
        /// </summary>
        public double[,] Covariance { get; set; }
        /// <summary>
        /// Weight of cluster in GMM
        /// </summary>
        public double Weight { get; set; }
        /// <summary>
        /// Sum of probabilities of all members in GMM
        /// </summary>
        public double Members { get; set; }

        /// <summary>
        /// Gets value of gaussian distribution of specified song
        /// </summary>
        /// <param name="song">MFCC coefficients of song</param>
        /// <returns>Gaussian distribution result</returns>
        public double GetGaussianValue(double[][] song)
        {
	        double gaussianSum = 0;

	        foreach (var mfccVector in song)
	        {
		        double gaussianValue = Math.Log(Weight*MathHelper.GaussianDistribution(Mean, Covariance, mfccVector));
		        
		        if (gaussianValue < minLogValue)
			        gaussianValue = minLogValue;
				gaussianSum += gaussianValue;
	        }
            return gaussianSum;
        }

        /// <summary>
        /// Creates new instance of cluster with specified mean vector, covariance matrix an weithg
        /// </summary>
        public Cluster(double[] _Mean, double[,] _Covariance, double _Weight)
	    {
		    Mean = _Mean;
		    Covariance = _Covariance;
		    Weight = _Weight;
	    }
    }
}
