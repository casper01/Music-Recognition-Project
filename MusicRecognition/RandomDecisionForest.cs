using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;

namespace MusicRecognition.GaussianMixtureModels
{
	public class RandomDecisionForest : AbstractClassification
	{
		public int nClasses { get; set; } // classification task with NClasses classes
		public int nTrees { get; set; } // number of trees in a forest, NTrees>=1.
										// recommended values: 50-100
		public double R { get; set; } // percent of a training set used to build
								   // individual trees. 0<R<=1.
								   // recommended values: 0.1 <= R <= 0.66
		public alglib.decisionforest Decisionforest { get; private set; }
		public alglib.dfreport Report { get; private set; }
		public int Info { get; private set; }

		public RandomDecisionForest(int _nClasses, int _nTrees, double _r)
		{
			nClasses = _nClasses;
			nTrees = _nTrees;
			R = _r;
			Name = "Random decision forest";
		}

		private double[,] prepareListData(List<DataElement> data, out int nvars, out int npoints)
		{
			if(data == null || data.Count==0 || data[0] == null
				|| data[0].MeanVector==null )
				throw new ArgumentException("Invalid training data set.");
			npoints = data.Count;
			nvars = data[0].MeanVector.Length;
			double[,] preparedData = new double[data.Count,nvars + 1];
			for (int i = 0; i < npoints; i++)
			{
				for (int j = 0; j < nvars; j++)
				{
					preparedData[i, j] = data[i].MeanVector[j];
				}
				preparedData[i, nvars] = (double)data[i].Label;
			}

			return preparedData;
		}

		public override void Learn(List<DataElement> data)
		{
			base.Learn(data);

			int nvars, npoints;
			double[,] preparedData = prepareListData(data, out nvars, out npoints);
			alglib.decisionforest df;
			alglib.dfreport rep;
			int info;
			alglib.dfbuildrandomdecisionforest(preparedData,npoints,nvars,nClasses,nTrees,R,
				out info, out df, out rep);
			Decisionforest = df;
			Report = rep;
			Info = info;
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

			int nvars = data.MeanVector.Length;
			double[] preparedData = new double[nvars];
			for (int j = 0; j < nvars; j++)
				preparedData[j] = data.MeanVector[j];
			double[] result = new double[nClasses];
			alglib.dfprocess(Decisionforest, preparedData, ref result);
			double max = Enumerable.Max(result);

			data.Probability = max;
			return (SongGenre)result.IndexOf(max);
		}

		public override string GetName()
		{
			return Name;
		}
	}
}
