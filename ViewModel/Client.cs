using Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ViewModel
{
    public class Client
    {
        public event Action<Transfer> OnProcessedPicture; //завершение обработки одной картинки
        public event Action<Transfer> OnGetStatistic; //получили значение статистики для класса в transfer
        public event Action OnServerIsUnreacheble;

        private HttpClient httpClient = new HttpClient();

        private CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

        public async void ScanDirectory(IEnumerable<string> files)
        {
            try
            {
                await httpClient.GetAsync("http://localhost:5000/api/Pictures/statistic");
            }
            catch
            {
                OnServerIsUnreacheble();
                return;
            }
            var taskList = new List<Task>();
            int i = 0;
            foreach (string s in files)
            {
                if (File.Exists(s))
                {
                    taskList.Add(Task.Factory.StartNew(async (path) =>
                    {
                        var bytes = File.ReadAllBytes(path as string);
                        var transfer = new Transfer() { DataToBase64 = Convert.ToBase64String(bytes) };
                        OnProcessedPicture(transfer); //добавляем все картинки на экран без подписи класса

                        var getRequest = JsonConvert.SerializeObject(transfer);
                        var c = new StringContent(getRequest);
                        c.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                        var result = await httpClient.PutAsync("http://localhost:5000/api/Pictures", c);
                        transfer.Name = await result.Content.ReadAsStringAsync();
                        OnProcessedPicture(transfer); // подписываем картинкам распознанный сервером класс
                        
                        
                    }, s, cancelTokenSource.Token));
                    i++;
                }

                if (i == 4)
                {
                    i = 0;
                    await Task.WhenAll(taskList);
                }
            }
        }

        public void Cancel()
        {
            //Console.WriteLine("Cancel");
            cancelTokenSource.Cancel();
        }

        public async void ClearDB()
        {
            try
            {
               await httpClient.DeleteAsync("http://localhost:5000/api/Pictures");
            }
            catch
            {
                OnServerIsUnreacheble();
            }
        }

        public async void LoadAllPictures()
        {
            try
            {
                var result = await httpClient.GetStringAsync("http://localhost:5000/api/Pictures");
                foreach (var i in JsonConvert.DeserializeObject<Transfer[]>(result))
                    OnProcessedPicture(i);
            }
            catch
            {
                OnServerIsUnreacheble();
            }
        }

        public async void GetDbStatistic()
        {
            try
            {
                var result = await httpClient.GetStringAsync("http://localhost:5000/api/Pictures/statistic");
                foreach (var i in JsonConvert.DeserializeObject<Transfer[]>(result))
                    OnGetStatistic(i);
            }
            catch
            {
                OnServerIsUnreacheble();
            }
        }
    }
}
