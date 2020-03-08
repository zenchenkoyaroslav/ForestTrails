using C5;
using ForestTrails.Paths;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ForestTrails
{
    public class AStar
    {
        

        private Dictionary<ICrossroad, double> pathLengthFromStart = new Dictionary<ICrossroad, double>();
        private Dictionary<ICrossroad, double> heuristicEstimatePathLength = new Dictionary<ICrossroad, double>();
        private Dictionary<ICrossroad, double> estimateFullPathLength = new Dictionary<ICrossroad, double>();
        private Dictionary<ICrossroad, ICrossroad> cameFrom = new Dictionary<ICrossroad, ICrossroad>();

        private class Comparer : IComparer<ICrossroad>
        {
            Dictionary<ICrossroad, double> estimateFullPathLength;
            public Comparer(Dictionary<ICrossroad, double> estimateFullPathLength)
            {
                this.estimateFullPathLength = estimateFullPathLength;
            }
            public int Compare(ICrossroad x, ICrossroad y)
            {
                return new CaseInsensitiveComparer().Compare(estimateFullPathLength[x], estimateFullPathLength[y]);
            }
        }

        private double GetEstimateFullPathLength(ICrossroad crossroad)
        {
             return pathLengthFromStart[crossroad] + heuristicEstimatePathLength[crossroad];

        }

        public List<ICrossroad> FindWay(ForestPaths forestPaths, 
            CampCrossroad start, BusStopCrossroad goal)
        {
            Comparer comparer = new Comparer(estimateFullPathLength);
            var openSet = new IntervalHeap<ICrossroad>(comparer);
            var closedSet = new List<ICrossroad>();
            

            cameFrom.Add(start, null);
            pathLengthFromStart.Add(start, 0);
            heuristicEstimatePathLength.Add(start, forestPaths.
                GetDistanceBetweenCrossroad(start, goal));
            estimateFullPathLength.Add(start, GetEstimateFullPathLength(start));
            openSet.Add(start);

            while(openSet.Count > 0)
            {
                ICrossroad current = openSet.DeleteMin();

                if (current == goal)
                    return GetPathForCrossroad(current);

                closedSet.Add(current);

                foreach (var next in forestPaths.GetNextCrossroads(current))
                {
                    double newPathLengthFromStart = pathLengthFromStart[current]
                        + forestPaths.GetDistanceBetweenCrossroad(current, next);
                    if (closedSet.Contains(next))
                        continue;
                    
                    if(!pathLengthFromStart.ContainsKey(next) ||
                        newPathLengthFromStart < pathLengthFromStart[next])
                    {
                        if (!pathLengthFromStart.ContainsKey(next))
                            pathLengthFromStart.Add(next, default);
                        pathLengthFromStart[next] = newPathLengthFromStart;

                        if (!heuristicEstimatePathLength.ContainsKey(next))
                            heuristicEstimatePathLength.Add(next, forestPaths.
                                GetDistanceBetweenCrossroad(next, goal));

                        if (!estimateFullPathLength.ContainsKey(next))
                            estimateFullPathLength.Add(next, default);
                        estimateFullPathLength[next] = GetEstimateFullPathLength(next);

                        if (!cameFrom.ContainsKey(next))
                            cameFrom.Add(next, default);
                        cameFrom[next] = current;
                        if (!openSet.Contains(next))
                        {
                            openSet.Add(next);
                        }
                    }
                }
            }
            return null;
        }

        private List<ICrossroad> GetPathForCrossroad(ICrossroad crossroad)
        {
            var result = new List<ICrossroad>();
            var current = crossroad;
            while(current != null)
            {
                result.Add(current);
                current = cameFrom[current];
            }
            result.Reverse();
            return result;
        }
    }
}
