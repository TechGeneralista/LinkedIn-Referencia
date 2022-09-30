using Common.Prop;
using Common.Tool;
using Editor;
using IOBoard;

namespace UserProgram
{
    public class InterpreterViewModel
    {
        public Interpreter Interpreter { get; }
        public INonSettableObservableProperty<bool> RunButtonIsEnable { get; } = new ObservableProperty<bool>(true);
        public INonSettableObservableProperty<bool> StopButtonIsEnable { get; } = new ObservableProperty<bool>(false);


        public InterpreterViewModel()
        {
            Interpreter = new Interpreter(ObjectContainer.Get<IOBoardClient>());
            Interpreter.Status.ValueChanged += Status_ValueChanged;
        }

        private void Status_ValueChanged(InterpreterStates newStatus)
        {
            if(newStatus == InterpreterStates.Stopped)
            {
                RunButtonIsEnable.ForceSet(true);
                StopButtonIsEnable.ForceSet(false);
                ObjectContainer.Get<EditorViewModel>().TexBoxIsReadOnly.Value = false;
                ObjectContainer.Get<EditorViewModel>().NewButtonIsEnable.Value = true;
                ObjectContainer.Get<EditorViewModel>().OpenButtonIsEnable.Value = true;
            }

            else if (newStatus == InterpreterStates.Running)
            {
                RunButtonIsEnable.ForceSet(false);
                StopButtonIsEnable.ForceSet(true);
                ObjectContainer.Get<EditorViewModel>().TexBoxIsReadOnly.Value = true;
                ObjectContainer.Get<EditorViewModel>().NewButtonIsEnable.Value = false;
                ObjectContainer.Get<EditorViewModel>().OpenButtonIsEnable.Value = false;
            }
        }

        public void RunButtonClick() => Interpreter.Run(ObjectContainer.Get<EditorViewModel>().Code.Value);

        internal void StopButtonClick() => Interpreter.Stop();
    }
}
