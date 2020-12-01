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
using Contracts;

namespace NeuralNetwork
{
    public class MNIST
    {
        InferenceSession session = new InferenceSession("model.onnx");// Создает сессию для предсказания нейросетью;
        public string ProcessPicture(Bitmap bitmap)
        {
            const int TargetWidth = 28;
            const int TargetHeight = 28;

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
                .Select((x, i) => new { Label = PictureInfo.classLabels[i], Confidence = x })
                .OrderByDescending(x => x.Confidence)
                .FirstOrDefault();

            return result.Label;
            
        }
    }
}
