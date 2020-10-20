using NeuralNetwork;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class ObservablePictureLibrary : ObservableCollection<ObservablePictureType>
    {
        public string args;
        public ObservablePictureLibrary()
        {
            foreach (var p in MNIST.classLabels)
            {
                var pictureType = new ObservablePictureType(p);
                base.Add(pictureType);
                ////когда изменяется любой объект PictureType (т.е. когда добавляется новая обработанная картинка PictureInfo):
                //pictureType.CollectionChanged += (s, e) =>
                //{
                //    args = "e.Action: " + e.Action + "\ne.NewItems: " + e.NewItems + "\ne.OldItems: " + e.OldItems + "\ne.NewStartingIndex: " + e.NewStartingIndex + "\ne.OldStartingIndex: " + e.OldStartingIndex;
                //    //var arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, s as ObservablePictureType, base.Items.IndexOf(s as ObservablePictureType));
                //    //OnCollectionChanged(e);
                //};
            }
        }

        public void AddPictureInfo(PictureInfo pictureInfo)
        {
            foreach (var type in base.Items)
            {
                if (type.TypeName == pictureInfo.TypeName)
                {
                    type.Add(pictureInfo);
                    break;
                }
            }
        }
    }
}
