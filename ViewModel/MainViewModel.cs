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

        private ObservablePictureLibrary observablePictureLibrary;

        public IEnumerable<PictureInfo> ShowedImages { get => observablePictureLibrary.GetProcessedImages(); }

        public IEnumerable<ObservablePictureType> Library { get => observablePictureLibrary; }

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
            observablePictureLibrary = new ObservablePictureLibrary();
            
            neuralNetwork.OnProcessedPicture += (s) =>
            {
                dispatcher.Invoke( () => 
                {
                    if (neuralNetwork.queue.TryDequeue(out PictureInfo result))
                    {
                        processedCount++;
                        OnPropertyChanged("Progress");
                      //try
                      //{
                        observablePictureLibrary.AddPictureInfo(result);
                      //}
                      //catch(Exception ex){ Exception = ex + ex.Message; OnPropertyChanged("Exception"); }
                        OnPropertyChanged("ShowedImages");
                    }
                });              
            };

            neuralNetwork.OnAllTasksFinished += () => { 
                dispatcher.Invoke(() => { canOpen = true; 
                    CommandManager.InvalidateRequerySuggested(); 
                });
            };


            openCommand = new RelayCommand(_ => canOpen,
                _ => {directory = uIServices.ConfirmOpen();
                    foreach (var l in Library)
                        l.Clear();
                    if (directory != null)
                    {
                        canOpen = false; processedCount = 0; OnPropertyChanged("Progress");
                        neuralNetwork.ScanDirectory(directory);
                    }
                }); 

            stopCommand = new RelayCommand(_ => !canOpen, _ => { canOpen = true; neuralNetwork.Cancel(); });
        }
        public void ApplySelection(object selectedItem)
        {
            observablePictureLibrary.SelectedItem = selectedItem;
            OnPropertyChanged("ShowedImages");
        }

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
