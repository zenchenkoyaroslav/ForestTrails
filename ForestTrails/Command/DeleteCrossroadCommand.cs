using ForestTrails.Paths;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace ForestTrails.Command
{
    class DeleteCrossroadCommand : ICommand
    {
        private Canvas canvas;
        private Ellipse ellipse;
        private ForestPaths forestPaths;

        public DeleteCrossroadCommand(object sender, Ellipse ellipse, ForestPaths forestPaths)
        {
            canvas = (Canvas)sender;
            this.ellipse = ellipse;
            this.forestPaths = forestPaths;
        }

        public void Execute()
        {
            string key = ellipse.Name;
            canvas.Children.Remove(ellipse);
            forestPaths.RemoveCrossroad(key);
            DeleteConnectedLines(key);
        }

        private void DeleteConnectedLines(string key)
        {
            List<Line> lines = new List<Line>();
            foreach (var element in canvas.Children)
            {
                if (element is Line line)
                {
                    string[] keys = line.Name.Split('_');
                    if (keys.Contains(key))
                    {
                        lines.Add(line);
                    }
                }
            }
            for (int i = 0; i < lines.Count; i++)
            {
                canvas.Children.Remove(lines[i]);
            }
        }
    }
}
