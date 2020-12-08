using Contracts;
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
        public object SelectedItem; //Выбранный пользователем класс в правом ListBox для просмотра всех изображений в этом классе

        public List<ObservablePictureType> Items; //Все классы, которые распознает нейронная сеть

        public List<PictureInfo> NonProcessedPictures; 
        public PictureLibrary()
        {
            Items = new List<ObservablePictureType>();
            NonProcessedPictures = new List<PictureInfo>();
            foreach (var p in PictureInfo.classLabels)
            {
                var pictureType = new ObservablePictureType(p);
                (Items as List<ObservablePictureType>).Add(pictureType); 
            }
        }

        public void AddPictureInfo(PictureInfo pictureInfo)
        {
            PictureInfo buf = null;
            if (string.IsNullOrEmpty(pictureInfo.TypeName))
                NonProcessedPictures.Add(pictureInfo);
            else
            {
                foreach (var pic in NonProcessedPictures)
                {
                    if (pic.Data == pictureInfo.Data)
                    {
                        buf = pic;
                        pic.TypeName = pictureInfo.TypeName;
                        Items.Find((p) => p.TypeName == pic.TypeName).Add(pic);
                        break;
                        //type.OnStatisicChanged();
                    }
                }
                if (buf != null)
                    NonProcessedPictures.Remove(buf);
                else
                    NonProcessedPictures.Add(pictureInfo);
            }
        }

        public ObservablePictureType AddStatistic(Transfer transfer)
        {
            var result = Items.Find((s) => s.TypeName == transfer.Name);
                result.Statistic = transfer.DataToBase64;
            return result;
        }

        /// <summary>
        /// Показать картинки в зависимости от выбранного класса
        /// </summary>
        /// <returns></returns>

        public IEnumerable<PictureInfo> GetProcessedImages()
        {
            if (SelectedItem == null)
            {
                foreach (var nonpic in NonProcessedPictures)
                    yield return nonpic;
                foreach (var type in Items)
                    foreach (var picture in type)
                        yield return picture;
            }
            else
                foreach (var picture in SelectedItem as ObservablePictureType)
                    yield return picture;
        }
    }
}
