﻿using ForestTrails.Paths.DrawInformation;
using ForestTrails.TwoDRange;
using System;
using System.Windows;

namespace ForestTrails.Paths
{
    public interface ICrossroad : ICoordinates<double>
    {
        Point Position { get; set; }
        string Key { get; set; }

        CrossroadDrawInformation GetDrawInformation();
        CrossroadHighlightedDrawInformation GetHighlightedDrawInformation();
    }
}
