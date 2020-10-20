using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    public class PictureInfo
    {
        public string Path;

        public string TypeName;

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
