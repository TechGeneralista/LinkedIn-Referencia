namespace CustomControl.VisualEditor
{
    internal interface IMoveable : ISelectable
    {
        double X { get; set; }
        double Y { get; set; }
        double OffsetX { get; set; }
        double OffsetY { get; set; }
        double Left { get; set; }
        double Top { get; set; }
        double ActualWidth { get; }
        double ActualHeight { get; }
    }
}
