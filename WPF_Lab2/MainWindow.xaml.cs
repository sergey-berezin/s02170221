using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ViewModel;

namespace WPF_Lab2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MyAppUIServices myAppUIServices;
        MainViewModel mainViewModel;
        public MainWindow()
        {
            InitializeComponent();
            myAppUIServices = new MyAppUIServices();
            mainViewModel = new MainViewModel(myAppUIServices);
            DataContext = mainViewModel;
            ListBox_Types.SelectionChanged += (s, e) => mainViewModel.ApplySelection(ListBox_Types.SelectedItem);
        }

        public class MyAppUIServices : IUIServices
        {
            public string ConfirmOpen()
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    return folderBrowserDialog.SelectedPath;
                return null;
            }
        }

        //Не знаю, как правильно снять выделение с ListBox, поэтому реализовала через кнопку
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ListBox_Types.SelectedItem = null;
        }
    }
}
