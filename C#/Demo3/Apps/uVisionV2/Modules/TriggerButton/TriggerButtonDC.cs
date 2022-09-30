using Common.Controls.PanZoomCanvasView.ModuleContainer;
using Common.Language;
using Common.ObservableProperty;


namespace uVisionV2.Modules.TriggerButton
{
    public class TriggerButtonDC : ModuleContainerDC
    {
        public TriggerButtonDC(LanguageDC languageDC, IObservableCollection<object> modules) : base(modules, languageDC, languageDC.TriggerColon)
        {
            modules.Add(this);
        }

        internal void Button_Click()
        {
            Inputs.Add(new ModuleInput());
        }
    }
}
