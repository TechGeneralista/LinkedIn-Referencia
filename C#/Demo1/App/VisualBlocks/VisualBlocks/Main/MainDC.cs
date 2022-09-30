using Common;
using Compute;
using System;
using VisualBlocks.Project;
using VisualBlocks.Settings;
using VisualBlocks.Toolbar;


namespace VisualBlocks.Main
{
    class MainDC : DCBase
    {
        public WindowTitleDC WindowTitleDC { get; }
        public SettingsDC SettingsDC { get; }
        public ToolbarDC ToolbarDC { get; }
        public ProjectDC ProjectDC { get; }


        readonly MainV mainWindow;


        public MainDC(string version, ComputeAccelerator computeAccelerator, MainV mainWindow)
        {
            this.mainWindow = mainWindow;
            mainWindow.Closing += (s, e) => e.Cancel = ProjectDC.Shutdown();

            WindowTitleDC = new WindowTitleDC("Amatic Visual Blocks", version);
            SettingsDC = new SettingsDC();
            ToolbarDC = new ToolbarDC();
            ProjectDC = new ProjectDC(computeAccelerator, mainWindow);

            WindowTitleDC.UpdateTextTitle();
            ProjectDC.ProjectChanged += (s, e) => WindowTitleDC.UpdateTextTitle(e.IsModified, e.FilePath);

            ToolbarDC.New += (s, e) => ProjectDC.New();
            ToolbarDC.Open += (s, e) => ProjectDC.Open();
            ToolbarDC.Save += (s, e) => ProjectDC.Save();
            ToolbarDC.SaveAs += (s, e) => ProjectDC.SaveAs();
            ToolbarDC.ApplicationClose += ApplicationClose;
            ToolbarDC.ResetView += (s, e) => ProjectDC.VisualEditorDC.ResetView();
        }

        private void ApplicationClose(object sender, EventArgs e)
            => mainWindow.Close();
    }
}
