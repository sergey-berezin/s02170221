using NeuralNetwork;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class ObservablePictureType : ObservableCollection<PictureInfo>
    {
        public string TypeName { get; set; }

        public int Statistic { get => pictureLibraryContext.GetStatisticType(TypeName); }

        private PictureLibraryContext pictureLibraryContext;

        public ObservablePictureType(string name, PictureLibraryContext pictureLibraryContext)
        {
            this.TypeName = name;
            this.pictureLibraryContext = pictureLibraryContext;
        }
        public void OnStatisicChanged()
        {
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Statistic)));
        }
    }
}
