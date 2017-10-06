using System.Collections.Generic;
using System.Linq;
using MusicRecognition.GaussianMixtureModels;

namespace MusicRecognition
{
    public class kMeans
    {
        public int K { get; private set; }
        private readonly string _name;
        private int maxIterations = 200; 

        public kMeans(int k)
        {
            _name = "kMeans";
            K = k;
        }

        private List<DataElement> initializeMeans(List<DataElement> data)
        {
            List<DataElement> initiativeMeans = new List<DataElement>();
            for (int i = 0; i < K; )
            {
                int chosenMeanIndex = RandomGenerator.RandomNumber(0, data.Count);
	            if (!initiativeMeans.Contains(data[chosenMeanIndex]))
	            {
		            initiativeMeans.Add(data[chosenMeanIndex]);
		            i++;
	            }
            }

            return initiativeMeans;
        }

        private int findClosestMean(DataElement data, List<DataElement> means)
        {
            double minDistance = double.MaxValue;
            int bestMeanIndex = -1;
	        int i = 0;
            foreach (var m in means)
            {
                // zwykła odl. miedzy wektorami
                //data.GetDistance(m);
                //double distance = data.NeighbourDistance;
                double distance = MathHelper.EuclideanDistance(data.MeanVector, m.MeanVector);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    bestMeanIndex = i;
                }
	            i++;
            }

            return bestMeanIndex;
        }

        private DataElement findAverageElement(List<DataElement> data)
        {
            DataElement avg = new DataElement();
            if (data.Count < 1)
                return null;
            int dim = data[0].MeanVector.Length;
            avg.CovarianceMatrix = new double[dim, dim];
            avg.MeanVector = new double[dim];

            for (int i = 0; i < dim; i++)
            {
                // average mean vector
                foreach (var d in data)
                    avg.MeanVector[i] += d.MeanVector[i];
                avg.MeanVector[i] /= data.Count;
            }

            return avg;
        }

        private List<DataElement> findNewMeans(List<List<DataElement>> elementsByLabels)
        {
            List<DataElement> means = new List<DataElement>();

            foreach (var group in elementsByLabels)
                means.Add(findAverageElement(group));

            return means;
        }

        private List<Cluster> createClusterElements(List<List<DataElement>> groupedData, List<DataElement> means, double allCount)
        {
            if (means.Count == 0)
                return null;
            List<Cluster> clusters = new List<Cluster>();
            int dim = means[0].MeanVector.Length;
            double[,] cov = new double[dim, dim];
            int i = 0;
            foreach (var group in groupedData)
            {
                // converting list to array
                double[][] groupArr = group.Select(x => x.MeanVector).ToArray();

                // init weights
                double[] weights = new double[group.Count];
                for (int k = 0; k < group.Count; k++)
                    weights[k] = 1;

				cov = MathHelper.GetMeanCovariance(groupArr, means[i].MeanVector);

				clusters.Add(new Cluster(means[i++].MeanVector, cov, group.Count / allCount));
            }

            return clusters;
        }

        /// <summary>
        /// Gets representatives of specified data set
        /// </summary>
        /// <param name="trainingData">data based on which representatives will be searched</param>
        /// <returns>List of representatives</returns>
        public List<Cluster> GetGroupRepresentatives(List<DataElement> trainingData, double threshold)
        {
            List<DataElement> data = new List<DataElement>();
            foreach (var td in trainingData)
                data.Add(new DataElement(td.Label, td.MeanVector, td.CovarianceMatrix));

            while (true)
            {
                List<DataElement> prevMeans;
                List<double> meansDistances;
                List<DataElement> means = initializeMeans(data);
                List<List<DataElement>> groupedElements = null;

                int iterations = 0;

                while (iterations < maxIterations)
                {
                    groupedElements = new List<List<DataElement>>();
                    for (int i = 0; i < K; i++)
                        groupedElements.Add(new List<DataElement>());

				    foreach (var d in data)
                        groupedElements[findClosestMean(d,means)].Add(d);

				    prevMeans = new List<DataElement>(means);
                    means = findNewMeans(groupedElements);

                    if (means.Exists(m => m == null))
                        break;

                    meansDistances = MathHelper.CountMeansDistances(prevMeans, means);

                    if (!MathHelper.meansDistancesChanged(threshold, meansDistances))
                        break;

                    iterations++;

                    bool timeToFinish = true;
                    foreach (var ge in groupedElements)
                        if (ge.Count <= 1)
                        {
                            timeToFinish = false;
                            break;
                        }
                    if (timeToFinish)
                        return createClusterElements(groupedElements, means, data.Count);
                }
            }
        }

        public string GetName()
        {
            return _name;
        }
    }
}