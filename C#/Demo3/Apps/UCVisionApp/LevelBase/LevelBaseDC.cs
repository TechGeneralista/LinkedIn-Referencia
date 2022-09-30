using Common.Interfaces;
using Common.Language;
using Common.Log;
using Common.NotifyProperty;
using LogicalEvaluator;


namespace UCVisionApp.LevelBase
{
    public class LevelBaseDC : IHasName, IHasId, IHasResult, IHasIsSelected
    {
        public IProperty<bool> IsSelected { get; } = new Property<bool>();
        public LanguageDC LanguageDC { get; }
        public IReadOnlyProperty<string> Name { get; protected set; }
        public IProperty<string> Id => LogicalEvaluatorDC.Id;
        public IReadOnlyProperty<bool?> Result => LogicalEvaluatorDC.Result;
        public IReadOnlyPropertyArray<object> Children { get; } = new PropertyArray<object>();
        public IProperty<object> SelectedChild { get; } = new Property<object>();
        public LogicalEvaluatorDC LogicalEvaluatorDC { get; }
        

        protected readonly LogDC logDC;


        public LevelBaseDC(LanguageDC languageDC, LogDC logDC)
        {
            LanguageDC = languageDC;
            this.logDC = logDC;

            LogicalEvaluatorDC = new LogicalEvaluatorDC(languageDC, logDC, Children);
        }
    }
}
