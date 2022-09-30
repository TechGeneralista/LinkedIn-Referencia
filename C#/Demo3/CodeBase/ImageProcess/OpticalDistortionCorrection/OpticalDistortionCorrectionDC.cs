using Common.NotifyProperty;
using Common.Settings;
using ImageProcess.Templates;
using Common.Language;
using OpenCLWrapper;
using OpenCLWrapper.Buffer;


namespace ImageProcess.OpticalDistortionCorrection
{
    public class OpticalDistortionCorrectionDC : ICanSaveLoadSettings
    {
        public LanguageDC LanguageDC { get; }
        public float StrengthMaximum { get; } = 10;
        public IProperty<float> Strength { get; } = new Property<float>((float)0);
        public float StrengthMinimum { get; } = 0;
        public float ZoomMaximum { get; } = 4;
        public IProperty<float> Zoom { get; } = new Property<float>((float)1);
        public float ZoomMinimum { get; } = (float).1;
        public Image2DBuffer CorrectedImageBuffer => barrelDistortion.Output;


        readonly BarrelDistortion barrelDistortion;


        public OpticalDistortionCorrectionDC(LanguageDC languageDC, OpenCLAccelerator openCLAccelerator)
        {
            LanguageDC = languageDC;
            barrelDistortion = new BarrelDistortion(openCLAccelerator);
        }

        public void Correct(Image2DBuffer inputImageBuffer) => barrelDistortion.Remove(inputImageBuffer, Strength.Value, Zoom.Value);

        public void ResetAllToDefault()
        {
            Strength.Value = 0;
            Zoom.Value = 1;
        }

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);
            settingsCollection.SetProperty(Strength.Value, nameof(Strength));
            settingsCollection.SetProperty(Zoom.Value, nameof(Zoom));
            settingsCollection.ExitPoint();
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);
            Strength.Value = settingsCollection.GetProperty<float>(nameof(Strength));
            Zoom.Value = settingsCollection.GetProperty<float>(nameof(Zoom));
            settingsCollection.ExitPoint();
        }
    }
}
