using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicRecognition.GaussianMixtureModels;

namespace MusicRecognition
{
    public class multivariateGaussian
    {
	    private SongGenre genre;
	    private int d;
	    private bool AfterLearning = false;

        private List<Cluster> _gaussianDistributions;
        private double threshold;
        private int maxIterations = 50;
        private ExpectationMaximization em;
        /// <summary>
        /// Count of all  elements
        /// </summary>
        public int ElementsCount { get; private set; }

        /// <summary>
        /// Creates new instance of gaussian mixture specified for one music genre
        /// </summary>
        /// <param name="_genre">Genre which this object will be representing</param>
        /// <param name="_d">Number of distributions inside this object</param>
        /// <param name="count">Count of elements in this object</param>
		public multivariateGaussian(double _threshold, SongGenre _genre, int _d, int count)
        {
            threshold = _threshold;
	        genre = _genre;
	        d = _d;
			ElementsCount = count;
		}

        /// <summary>
        /// Gets maximum of all gaussian distributions for specified song
        /// </summary>
        /// <param name="data">Song</param>
        public double GetGaussianValue(DataElement data)
        {
			if(!AfterLearning)
				throw new NotSupportedException("Could not get GMM value before learning phase!");
			if(!data.HasFeaturesExtracted())
				data.GetFeatures();
			double max = em.GetGaussianMixtureMax(data.MfccCoefficients);			

			return max; 
        }

        public void Learn(List<DataElement> data)
        {
            kMeans _kMeans = new kMeans(d);
            _gaussianDistributions = _kMeans.GetGroupRepresentatives(data, threshold);

            List<double[][]> mfccVectors = new List<double[][]>();
            for (int i = 0; i < data.Count; i++)
                mfccVectors.Add(data[i].MfccCoefficients);

            em = new ExpectationMaximization(_gaussianDistributions.ToArray(), mfccVectors);

            int iterations = 0;
            while (iterations < maxIterations)
            {
                List<DataElement> prevMeans = new List<DataElement>();

                foreach (var gd in _gaussianDistributions)
                {
                    prevMeans.Add(new DataElement(gd.Mean));
                }

                em.Compute();

                List<DataElement> currMeans = new List<DataElement>();

                foreach (var gd in _gaussianDistributions)
                {
                    currMeans.Add(new DataElement(gd.Mean));
                }
                List<double> meansDistances = MathHelper.CountMeansDistances(prevMeans, currMeans);

                if (!MathHelper.meansDistancesChanged(threshold, meansDistances))
                    break;

                iterations++;
            }

	        AfterLearning = true;
        }

        /// <summary>
        /// Gets genre represented by this instance
        /// </summary>
        public SongGenre GetGenre()
        {
            return genre;
        }
    }
}