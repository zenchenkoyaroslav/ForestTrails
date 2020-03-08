using ForestTrails.Paths.DrawInformation;

namespace ForestTrails.Paths
{
    public class CrossroadContext
    {
        public ICrossroad Сrossroad { get; set; }

        public CrossroadContext()
        { }

        public CrossroadContext(ICrossroad crossroad)
        {
            Сrossroad = crossroad;
        }

        public CrossroadDrawInformation GetDrawInformation()
        {
            return Сrossroad.GetDrawInformation();
        }

        public CrossroadHighlightedDrawInformation GetHighlightedDrawInformation()
        {
            return Сrossroad.GetHighlightedDrawInformation();
        }
    }
}
