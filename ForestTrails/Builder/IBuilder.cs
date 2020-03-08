using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace ForestTrails.Builder
{
    interface IBuilder
    {
        void Reset();
        void BuildName(string name);
        void BuildFill(Brush brush);
        void BuildStroke(Brush brush);
        void BuildStrokeThickness(double thickness);
        void BuildCursor(Cursor cursor);
        void BuildToolTip(string tip);

    }
}
