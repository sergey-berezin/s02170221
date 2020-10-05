// Пример на основе https://github.com/microsoft/onnxruntime/tree/master/csharp/sample/Microsoft.ML.OnnxRuntime.ResNet50v2Sample

using System;
using SixLabors.ImageSharp; // Из одноимённого пакета NuGet
using SixLabors.ImageSharp.PixelFormats;
using System.Linq;
using SixLabors.ImageSharp.Processing;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.OnnxRuntime;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Concurrent;
using System.Threading;

namespace NeuralNetwork
{
    public class MNIST
    {
        InferenceSession session;
        public List<Task> taskList;
        CancellationTokenSource cancelTokenSource;
        public event Action OnAllTasksFinished; //завершение обработки всех картинок
        public event Action<string> OnProcessedPicture; //завершение обработки одной картинки

        public MNIST()
        {            
            session = new InferenceSession("model.onnx");// Создает сессию для предсказания нейросетью
            taskList = new List<Task>();
        }
        public void ScanDirectory(string dirName)
        {
            if (Directory.Exists(dirName))
            {

                cancelTokenSource = new CancellationTokenSource();

                string[] files = Directory.GetFiles(dirName);
                foreach (string s in files)
                {
                    if (s.Contains(".jpg") || s.Contains(".jpeg") || s.Contains(".png") || s.Contains(".bmp"))
                    {
                        taskList.Add(Task.Factory.StartNew(() =>
                        {
                            OnProcessedPicture(ProcessPicture(s));
                        }, cancelTokenSource.Token));
                        
                    }
                }

                Task.Run(() => {
                    Task.WaitAll(taskList.ToArray());
                    OnAllTasksFinished();
                    });
            }
            else throw new Exception("Directory doesn't exist");
        }
        public void Cancel()
        {
            Console.WriteLine("Cancel");
            cancelTokenSource.Cancel();
        }
        string ProcessPicture(string pictureName)
        {
            using var image = Image.Load<Rgb24>(pictureName);

            const int TargetWidth = 28;
            const int TargetHeight = 28;

            // Изменяем размер картинки до 28 x 28
            image.Mutate(x =>
            {
                x.Resize(new ResizeOptions
                {
                    Size = new Size(TargetWidth, TargetHeight),
                    Mode = ResizeMode.Crop // Сохраняем пропорции обрезая лишнее
                });
            });

            // Перевод пикселов в тензор и нормализация
            var input = new DenseTensor<float>(new[] { 1, 1, TargetHeight, TargetWidth });

            // Перевод картинки в ЧБ
            for (int y = 0; y < TargetHeight; y++)
            {
                Span<Rgb24> pixelSpan = image.GetPixelRowSpan(y);
                for (int x = 0; x < TargetWidth; x++)
                    input[0, 0, y, x] = (pixelSpan[x].R + pixelSpan[x].G + pixelSpan[x].B) / 3f / 255f; // - mean[0]) / stddev[0];
            }

            // Подготавливаем входные данные нейросети. Имя input задано в файле модели
            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor(session.InputMetadata.Keys.First(), input)
            };

            using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = session.Run(inputs);

            ////Утяжеление задачи, чтобы проверить, как выполняются таски на ядрах
            //int k = 1;
            //for (int i = 1; i < 100000000; i++)
            //    k = k * i;

            // Получаем 1000 выходов и считаем для них softmax
            var output = results.First().AsEnumerable<float>().ToArray();

            // Формула softmax из Википедии
            var sum = output.Sum(x => (float)Math.Exp(x));
            var softmax = output.Select(x => (float)Math.Exp(x) / sum);

            // Выдаем наиболее вероятный результат на экран
            return softmax
                .Select((x, i) => new { Label = classLabels[i], Confidence = x })
                .OrderByDescending(x => x.Confidence)
                .FirstOrDefault().Label;           
        }

        static readonly string[] classLabels = new[]
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
