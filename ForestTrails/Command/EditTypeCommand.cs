using ForestTrails.Builder;
using ForestTrails.Paths;
using ForestTrails.Paths.DrawInformation;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ForestTrails.Command
{
    class EditTypeCommand : ICommand
    {
        private Canvas canvas;
        private Ellipse ellipse;
        private ForestPaths forestPaths;


        private ICrossroad crossroad;
        private ICrossroad newCrossroad;
        private CrossroadContext context;
        private string key;

        private CrossroadDrawInformation drawParams;

        private Point point;
        private double height;
        private double width;
        private Brush fill;
        private Brush stroke;
        private double thickness;

        public EditTypeCommand(object sender, Ellipse ellipse, ForestPaths forestPaths, CrossroadContext context)
        {
            canvas = (Canvas)sender;
            this.ellipse = ellipse;
            this.forestPaths = forestPaths;
            key = ellipse.Name;
            this.context = context;
            drawParams = context.GetDrawInformation();
            crossroad = forestPaths.GetCrossroad(key);
            newCrossroad = context.Сrossroad;
            point = forestPaths.GetCrossroad(key).Position;
            DrawParamsInit();
        }

        public void Execute()
        {
            crossroadInit();
            forestPaths.EditCrossroad(key, newCrossroad);
            ellipseRedraw();
            Canvas.SetLeft(ellipse, crossroad.Position.X - height / 2);
            Canvas.SetTop(ellipse, crossroad.Position.Y - width / 2);

        }

        private void ellipseRedraw()
        {
            ellipse.Height = height;
            ellipse.Width = width;
            ellipse.Fill = fill;
            ellipse.Stroke = stroke;
            ellipse.StrokeThickness = thickness;
        }

        private void crossroadInit()
        {
            newCrossroad.Key = crossroad.Key;
            newCrossroad.Position = crossroad.Position;
        }

        private void DrawParamsInit()
        {
            height = drawParams.Height;
            width = drawParams.Width;
            fill = drawParams.Fill;
            stroke = drawParams.Stroke;
            thickness = drawParams.Thickness;
        }
    }
}
