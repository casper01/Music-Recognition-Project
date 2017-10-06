using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicRecognition
{
	public class kNN : AbstractClassification
	{
        /// <summary>
        /// k parameter value of kNN algorithm
        /// </summary>
		public int K { get; set; }

        /// <summary>
        /// Creates new instance of kNN algorithm
        /// </summary>
        /// <param name="k">k parameter of the algorithm</param>
		public kNN(int k)
		{
			Name = "kNN";
			K = k;
		}

		public override double GetProbability(DataElement data)
		{
			GetClass(data);
			return data.Probability;
		}

		public override void Learn(List<DataElement> trainingData)
		{
            base.Learn(trainingData);
            TrainingData = trainingData;
            for (int i = 0; i < trainingData.Count; i++)
            {
                if (!trainingData[i].HasFeaturesExtracted())
                    trainingData[i].GetFeatures();
            }
		}

		public override SongGenre GetClass(DataElement testedData)
		{
            base.GetClass(testedData);
            if (!testedData.HasFeaturesExtracted())
                testedData.GetFeatures();

			for(int i=0; i<TrainingData.Count; i++)
				TrainingData[i].GetDistance(testedData); 
			TrainingData.Sort( (data1,data2)=>data1.NeighbourDistance.CompareTo(data2.NeighbourDistance) );

			List<int> labelHistogram = new int[Enum.GetNames(typeof (SongGenre)).Length].ToList();
			List<DataElement> nearestNeighbours = TrainingData.GetRange(0, K);
			foreach(DataElement neighbour in nearestNeighbours)
			{
				SongGenre genre = neighbour.GetClass();
				labelHistogram[(int) genre]++;
			}

			int max = labelHistogram.Max();
			List<int> nearestGenre = new List<int>();
			for (int i = 0; i < labelHistogram.Count; i++)
			{
				if(labelHistogram[i] == max)
					nearestGenre.Add(i);
			}
			nearestNeighbours.RemoveAll(data => !nearestGenre.Contains((int) data.GetClass()));
			Random r = new Random();
			testedData.Probability = 1.0;
			return nearestNeighbours[r.Next(nearestNeighbours.Count)].GetClass();
		}

		public override string GetName()
		{
			return Name;
		}
	}
}
