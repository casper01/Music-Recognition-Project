using MusicRecognition;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingMethods
{
    public class ConfusionMatrixValidation
    {
        private List<DataElement> _dataSet;
        private AbstractClassification _classificator;
        private List<DataElement>[,,] _confusionMatrix;
        /// <summary>
        /// CrossValidation sets count
        /// </summary>
        private int cvCount;
        private CrossValidation crossValidator;
        public int SongsCount => Enum.GetNames(typeof(SongGenre)).Length;
        public int TestCount => _dataSet.Count;
        public int CorrectCount
        {
            get
            {
                int sum = 0;
                for (int i=0; i<cvCount; i++)
                    foreach (var el in diagonalValues(i))
                        if (el == null)
                            return 0;
                        else
                            sum += el.Count;
                return sum;
            }
        }
        public int ErrorCount => _dataSet.Count - CorrectCount;
        public double AccuracyRate => (double)CorrectCount / (double)TestCount;
        public double ErrorRate => (double)ErrorCount / (double)TestCount;
        public readonly string ComputationTime;
        private List<DataElement> this[int cvTest, SongGenre source, SongGenre dest]
        {
            get
            {
                int src = (int)source;
                int dst = (int)dest;
                if (_confusionMatrix[cvTest, src, dst] == null)
                    _confusionMatrix[cvTest, src, dst] = new List<DataElement>();
                return _confusionMatrix[cvTest, src, dst];
            }
        }
        /// <summary>
        /// Gets count of songs of all crossvalidation tests 
        /// </summary>
        /// <param name="source">Real genre of song</param>
        /// <param name="dest">Computed genre of song</param>
        /// <returns></returns>
        public int this[SongGenre source, SongGenre dest]
        {
            get
            {
                int sum = 0;
                for (int i = 0; i < cvCount; i++)
                    sum += this[i, source, dest].Count;
                return sum;
            }
        }

        public ConfusionMatrixValidation(AbstractClassification classificator, List<DataElement> dataSet, int crossValidationSetsCount)
        {
            // start timer
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            _dataSet = dataSet;
            _classificator = classificator;
            cvCount = crossValidationSetsCount;
            crossValidator = new CrossValidation(dataSet, cvCount);
            _confusionMatrix = new List<DataElement>[cvCount, SongsCount, SongsCount];
            ComputeMatrix();

            // save measured time
            stopwatch.Stop();
            ComputationTime = stopwatch.Elapsed.TotalMilliseconds.ToString();
        }
        /// <summary>
        /// Gets diagonal values (songs properly identified) of confussion matrix
        /// </summary>
        /// <param name="cvTest">Number of crossvalidation test</param>
        /// <returns></returns>
        private IEnumerable<List<DataElement>> diagonalValues(int cvTest)
        {
            for (int i = 0; i < SongsCount; i++)
                yield return _confusionMatrix[cvTest, i, i];
        }
        private void ComputeMatrix()
        {
            for (int test=0; test<cvCount; test++)
            {
                var dataSet = crossValidator[test];
                _classificator.Learn(dataSet.TrainingSet);
                for (int i=0; i<dataSet.TestingSet.Count; i++)
                {
                    var data = dataSet.TestingSet[i];
                    SongGenre outGenre = _classificator.GetClass(data);
                    this[test, data.Label, outGenre].Add(data);
                }
            }
        }

        /// <summary>
        /// Counts sum of fun(SongGenre song, int cvTets) results for each cvTest
        /// </summary>
        /// <returns>Sum of fun for each cvTest: [0, cvCount]</returns>
        private int CountSumForEachCvTest(Func<SongGenre, int, int> fun, SongGenre song)
        {
            int sum = 0;
            for (int i = 0; i < cvCount; i++)
                sum += fun(song, i);
            return sum;
        }

        /// <summary>
        /// True Positive
        /// </summary>
        public int TP(SongGenre song)
        {
            return CountSumForEachCvTest(TP, song);
        }
        private int TP(SongGenre song, int cvTest)
        {
            return this[cvTest, song, song].Count;
        }
        /// <summary>
        /// False Positive
        /// </summary>
        public int FP(SongGenre song)
        {
            return CountSumForEachCvTest(FP, song);
        }
        private int FP(SongGenre song, int cvTest)
        {
            int songInd = (int)song;
            int sum = 0;
            for (int i = 0; i < SongsCount; i++)
                if (i != songInd)
                    sum += this[cvTest, (SongGenre)i, song].Count;
            return sum;
        }
        /// <summary>
        /// False Negative
        /// </summary>
        public int FN(SongGenre song)
        {
            return CountSumForEachCvTest(FN, song);
        }
        private int FN(SongGenre song, int cvTest)
        {
            int songInd = (int)song;
            int sum = 0;
            for (int i = 0; i < SongsCount; i++)
                if (i != songInd)
                    sum += this[cvTest, song, (SongGenre)i].Count;
            return sum;
        }
        /// <summary>
        /// True Negative
        /// </summary>
        public int TN(SongGenre song)
        {
            return CountSumForEachCvTest(TN, song);
        }
        private int TN(SongGenre song, int cvTest)
        {
            return CorrectCount - this[cvTest, song, song].Count;
        }
        public double TPRate(SongGenre song)
        {
            return ((double)TP(song)) / ((double)(TP(song) + FN(song)));
        }
        public double FPRate(SongGenre song)
        {
            return ((double)FP(song)) / ((double)(FP(song) + TN(song)));
        }
        public double Precision(SongGenre song)
        {
            return ((double)TP(song)) / ((double)(TP(song) + FP(song)));
        }
        public double Recall(SongGenre song)
        {
            return ((double)TP(song)) / ((double)(TP(song) + FN(song)));
        }
    }
}
