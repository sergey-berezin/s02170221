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

        public string ImageClassification;

        public float Confidence;

        public PictureInfo(string path, string imageClassification, float confidence)
        {
            Path = path;
            ImageClassification = imageClassification;
            Confidence = confidence;
        }

        public override string ToString()
        {
            return "Path: " + Path + ", Class: " + ImageClassification + ", Confidence: " + Confidence + ";";
        }
    }
}
