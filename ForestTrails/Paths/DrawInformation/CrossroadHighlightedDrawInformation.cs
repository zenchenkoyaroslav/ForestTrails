using System.Windows.Media;

namespace ForestTrails.Paths.DrawInformation
{
    public class CrossroadHighlightedDrawInformation : IDrawInformation
    {
        public double Height { get; set; }
        public double Width { get; set; }
        public Brush Stroke { get; set; }
        public double Thickness { get; set; }
    }
}