using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Task2
{
    /// <summary>
    /// Логика взаимодействия для GeneralTableWindow.xaml
    /// </summary>
    public partial class GeneralTableWindow : Window
    {
        public GeneralTableWindow(string selectedFile)
        {
            InitializeComponent();
            this.selectedFile = selectedFile;
            XlsParser xlsParser = new XlsParser();
            DbOperator dbOperator = new DbOperator();
            DataGridGeneral.DataContext = dbOperator.ConvertToGeneralAccount(xlsParser.ParseFile(selectedFile));
        }

        private string selectedFile;
    }
}
