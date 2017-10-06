using MusicRecognition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingMethods
{
    public class CrossValidation
    {
        private int _k;
        List<List<DataElement>> _subsets;

        /// <summary>
        /// Returns training set and testing set
        /// </summary>
        /// <param name="setNumber">Number of subset which should be used as testing set
        /// (the rest is used as testing set) </param>
        /// <returns></returns>
        public DataSets this[int setNumber]
        {
            get
            {
                if (setNumber > _k && setNumber < 0)
                    throw new ArgumentOutOfRangeException($"expected setNumber: {setNumber} but there are {_k} sets");

                var testingSet = _subsets[setNumber];
                List<DataElement> trainingSet = new List<DataElement>();
                for (int i=0; i<_subsets.Count; i++)
                {
                    if (i == setNumber)
                        continue;
                    trainingSet.AddRange(_subsets[i]);
                }
                return new DataSets(trainingSet, testingSet);
            }
        }

        /// <summary>
        /// Constructor of class testing by using cross validation
        /// </summary>
        /// <param name="testedSet">Set to test</param>
        /// <param name="k">Count od subsets on which testedSet will be divided</param>
        public CrossValidation(List<DataElement> testedSet, int k)
        {
            _k = k;
            _subsets = DivideToRandomSubsets(testedSet, k);
        }

        private List<List<DataElement>> DivideToRandomSubsets(List<DataElement> set, int count)
        {
            var subsets = new List<List<DataElement>>();
            set.Shuffle();

            for (int i = 0; i < count; i++)
                subsets.Add(new List<DataElement>());
            for (int i=0; i<set.Count; i++)
            {
                int index = i % count;
                if (subsets[index] == null)
                    subsets[index] = new List<DataElement>();
                subsets[index].Add(set[i]);
            }
            return subsets;
        }
    }

    public class DataSets
    {
        public List<DataElement> TrainingSet { get; private set; }
        public List<DataElement> TestingSet { get; private set; }
        public DataSets(List<DataElement> trainingSet, List<DataElement> testingSet)
        {
            TrainingSet = trainingSet;
            TestingSet = testingSet;
        }
    }
}
