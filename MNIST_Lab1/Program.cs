// Пример на основе https://github.com/microsoft/onnxruntime/tree/master/csharp/sample/Microsoft.ML.OnnxRuntime.ResNet50v2Sample

using System;
using System.Linq;
using System.Collections.Generic;
using NeuralNetwork;

namespace OnnxLab1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Input directory name you want to process: ");
            string dirName = Console.ReadLine();

            MNIST neuralNetwork = new MNIST();
            neuralNetwork.OnProcessedPicture += (s) => Console.WriteLine(s);
            neuralNetwork.OnAllTasksFinished += () => { Environment.Exit(0); };

            try
            {
                neuralNetwork.ScanDirectory(dirName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            while (Console.ReadKey().Key != ConsoleKey.Escape);
            neuralNetwork.Cancel();
        }
    }
}
