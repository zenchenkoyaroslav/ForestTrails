using System.Windows.Input;
using System.Windows.Media;

namespace ForestTrails.Paths.DrawInformation
{
    public class CrossroadDrawInformation : IDrawInformation
    {
        public double Height { get; set; }
        public double Width { get; set; }
        public Brush Fill { get; set; }
        public Brush Stroke { get; set; }
        public double Thickness { get; set; }
        public Cursor Cursor { get; set; }
    }
}
