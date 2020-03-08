using ForestTrails.Builder;
using ForestTrails.Paths;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;


namespace ForestTrails.Command
{
    class DrawConnectionCommand : ICommand
    {

        private Canvas canvas;
        private ForestPaths forestPaths;
        private Ellipse firstEllipse;
        private Ellipse secondEllipse;
        
        private string firstKey;
        private string secondKey;



        public DrawConnectionCommand(object sender, Ellipse firstEllipse, Ellipse secondEllipse, ForestPaths forestPaths)
        {
            canvas = (Canvas)sender;
            this.forestPaths = forestPaths;
            this.firstEllipse = firstEllipse;
            this.secondEllipse = secondEllipse;

            firstKey = firstEllipse.Name;
            secondKey = secondEllipse.Name;

            
        }


        public void Execute()
        {
            forestPaths.AddRoad(firstKey, secondKey);

            BuildDirector director = new BuildDirector();
            LineBuilder builder = director.BuildLine(forestPaths, firstKey, secondKey);
            Line line = builder.GetLine();

            Canvas.SetZIndex(line, 1);
            canvas.Children.Add(line);
        }
    }
}