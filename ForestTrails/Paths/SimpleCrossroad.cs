using ForestTrails.Paths.DrawInformation;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ForestTrails.Paths
{
    [Serializable]
    public class SimpleCrossroad : Crossroad
    { 
        public override CrossroadDrawInformation GetDrawInformation()
        {
            return new CrossroadDrawInformation()
            {
                Height = 10,
                Width = 10,
                Fill = Brushes.Black,
                Stroke = Brushes.DarkGray,
                Thickness = 1,
                Cursor = Cursors.Hand
            };
        }

        public override CrossroadHighlightedDrawInformation GetHighlightedDrawInformation()
        {
            return new CrossroadHighlightedDrawInformation()
            {
                Height = 13,
                Width = 13,
                Stroke = Brushes.Red,
                Thickness = 3
            };
        }
    }
}