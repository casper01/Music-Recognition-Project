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
	public static class MathHelper
	{
		private const double eps = 1E-10;

        //public static double[] ScaleGaussiansToProbabilities(double[] gaussianValues, double sum)
        //{
        //          double[] probabilities = new double[gaussianValues.Length];
        //          int posinfCount = gaussianValues.Where(v => double.IsPositiveInfinity(v)).Count();
        //          int neginfCount = gaussianValues.Where(v => double.IsNegativeInfinity(v)).Count();
        //          double minValue = gaussianValues.Where(v => !double.IsNegativeInfinity(v)).Min();
        //          // if there exists +inf
        //          if (posinfCount > 0)
        //          {
        //              for (int i = 0; i < gaussianValues.Length; i++)
        //                  if (gaussianValues[i] == double.PositiveInfinity)
        //                      probabilities[i] = 1.0 / (double)posinfCount;
        //                  else
        //                      probabilities[i] = 0;
        //              return probabilities;
        //          }
        //          // if there are only -inf
        //          if (neginfCount == probabilities.Length)
        //          {
        //              for (int i = 0; i < probabilities.Length; i++)
        //                  probabilities[i] = 1.0 / (double)neginfCount;
        //              return probabilities;
        //          }

        //          //// for the rest values != -inf
        //          //sum = 0;
        //          //if (minValue < 0)
        //          //    for (int i = 0; i < probabilities.Length; i++)
        //          //    {
        //          //        if (double.IsNegativeInfinity(gaussianValues[i]))
        //          //            continue;
        //          //        gaussianValues[i] -= minValue;
        //          //        sum += gaussianValues[i];
        //          //    }
        //          //else
        //          //    sum = gaussianValues.Where(v => !double.IsNegativeInfinity(v)).Sum();

        //          //for (int i = 0; i < probabilities.Length; i++)
        //          //{
        //          //    if (double.IsNegativeInfinity(gaussianValues[i]))
        //          //        continue;
        //          //    probabilities[i] = gaussianValues[i] / sum;
        //          //}
        //          //return probabilities;

        //          //// in case of +inf
        //          //if (gaussianValues.Contains(double.PositiveInfinity))
        //          //{
        //          //    int count = gaussianValues.Where(v => double.IsPositiveInfinity(v)).Count();
        //          //    for (int i = 0; i < gaussianValues.Length; i++)
        //          //        if (gaussianValues[i] == double.PositiveInfinity)
        //          //            gaussianValues[i] = 1 / count;
        //          //        else
        //          //            gaussianValues[i] = 0;
        //          //    return gaussianValues;
        //          //}

        //          //double minVal = gaussianValues.Where(v=>!double.IsNegativeInfinity(v)).Min();
        //          //if (minVal < 0)
        //          //{
        //          //    sum = 0;
        //          //    for (int i = 0; i < gaussianValues.Length; i++)
        //          //    {
        //          //        if (double.IsNegativeInfinity(gaussianValues[i]))
        //          //            continue;
        //          //        gaussianValues[i] -= minVal;
        //          //        sum += gaussianValues[i];
        //          //    }
        //          //}

        //          //// in case of -inf
        //          //if (gaussianValues.Contains(double.NegativeInfinity))
        //          //{
        //          //    for (int i = 0; i < gaussianValues.Length; i++)
        //          //        if (gaussianValues[i] == double.NegativeInfinity)
        //          //            gaussianValues[i] = 0;
        //          //}

        //          //for (int i = 0; i < gaussianValues.Length; i++)
        //          //{
        //          //    gaussianValues[i] /= sum;
        //          //}
        //          //return gaussianValues;

        //          double scaleNegative = 0;
        //          for (int i = 0; i < gaussianValues.Length; i++)
        //          {
        //              gaussianValues[i] /= sum;
        //              if (sum < 0)
        //              {
        //                  gaussianValues[i] = 1 / gaussianValues[i];
        //                  scaleNegative += gaussianValues[i];
        //              }
        //          }
        //          if (sum < 0)
        //              for (int i = 0; i < gaussianValues.Length; i++)
        //                  gaussianValues[i] /= scaleNegative;
        //          for (int i = 0; i < gaussianValues.Length; i++)
        //              if (gaussianValues[i] < eps)
        //                  gaussianValues[i] = eps;
        //          return gaussianValues;
        //      }
//        public static double[] ScaleGaussiansToProbabilities(double[] gaussianValues, double sum)
//        {
//            // in case of +inf
//            if (gaussianValues.Contains(double.PositiveInfinity))
//            {
//                int count = gaussianValues.Where(v => double.IsPositiveInfinity(v)).Count();
//                for (int i = 0; i < gaussianValues.Length; i++)
//                    if (gaussianValues[i] == double.PositiveInfinity)
//                        gaussianValues[i] = 1 / count;
//                    else
//                        gaussianValues[i] = 0;
//                return gaussianValues;
//            }
//            double scaleNegative = 0;
//            for (int i = 0; i<gaussianValues.Length; i++)
//            {
//                gaussianValues[i] /= sum;
//                if (sum< 0)
//                {
//                    gaussianValues[i] = 1 / gaussianValues[i];
//                    scaleNegative += gaussianValues[i];
//                }
//}
//            if (sum< 0)
//                for (int i = 0; i<gaussianValues.Length; i++)
//                    gaussianValues[i] /= scaleNegative;
//            for (int i = 0; i<gaussianValues.Length; i++)
//                if (gaussianValues[i] < eps)
//                    gaussianValues[i] = eps;
//            return gaussianValues;
//        }

        public static double[] ScaleGaussiansToProbabilities(double[] probabilities, double sum)
        {
            // if everything is zero, return array full of zeros
            if (sum == 0)
                return new double[probabilities.Length];

            double scaleNegative = 0;
            for (int i = 0; i < probabilities.Length; i++)
            {
                if (double.IsInfinity(sum))
                    probabilities[i] = 0;
                else
                    probabilities[i] /= sum;
                if (sum < 0)
                {
                    probabilities[i] = 1 / probabilities[i];
                    scaleNegative += probabilities[i];
                }
            }
            if (sum < 0)
                for (int i = 0; i < probabilities.Length; i++)
                    probabilities[i] /= scaleNegative;
            for (int i = 0; i < probabilities.Length; i++)
                if (probabilities[i] < eps)
                    probabilities[i] = eps;
            return probabilities;
        }


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

		public static List<double> countMeansDistances(List<DataElement> prevMeans, List<DataElement> means)
		{
			List<double> meansDistances = new List<double>();
			for (int i = 0; i < means.Count; i++)
			{
				meansDistances.Add(MathHelper.EuclideanDistance(means[i].MeanVector, prevMeans[i].MeanVector));
			}

			return meansDistances;
		}

		public static bool meansDistancesChanged(double threshold, List<double> distances)
		{
			foreach (var d in distances)
			{
				if (d > threshold)
					return true;
			}

			return false;
		}

        //     public static double NormalDistribution(double[] mean, double[,] covarianceMatrix, double[] value)
        //     {
        //         int d = mean.Length;
        //         Matrix<double> covariance = Matrix<double>.Build.DenseOfArray(covarianceMatrix);
        //         double covarianceDeterminant = covariance.Determinant();
        //         while (covarianceDeterminant <= 0)
        //         {
        //             for (int i = 0; i < covariance.RowCount; i++)
        //                 covariance[i, i] += eps;
        //             covarianceDeterminant = covariance.Determinant();
        //         }

        //         double ans = 1 / Math.Sqrt(Math.Pow((2 * Math.PI), d) * covarianceDeterminant);
        //         Vector<double> meanVector = Vector<double>.Build.DenseOfArray(mean);
        //         Vector<double> valueVector = Vector<double>.Build.DenseOfArray(value);
        //         Matrix<double> invertedCovariance = covariance.Inverse();
        //         var diff = valueVector.Subtract(meanVector);
        //         Vector<double> result = invertedCovariance.LeftMultiply(diff);
        //double exp = result * diff;
        //exp *= -0.5;
        //         exp = Math.Exp(exp);

        //         exp = exp == 0 ? eps : exp;

        //         if (ans * exp == 0)
        //             return eps;
        //         if (double.IsNaN(ans * exp))
        //             throw new Exception();      // TODO: usunac
        //         return ans * exp;
        //     }
        public static double NormalDistribution(double[] mean, double[,] covarianceMatrix, double[] value)
        {
            int d = mean.Length;
            Matrix<double> covariance = Matrix<double>.Build.DenseOfArray(covarianceMatrix);
            double covarianceDeterminant = covariance.Determinant();
            while (covarianceDeterminant <= 0)
            {
                for (int i = 0; i < covariance.RowCount; i++)
                    covariance[i, i] += eps;
                covarianceDeterminant = covariance.Determinant();
            }

            // TODO:: ogarnac to obejście na piechote
            /* start obejsca */
            double nearToZero = Math.Pow(0.1, 5);
            if (covarianceDeterminant <= 0 || Math.Abs(covarianceDeterminant) <= nearToZero)
            {
                covarianceDeterminant = nearToZero;
                //throw new ArgumentException($"Non positive determinant of covariance matrix: {covarianceDeterminant}");
            }
            /* koniec obejsca */

            double ans = 1 / Math.Sqrt(Math.Pow((2 * Math.PI), d) * covarianceDeterminant);
            //ans *= Math.Pow(covariance.Determinant(), -0.5);

            Vector<double> meanVector = Vector<double>.Build.DenseOfArray(mean);
            Vector<double> valueVector = Vector<double>.Build.DenseOfArray(value);
            Matrix<double> invertedCovariance = covariance.Inverse();
            var diff = valueVector.Subtract(meanVector);
            Vector<double> result = invertedCovariance.LeftMultiply(diff);
            double exp = result * diff;
            exp *= -0.5;
            exp = Math.Exp(exp);
            return ans * exp;
        }

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

		// TODO:: może wagi uwzględniać?
        public static double[,] GetMeanCovariance(List<double[][]> data, double[] meanVector, double[] weights)
        {
            int m = meanVector.Length;
            double[,] ans = new double[m, m];
            Vector<double> mean = Vector<double>.Build.DenseOfArray(meanVector);

            int k = 0;
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

                k++;
            }

            for (int i = 0; i < m; i++)
                for (int j = 0; j < m; j++)
                {
                    ans[i, j] /= howManyVectors;
                    if (double.IsNaN(ans[i, j]))
                        throw new NotSupportedException("NaN in covariance");       // TODO: usunac
                }
            return ans;
        }


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

		public static bool checkIfTheSame(double[,] cov1, double[,] cov2)
		{
			int dim = cov2.GetLength(0);
			bool same = true;
			for (int i = 0; i < dim; i++)
			{
				for (int j = 0; j < dim; j++)
				{
					if (Math.Abs(cov1[i, j] - cov2[i, j]) > 0.00001)
						return false;
				}
			}

			return true;
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
        public static double KLdistance(double[] mean1, double[,] covariance1, double[] mean2, double[,] covariance2)
        {
            return KLdivergence(mean1, covariance1, mean2, covariance2) + KLdivergence(mean2, covariance2, mean1, covariance1);
        }
        #endregion
    }
}
