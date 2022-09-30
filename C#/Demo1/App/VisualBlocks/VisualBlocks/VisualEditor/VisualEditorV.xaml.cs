using CustomControl.VisualEditor;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisualBlocks.Module.Base;
using VisualBlocks.Module.Branch;
using VisualBlocks.Module.ImageSource.UniversalVideoClass;
using VisualBlocks.Module.Trigger;
using VisualBlocks.Module.TypeBool;
using VisualBlocks.Module.TypeFloat;
using VisualBlocks.Module.TypeImageBufferBGRA32;
using VisualBlocks.Module.TypeInt;
using VisualBlocks.Module.TypeString;


namespace VisualBlocks.VisualEditor
{
    public partial class VisualEditorV : UserControl
    {
        VisualEditorDC dc => (VisualEditorDC)DataContext;

        public VisualEditorV()
        {
            InitializeComponent();
        }

        private void ImportButtonClick(object sender, RoutedEventArgs e)
            => dc.Import(blocksEditor.LastContextMenuOpenPosition);

        private void BlocksEditor_NewDataConnection(object sender, NewDataConnectionAddedEventArgs e)
              => dc.BlocksEditor_NewDataConnection(e);

        private void BlocksEditor_AddNewTriggerConnection(object sender, NewTriggerConnectionAddedEventArgs e)
              => dc.BlocksEditor_NewTriggerConnection(e);

        private void BlocksEditor_KeyPressOrRelease(object sender, KeyEventArgs e)
        {
            if (sender is BlocksEditor blocksEditor && blocksEditor.IsFocused)
                dc.KeyPressOrReleaseOccurred(e, blocksEditor.CurrentMousePosition);
        }

        private void BlocksEditor_RemoveItems(object sender, RemoveItemsEventArgs e) 
            => dc.RemoveItems(e);

        // Indító
        private void AddNewTriggerInitializeButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(TriggerInitializeDC));

        private void AddNewTriggerButtonButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(TriggerButtonDC));

        private void AddNewTriggerKeyPressButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(TriggerKeyPressDC));

        private void AddNewTriggerDelayButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(TriggerDelayDC));

        private void AddNewTriggerExtensionButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(TriggerExtensionDC));

        private void AddNewTriggerContinousButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(TriggerContinousDC));

        private void AddNewTriggerSerialButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(TriggerSerialDC));

        // Szöveg
        private void AddNewStringConstantButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(StringConstantDC));

        private void AddNewMonitorStringButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(StringMonitorDC));

        private void AddNewStringCompareButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(StringCompareDC));

        private void AddNewStringDataSelectorButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(StringDataSelectorDC));

        private void AddNewStringExtensionButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(StringExtensionDC));

        private void AddNewStringContainerButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(StringContainerDC));

        private void AddNewStringContainerWriteButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(StringContainerWriteDC));

        private void AddNewStringContainerReadButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(StringContainerReadDC));

        private void AddNewStringContainerExtensionButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(StringContainerExtensionDC));

        // Szám (egész)
        private void AddNewIntConstantButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(IntConstantDC));

        private void AddNewIntMonitorButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(IntMonitorDC));

        private void AddNewIntCompareButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(IntCompareDC));

        private void AddNewIntDataSelectorButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(IntDataSelectorDC));

        private void AddNewIntMathOperationButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(IntMathOperationDC));

        private void AddNewIntExtensionButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(IntExtensionDC));

        private void AddNewIntRandomButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(IntRandomDC));

        private void AddNewIntContainerButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(IntContainerDC));

        private void AddNewIntContainerWriteButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(IntContainerWriteDC));

        private void AddNewIntContainerReadButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(IntContainerReadDC));

        private void AddNewIntContainerExtensionButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(IntContainerExtensionDC));

        // Szám (tört)
        private void AddNewFloatConstantButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(FloatConstantDC));

        private void AddNewFloatMonitorButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(FloatMonitorDC));

        private void AddNewFloatCompareButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(FloatCompareDC));

        private void AddNewFloatDataSelectorButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(FloatDataSelectorDC));

        private void AddNewFloatMathOperationButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(FloatMathOperationDC));

        private void AddNewFloatExtensionButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(FloatExtensionDC));

        private void AddNewFloatRandomButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(FloatRandomDC));

        private void AddNewFloatContainerButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(FloatContainerDC));

        private void AddNewFloatContainerWriteButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(FloatContainerWriteDC));

        private void AddNewFloatContainerReadButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(FloatContainerReadDC));

        private void AddNewFloatContainerExtensionButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(FloatContainerExtensionDC));

        // Logikai
        private void AddNewBoolConstantButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(BoolConstantDC));

        private void AddNewBoolMonitorButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(BoolMonitorDC));

        private void AddNewBoolCompareButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(BoolCompareDC));

        private void AddNewBoolNotButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(BoolNotDC));

        private void AddNewBoolOperationButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(BoolOperationDC));

        private void AddNewBoolExtensionButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(BoolExtensionDC));

        private void AddNewBoolContainerButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(BoolContainerDC));

        private void AddNewBoolContainerWriteButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(BoolContainerWriteDC));

        private void AddNewBoolContainerReadButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(BoolContainerReadDC));

        private void AddNewBoolContainerExtensionButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(BoolContainerExtensionDC));

        // Kép forrás
        private void AddNewUniversalVideoButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(UniversalVideoClassDC));

        // Kép (színes)
        private void AddNewImageBufferBGRA32MonitorButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(ImageBufferBGRA32MonitorDC));

        private void AddNewImageBufferBGRA32DataSelectorButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(ImageBufferBGRA32DataSelectorDC));

        private void AddNewImageBufferBGRA32ExtensionButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(ImageBufferBGRA32ExtensionDC));

        // Csoport
        private void AddNewGroupButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(BlockItemGroupDC));

        // Elágazás
        private void AddNewBranchButtonClick(object sender, RoutedEventArgs e)
            => dc.AddNew(typeof(BranchDC));
    }
}
