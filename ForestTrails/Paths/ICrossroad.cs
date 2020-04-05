﻿using ForestTrails.Paths.DrawInformation;
using System;
using System.Windows;

namespace ForestTrails.Paths
{
    public interface ICrossroad : IPoint
    {
        Point Position { get; set; }
        string Key { get; set; }

        CrossroadDrawInformation GetDrawInformation();
        CrossroadHighlightedDrawInformation GetHighlightedDrawInformation();
    }
}
