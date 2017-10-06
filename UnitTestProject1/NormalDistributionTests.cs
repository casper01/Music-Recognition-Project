using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MusicRecognition;
using Accord.Statistics;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Distributions.Univariate;

namespace UnitTestProject1
{
	[TestClass]
	public class NormalDistributionTests
	{
		private const double eps = 0.01;
		[TestMethod]
		public void TestNormalDistribution2Dimensions1()
		{
			double[] mean = { 1.5, 9.0 };
			double[,] covarianceMatrix =
			{
				{ 3.4, 7.9},
				{ -1.0, 7.4}
			};
			double[] value = { 2.5, 4.0 };

			MultivariateNormalDistribution mnd = new MultivariateNormalDistribution(mean, covarianceMatrix);
			double expected = mnd.ProbabilityDensityFunction(value);

			double actual = MathHelper.GaussianDistribution(mean, covarianceMatrix, value);

			Assert.IsTrue(Math.Abs(expected - actual) < eps);
		}

		[TestMethod]
		public void TestNormalDistribution2Dimensions2()
		{
			double[] mean = { -0.5, 9.0 };
			double[,] covarianceMatrix =
			{
				{ 10.4, 7.9},
				{ 1.0, 17.4}
			};
			double[] value = { -5.5, 4.0 };

			MultivariateNormalDistribution mnd = new MultivariateNormalDistribution(mean, covarianceMatrix);
			double expected = mnd.ProbabilityDensityFunction(value);

			double actual = MathHelper.GaussianDistribution(mean, covarianceMatrix, value);

			Assert.IsTrue(Math.Abs(expected - actual) < eps);
		}

		[TestMethod]
		public void TestNormalDistribution2Dimensions3()
		{
			double[] mean = { 1.5, 2.0 };
			double[,] covarianceMatrix =
			{
				{ 1.9, 1.9},
				{ 1.0, 3.4}
			};
			double[] value = { 2.5, 1.0 };

			MultivariateNormalDistribution mnd = new MultivariateNormalDistribution(mean, covarianceMatrix);
			double expected = mnd.ProbabilityDensityFunction(value);

			double actual = MathHelper.GaussianDistribution(mean, covarianceMatrix, value);

			Assert.IsTrue(Math.Abs(expected - actual) < eps);
		}

		[TestMethod]
		public void TestNormalDistribution2Dimensions4()
		{
			double[] mean = { 0, 0 };
			double[,] covarianceMatrix =
			{
				{ 1.1, 1},
				{ 1, 1}
			};
			double[] value = { 0.1, 0.1 };

			MultivariateNormalDistribution mnd = new MultivariateNormalDistribution(mean, covarianceMatrix);
			double expected = mnd.ProbabilityDensityFunction(value);

			double actual = MathHelper.GaussianDistribution(mean, covarianceMatrix, value);

			Assert.IsTrue(Math.Abs(expected - actual) < eps);
		}

		[TestMethod]
		public void TestNormalDistribution1Dimension1()
		{
			double[] mean = { 2 };
			double[,] covarianceMatrix =
			{
				{ 1 }
			};
			double[] value = { 0 };
			double stdDev = Math.Sqrt(covarianceMatrix[0, 0]);

			NormalDistribution nd = new NormalDistribution(mean[0], stdDev);
			double expected = nd.ProbabilityDensityFunction(value[0]);

			double actual = MathHelper.GaussianDistribution(mean, covarianceMatrix, value);

			Assert.IsTrue(Math.Abs(expected - actual) < eps);
		}

		[TestMethod]
		public void TestNormalDistribution1Dimension2()
		{
			double[] mean = { 2.7 };
			double[,] covarianceMatrix =
			{
				{ 1.2 }
			};
			double[] value = { 1.3 };
			double stdDev = Math.Sqrt(covarianceMatrix[0, 0]);

			NormalDistribution nd = new NormalDistribution(mean[0], stdDev);
			double expected = nd.ProbabilityDensityFunction(value[0]);

			double actual = MathHelper.GaussianDistribution(mean, covarianceMatrix, value);

			Assert.IsTrue(Math.Abs(expected - actual) < eps);
		}

		[TestMethod]
		public void TestNormalDistribution1Dimension3()
		{
			double[] mean = { 5 };
			double[,] covarianceMatrix =
			{
				{ 10 }
			};
			double[] value = { 9 };
			double stdDev = Math.Sqrt(covarianceMatrix[0, 0]);

			NormalDistribution nd = new NormalDistribution(mean[0], stdDev);
			double expected = nd.ProbabilityDensityFunction(value[0]);

			double actual = MathHelper.GaussianDistribution(mean, covarianceMatrix, value);

			Assert.IsTrue(Math.Abs(expected - actual) < eps);
		}

		
	}
}
