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

        public float Confidence;

        public PictureInfo(string path, string type, float confidence)
        {
            Path = path;
            TypeName = type;
            Confidence = confidence;
        }

        public override string ToString()
        {
            return "Path: " + Path + ", Class: " + TypeName + ", Confidence: " + Confidence + ";";
        }

    }
}
