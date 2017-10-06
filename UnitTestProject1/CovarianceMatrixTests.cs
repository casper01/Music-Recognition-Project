using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MusicRecognition;

namespace UnitTestProject1
{
	[TestClass]
	public class CovarianceMatrixTests
	{
		[TestMethod]
		public void SimpleTest()
		{
			double[][] vectors = new[]
			{
				new []{ 4.0, 2.0, 0.6},
				new []{ 4.2, 2.1, 0.59},
				new []{ 3.9, 2.0, 0.58},
				new []{ 4.3, 2.1, 0.62},
				new []{ 4.1, 2.2, 0.63}
			};

			double[] means = {4.1, 2.08, 0.604};

			double[,] expected =
			{
				{ 0.025, 0.0075, 0.00175},
				{ 0.0075, 0.0070, 0.00135},
				{ 0.00175, 0.00135, 0.00043}
			};

			double[,] actual = MathHelper.GetMeanCovariance(vectors, means);

			Assert.IsTrue(checkIfTheSame(expected,actual));
			
		}

        public static bool checkIfTheSame(double[,] cov1, double[,] cov2)
        {
            int dim = cov2.GetLength(0);
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
    }
}
