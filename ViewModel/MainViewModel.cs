using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Threading;
using System.Threading;
using System.Net.Http;
using System.IO;
using Contracts;
using System.Collections.Concurrent;

namespace ViewModel
{
    public interface IUIServices
    {
        string ConfirmOpen();
        void Warning();
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        Client client;

        private IUIServices uIServices;

        private string directory;

        private PictureLibrary pictureLibrary;

        public IEnumerable<PictureInfo> ShowedImages { get => pictureLibrary?.GetProcessedImages(); }

        public IEnumerable<ObservablePictureType> Library { get => pictureLibrary?.Items; }

        //public string Exception { get; set; }

        private readonly ICommand openCommand;

        private readonly ICommand stopCommand;

        public event PropertyChangedEventHandler PropertyChanged;

        private Dispatcher dispatcher;

        public ICommand OpenCommand { get => openCommand; }
        public ICommand StopCommand { get => stopCommand; }

        ConcurrentQueue<Transfer> queue;
        ConcurrentQueue<Transfer> queueStatistic;

        public MainViewModel(IUIServices uIServices)
        {
            queue = new ConcurrentQueue<Transfer>();
            queueStatistic = new ConcurrentQueue<Transfer>();
            this.uIServices = uIServices;
            dispatcher = Dispatcher.CurrentDispatcher;
            //Exception = "Good";
            client = new Client();
            
            pictureLibrary = new PictureLibrary();
            OnPropertyChanged(nameof(Library));


            client.OnProcessedPicture += (s) => { queue.Enqueue(s);  dispatcher.Invoke(OnProcessedPictureHandler, DispatcherPriority.Background); };
            client.OnGetStatistic += (s) => { queueStatistic.Enqueue(s); dispatcher.BeginInvoke(OnGetStatisticHandler, DispatcherPriority.Background); };
            client.OnServerIsUnreacheble += () => dispatcher.BeginInvoke(() => uIServices.Warning()); 
            client.LoadAllPictures();

            openCommand = new RelayCommand(_ => true,
                _ => {
                    directory = uIServices.ConfirmOpen();
                    foreach (var l in Library)
                            l.Clear();
                    if (directory != null)
                    {
                        var files = GetFilesFromDirectory(directory);
                        client.ScanDirectory(GetFilesFromDirectory(directory));
                    }
                }); 

            stopCommand = new RelayCommand(_ => true, _ => { client.Cancel(); });
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
       
        private void OnProcessedPictureHandler()
        {
            if (queue.TryDequeue(out Transfer result))
            {
                pictureLibrary.AddPictureInfo(new PictureInfo(result));
                OnPropertyChanged(nameof(ShowedImages));
            }
        }

        private void OnGetStatisticHandler()
        {
            if (queueStatistic.TryDequeue(out Transfer result))
            {
                var p = pictureLibrary.AddStatistic(result);//для обновления статистики
                p.OnStatisicChanged();
            }
        }

        public void ApplySelection(object selectedItem)
        {
            pictureLibrary.SelectedItem = selectedItem;
            OnPropertyChanged(nameof(ShowedImages));
        }

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public void ClearDB()
        {
            client.ClearDB();
        }

        public void GetDbStatistic()
        {
            client.GetDbStatistic();
        }
    }
}
