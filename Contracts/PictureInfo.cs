using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Contracts
{
    public class PictureInfo
    {
        public string Data { get; set; }

        public string TypeName { get; set; }

        public BitmapSource BitmapSource { get; set; }

        public PictureInfo(Transfer transfer)
        {
            TypeName = transfer.Name;
            //BitmapSource = Utils.ConvertToBitmapSource(Utils.BitmapFromBytes(Convert.FromBase64String(transfer.DataToBase64)));
            BitmapSource = Utils.LoadImage(Convert.FromBase64String(transfer.DataToBase64));
            Data = transfer.DataToBase64;
        }
        
        public static readonly string[] classLabels = new[]
{
            "Zero",
            "One",
            "Two",
            "Three",
            "Four",
            "Five",
            "Six",
            "Seven",
            "Eight",
            "Nine"
        };
    }
}
