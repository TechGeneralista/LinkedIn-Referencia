using Common.NotifyProperty;
using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Windows;


namespace FileToBase64Converter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IProperty<string> FileName { get; } = new Property<string>();
        public IProperty<string> Base64 { get; } = new Property<string>();
        public IProperty<int> BinSize { get; } = new Property<int>();
        public IProperty<int> Base64Size { get; } = new Property<int>();


        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void SelectFileButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog(this) == true)
            {
                FileName.Value = Path.GetFileName(openFileDialog.FileName);
                byte[] binData = File.ReadAllBytes(openFileDialog.FileName);
                BinSize.Value = binData.Length;

                Base64.Value = string.Format(
                    "declare @fName as varchar(50) = '{0}'\n" +
                    "declare @fBase64 as varchar(max) = '{1}'\n" +
                    "\n" +
                    "update [File]\n" +
                    "set fBase64 = @fBase64\n" +
                    "where fName = @fName and fBase64 <> @fBase64\n",
                    FileName.Value, Convert.ToBase64String(binData));

                //Base64.Value = Convert.ToBase64String(binData);
                Base64Size.Value = Encoding.UTF8.GetByteCount(Base64.Value);
            }
        }
    }
}
