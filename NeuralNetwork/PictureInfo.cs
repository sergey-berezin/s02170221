using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    public class PictureInfo
    {
        public string Path { get; set; }

        public string TypeName { get; set; }

        public PictureInfo(string path, string type)
        {
            Path = path;
            TypeName = type;
        }

        public override string ToString()
        {
            return "Path: " + Path + ", Class: " + TypeName + ";";
        }

    }
}
