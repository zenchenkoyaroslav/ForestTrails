using ForestTrails.Paths;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ForestTrails.Extentions;
using ForestTrails.Paths.DrawInformation;
using System.Collections.Generic;

namespace ForestTrails.Builder
{
    public class EllipseBuilder : IBuilder
    {
        private MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
        private ForestPaths forestPaths;

        Ellipse ellipse;

        public EllipseBuilder()
        {
            Reset();
            forestPaths = mainWindow.GlobalForestPaths;
        }

        public void Reset() => ellipse = new Ellipse();

        public void BuildWidth(double width) => ellipse.Width = width;

        public void BuildHeight(double height) => ellipse.Height = height;

        public void BuildCursor(Cursor cursor) => ellipse.Cursor = cursor;

        public void BuildFill(Brush brush) => ellipse.Fill = brush;

        public void BuildName(string name) => ellipse.Name = name;

        public void BuildStroke(Brush brush) => ellipse.Stroke = brush;

        public void BuildStrokeThickness(double thickness) => ellipse.StrokeThickness = thickness;

        public void BuildToolTip(string tip) => ellipse.ToolTip = tip;


        public Ellipse GetEllipse()
        {
            Ellipse result = ellipse;
            Reset();

            result.MouseEnter += mainWindow.Ellipse_MouseEnter;
            result.MouseLeave += mainWindow.Ellipse_MouseLeave;
            return result;
        }
    }
}
