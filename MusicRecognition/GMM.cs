using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicRecognition
{
    public class GMM : AbstractClassification
    {
	    private DataElement[] _gaussianDistributions;

	    public GMM()
	    {
		    Name = "GMM";
			kMeans _kMeans = new kMeans(Enum.GetNames(typeof(SongGenre)).Length);
		    //_gaussianDistributions = _kMeans.GetGroupRepresentatives(TrainingData);
	    }

		public override double? GetProbability(DataElement data)
	    {
		    return data.Probability;
	    }

	    public override void Reset()
	    {
		    // TODO:: ustawienie domyślnych parametrów (na potrzeby cross validation)
	    }

	    public override void Learn(List<DataElement> data, Action<int> reportProgress)
	    {
		    throw new NotImplementedException();
			// TODO:: uczenie algorytmem GMM
	    }

	    public override SongGenre GetClass(DataElement data)
	    {
		    throw new NotImplementedException();
			//TODO:: wyznaczenie gatunku testowanego utworu
			// TODO:: ustawienie data.Probability = <prawdopodobieństwo_przynależenia_do_znalezionego_gatunku>
		}

		public override string GetName()
	    {
		    return Name;
	    }
    }
}
