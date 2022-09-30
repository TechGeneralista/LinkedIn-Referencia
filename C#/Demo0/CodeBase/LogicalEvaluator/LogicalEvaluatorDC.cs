using AppLog;
using Common;
using Common.Interface;
using Common.NotifyProperty;
using Common.Settings;
using CustomControl.Id;
using Language;
using System;
using System.Collections.Generic;
using System.Linq;


namespace LogicalEvaluator
{
    public class LogicalEvaluatorDC : IHasId, IHasResult, ICanSaveLoadSettings, ICanEvulate
    {
        public LanguageDC LanguageDC { get; }
        public IdDC IdDC { get; }
        public INonSettableObservableProperty<string>[] AvailableLogicOperations { get; }
        public ISettableObservableProperty<int> SelectedLogicIndex { get; } = new ObservableProperty<int>();
        public ISettableObservableProperty<object> SelectedElementToAdd { get; } = new ObservableProperty<object>();
        public INonSettableObservablePropertyArray<object> ElementsSource { get; }
        public INonSettableObservablePropertyArray<object> Elements { get; } = new ObservablePropertyArray<object>();
        public ISettableObservableProperty<object> SelectedElement { get; } = new ObservableProperty<object>();
        public INonSettableObservableProperty<bool?> Result { get; } = new ObservableProperty<bool?>();


        readonly object lockObject = new object();
        readonly ILog log;


        public LogicalEvaluatorDC(LanguageDC languageDC, ILog log, INonSettableObservablePropertyArray<object> elementsSource)
        {
            LanguageDC = languageDC;
            ElementsSource = elementsSource;
            ElementsSource.CurrentValueChanged += Elements_CurrentValueChanged;
            this.log = log;

            IdDC = new IdDC(languageDC, log);

            AvailableLogicOperations = new INonSettableObservableProperty<string>[]
            {
                LanguageDC.And, LanguageDC.Or
            };
        }

        private void Elements_CurrentValueChanged(object[] ov, object[] nv)
        {
            if(ov.Length > nv.Length)
            {
                lock(lockObject)
                {
                    List<object> removeList = new List<object>();

                    foreach (object o in ov)
                    {
                        if (!nv.Contains(o))
                            removeList.Add(o);
                    }

                    foreach (object o in removeList)
                    {
                        if (Elements.CurrentValue.Contains(o))
                            Elements.ForceRemove(o);
                    }
                }
            }
        }

        public void Evulate()
        {
            lock (lockObject)
            {
                if (Elements.CurrentValue.IsNull() || Elements.CurrentValue.Length == 0)
                {
                    Result.ForceSet(null);
                    return;
                }

                (Elements.CurrentValue[0] as ICanEvulate)?.Evulate();

                if (((IHasResult)Elements.CurrentValue[0]).Result.CurrentValue.IsNull())
                {
                    Result.ForceSet(null);
                    return;
                }

                bool result = (bool)((IHasResult)Elements.CurrentValue[0]).Result.CurrentValue;

                if (Elements.CurrentValue.Length == 1)
                {
                    Result.ForceSet(result);
                    return;
                }

                for (int i = 1; i < Elements.CurrentValue.Length; i++)
                {
                    (Elements.CurrentValue[i] as ICanEvulate)?.Evulate();

                    if (((IHasResult)Elements.CurrentValue[i]).Result.CurrentValue.IsNull())
                    {
                        Result.ForceSet(null);
                        return;
                    }

                    switch (SelectedLogicIndex.CurrentValue)
                    {
                        case 0:
                            result &= (bool)((IHasResult)Elements.CurrentValue[i]).Result.CurrentValue;
                            break;

                        case 1:
                            result |= (bool)((IHasResult)Elements.CurrentValue[i]).Result.CurrentValue;
                            break;
                    }
                }

                Result.ForceSet(result);
            }
        }

        internal void AddSelectedElement()
        {
            if (SelectedElementToAdd.CurrentValue.IsNull())
                return;

            lock (lockObject) 
            { 
                Elements.ForceAdd(SelectedElementToAdd.CurrentValue); 
            }

            SelectedElementToAdd.CurrentValue = null;
        }

        internal void AddNewGroup()
        {
            lock (lockObject)
            {
                Elements.ForceAdd(new LogicalEvaluatorDC(LanguageDC, log, ElementsSource));
            }
        }

        internal void RemoveSelectedElement()
        {
            if (SelectedElement.CurrentValue.IsNull())
                return;

            object o = SelectedElement.CurrentValue;
            SelectedElement.CurrentValue = null;
            
            lock (lockObject)
            {
                Elements.ForceRemove(o);
            }
        }

        internal void RemoveAllElement()
        {
            lock (lockObject)
            {
                Elements.ForceClear();
            }
        }

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(nameof(LogicalEvaluatorDC));

            IdDC.SaveSettings(settingsCollection);

            settingsCollection.KeyCreator.AddNew(nameof(SelectedLogicIndex));
            settingsCollection.SetValue(SelectedLogicIndex.CurrentValue);

            settingsCollection.KeyCreator.ReplaceLast(nameof(Elements));
            settingsCollection.KeyCreator.AddNew(nameof(Elements.CurrentValue.Length));
            settingsCollection.SetValue(Elements.CurrentValue.Length);

            for(int i = 0;i<Elements.CurrentValue.Length;i++)
            {
                settingsCollection.KeyCreator.ReplaceLast(i.ToString());
                object element = Elements.CurrentValue[i];

                if(element.GetType() == typeof(LogicalEvaluatorDC))
                {
                    settingsCollection.KeyCreator.AddNew(nameof(Type));
                    settingsCollection.SetValue(element.GetType().Name);
                    settingsCollection.KeyCreator.RemoveLast();

                    (element as ICanSaveLoadSettings)?.SaveSettings(settingsCollection);
                }
                else
                {
                    IHasId hasId = (IHasId)element;

                    settingsCollection.KeyCreator.AddNew(nameof(Type));
                    settingsCollection.SetValue(nameof(SelectedElement));

                    settingsCollection.KeyCreator.ReplaceLast("Id");
                    settingsCollection.SetValue(hasId.IdDC.Id.CurrentValue);

                    settingsCollection.KeyCreator.RemoveLast();
                }
            }

            settingsCollection.KeyCreator.RemoveLast(3);
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(nameof(LogicalEvaluatorDC));

            IdDC.LoadSettings(settingsCollection);

            settingsCollection.KeyCreator.AddNew(nameof(SelectedLogicIndex));
            SelectedLogicIndex.CurrentValue = settingsCollection.GetValue<int>();

            settingsCollection.KeyCreator.ReplaceLast(nameof(Elements));
            settingsCollection.KeyCreator.AddNew(nameof(Elements.CurrentValue.Length));
            int elementsLength = settingsCollection.GetValue<int>();

            for (int i = 0; i < elementsLength; i++)
            {
                settingsCollection.KeyCreator.ReplaceLast(i.ToString());
                settingsCollection.KeyCreator.AddNew(nameof(Type));
                string elementType = settingsCollection.GetValue<string>();
                settingsCollection.KeyCreator.RemoveLast();

                if (elementType == nameof(LogicalEvaluatorDC))
                {
                    AddNewGroup();
                    (Elements.CurrentValue[i] as ICanSaveLoadSettings)?.LoadSettings(settingsCollection);
                }
                else
                {
                    settingsCollection.KeyCreator.AddNew("Id");
                    string id = settingsCollection.GetValue<string>();
                    settingsCollection.KeyCreator.RemoveLast();

                    object selectedElement = null;

                    foreach (object o in ElementsSource.CurrentValue)
                    {
                        IHasId hasId = o as IHasId;

                        if (hasId.IsNotNull() && hasId.IdDC.Id.CurrentValue == id)
                        {
                            selectedElement = o;
                            break;
                        }
                    }

                    SelectedElementToAdd.CurrentValue = selectedElement;
                    AddSelectedElement();
                }
            }

            settingsCollection.KeyCreator.RemoveLast(3);
        }
    }
}
