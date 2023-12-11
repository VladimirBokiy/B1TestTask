using Microsoft.Win32;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.IO;
using System.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Task2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Collection with sotred files
        /// </summary>
        public ObservableCollection<string> Files { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            xlsParser = new XlsParser();
            dbOperator = new DbOperator();
            Files = new ObservableCollection<string>();
            
            //dbOperator.ImportTable("..\\..\\data\\data.xls");

            foreach(var file in dbOperator.GetLoadedFiles())
            {
                Files.Add(file);
            }

            LoadedFilesListBox.ItemsSource = Files;

        }

        /// <summary>
        /// Xlsparser object
        /// </summary>
        private XlsParser xlsParser { get; set; }   

        /// <summary>
        /// BbOperator object
        /// </summary>
        private DbOperator dbOperator { get; set; }

        /// <summary>
        /// This method opens separate windows with table representations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadedFilesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedFile = LoadedFilesListBox.SelectedItem as string;

            ShallowTableWindow shallowTableWindow = new ShallowTableWindow(selectedFile);
            shallowTableWindow.Show(); 
            GeneralTableWindow generalTableWindow = new GeneralTableWindow(selectedFile);
            generalTableWindow.Show(); 
        }

        /// <summary>
        /// This method stores choosen files in database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChoose_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string filePath = string.Empty;
            string fileExt = string.Empty;
            OpenFileDialog file = new OpenFileDialog(); //open dialog to choose file
            if (file.ShowDialog() == true) //if there is a file chosen by the user
            {
                filePath = file.FileName; //get the path of the file
                fileExt = Path.GetExtension(filePath); //get the file extension
                if (fileExt.CompareTo(".xls") == 0 || fileExt.CompareTo(".xlsx") == 0)
                {
                    try
                    {
                        dbOperator.ImportTable(filePath);
                        Files.Add(filePath);
                    }
                    catch (Exception ex) 
                    {
                        MessageBox.Show("Файл уже загружен или содержит неверную структуру.");
                    }
                }
                else
                {
                    MessageBox.Show("Расширение файла может быть только .xls или .xlsx"); 
                }
            }
        }
    }
}