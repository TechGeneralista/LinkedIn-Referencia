using Common;
using System;


namespace VisualBlocks.Toolbar
{
    internal class ToolbarDC : DCBase
    {
        public event EventHandler New, Open, Save, SaveAs, ApplicationClose, ResetView, Settings;

        internal void NewButtonClick()
            => New?.Invoke(this, EventArgs.Empty);

        internal void OpenButtonClick()
            => Open?.Invoke(this, EventArgs.Empty);

        internal void SaveButtonClick()
            => Save?.Invoke(this, EventArgs.Empty);

        internal void SaveAsButtonClick()
            => SaveAs?.Invoke(this, EventArgs.Empty);

        internal void CloseButtonClick()
            => ApplicationClose?.Invoke(this, EventArgs.Empty);

        internal void ResetViewButtonClick()
            => ResetView?.Invoke(this, EventArgs.Empty);

        internal void SettingsButtonClick()
            => Settings?.Invoke(this, EventArgs.Empty);
    }
}
