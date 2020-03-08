using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ForestTrails.Paths.DrawInformation
{
    interface IDrawInformation
    {
        double Height { get; set; }
        double Width { get; set; }
        Brush Stroke { get; set; }
        double Thickness { get; set; }
    }
}
