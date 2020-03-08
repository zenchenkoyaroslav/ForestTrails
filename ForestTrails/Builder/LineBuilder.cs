using ForestTrails.Paths;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ForestTrails.Builder
{
    public class LineBuilder : IBuilder
    {
        private MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        Line line;

        public LineBuilder() => Reset();

        public void BuildCursor(Cursor cursor) => line.Cursor = cursor;

        public void BuildFill(Brush brush) => line.Fill = brush;

        public void BuildName(string name) => line.Name = name;

        public void BuildStroke(Brush brush) => line.Stroke = brush;

        public void BuildStrokeThickness(double thickness) => line.StrokeThickness = thickness;

        public void BuildCoordinates(double x1, double x2, double y1, double y2)
        {
            line.X1 = x1;
            line.X2 = x2;
            line.Y1 = y1;
            line.Y2 = y2;
        }

        public void BuildToolTip(string tip)
        {
            line.ToolTip = tip;
        }

        public void Reset() => line = new Line();

        public Line GetLine()
        {
            Line result = line;
            Reset();

            result.MouseEnter += mainWindow.Line_MouseEnter;
            result.MouseLeave += mainWindow.Line_MouseLeave;

            return result;
        }

        

    }
}
