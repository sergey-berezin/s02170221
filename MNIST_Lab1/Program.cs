// Пример на основе https://github.com/microsoft/onnxruntime/tree/master/csharp/sample/Microsoft.ML.OnnxRuntime.ResNet50v2Sample

using System;
using SixLabors.ImageSharp; // Из одноимённого пакета NuGet
using SixLabors.ImageSharp.PixelFormats;
using System.Linq;
using SixLabors.ImageSharp.Processing;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.OnnxRuntime;
using System.Collections.Generic;
using NeuralNetwork;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace OnnxLab1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Input directory name you want to process: ");
            string dirName = Console.ReadLine();

            MNIST neuralNetwork = new MNIST();
            neuralNetwork.OnProcessedPicture += (s) => Console.WriteLine(s);
            Console.CancelKeyPress += (s, e) =>
            {
                neuralNetwork.Cancel();
                e.Cancel = false;
            };

            try
            {
                neuralNetwork.ScanDirectory(dirName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            await Task.WhenAll(neuralNetwork.taskList);
        }
    }
}
