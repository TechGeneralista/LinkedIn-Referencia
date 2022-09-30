using Common;
using Common.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace CustomControl.VisualEditor
{
    internal enum State 
    { 
        Ready,
        Move,
        Resize,
        WiringTriggerOutputToInput,
        WiringTriggerInputToOutput,
        WiringDataOutputToInput,
        WiringDataInputToOutput
    }

    public class BlocksEditor : Control
    {
        static BlocksEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BlocksEditor), new FrameworkPropertyMetadata(typeof(BlocksEditor)));
        }

        public event EventHandler<NewDataConnectionAddedEventArgs> AddNewDataConnection;
        public event EventHandler<NewTriggerConnectionAddedEventArgs> AddNewTriggerConnection;
        public event EventHandler<RemoveItemsEventArgs> RemoveItems;

        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }
        public static readonly DependencyProperty XProperty = DependencyProperty.Register(nameof(X), typeof(double), typeof(BlocksEditor), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }
        public static readonly DependencyProperty YProperty = DependencyProperty.Register(nameof(Y), typeof(double), typeof(BlocksEditor), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }
        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(nameof(Scale), typeof(double), typeof(BlocksEditor), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(BlocksEditor));

        public MouseButton SelectAndMoveMouseButton
        {
            get { return (MouseButton)GetValue(MoveMouseButtonProperty); }
            set { SetValue(MoveMouseButtonProperty, value); }
        }
        public static readonly DependencyProperty MoveMouseButtonProperty = DependencyProperty.Register(nameof(SelectAndMoveMouseButton), typeof(MouseButton), typeof(BlocksEditor), new PropertyMetadata(MouseButton.Left));

        public MouseButton ResizeMouseButton
        {
            get { return (MouseButton)GetValue(ResizeMouseButtonProperty); }
            set { SetValue(ResizeMouseButtonProperty, value); }
        }
        public static readonly DependencyProperty ResizeMouseButtonProperty = DependencyProperty.Register(nameof(ResizeMouseButton), typeof(MouseButton), typeof(BlocksEditor), new PropertyMetadata(MouseButton.Left));

        public MouseButton WiringMouseButton
        {
            get { return (MouseButton)GetValue(WiringMouseButtonProperty); }
            set { SetValue(WiringMouseButtonProperty, value); }
        }
        public static readonly DependencyProperty WiringMouseButtonProperty = DependencyProperty.Register(nameof(WiringMouseButton), typeof(MouseButton), typeof(BlocksEditor), new PropertyMetadata(MouseButton.Left));

        public Point? LastContextMenuOpenPosition
        {
            get
            {
                Point? temp = lastContextMenuOpenPosition;
                lastContextMenuOpenPosition = null;
                return temp;
            }
        }
        Point? lastContextMenuOpenPosition;

        public Point CurrentMousePosition { get; private set; }
        internal Canvas MainCanvas { get; private set; }
        internal Canvas SignalCanvas { get; private set; }
        internal State State { get; private set; }


        PanZoomViewer panZoomViewer;
        ItemsControl itemsControl;
        Point startMousePosition;
        Vector? lastGoodMouseOffset;
        IEnumerable<IMoveable> cachedSelectedMoveables, cachedNonSelectedMoveables;
        BlockItemDataOutput wiringBlockItemDataOutput;
        BlockItemDataInput wiringBlockItemDataInput;
        BlockItemTriggerOutput wiringBlockItemTriggerOutput;
        BlockItemTriggerInput wiringBlockItemTriggerInput;
        BlockItemTempConnection blockItemTempConnection;
        


        public BlocksEditor()
        {
            MouseMove += BlocksEditor_MouseMove;
            KeyDown += BlocksEditor_KeyDown;
            Loaded += (s, e) => Focus();
        }

        private void BlocksEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Handled &&
               e.Key == Key.Delete &&
               e.IsRepeat == false &&
               e.IsDown)
            {
                List<FrameworkElement> itemsToRemove = new List<FrameworkElement>();

                foreach (object item in ItemsSource)
                {
                    if (itemsControl.ItemContainerGenerator.ContainerFromItem(item) is ContentPresenter container &&
                       container.GetVisualChild<ISelectable>() is ISelectable selectable &&
                       selectable.IsSelected && selectable is FrameworkElement frameworkElement)
                    {
                        itemsToRemove.Add(frameworkElement);
                    }
                }

                itemsToRemove.ForEach(i => RemoveChild(i));

                e.Handled = true;
            }
        }

        public override void OnApplyTemplate()
        {
            ContextMenu.Opened += (s, e) =>
            {
                lastContextMenuOpenPosition = Mouse.GetPosition(MainCanvas);
                e.Handled = true;
            };

            itemsControl = GetTemplateChild("PART_ItemsControl") as ItemsControl;
            blockItemTempConnection = GetTemplateChild("PART_BlockItemTempConnection") as BlockItemTempConnection;
            SignalCanvas = GetTemplateChild("PART_SignalCanvas") as Canvas;
            panZoomViewer = GetTemplateChild("PART_PanZoomViewer") as PanZoomViewer;
            MainCanvas = GetTemplateChild("PART_MainCanvas") as Canvas;

            if(panZoomViewer != null)
                panZoomViewer.NotPanned += PanZoomViewer_NotPanned;

            MouseUp += BlocksEditor_MouseUp;
            MouseLeave += BlocksEditor_MouseLeave;
        }

        private void PanZoomViewer_NotPanned(object sender, EventArgs e)
        {
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
            {
                ClearSelection();
                Focus();
            }
        }

        private void ClearSelection()
        {
            foreach (object obj in itemsControl.Items)
            {
                if (itemsControl.ItemContainerGenerator.ContainerFromItem(obj) is ContentPresenter contentPresenter &&
                    contentPresenter.GetVisualChild<ISelectable>() is ISelectable selectable)
                {
                    selectable.IsSelected = false;
                }
            }
        }

        private void BlocksEditor_MouseLeave(object sender, MouseEventArgs e)
        {
            if(State == State.WiringTriggerOutputToInput ||
               State == State.WiringTriggerInputToOutput ||
               State == State.WiringDataOutputToInput ||
               State == State.WiringDataInputToOutput ||
               lastContextMenuOpenPosition != null)
            {
                Reset();
            }
        }

        private void BlocksEditor_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == WiringMouseButton && e.ButtonState == MouseButtonState.Released)
                Reset();
        }

        public void Reset()
        {
            wiringBlockItemTriggerInput = null;
            wiringBlockItemTriggerOutput = null;
            wiringBlockItemDataInput = null;
            wiringBlockItemDataOutput = null;

            if (blockItemTempConnection != null)
                blockItemTempConnection.Visibility = Visibility.Hidden;

            lastContextMenuOpenPosition = null;
            State = State.Ready;
        }

        private bool OutputIsNotWired(BlockItemTriggerOutput blockItemTriggerOutput)
        {
            foreach (object item in ItemsSource)
            {
                if (itemsControl.ItemContainerGenerator.ContainerFromItem(item) is ContentPresenter container &&
                   container.GetVisualChild<BlockItemTriggerConnection>() is BlockItemTriggerConnection blockItemTriggerConnection &&
                   blockItemTriggerConnection.OutputDataContext == blockItemTriggerOutput.DataContext)
                {
                    return false;
                }
            }

            return true;
        }

        private bool InputIsNotWired(BlockItemDataInput blockItemDataInput)
        {
            foreach(object item in ItemsSource)
            {
                if(itemsControl.ItemContainerGenerator.ContainerFromItem(item) is ContentPresenter container &&
                   container.GetVisualChild<BlockItemDataConnection>() is BlockItemDataConnection blockItemDataConnection &&
                   blockItemDataConnection.InputDataContext == blockItemDataInput.DataContext)
                {
                    return false;
                }
            }

            return true;
        }

        private bool OutputIsNotWiredOrIsContainer(BlockItemDataOutput blockItemDataOutput)
        {
            if(blockItemDataOutput.DataContext.GetType() is Type dataContextType &&
               dataContextType.IsGenericType &&
               dataContextType.GetGenericArguments() is Type[] types &&
               types.Length != 0 &&
               types[0] is Type genericType &&
               genericType.IsGenericType &&
               genericType.GetGenericTypeDefinition() == typeof(Container<>))
            {
                return true;
            }

            foreach (object item in ItemsSource)
            {
                if (itemsControl.ItemContainerGenerator.ContainerFromItem(item) is ContentPresenter container &&
                   container.GetVisualChild<BlockItemDataConnection>() is BlockItemDataConnection blockItemDataConnection &&
                   blockItemDataConnection.OutputDataContext == blockItemDataOutput.DataContext)
                {
                    return false;
                }
            }

            return true;
        }

        private void CacheMovables()
        {
            List<IMoveable> selectedMovables = new List<IMoveable>();
            List<IMoveable> nonSelectedMovables = new List<IMoveable>();

            foreach (object obj in itemsControl.Items)
            {
                if (itemsControl.ItemContainerGenerator.ContainerFromItem(obj) is ContentPresenter contentPresenter &&
                   contentPresenter.GetVisualChild<IMoveable>() is IMoveable moveable)
                {
                    if (moveable.IsSelected)
                        selectedMovables.Add(moveable);
                    else
                        nonSelectedMovables.Add(moveable);
                }
            }

            cachedSelectedMoveables = selectedMovables;
            cachedNonSelectedMoveables = nonSelectedMovables;
        }

        internal void StartWiring(BlockItemTriggerOutput blockItemTriggerOutput, MouseButtonEventArgs e)
        {
            wiringBlockItemTriggerOutput = blockItemTriggerOutput;

            if (blockItemTempConnection != null)
            {
                blockItemTempConnection.Visibility = Visibility.Visible;
                blockItemTempConnection.End = blockItemTempConnection.Start = e.MouseDevice.GetPosition(MainCanvas);
            }

            State = State.WiringTriggerOutputToInput;
        }

        internal void EndWiring(BlockItemTriggerInput blockItemTriggerInput, MouseButtonEventArgs e)
        {
            if (wiringBlockItemTriggerOutput != null && blockItemTriggerInput != null && OutputIsNotWired(wiringBlockItemTriggerOutput))
                AddNewTriggerConnection?.Invoke(this, new NewTriggerConnectionAddedEventArgs(wiringBlockItemTriggerOutput.DataContext, blockItemTriggerInput.DataContext));

            Reset();
        }

        internal void StartWiring(BlockItemTriggerInput blockItemTriggerInput, MouseButtonEventArgs e)
        {
            wiringBlockItemTriggerInput = blockItemTriggerInput;

            if (blockItemTempConnection != null)
            {
                blockItemTempConnection.Visibility = Visibility.Visible;
                blockItemTempConnection.End = blockItemTempConnection.Start = e.MouseDevice.GetPosition(MainCanvas);
            }

            State = State.WiringTriggerInputToOutput;
        }

        internal void EndWiring(BlockItemTriggerOutput blockItemTriggerOutput, MouseButtonEventArgs e)
        {
            if (blockItemTriggerOutput != null && wiringBlockItemTriggerInput != null && OutputIsNotWired(blockItemTriggerOutput))
                AddNewTriggerConnection?.Invoke(this, new NewTriggerConnectionAddedEventArgs(blockItemTriggerOutput.DataContext, wiringBlockItemTriggerInput.DataContext));

            Reset();
        }

        internal void StartWiring(BlockItemDataOutput blockItemDataOutput, MouseButtonEventArgs e)
        {
            wiringBlockItemDataOutput = blockItemDataOutput;

            if (blockItemTempConnection != null)
            {
                blockItemTempConnection.Visibility = Visibility.Visible;
                blockItemTempConnection.End = blockItemTempConnection.Start = e.MouseDevice.GetPosition(MainCanvas);
            }

            State = State.WiringDataOutputToInput;
        }

        internal void EndWiring(BlockItemDataInput blockItemDataInput, MouseButtonEventArgs e)
        {
            if (wiringBlockItemDataOutput != null && blockItemDataInput != null && OutputIsNotWiredOrIsContainer(wiringBlockItemDataOutput) && InputIsNotWired(blockItemDataInput))
                AddNewDataConnection?.Invoke(this, new NewDataConnectionAddedEventArgs(wiringBlockItemDataOutput.DataContext, blockItemDataInput.DataContext));

            Reset();
        }

        internal void StartWiring(BlockItemDataInput blockItemDataInput, MouseButtonEventArgs e)
        {
            wiringBlockItemDataInput = blockItemDataInput;

            if (blockItemTempConnection != null)
            {
                blockItemTempConnection.Visibility = Visibility.Visible;
                blockItemTempConnection.End = blockItemTempConnection.Start = e.MouseDevice.GetPosition(MainCanvas);
            }

            State = State.WiringDataInputToOutput;
        }

        internal void EndWiring(BlockItemDataOutput blockItemDataOutput, MouseButtonEventArgs e)
        {
            if (blockItemDataOutput != null && wiringBlockItemDataInput != null && OutputIsNotWiredOrIsContainer(blockItemDataOutput) && InputIsNotWired(wiringBlockItemDataInput))
                AddNewDataConnection?.Invoke(this, new NewDataConnectionAddedEventArgs(blockItemDataOutput.DataContext, wiringBlockItemDataInput.DataContext));

            Reset();
        }

        internal void SelectOrStartMove(ISelectable selectable, MouseButtonEventArgs e)
        {
            CurrentMousePosition = startMousePosition = e.MouseDevice.GetPosition(MainCanvas);
            lastGoodMouseOffset = new Vector();

            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if(!selectable.IsSelected)
                {
                    ClearSelection();
                    selectable.IsSelected = true;
                }

                CacheMovables();
                State = State.Move;
            }
            else
            {
                if (!selectable.IsSelected)
                    selectable.IsSelected = true;
                else
                    selectable.IsSelected = false;
            }
        }

        private void BlocksEditor_MouseMove(object sender, MouseEventArgs e)
        {
            CurrentMousePosition = e.MouseDevice.GetPosition(MainCanvas);

            if (State == State.Move)
            {
                Vector offset = CurrentMousePosition - startMousePosition;

                cachedSelectedMoveables.ForEach(x => 
                {
                    if(x.IsSelected)
                    {
                        x.OffsetX = offset.X;
                        x.OffsetY = offset.Y;
                    }
                });

                cachedSelectedMoveables.ForEach(x =>
                {
                    if (x is IHasCollisionWhileMotion hasCollisionWhileMotion)
                        hasCollisionWhileMotion.CollisionWhileMotion = false;
                });

                cachedNonSelectedMoveables.ForEach(x =>
                {
                    if (x is IHasCollisionWhileMotion hasCollisionWhileMotion)
                        hasCollisionWhileMotion.CollisionWhileMotion = false;
                });

                bool collisionDetected = false;

                foreach(IMoveable movableSelected in cachedSelectedMoveables)
                {
                    foreach (IMoveable movableNonSelected in cachedNonSelectedMoveables)
                    {
                        if (movableSelected.GetType() != typeof(BlockItemGroup) && movableNonSelected.GetType() != typeof(BlockItemGroup) &&
                            movableSelected is IHasCollisionWhileMotion movableSelectedHasCollisionWhileMotion && movableNonSelected is IHasCollisionWhileMotion movableNonSelectedHasCollisionWhileMotion)
                        {
                            Rect selectedRect = new Rect(movableSelected.Left, movableSelected.Top, movableSelected.ActualWidth, movableSelected.ActualHeight);
                            Rect nonSelectedRect = new Rect(movableNonSelected.Left, movableNonSelected.Top, movableNonSelected.ActualWidth, movableNonSelected.ActualHeight);

                            if (selectedRect.IntersectsWith(nonSelectedRect))
                            {
                                movableSelectedHasCollisionWhileMotion.CollisionWhileMotion = true;
                                movableNonSelectedHasCollisionWhileMotion.CollisionWhileMotion = true;
                                collisionDetected = true;
                            }
                        }
                    }
                }

                if (!collisionDetected)
                    lastGoodMouseOffset = offset;

                e.Handled = true;
            }

            else if(State == State.WiringTriggerOutputToInput || State == State.WiringDataOutputToInput)
            {
                blockItemTempConnection.End = CurrentMousePosition;
                e.Handled = true;
            }

            else if (State == State.WiringTriggerInputToOutput || State == State.WiringDataInputToOutput)
            {
                blockItemTempConnection.Start = CurrentMousePosition;
                e.Handled = true;
            }
        }

        internal void EndMove(ISelectable selectable, MouseButtonEventArgs e)
        {
            if (lastGoodMouseOffset.HasValue)
            {
                cachedSelectedMoveables.ForEach(x =>
                {
                    if (x.IsSelected)
                    {
                        x.X += lastGoodMouseOffset.Value.X;
                        x.Y += lastGoodMouseOffset.Value.Y;
                        x.OffsetX = 0;
                        x.OffsetY = 0;
                    }
                });

                lastGoodMouseOffset = null;
            }

            cachedSelectedMoveables.ForEach(x =>
            {
                if (x is IHasCollisionWhileMotion hasCollisionWhileMotion)
                    hasCollisionWhileMotion.CollisionWhileMotion = false;
            });

            cachedNonSelectedMoveables.ForEach(x =>
            {
                if (x is IHasCollisionWhileMotion hasCollisionWhileMotion)
                    hasCollisionWhileMotion.CollisionWhileMotion = false;
            });

            Reset();
        }

        internal void GroupSelectionChanged(BlockItemGroup blockItemGroup)
        {
            foreach (object obj in itemsControl.Items)
            {
                if (itemsControl.ItemContainerGenerator.ContainerFromItem(obj) is ContentPresenter contentPresenter)
                {
                    if (contentPresenter.GetVisualChild<BlockItemGroup>() is BlockItemGroup anotherBlockItemGroup)
                    {
                        if(anotherBlockItemGroup == blockItemGroup)
                            continue;
                        else
                        {
                            Rect blockItemGroupRect = new Rect(blockItemGroup.Left, blockItemGroup.Top, blockItemGroup.ActualWidth, blockItemGroup.ActualHeight);
                            Rect anotherBlockItemGroupRect = new Rect(anotherBlockItemGroup.Left, anotherBlockItemGroup.Top, anotherBlockItemGroup.ActualWidth, anotherBlockItemGroup.ActualHeight);

                            if (blockItemGroupRect.Contains(anotherBlockItemGroupRect))
                                anotherBlockItemGroup.IsSelected = blockItemGroup.IsSelected;
                        }
                    }

                    else if (contentPresenter.GetVisualChild<BlockItem>() is BlockItem blockItem)
                    {
                        Rect blockItemGroupRect = new Rect(blockItemGroup.Left, blockItemGroup.Top, blockItemGroup.ActualWidth, blockItemGroup.ActualHeight);
                        Rect blockItemRect = new Rect(blockItem.Left, blockItem.Top, blockItem.ActualWidth, blockItem.ActualHeight);

                        if (blockItemGroupRect.Contains(blockItemRect))
                            blockItem.IsSelected = blockItemGroup.IsSelected;
                    }

                    else if(contentPresenter.GetVisualChild<BlockItemTriggerConnection>() is BlockItemTriggerConnection blockItemTriggerConnection)
                    {
                        Rect blockItemGroupRect = new Rect(blockItemGroup.Left, blockItemGroup.Top, blockItemGroup.ActualWidth, blockItemGroup.ActualHeight);

                        if (blockItemGroupRect.Contains(blockItemTriggerConnection.Start) && blockItemGroupRect.Contains(blockItemTriggerConnection.End))
                            blockItemTriggerConnection.IsSelected = blockItemGroup.IsSelected;
                    }

                    else if (contentPresenter.GetVisualChild<BlockItemDataConnection>() is BlockItemDataConnection blockItemDataConnection)
                    {
                        Rect blockItemGroupRect = new Rect(blockItemGroup.Left, blockItemGroup.Top, blockItemGroup.ActualWidth, blockItemGroup.ActualHeight);

                        if (blockItemGroupRect.Contains(blockItemDataConnection.Start) && blockItemGroupRect.Contains(blockItemDataConnection.End))
                            blockItemDataConnection.IsSelected = blockItemGroup.IsSelected;
                    }
                }
            }
        }

        internal void RemoveChild(FrameworkElement frameworkElement)
        {
            if (State != State.Ready)
            {
                Reset();
                return;
            }

            List<object> objectToRemove = new List<object>();

            if(frameworkElement is BlockItem blockItem)
            {
                blockItem.DataInputs.ForEach((dataInput) =>
                {
                    foreach (object obj in ItemsSource)
                    {
                        if (itemsControl.ItemContainerGenerator.ContainerFromItem(obj) is ContentPresenter container &&
                           container.GetVisualChild<BlockItemDataConnection>() is BlockItemDataConnection blockItemDataConnection &&
                           blockItemDataConnection.InputDataContext == dataInput.DataContext)
                        {
                            objectToRemove.Add(blockItemDataConnection.DataContext);
                        }
                    }
                });

                blockItem.DataOutputs.ForEach((dataOutput) =>
                {
                    foreach (object obj in ItemsSource)
                    {
                        if (itemsControl.ItemContainerGenerator.ContainerFromItem(obj) is ContentPresenter container &&
                           container.GetVisualChild<BlockItemDataConnection>() is BlockItemDataConnection blockItemDataConnection &&
                           blockItemDataConnection.OutputDataContext == dataOutput.DataContext)
                        {
                            objectToRemove.Add(blockItemDataConnection.DataContext);
                        }
                    }
                });

                blockItem.TriggerInputs.ForEach((triggerInput) =>
                {
                    foreach (object obj in ItemsSource)
                    {
                        if (itemsControl.ItemContainerGenerator.ContainerFromItem(obj) is ContentPresenter container &&
                           container.GetVisualChild<BlockItemTriggerConnection>() is BlockItemTriggerConnection blockItemTriggerConnection &&
                           blockItemTriggerConnection.InputDataContext == triggerInput.DataContext)
                        {
                            objectToRemove.Add(blockItemTriggerConnection.DataContext);
                        }
                    }
                });

                blockItem.TriggerOutputs.ForEach((triggerOutput) =>
                {
                    foreach (object obj in ItemsSource)
                    {
                        if (itemsControl.ItemContainerGenerator.ContainerFromItem(obj) is ContentPresenter container &&
                           container.GetVisualChild<BlockItemTriggerConnection>() is BlockItemTriggerConnection blockItemTriggerConnection &&
                           blockItemTriggerConnection.OutputDataContext == triggerOutput.DataContext)
                        {
                            objectToRemove.Add(blockItemTriggerConnection.DataContext);
                        }
                    }
                });
            }

            objectToRemove.Add(frameworkElement.DataContext);
            RemoveItems?.Invoke(this, new RemoveItemsEventArgs(objectToRemove));
        }

        internal BlockItemDataConnection GetBlockItemDataConnection(BlockItemDataInput blockItemDataInput)
        {
            foreach(object obj in ItemsSource)
            {
                if(itemsControl.ItemContainerGenerator.ContainerFromItem(obj) is ContentPresenter container &&
                   container.GetVisualChild<BlockItemDataConnection>() is BlockItemDataConnection blockItemDataConnection &&
                   blockItemDataConnection.InputDataContext == blockItemDataInput.DataContext)
                {
                    return blockItemDataConnection;
                }
            }

            return null;
        }

        internal IEnumerable<BlockItemDataConnection> GetBlockItemDataConnections(BlockItemDataOutput blockItemDataOutput)
        {
            List<BlockItemDataConnection> connections = new List<BlockItemDataConnection>();

            foreach (object obj in ItemsSource)
            {
                if (itemsControl.ItemContainerGenerator.ContainerFromItem(obj) is ContentPresenter container &&
                   container.GetVisualChild<BlockItemDataConnection>() is BlockItemDataConnection blockItemDataConnection &&
                   blockItemDataConnection.OutputDataContext == blockItemDataOutput.DataContext)
                {
                    connections.Add(blockItemDataConnection);
                }
            }

            return connections;
        }

        internal IEnumerable<BlockItemTriggerConnection> GetBlockItemTriggerConnections(BlockItemTriggerInput blockItemTriggerInput)
        {
            List<BlockItemTriggerConnection> connections = new List<BlockItemTriggerConnection>();

            foreach (object obj in ItemsSource)
            {
                if (itemsControl.ItemContainerGenerator.ContainerFromItem(obj) is ContentPresenter container &&
                   container.GetVisualChild<BlockItemTriggerConnection>() is BlockItemTriggerConnection blockItemTriggerConnection &&
                   blockItemTriggerConnection.InputDataContext == blockItemTriggerInput.DataContext)
                {
                    connections.Add(blockItemTriggerConnection);
                }
            }

            return connections;
        }

        internal BlockItemTriggerConnection GetBlockItemTriggerConnection(BlockItemTriggerOutput blockItemTriggerOutput)
        {
            foreach (object obj in ItemsSource)
            {
                if (itemsControl.ItemContainerGenerator.ContainerFromItem(obj) is ContentPresenter container &&
                   container.GetVisualChild<BlockItemTriggerConnection>() is BlockItemTriggerConnection blockItemTriggerConnection &&
                   blockItemTriggerConnection.OutputDataContext == blockItemTriggerOutput.DataContext)
                {
                    return blockItemTriggerConnection;
                }
            }

            return null;
        }

        internal BlockItemDataOutput GetBlockItemDataOutput(BlockItemDataConnection blockItemDataConnection)
        {
            foreach (object blockItemDataContext in ItemsSource)
            {
                if(itemsControl.ItemContainerGenerator.ContainerFromItem(blockItemDataContext) is ContentPresenter container &&
                   container.GetVisualChild<BlockItem>() is BlockItem blockItem)
                {
                    foreach(BlockItemDataOutput blockItemDataOutput in blockItem.DataOutputs)
                    {
                        if (blockItemDataOutput.DataContext == blockItemDataConnection.OutputDataContext)
                            return blockItemDataOutput;
                    }
                }
            }

            throw new Exception($"{nameof(BlockItemDataOutput)} of {nameof(BlockItemDataConnection)} not found");
        }

        internal BlockItemDataInput GetBlockItemDataInput(BlockItemDataConnection blockItemDataConnection)
        {
            foreach (object blockItemDataContext in ItemsSource)
            {
                if (itemsControl.ItemContainerGenerator.ContainerFromItem(blockItemDataContext) is ContentPresenter container &&
                   container.GetVisualChild<BlockItem>() is BlockItem blockItem)
                {
                    foreach (BlockItemDataInput blockItemDataInput in blockItem.DataInputs)
                    {
                        if (blockItemDataInput.DataContext == blockItemDataConnection.InputDataContext)
                            return blockItemDataInput;
                    }
                }
            }

            throw new Exception($"{nameof(BlockItemDataInput)} of {nameof(BlockItemDataConnection)} not found");
        }

        internal BlockItemTriggerOutput GetBlockItemTriggerOutput(BlockItemTriggerConnection blockItemTriggerConnection)
        {
            foreach (object blockItemDataContext in ItemsSource)
            {
                if (itemsControl.ItemContainerGenerator.ContainerFromItem(blockItemDataContext) is ContentPresenter container &&
                   container.GetVisualChild<BlockItem>() is BlockItem blockItem)
                {
                    foreach (BlockItemTriggerOutput blockItemTriggerOutput in blockItem.TriggerOutputs)
                    {
                        if (blockItemTriggerOutput.DataContext == blockItemTriggerConnection.OutputDataContext)
                            return blockItemTriggerOutput;
                    }
                }
            }

            throw new Exception($"{nameof(BlockItemTriggerOutput)} of {nameof(BlockItemTriggerConnection)} not found");
        }

        internal BlockItemTriggerInput GetBlockItemTriggerInput(BlockItemTriggerConnection blockItemTriggerConnection)
        {
            foreach (object blockItemDataContext in ItemsSource)
            {
                if (itemsControl.ItemContainerGenerator.ContainerFromItem(blockItemDataContext) is ContentPresenter container &&
                   container.GetVisualChild<BlockItem>() is BlockItem blockItem)
                {
                    foreach (BlockItemTriggerInput blockItemTriggerInput in blockItem.TriggerInputs)
                    {
                        if (blockItemTriggerInput.DataContext == blockItemTriggerConnection.InputDataContext)
                            return blockItemTriggerInput;
                    }
                }
            }

            throw new Exception($"{nameof(BlockItemTriggerInput)} of {nameof(BlockItemTriggerConnection)} not found");
        }
    }
}
