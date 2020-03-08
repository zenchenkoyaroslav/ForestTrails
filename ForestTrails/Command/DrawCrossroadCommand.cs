using ForestTrails.Builder;
using ForestTrails.Paths;
using ForestTrails.Paths.DrawInformation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ForestTrails.Command
{
    class DrawCrossroadCommand : ICommand
    {

        private Canvas canvas;
        private ForestPaths forestPaths;

        private ICrossroad crossroad;
        private CrossroadContext context;

        private string key;

        public DrawCrossroadCommand(object sender, ForestPaths forestPaths, CrossroadContext context)
        {
            canvas = (Canvas)sender;
            this.forestPaths = forestPaths;
            this.context = context;
            crossroad = context.Сrossroad;
        }

        public void Execute()
        {
            key = forestPaths.AddCrossroad(crossroad);


            BuildDirector director = new BuildDirector();
            EllipseBuilder builder = director.BuildEllipse(crossroad);
            Ellipse ellipse = builder.GetEllipse();

            Canvas.SetLeft(ellipse, crossroad.Position.X - ellipse.Height / 2);
            Canvas.SetTop(ellipse, crossroad.Position.Y - ellipse.Width / 2);
            Canvas.SetZIndex(ellipse, 2);
            canvas.Children.Add(ellipse);
        }

        
    }
}
