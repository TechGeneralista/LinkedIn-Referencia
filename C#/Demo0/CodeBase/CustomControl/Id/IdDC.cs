using AppLog;
using Common;
using Common.NotifyProperty;
using Common.Settings;
using Language;
using System.Text;


namespace CustomControl.Id
{
    public class IdDC : ICanSaveLoadSettings
    {
        public LanguageDC LanguageDC { get; }
        public ISettableObservableProperty<string> Id { get; } = new ObservableProperty<string>(Utils.NewGuid(8));


        readonly ILog log;


        public IdDC(LanguageDC languageDC, ILog log)
        {
            LanguageDC = languageDC;
            this.log = log;

            Id.CurrentValueChanged += CheckId;
        }

        private void CheckId(string ov, string nv)
        {
            if (string.IsNullOrEmpty(nv) || string.IsNullOrWhiteSpace(nv))
            {
                log.NewMessage(LogTypes.Warning, LanguageDC.IdCannotBeEmpty.CurrentValue);
                Id.CurrentValue = ov;
            }

            if(Encoding.UTF8.GetByteCount(nv) != nv.Length)
            {
                log.NewMessage(LogTypes.Warning, LanguageDC.IdCanOnlyContainASCIICharacters.CurrentValue);
                Id.CurrentValue = ov;
            }

            if (nv.Contains(" "))
            {
                log.NewMessage(LogTypes.Warning, LanguageDC.IdCannotContainSpaces.CurrentValue);
                Id.CurrentValue = ov;
            }
        }

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(nameof(Id));
            settingsCollection.SetValue(Id.CurrentValue);
            settingsCollection.KeyCreator.RemoveLast();
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(nameof(Id));
            Id.CurrentValue = settingsCollection.GetValue<string>();
            settingsCollection.KeyCreator.RemoveLast();
        }
    }
}
