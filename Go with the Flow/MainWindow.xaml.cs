/********************************************
* Author: Yao Zhou
 * *****************************************/

using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace Go_with_the_Flow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly VM vm;
        public MainWindow()
        {
            InitializeComponent();
            vm = VM.Instance;
            DataContext = vm;

            Title = "Go with the Flow";
        }

        private void ImportFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text file (*.txt)|*.txt|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                vm.Input = openFileDialog.FileName;
                try
                {
                    vm.Input = File.ReadAllText(openFileDialog.FileName);
                }
                catch (IOException ex)
                {
                    MessageBox.Show("The file could not be read: " + ex);
                }
            }
            if (vm.Input != "")
            {
                vm.OpenFile();
                vm.RiverCheck();
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            vm.Paragraph = "";
            vm.NumWords = 0;
            vm.LineWidth = 0;
            vm.RiverLength = 0;
        }
    }
}
