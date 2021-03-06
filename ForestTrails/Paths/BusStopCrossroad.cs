﻿using ForestTrails.Paths.DrawInformation;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ForestTrails.Paths
{
    [Serializable]
    public class BusStopCrossroad : Crossroad
    { 
        
        public override CrossroadDrawInformation GetDrawInformation()
        {
            return new CrossroadDrawInformation()
            {
                Height = 15,
                Width = 15,
                Fill = Brushes.Green,
                Stroke = Brushes.LightGreen,
                Thickness = 2,
                Cursor = Cursors.Hand
            };
        }

        public override CrossroadHighlightedDrawInformation GetHighlightedDrawInformation()
        {
            return new CrossroadHighlightedDrawInformation()
            {
                Height = 18,
                Width = 18,
                Stroke = Brushes.Red,
                Thickness = 3
            };
        }
    }
}