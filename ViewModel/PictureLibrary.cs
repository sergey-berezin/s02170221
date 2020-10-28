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
    public class PictureLibrary
    {
        public object SelectedItem;

        public IEnumerable<ObservablePictureType> Items;

        public IEnumerable<string> AllPathes;
        public PictureLibrary(IEnumerable<string> allPathes)
        {
            AllPathes = allPathes;
            Items = new List<ObservablePictureType>();
            foreach (var p in MNIST.classLabels)
            {
                var pictureType = new ObservablePictureType(p);
                (Items as List<ObservablePictureType>).Add(pictureType); 
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
            foreach (var type in Items)
            {
                if (type.TypeName == pictureInfo.TypeName)
                {
                    type.Add(pictureInfo);
                    break;
                }
            }
        }

        public IEnumerable<PictureInfo> GetProcessedImages()
        {
            if (SelectedItem == null && AllPathes != null)
            {
                bool flag = false;
                foreach (var path in AllPathes)
                {
                    flag = false;
                    foreach (var type in Items)
                    {
                        foreach (var picture in type)
                            if (path == picture.Path)
                            {
                                yield return picture;
                                flag = true;
                                break;
                            }
                        if (flag)
                            break;
                    }
                    if (!flag)
                        yield return new PictureInfo(path, "", 0);
                }
            }
            else if (AllPathes != null)
                foreach (var picture in SelectedItem as ObservablePictureType)
                    yield return picture;

        }
    }
}
