using System;
using System.Collections.Generic;

namespace MusicRecognition
{
	public abstract class AbstractClassification
	{
		protected string Name;
		protected List<DataElement> TrainingData;
        public bool AfterLearning { get; private set; } = false;

        /// <summary>
        /// Gets probability thad specified song belongs to classificator (important in GMM implementation)
        /// </summary>
        /// <returns>Value in range [0, 1]</returns>
		public abstract double GetProbability(DataElement data);

        /// <summary>
        /// Learns classificator. Must be executed before GetClass() method
        /// </summary>
        /// <param name="data">Training set</param>
        public virtual void Learn(List<DataElement> data)
        {
            AfterLearning = true;
        }

        /// <summary>
        /// Gets class of specified song. First there must be executed Learn() method
        /// </summary>
        /// <returns>Genre of the song</returns>
		public virtual SongGenre GetClass(DataElement data)
        {
            if (!AfterLearning)
                throw new NotSupportedException("Could not get class of song before learning phase!");
            return SongGenre.Classical;
        }

        /// <summary>
        /// Gets name of classificator
        /// </summary>
        public abstract string GetName();
    }
}
