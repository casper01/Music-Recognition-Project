using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Statistics;
using MathNet.Numerics.LinearAlgebra;
using System.Windows;

namespace MusicRecognition
{
    /// <summary>
    /// Class used to perform advanced math operations
    /// </summary>
	public static class MathHelper
	{
		private const double eps = 1E-10;

        /// <summary>
        /// Scales results of gaussian distribution to probability scale
        /// </summary>
        /// <param name="gaussianValues">Results of gaussian distribution</param>
        /// <param name="sum">Sum of the results</param>
        /// <returns>Results of gaussian distribution scaled to range [0,1]</returns>
        public static double[] ScaleGaussiansToProbabilities(double[] gaussianValues, double sum)
		{
			if (gaussianValues.Contains(double.PositiveInfinity))				
            {
				int count = gaussianValues.Where(v => double.IsPositiveInfinity(v)).Count();

				for (int i = 0; i < gaussianValues.Length; i++)
                    if (gaussianValues[i] == double.PositiveInfinity)
						gaussianValues[i] = 1 / count;
					else
						gaussianValues[i] = 0;
				return gaussianValues;
			}

            double scaleNegative = 0;
            for (int i = 0; i < gaussianValues.Length; i++)
            {
                gaussianValues[i] /= sum;
                if (sum < 0)
                {
                    gaussianValues[i] = 1 / gaussianValues[i];
                    scaleNegative += gaussianValues[i];
                }
            }
            if (sum < 0)
                for (int i = 0; i < gaussianValues.Length; i++)
                    gaussianValues[i] /= scaleNegative;
            for (int i = 0; i < gaussianValues.Length; i++)
                if (gaussianValues[i] < eps)
                    gaussianValues[i] = eps;
            return gaussianValues;
        }

        /// <summary>
        /// Computes euclidean distance between two vectors
        /// </summary>
        public static double EuclideanDistance(double[] vector1, double[] vector2)
		{
			if(vector1.Length != vector2.Length)
				throw new ArgumentException("Vectors for Euclidean distance must be the same length!");
			double distance = 0;
			for (int i = 0; i < vector1.Length; i++)
			{
				distance += (vector1[i] - vector2[i])*(vector1[i] - vector2[i]);
			}
			distance = Math.Sqrt(distance);

			return distance;
		}

        /// <summary>
        /// Computes distances between means of corresponding elements of the lists
        /// </summary>
        /// <returns>list of means distances</returns>
		public static List<double> CountMeansDistances(List<DataElement> prevMeans, List<DataElement> means)
		{
			List<double> meansDistances = new List<double>();
			for (int i = 0; i < means.Count; i++)
			{
				meansDistances.Add(MathHelper.EuclideanDistance(means[i].MeanVector, prevMeans[i].MeanVector));
			}

			return meansDistances;
		}

        /// <summary>
        /// Checks if any of distances is smaller than treshold
        /// </summary>
        /// <param name="distances">List of distances to analyse</param>
        /// <returns>True if at least one of distance is smaller than treshold</returns>
		public static bool meansDistancesChanged(double threshold, List<double> distances)
		{
			foreach (var d in distances)
			{
				if (d > threshold)
					return true;
			}

			return false;
		}

        /// <summary>
        /// Computes value of gaussian distribution
        /// </summary>
        /// <param name="mean">Mean vector which describes gaussian distribution</param>
        /// <param name="covarianceMatrix">Covariance matrix which describes gaussian distribution</param>
        /// <param name="value">Arugment to be computed</param>
        public static double GaussianDistribution(double[] mean, double[,] covarianceMatrix, double[] value)
        {
            int d = mean.Length;
            Matrix<double> covariance = Matrix<double>.Build.DenseOfArray(covarianceMatrix);
            double covarianceDeterminant = covariance.Determinant();

            // determinant must be positive, if it isn't, it's round error. Needs to be changed
            while (covarianceDeterminant <= 0)
            {
                for (int i = 0; i < covariance.RowCount; i++)
                    covariance[i, i] += eps;
                covarianceDeterminant = covariance.Determinant();
            }

            double ans = 1 / Math.Sqrt(Math.Pow((2 * Math.PI), d) * covarianceDeterminant);
            Vector<double> meanVector = Vector<double>.Build.DenseOfArray(mean);
            Vector<double> valueVector = Vector<double>.Build.DenseOfArray(value);
            Matrix<double> invertedCovariance = covariance.Inverse();
            var diff = valueVector.Subtract(meanVector);
            Vector<double> result = invertedCovariance.LeftMultiply(diff);
			double exp = result * diff;
			exp *= -0.5;
            exp = Math.Exp(exp);

            // exp cannot be 0, if it is, take epsilon
            exp = exp == 0 ? eps : exp;

            if (ans * exp == 0)
                return eps;
            if (double.IsNaN(ans * exp))
                throw new Exception();  // TODO: usun
            return ans * exp;
        }
        
        /// <summary>
        /// Computes mean vector of vectors given as an argument, including weights
        /// </summary>
        /// <returns>Mean vector</returns>
        public static double[] GetMeanVector(List<double[][]> vectors, double[] weights)
        {
            if (vectors == null || vectors.Count == 0 || vectors[0] == null || vectors[0].Length == 0 || vectors[0][0] == null)
                return null;
            int m = vectors.Count;

            int dim = vectors[0][0].Length;
            double[] mean = new double[dim];

            int j = 0;
            int howManyVectors = 0;
            foreach (double[][] songMfccVectors in vectors)
            {
                howManyVectors += songMfccVectors.Length;
                for (int k = 0; k < dim; k++)
                    for (int i = 0; i < songMfccVectors.Length; i++)
                        mean[k] += songMfccVectors[i][k] * weights[j];
                j++;
            }
            for (int i = 0; i < mean.Length; i++)
                mean[i] /= howManyVectors;

            return mean;
        }

        /// <summary>
        /// Computes covariance matrix using MFCC vectors of songs and mean vector of song
        /// </summary>
        /// <param name="data">array of MFCC vectors of each song</param>
        /// <param name="meanVector">Mean vector</param>
        public static double[,] GetMeanCovariance(List<double[][]> data, double[] meanVector)
        {
            int m = meanVector.Length;
            double[,] ans = new double[m, m];
            Vector<double> mean = Vector<double>.Build.DenseOfArray(meanVector);

            int howManyVectors = 0;
            foreach (double[][] vectors in data)
            {
                foreach (var vector in vectors)
                {
                    howManyVectors++;
                    Vector<double> v = Vector<double>.Build.DenseOfArray(vector);
                    var diff = v.Subtract(mean);
                    for (int i = 0; i < m; i++)
                        for (int j = 0; j < m; j++)
							ans[i, j] += diff[i] * diff[j];
				}
            }

            for (int i = 0; i < m; i++)
                for (int j = 0; j < m; j++)
                    ans[i, j] /= howManyVectors;
            return ans;
        }

        /// <summary>
        /// Computes covariance matrix using mean MFCC vector of songs and mean vector of song
        /// </summary>
        /// <param name="vectors">mean MFCC vectors of each song</param>
        /// <param name="meanVector">Mean vector of song</param>
		public static double[,] GetMeanCovariance(double[][] vectors, double[] meanVector)
		{
			int m = meanVector.Length;
			double[,] ans = new double[m, m];
			Vector<double> mean = Vector<double>.Build.DenseOfArray(meanVector);

			int k = 0;
			foreach (double[] vector in vectors)
			{
				Vector<double> v = Vector<double>.Build.DenseOfArray(vector);
				var diff = v.Subtract(mean);
				for (int i = 0; i < m; i++)
					for (int j = 0; j < m; j++)
						ans[i, j] += diff[i] * diff[j];
				k++;
			}

			int n = vectors.Length;
			for (int i = 0; i < m; i++)
				for (int j = 0; j < m; j++)
					ans[i, j] = ans[i, j] / (n-1);
			return ans;
		}

        /// <summary>
        /// Converts all values to probability range
        /// </summary>
        /// <param name="values">Values to change</param>
        public static double[] ConvertToProbability(double[] values)
        {
            // in case of +inf
            if (values.Contains(double.PositiveInfinity))
            {
                int count = values.Where(v => double.IsPositiveInfinity(v)).Count();

                for (int i = 0; i < values.Length; i++)
                    if (values[i] == double.PositiveInfinity)
                        values[i] = 1 / count;
                    else
                        values[i] = 0;
                return values;
            }
            double minVal = values.Where(v => !double.IsNegativeInfinity(v)).Min();
            double sum = values.Sum();
            if (minVal < 0)
            {
                sum = 0;
                for (int i = 0; i < values.Length; i++)
                {
                    if (double.IsNegativeInfinity(values[i]))
                        continue;
                    values[i] -= minVal;
                    sum += values[i];
                }
            }

            // in case of -inf
            if (values.Contains(double.NegativeInfinity))
            {
                for (int i = 0; i < values.Length; i++)
                    if (values[i] == double.NegativeInfinity)
                        values[i] = 0;
            }

            for (int i = 0; i < values.Length; i++)
            {
                values[i] /= sum;
            }
            return values;
        }

        #region Python Substitutes
        private static double KLdivergence(double[] mean1, double[,] covariance1, double[] mean2, double[,] covariance2)
        {
            Vector<double> pm = Vector<double>.Build.DenseOfArray(mean1);
            Matrix<double> pv = Matrix<double>.Build.DenseOfArray(covariance1);
            Vector<double> qm = Vector<double>.Build.DenseOfArray(mean2);
            Matrix<double> qv = Matrix<double>.Build.DenseOfArray(covariance2);

            Matrix<double> invQv = qv.Inverse();
            double traceQvPv = (invQv * pv).Trace();
            Vector<double> meanDiff = qm - pm;
            double detPv = pv.Determinant();
            double detQv = qv.Determinant();
            double detln = Math.Log(detPv / detQv);
            double d = 12;
            double dist = 0.5 * (traceQvPv + ((meanDiff * invQv) * meanDiff) - d - detln);
            return dist;
        }
        /// <summary>
        /// Kullback Liebler distance implementation
        /// </summary>
        public static double KLdistance(double[] mean1, double[,] covariance1, double[] mean2, double[,] covariance2)
        {
            return KLdivergence(mean1, covariance1, mean2, covariance2) + KLdivergence(mean2, covariance2, mean1, covariance1);
        }
        #endregion
    }
}
