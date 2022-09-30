using System.Windows;


namespace CustomControl.VisualEditor
{
    internal interface IHasQuadraticBezierCurvePoints
    {
        Point Start { get; }
        Point StartDirection { get; }
        Point EndDirection { get; }
        Point End { get; }
    }
}