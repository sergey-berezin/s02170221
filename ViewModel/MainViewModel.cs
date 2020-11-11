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
using NeuralNetwork;

namespace ViewModel
{
    public interface IUIServices
    {
        string ConfirmOpen();
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        MNIST neuralNetwork;

        private IUIServices uIServices;

        private string directory;

        private bool canOpen;

        private PictureLibrary pictureLibrary;

        public IEnumerable<PictureInfo> ShowedImages { get => pictureLibrary?.GetProcessedImages(); }

        public IEnumerable<ObservablePictureType> Library { get => pictureLibrary?.Items; }

        private int processedCount;

        private int pictureCount = 1;
        public string Progress { get => (processedCount * 100 / pictureCount) + " %" ; }

        //public string Exception { get; set; }

        private readonly ICommand openCommand;

        private readonly ICommand stopCommand;

        public event PropertyChangedEventHandler PropertyChanged;

        private Dispatcher dispatcher;

        public ICommand OpenCommand { get => openCommand; }
        public ICommand StopCommand { get => stopCommand; }


        public MainViewModel(IUIServices uIServices)
        {
            this.uIServices = uIServices;
            dispatcher = Dispatcher.CurrentDispatcher;
            //Exception = "Good";
            neuralNetwork = new MNIST();
            canOpen = true;

            pictureLibrary = new PictureLibrary(null);
            OnPropertyChanged(nameof(Library));

            neuralNetwork.OnProcessedPicture += (s) => dispatcher.Invoke(OnProcessedPictureHandler);              

            neuralNetwork.OnAllTasksFinished += () => { 
                dispatcher.Invoke(() => { 
                    canOpen = true; 
                    CommandManager.InvalidateRequerySuggested(); 
                });
            };

            openCommand = new RelayCommand(_ => canOpen,
                _ => {
                    directory = uIServices.ConfirmOpen();
                    foreach (var l in Library)
                            l.Clear();
                    if (directory != null)
                    {
                        canOpen = false; processedCount = 0; OnPropertyChanged(nameof(Progress));
                        var files = MNIST.GetFilesFromDirectory(directory);
                        
                        pictureLibrary.AllPathes = files;

                        pictureLibrary.PictureLibraryContext.UnknownPictures.Clear();
                        pictureCount = pictureLibrary.AllPathes.Count();
                        foreach (var f in files)
                        {
                            var info = pictureLibrary.PictureLibraryContext.FindPicture(f);
                            if (info != null)
                                neuralNetwork.FakeProcessedPicture(info);
                        }
                        neuralNetwork.ScanDirectory(pictureLibrary.PictureLibraryContext.UnknownPictures);
                    }
                }); 

            stopCommand = new RelayCommand(_ => !canOpen, _ => { canOpen = true; neuralNetwork.Cancel(); });
        }

        private void OnProcessedPictureHandler()
        {
            if (neuralNetwork.queue.TryDequeue(out PictureInfo result))
            {
                processedCount++;
                OnPropertyChanged(nameof(Progress));
                pictureLibrary.AddPictureInfo(result);
                OnPropertyChanged(nameof(ShowedImages));
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
            pictureLibrary.PictureLibraryContext.ClearDB();
            foreach (var p in pictureLibrary.Items) //для обновления статистики
                p.OnStatisicChanged();
        }
    }
}
