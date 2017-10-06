using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;
using Accord.Statistics.Links;

namespace MusicRecognition.GaussianMixtureModels
{
	public class GMM : AbstractClassification
	{
		private List<List<DataElement>> trainingDataByGenre;
		private List<multivariateGaussian> multivariateGaussians;
		private double threshold;

        /// <summary>
        /// number of distributions in multivariate Gauss distribution
        /// </summary>
        public int D { get; private set; }
	
		public GMM(int d, double _threshold)
		{
			D = d;
			threshold = _threshold;
			Name = "Gaussian mixture model";
		}

		private List<List<DataElement>> divideDataIntoSets(List<DataElement> data)
		{
			List<List<DataElement>> dividedData = new List<List<DataElement>>();
			foreach (var genre in Enum.GetNames(typeof(SongGenre)))
			{
				dividedData.Add(new List<DataElement>());
			}
			foreach (var d in data)
			{
				dividedData[(int)d.Label].Add(d);
			}

			return dividedData;
		}

		public override void Learn(List<DataElement> data)
		{
			base.Learn(data);
			TrainingData = data;

			trainingDataByGenre = divideDataIntoSets(TrainingData);
			multivariateGaussians = new List<multivariateGaussian>();
			int i = 0;
			foreach (var genreData in trainingDataByGenre)
			{
				multivariateGaussian mg = new multivariateGaussian(threshold, (SongGenre) i, D, genreData.Count);
				multivariateGaussians.Add(mg);
				mg.Learn(genreData);

				i++;
			}			
		}

		public override double GetProbability(DataElement data)
		{
			GetClass(data);
			return data.Probability;
		}

		public override SongGenre GetClass(DataElement data)
		{
			base.GetClass(data);
			if (!data.HasFeaturesExtracted())
				data.GetFeatures();

			double sum = 0;
			int genresCount = multivariateGaussians.Count;
			double[] values = new double[genresCount];
			int[] elementsCount = new int[genresCount];
			int elementsSum = 0;
			int i = 0;
			foreach (var mg in multivariateGaussians)
			{
				values[i] = mg.GetGaussianValue(data);
				sum += values[i];
				elementsCount[i] = mg.ElementsCount;
				elementsSum += elementsCount[i];
				i++;
			}

			values = MathHelper.ConvertToProbability(values);

            // Bayes
            for (int k = 0; k < genresCount; k++)
                values[k] *= ((double)elementsCount[k] / (double)elementsSum);

            data.Probability = Enumerable.Max(values);
			return multivariateGaussians[Matrix.IndexOf(values, data.Probability)].GetGenre(); 
		}

		public override string GetName()
		{
			return Name;
		}
	}
}
