using ForestTrails.Paths;
using ForestTrails.Paths.DrawInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace ForestTrails.Builder
{
    public class BuildDirector
    {
        private double height;
        private double width;
        private Brush fill;
        private Brush stroke;
        private double thickness;
        private Cursor cursor;
        private ICrossroad crossroad;

        private double x1;
        private double x2;
        private double y1;
        private double y2;

        private CrossroadDrawInformation drawParams;

        public EllipseBuilder BuildEllipse(ICrossroad crossroad)
        {
            this.crossroad = crossroad;
            drawParams = crossroad.GetDrawInformation();
            DrawParamsInit();
            EllipseBuilder builder = new EllipseBuilder();
            builder.BuildName(crossroad.Key);
            builder.BuildHeight(height);
            builder.BuildWidth(width);
            builder.BuildFill(fill);
            builder.BuildStroke(stroke);
            builder.BuildStrokeThickness(thickness);
            builder.BuildCursor(cursor);
            builder.BuildToolTip($"{crossroad.Key} ({crossroad.Position.X}, {crossroad.Position.Y})");
            return builder;
        }

        public LineBuilder BuildLine(ForestPaths forestPaths, string firstKey, string secondKey)
        {
            x1 = forestPaths.GetCrossroad(firstKey).Position.X;
            y1 = forestPaths.GetCrossroad(firstKey).Position.Y;
            x2 = forestPaths.GetCrossroad(secondKey).Position.X;
            y2 = forestPaths.GetCrossroad(secondKey).Position.Y;

            double weight = forestPaths.GetRoadWeight(firstKey, secondKey);
            string formatWeight = string.Format("{0:0.##}", weight);

            LineBuilder builder = new LineBuilder();
            builder.BuildName($"{firstKey}_{secondKey}");
            builder.BuildStroke(Brushes.Gray);
            builder.BuildStrokeThickness(3);
            builder.BuildCursor(Cursors.Hand);
            builder.BuildCoordinates(x1, x2, y1, y2);
            builder.BuildToolTip($"{firstKey}_{secondKey} ({formatWeight})");
            return builder;
        }

        private void DrawParamsInit()
        {
            height = drawParams.Height;
            width = drawParams.Width;
            fill = drawParams.Fill;
            stroke = drawParams.Stroke;
            thickness = drawParams.Thickness;
            cursor = drawParams.Cursor;
        }
    }
}
