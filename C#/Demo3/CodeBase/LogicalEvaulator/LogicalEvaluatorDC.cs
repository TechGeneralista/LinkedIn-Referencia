using Common;
using Common.Communication;
using Common.Id;
using Common.Interfaces;
using Common.Language;
using Common.Log;
using Common.NotifyProperty;
using Common.Settings;
using System;
using System.Linq;


namespace LogicalEvaluator
{
    public class LogicalEvaluatorDC : IHasId, IHasResult, ICanEvulate, ICanRemote, ICanSaveLoadSettings
    {
        public IProperty<string> Id => idDC.Id;
        public LanguageDC LanguageDC { get; }
        public IReadOnlyProperty<string>[] AvailableLogicOperations { get; private set; }
        public IProperty<int> SelectedLogicIndex { get; } = new Property<int>();
        public IReadOnlyPropertyArray<object> ElementsSource { get; }
        public IProperty<object> SelectedElementToAdd { get; } = new Property<object>();
        public IReadOnlyPropertyArray<object> Elements { get; } = new PropertyArray<object>();
        public IProperty<object> SelectedElement { get; } = new Property<object>();
        public IReadOnlyProperty<bool?> Result { get; } = new Property<bool?>();

        
        readonly LogDC logDC;
        readonly IdDC idDC;


        public LogicalEvaluatorDC(LanguageDC languageDC, LogDC logDC, IReadOnlyPropertyArray<object> elementsSource)
        {
            LanguageDC = languageDC;
            this.logDC = logDC;
            ElementsSource = elementsSource;
            idDC = new IdDC(languageDC, logDC);

            ElementsSource.OnValueChanged += (o, n) =>
            {
                if (o.Length > n.Length)
                {
                    foreach (object obj in o)
                    {
                        if (!n.Contains(obj) || Elements.Value.Contains(obj))
                            Elements.ToSettable().Remove(obj);
                    }
                }
            };

            AvailableLogicOperations = new IReadOnlyProperty<string>[]
            {
                LanguageDC.And, LanguageDC.Or
            };
        }

        public void Evulate()
        {
                if (Elements.Value.IsNull() || Elements.Value.Length == 0)
                {
                    Result.ToSettable().Value = null;
                    return;
                }

                (Elements.Value[0] as ICanEvulate)?.Evulate();

                if (((IHasResult)Elements.Value[0]).Result.Value.IsNull())
                {
                    Result.ToSettable().Value = null;
                    return;
                }

                bool result = (bool)((IHasResult)Elements.Value[0]).Result.Value;

                if (Elements.Value.Length == 1)
                {
                    Result.ToSettable().Value = result;
                    return;
                }

                for (int i = 1; i < Elements.Value.Length; i++)
                {
                    (Elements.Value[i] as ICanEvulate)?.Evulate();

                    if (((IHasResult)Elements.Value[i]).Result.Value.IsNull())
                    {
                        Result.ToSettable().Value = null;
                        return;
                    }

                    switch (SelectedLogicIndex.Value)
                    {
                        case 0:
                            result &= (bool)((IHasResult)Elements.Value[i]).Result.Value;
                            break;

                        case 1:
                            result |= (bool)((IHasResult)Elements.Value[i]).Result.Value;
                            break;
                    }
                }

                Result.ToSettable().Value = result;
        }

        internal void AddSelectedElement()
        {
            if (SelectedElementToAdd.Value.IsNull())
                return;

                Elements.ToSettable().Add(SelectedElementToAdd.Value); 

            SelectedElementToAdd.Value = null;
        }

        internal void AddNewGroup() => Elements.ToSettable().Add(new LogicalEvaluatorDC(LanguageDC, logDC, ElementsSource));

        internal void RemoveSelectedElement()
        {
            if (SelectedElement.Value.IsNull())
                return;

            object o = SelectedElement.Value;
            SelectedElement.Value = null;
            Elements.ToSettable().Remove(o);
        }

        internal void RemoveAllElement() => Elements.ToSettable().Clear();

        public string Remote(string command, string[] ids)
        {
            string response = null;

            if (ids.First() == Id.Value)
            {
                if (ids.Length == 1)
                {
                    if (command == Commands.GetResult.ToString())
                    {
                        if (Result.Value.IsNull())
                            response = Responses.NotAvailable.ToString();

                        else if (Result.Value == true)
                            response = Responses.Ok.ToString();

                        else if (Result.Value == false)
                            response = Responses.Nok.ToString();
                    }
                }

                else if (ids.Length > 1)
                {
                    string[] idsFirstRemoved = ids.RemoveFirst();

                    foreach (object o in Elements.Value)
                    {
                        o.CastTo<ICanRemote>().Remote(command, idsFirstRemoved);

                        if (response.IsNotNull())
                            break;
                    }
                }
            }

            return response;
        }

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);
            idDC.SaveSettings(settingsCollection);

            settingsCollection.SetProperty(SelectedLogicIndex.Value, nameof(SelectedLogicIndex));

            settingsCollection.AddKey(nameof(Elements));
            int length = Elements.Value.Length;
            settingsCollection.SetProperty(length, nameof(Elements.Value.Length));

            for (int i = 0; i < length; i++)
            {
                settingsCollection.AddKey(i.ToString());
                object element = Elements.Value[i];
                string elementTypeName = element.GetType().Name;
                settingsCollection.SetProperty(elementTypeName, nameof(Type));

                if (elementTypeName == nameof(LogicalEvaluatorDC))
                    element.CastTo<ICanSaveLoadSettings>().SaveSettings(settingsCollection);
                else
                    settingsCollection.SetProperty(element.CastTo<IHasId>().Id.Value, "Id");

                settingsCollection.RemoveLastKey(); // i.ToString()
            }

            settingsCollection.RemoveLastKey(); // nameof(Elements)
            settingsCollection.ExitPoint();
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);
            idDC.LoadSettings(settingsCollection);

            SelectedLogicIndex.Value = settingsCollection.GetProperty<int>(nameof(SelectedLogicIndex));

            settingsCollection.AddKey(nameof(Elements));
            int length = settingsCollection.GetProperty<int>(nameof(Elements.Value.Length));

            for (int i = 0; i < length; i++)
            {
                settingsCollection.AddKey(i.ToString());
                string elementTypeName = settingsCollection.GetProperty<string>(nameof(Type));

                if (elementTypeName == nameof(LogicalEvaluatorDC))
                {
                    AddNewGroup();
                    Elements.Value[i].CastTo<ICanSaveLoadSettings>().LoadSettings(settingsCollection);
                }
                else
                {
                    string id = settingsCollection.GetProperty<string>("Id");

                    object selectedElement = null;

                    foreach (object o in ElementsSource.Value)
                    {
                        IHasId hasId = o as IHasId;

                        if (hasId.IsNotNull() && hasId.Id.Value == id)
                        {
                            selectedElement = o;
                            break;
                        }
                    }

                    SelectedElementToAdd.Value = selectedElement;
                    AddSelectedElement();
                }

                settingsCollection.RemoveLastKey(); // i.ToString()
            }

            settingsCollection.RemoveLastKey(); // nameof(Elements)
            settingsCollection.ExitPoint();
        }
    }
}
