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
    /// Логика взаимодействия для ShallowTableWindow.xaml
    /// </summary>
    public partial class ShallowTableWindow : Window
    {
        /// <summary>
        /// This method shows shallow accounts of the table in separate window
        /// </summary>
        /// <param name="selectedFile">string containing the name of the selected file</param>
        public ShallowTableWindow(string selectedFile)
        {
            InitializeComponent();
            this.selectedFile = selectedFile;
            XlsParser xlsParser = new XlsParser();
            DbOperator dbOperator = new DbOperator();
            DataGridShallow.DataContext = dbOperator.ConvertToShallowAccount(xlsParser.ParseFile(selectedFile));
        }

        private string selectedFile;
    }
}
