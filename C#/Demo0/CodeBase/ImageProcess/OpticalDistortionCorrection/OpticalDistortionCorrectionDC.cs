using Common.NotifyProperty;
using Common.Settings;
using ImageProcess.Buffer;
using ImageProcess.Templates;
using Language;
using OpenCLWrapper;


namespace ImageProcess.OpticalDistortionCorrection
{
    public class OpticalDistortionCorrectionDC : ICanSaveLoadSettings
    {
        public LanguageDC LanguageDC { get; }
        public float StrengthMaximum { get; } = 10;
        public ISettableObservableProperty<float> Strength { get; } = new ObservableProperty<float>((float)0);
        public float StrengthMinimum { get; } = 0;
        public float ZoomMaximum { get; } = 4;
        public ISettableObservableProperty<float> Zoom { get; } = new ObservableProperty<float>((float)1);
        public float ZoomMinimum { get; } = (float).1;
        public Image2DBuffer OutputImageBuffer => barrelDistortion.Output;


        readonly BarrelDistortion barrelDistortion;


        public OpticalDistortionCorrectionDC(LanguageDC languageDC, OpenCLAccelerator openCLAccelerator)
        {
            LanguageDC = languageDC;
            barrelDistortion = new BarrelDistortion(openCLAccelerator);
        }

        public void Correct(Image2DBuffer inputImageBuffer) => barrelDistortion.Remove(inputImageBuffer, Strength.CurrentValue, Zoom.CurrentValue);

        public void ResetAllToDefault()
        {
            Strength.CurrentValue = 0;
            Zoom.CurrentValue = 1;
        }

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(nameof(OpticalDistortionCorrectionDC));
            settingsCollection.KeyCreator.AddNew(nameof(Strength));
            settingsCollection.SetValue(Strength.CurrentValue);

            settingsCollection.KeyCreator.ReplaceLast(nameof(Zoom));
            settingsCollection.SetValue(Zoom.CurrentValue);

            settingsCollection.KeyCreator.RemoveLast(2);
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(nameof(OpticalDistortionCorrectionDC));
            settingsCollection.KeyCreator.AddNew(nameof(Strength));
            Strength.CurrentValue = settingsCollection.GetValue<float>();

            settingsCollection.KeyCreator.ReplaceLast(nameof(Zoom));
            Zoom.CurrentValue = settingsCollection.GetValue<float>();

            settingsCollection.KeyCreator.RemoveLast(2);
        }
    }
}
