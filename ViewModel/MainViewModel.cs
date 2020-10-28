using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
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

        public string Progress { get => (processedCount * 100 / neuralNetwork.Count).ToString() + " %" ; }

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
            
            neuralNetwork.OnProcessedPicture += (s) =>
            {
                dispatcher.Invoke( () => 
                {
                    if (neuralNetwork.queue.TryDequeue(out PictureInfo result))
                    {
                        processedCount++;
                        OnPropertyChanged(nameof(Progress));
                      //try
                      //{
                        pictureLibrary.AddPictureInfo(result);
                      //}
                      //catch(Exception ex){ Exception = ex + ex.Message; OnPropertyChanged("Exception"); }
                        OnPropertyChanged(nameof(ShowedImages));
                      //OnPropertyChanged(nameof(Library));
                    }
                });              
            };

            neuralNetwork.OnAllTasksFinished += () => { 
                dispatcher.Invoke(() => { 
                    canOpen = true; 
                    CommandManager.InvalidateRequerySuggested(); 
                });
            };

            openCommand = new RelayCommand(_ => canOpen,
                _ => {directory = uIServices.ConfirmOpen();
                    if (pictureLibrary != null)
                        foreach (var l in Library)
                            l.Clear();
                    if (directory != null)
                    {
                        canOpen = false; processedCount = 0; OnPropertyChanged(nameof(Progress));
                        var files = MNIST.GetFilesFromDirectory(directory);
                        if (pictureLibrary == null)
                        {
                            pictureLibrary = new PictureLibrary(files);
                            OnPropertyChanged(nameof(Library));
                        }
                        else
                            pictureLibrary.AllPathes = files;
                        neuralNetwork.ScanDirectory(files);
                    }
                }); 

            stopCommand = new RelayCommand(_ => !canOpen, _ => { canOpen = true; neuralNetwork.Cancel(); });
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
    }
}
