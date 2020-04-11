using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForestTrails.Paths
{
    class CrossroadComparerByY : IComparer<ICrossroad>
    {
        public int Compare(ICrossroad first, ICrossroad second)
        {
            return first.Position.Y.CompareTo(second.Position.Y);
        }
    }
}
