using NeuralNetwork;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class ObservablePictureType : ObservableCollection<PictureInfo>
    {
        public string TypeName { get; set; }
        
        public ObservablePictureType(string name)
        {
            this.TypeName = name;
        }
    }
}
