using Common;


namespace VisualBlocks.Module.Base
{
    internal enum Status { Ok, Error };

    internal class BlockItemDC : DCBase, ISelectable
    {
        public double Left
        {
            get => left;
            set 
            { 
                SetField(ref left, value);
                dependencyParams.NotifyProjectChanged();
            }
        }
        double left;

        public double Top
        {
            get => top;
            set 
            { 
                SetField(ref top, value);
                dependencyParams.NotifyProjectChanged();
            }
        }
        double top;

        public bool ErrorOccured
        {
            get => errorOccured;
            private set => SetField(ref errorOccured, value);
        }
        bool errorOccured;

        public string Message
        {
            get => message;
            private set => SetField(ref message, value);
        }
        string message;

        protected void SetStatus(Status status, string message = nameof(Status.Ok))
        {
            ErrorOccured = status == Status.Error;
            Message = message;
        }

        public int ZIndex
        {
            get => zIndex;
            protected set => SetField(ref zIndex, value);
        }
        int zIndex = 2;

        public bool IsSelected
        {
            get => isSelected;
            set => SetField(ref isSelected, value);
        }
        bool isSelected;


        protected readonly DependencyParams dependencyParams;


        public BlockItemDC(DependencyParams dependencyParams)
        {
            this.dependencyParams = dependencyParams;
            SetStatus(Status.Ok);
        }
    }
}
