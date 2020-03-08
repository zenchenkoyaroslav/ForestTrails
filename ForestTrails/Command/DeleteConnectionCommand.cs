using ForestTrails.Paths;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace ForestTrails.Command
{
    class DeleteConnectionCommand : ICommand
    {

        private Canvas canvas;
        private Line line;
        private ForestPaths forestPaths;

        public DeleteConnectionCommand(object sender, Line line, ForestPaths forestPaths)
        {
            canvas = (Canvas)sender;
            this.line = line;
            this.forestPaths = forestPaths;
        }

        public void Execute()
        {
            string[] keys = line.Name.Split('_');
            canvas.Children.Remove(line);
            forestPaths.RemoveRoad(keys[0], keys[1]);
        }
    }
}
