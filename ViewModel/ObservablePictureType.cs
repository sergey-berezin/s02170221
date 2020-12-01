using Contracts;
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
        public string Statistic { get; set; }
        public string TypeName { get; set; }

        public ObservablePictureType(string name)
        {
            this.TypeName = name;
            this.Statistic = "0";
        }

        public void OnStatisicChanged()
        {
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Statistic)));
        }
    }
}
