using System.Windows;

namespace ForestTrails.TwoDRangeTree
{
    public interface ICoordinates<T>
    {
        T X { get; }
        T Y { get; }
    }
}