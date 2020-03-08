using ForestTrails.Paths;
using ForestTrails.Paths.DrawInformation;
using System.Windows.Shapes;

namespace ForestTrails.Extentions
{
    public static class Extention
    {
        public static CrossroadDrawInformation GetDrawInformation(this Ellipse ellipse, ForestPaths forestPaths)
        {
            return forestPaths.GetCrossroad(ellipse.Name).GetDrawInformation();
        }

        public static CrossroadHighlightedDrawInformation GetHighlightedDrawInformation(this Ellipse ellipse, ForestPaths forestPaths)
        {
            return forestPaths.GetCrossroad(ellipse.Name).GetHighlightedDrawInformation();
        }

        public static void Highlight(this Ellipse ellipse, ForestPaths forestPaths)
        {
            var drawParams = ellipse.GetHighlightedDrawInformation(forestPaths);
            DrawEllipse(ellipse, drawParams);
        }

        public static void Unhighlight(this Ellipse ellipse, ForestPaths forestPaths)
        {
            var drawParams = ellipse.GetDrawInformation(forestPaths);
            DrawEllipse(ellipse, drawParams);
        }

        private static void DrawEllipse(Ellipse ellipse, IDrawInformation drawParams)
        {
            ellipse.Height = drawParams.Height;
            ellipse.Width = drawParams.Width;
            ellipse.Stroke = drawParams.Stroke;
            ellipse.StrokeThickness = drawParams.Thickness;
        }
    }
}
