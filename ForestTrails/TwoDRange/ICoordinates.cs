using System.Windows;

namespace ForestTrails.TwoDRange
{
    public interface ICoordinates<T>
    {
        T X { get; }
        T Y { get; }
    }
}