// Пример на основе https://github.com/microsoft/onnxruntime/tree/master/csharp/sample/Microsoft.ML.OnnxRuntime.ResNet50v2Sample

using System;
using System.Linq;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.OnnxRuntime;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Concurrent;
using System.Threading;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace NeuralNetwork
{
    public class MNIST
    {
        InferenceSession session;
        public List<Task> taskList;
        public int Count { get {
                if (taskList == null) return 1; else return taskList.Count; } }

        CancellationTokenSource cancelTokenSource;
        public event Action OnAllTasksFinished; //завершение обработки всех картинок
        public event Action<PictureInfo> OnProcessedPicture; //завершение обработки одной картинки
        public ConcurrentQueue<PictureInfo> queue;
        public MNIST()
        {
            session = new InferenceSession("model.onnx");// Создает сессию для предсказания нейросетью
            queue = new ConcurrentQueue<PictureInfo>();
        }

        public Task ScanDirectory(IEnumerable<string> files) //сюда попадут только необработанные картинки
        {
            
            taskList = new List<Task>();
            
            cancelTokenSource = new CancellationTokenSource();

            foreach (string s in files)
            {
                 taskList.Add(Task.Factory.StartNew( (path) =>
                    {
                         var job = ProcessPicture((string)path);
                         queue.Enqueue(job);
                         OnProcessedPicture?.Invoke(job);
                    }, s, cancelTokenSource.Token));
            }

            return Task.Run(() =>
                {
                    if (taskList.Count != 0)
                        Task.WaitAll(taskList.ToArray());
                    OnAllTasksFinished();
                });          
        }

        public void Cancel()
        {
            //Console.WriteLine("Cancel");
            cancelTokenSource.Cancel();
        }

        public void FakeProcessedPicture(PictureInfo pictureInfo)
        {
            queue.Enqueue(pictureInfo);
            OnProcessedPicture(pictureInfo);
        }

        PictureInfo ProcessPicture(string pictureName)
        {
            var image = Image.FromFile(pictureName);

            const int TargetWidth = 28;
            const int TargetHeight = 28;

            // Изменяем размер картинки до 28 x 28
            var bitmap = ResizeImage(image, TargetWidth, TargetHeight);

            // Перевод пикселов в тензор и нормализация
            DenseTensor<float> input = new DenseTensor<float>(new[] { 1, 1, TargetHeight, TargetWidth });
            
            // Перевод картинки в ЧБ
            for (int y = 0; y < TargetHeight; y++)
            {
                for (int x = 0; x < TargetWidth; x++)
                {
                    var color = bitmap.GetPixel(x, y);
                    input[0, 0, y, x] = (color.R + color.G + color.B) / 3f / 255f;
                }
            }

            // Подготавливаем входные данные нейросети. Имя input задано в файле модели
            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor(session.InputMetadata.Keys.First(), input)
            };

            IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = session.Run(inputs);

            ////Утяжеление задачи, чтобы проверить, как выполняются таски на ядрах
            int k = 1;
            for (int i = 1; i < 100000000; i++)
                k = k * i;

            // Получаем 1000 выходов и считаем для них softmax
            var output = results.First().AsEnumerable<float>().ToArray();

            // Формула softmax из Википедии
            var sum = output.Sum(x => (float)Math.Exp(x));
            var softmax = output.Select(x => (float)Math.Exp(x) / sum);

            // Выдаем наиболее вероятный результат на экран
            var result = softmax
                .Select((x, i) => new { Label = classLabels[i], Confidence = x })
                .OrderByDescending(x => x.Confidence)
                .FirstOrDefault();

            return new PictureInfo(pictureName, result.Label);
            
        }

        public static IEnumerable<string> GetFilesFromDirectory(string dirName)
        {
            if (Directory.Exists(dirName))
            {
                foreach (var f in Directory.GetFiles(dirName))
                    yield return f;
            }
            else 
                yield return null;
        }
        Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
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
