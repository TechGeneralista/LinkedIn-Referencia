using Common;
using Common.Language;
using Common.NotifyProperty;
using Common.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WinForms = System.Windows.Forms;


namespace UCVisionResultExplorerApp
{
    public class ResultExplorerDC
    {
        public LanguageDC LanguageDC { get; }
        public IReadOnlyProperty<string> SourcePath { get; }
        public IReadOnlyPropertyArray<ResultDataDC> Results { get; } = new PropertyArray<ResultDataDC>();

        public IReadOnlyProperty<Visibility> LoadingVVisibility { get; } = new Property<Visibility>(Visibility.Hidden);
        public IReadOnlyProperty<int> LoadingVMaximum { get; } = new Property<int>();
        public IReadOnlyProperty<int> LoadingVValue { get; } = new Property<int>();
        public IProperty<int> ImageHeight { get; }

        public int[] HourFilterSource { get; }
        public IProperty<DateTime> FilterFromDate { get; } = new Property<DateTime>();
        public IProperty<int> SelectedFilerFromHour { get; } = new Property<int>();
        public IProperty<DateTime> FilterToDate { get; } = new Property<DateTime>();
        public IProperty<int> SelectedFilerToHour { get; } = new Property<int>();
        public IReadOnlyPropertyArray<string> FilterTextOptions { get; } = new PropertyArray<string>();
        public IProperty<string> FilterText { get; } = new Property<string>();


        Task task;
        CancellationTokenSource cancellationTokenSource;
        ResultDataDC[] results;


        public ResultExplorerDC(ISettingsCollection settingsCollection, LanguageDC languageDC)
        {
            LanguageDC = languageDC;
            SourcePath = new StorableProperty<string>((o, n) => RefreshSourceDatas(n), settingsCollection, nameof(SourcePath));
            ImageHeight = new StorableProperty<int>(100, (o, n) => SetImageHeight(n), settingsCollection, nameof(ImageHeight));

            DateTime now = DateTime.Now;
            FilterFromDate.Value = new DateTime(now.Year, now.Month, now.Day);
            FilterToDate.Value = new DateTime(now.Year, now.Month, now.Day).AddDays(1);

            HourFilterSource = Enumerable.Range(0, 24).ToArray();
            SelectedFilerFromHour.Value = HourFilterSource[0];
            SelectedFilerToHour.Value = HourFilterSource[0];
        }

        private void SetImageHeight(int n)
            => Results.ForEach(x => x.SetHeight(n));

        internal void BrowseButtonClick()
        {
            WinForms.FolderBrowserDialog folderBrowserDialog = new WinForms.FolderBrowserDialog();
            folderBrowserDialog.SelectedPath = SourcePath.Value;
            WinForms.DialogResult dialogResult = folderBrowserDialog.ShowDialog();

            if (dialogResult != WinForms.DialogResult.OK)
                return;

            SourcePath.ToSettable().Value = folderBrowserDialog.SelectedPath;
        }

        private void RefreshSourceDatas(string n)
        {
            if (!Directory.Exists(n))
                return;

            if (task.IsNotNull() && task.Status == TaskStatus.Running)
            {
                cancellationTokenSource.Cancel();
                task.Wait();
            }

            cancellationTokenSource = new CancellationTokenSource();
            task = Task.Run(() => ReadMethod(n), cancellationTokenSource.Token);
        }

        private void ReadMethod(string n)
        {
            LoadingVVisibility.ToSettable().Value = Visibility.Visible;

            string[] dirs = Directory.GetDirectories(n);
            LoadingVMaximum.ToSettable().Value = dirs.Length;
            LoadingVValue.ToSettable().Value = 0;
            List<ResultDataDC> resultList = new List<ResultDataDC>();

            foreach (string directoryPath in dirs)
            {
                if (cancellationTokenSource.IsCancellationRequested)
                    break;

                resultList.Add(new ResultDataDC(directoryPath, ImageHeight.Value));
                LoadingVValue.ToSettable().Value = LoadingVValue.Value + 1;
            }

            List<string> filterOptions = new List<string>();
            foreach(ResultDataDC resultDataDC in resultList)
            {
                foreach(ResultDataTextDC resultDataTextDC in resultDataDC.ResultLines)
                {
                    if (!filterOptions.Contains(resultDataTextDC.Text))
                        filterOptions.Add(resultDataTextDC.Text);
                }
            }

            filterOptions.Add("");
            FilterTextOptions.ToSettable().ReAddRange(filterOptions.ToArray());

            LoadingVVisibility.ToSettable().Value = Visibility.Hidden;
            results = resultList.ToArray();

            Results.ToSettable().ReAddRange(results);
        }

        public void ApplyFilter()
        {
            Results.ToSettable().Clear();

            if (results.IsNull())
            {
                MessageBox.Show(LanguageDC.FirstPleaseSelectADataSource.Value, LanguageDC.ErrorColon.Value, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Task.Run(() =>
            {
                foreach (ResultDataDC resultDataDC in results)
                {
                    if (resultDataDC.CheckFilter(FilterFromDate.Value.AddHours(SelectedFilerFromHour.Value), FilterToDate.Value.AddHours(SelectedFilerToHour.Value), FilterText.Value))
                        Results.ToSettable().Add(resultDataDC);
                }
            });
        }
    }
}
