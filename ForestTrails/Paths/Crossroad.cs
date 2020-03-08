using ForestTrails.Paths.DrawInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ForestTrails.Paths
{
    [Serializable]
    public abstract class Crossroad : ICrossroad
    {
        public Point Position { get; set; }
        public string Key { get; set; }

        public Crossroad() { }

        public Crossroad(Point point, string key)
        {
            Position = point;
            Key = key;
        }

        public Crossroad(ICrossroad crossroad)
        {
            Position = crossroad.Position;
            Key = crossroad.Key;
        }

        public abstract CrossroadDrawInformation GetDrawInformation();

        public abstract CrossroadHighlightedDrawInformation GetHighlightedDrawInformation();

    }
}
