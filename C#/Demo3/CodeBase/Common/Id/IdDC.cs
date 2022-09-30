using Common.Interfaces;
using Common.Language;
using Common.Log;
using Common.NotifyProperty;
using Common.Settings;
using System.Text;


namespace Common.Id
{
    public class IdDC : IHasId, ICanSaveLoadSettings
    {
        public LanguageDC LanguageDC { get; }
        public IProperty<string> Id { get; } = new Property<string>(Utils.NewGuid(8));


        readonly LogDC logDC;


        public IdDC(LanguageDC languageDC, LogDC logDC)
        {
            LanguageDC = languageDC;
            this.logDC = logDC;

            Id.OnValueChanged += CheckId;
        }

        private void CheckId(string ov, string nv)
        {
            if (string.IsNullOrEmpty(nv) || string.IsNullOrWhiteSpace(nv))
            {
                logDC.NewMessage(LogTypes.Warning, LanguageDC.IdCannotBeEmpty);
                Id.Value = ov;
            }

            if(Encoding.UTF8.GetByteCount(nv) != nv.Length)
            {
                logDC.NewMessage(LogTypes.Warning, LanguageDC.IdCanOnlyContainASCIICharacters);
                Id.Value = ov;
            }

            if (nv.Contains(" "))
            {
                logDC.NewMessage(LogTypes.Warning, LanguageDC.IdCannotContainSpaces);
                Id.Value = ov;
            }
        }

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);
            settingsCollection.SetProperty(Id.Value, nameof(Id));
            settingsCollection.ExitPoint();
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);
            Id.Value = settingsCollection.GetProperty<string>(nameof(Id));
            settingsCollection.ExitPoint();
        }
    }
}
