using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ForestTrails.Paths
{
    [Serializable]
    public class ForestPaths : INotifyPropertyChanged
    {
        private Graph<string, ICrossroad, double> Graph { get; set; }

        private int crossroadsKeyNumber = 0;
        private int crossroadsCount = 0;
        public int CrossroadsCount {
            get
            {
                return crossroadsCount;
            }
            private set
            {
                crossroadsCount = value;
                OnPropertyChanged("CrossroadsCount");
            }
        }
        private int roadsCount = 0;
        public int RoadsCount
        {
            get
            {
                return roadsCount;
            }
            private set
            {
                roadsCount = value;
                OnPropertyChanged("RoadsCount");
            }
        }

        private string predicate = "C";

        [field: NonSerialized()]
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public ForestPaths()
        {
            Graph = new Graph<string, ICrossroad, double>();
        }

        public void Clear()
        {
            Graph.Clear();
            CrossroadsCount = 0;
            RoadsCount = 0;
        }

        public void EditCrossroad(string key, ICrossroad crossroad)
        {
            Graph.EditData(key, crossroad);
        }

        public string AddCrossroad(ICrossroad crossroad)
        {
            CrossroadsCount++;
            crossroadsKeyNumber++;
            string key = GenerateKey(predicate, crossroadsKeyNumber);
            crossroad.Key = key;
            Graph.AddData(key, crossroad);
            return key;
        }

        public ICrossroad GetCrossroad(string key)
        {
            return Graph.GetData(key);
        }

        public List<ICrossroad> GetNextCrossroads(string key)
        {
            return Graph.GetNextDatas(key);
        }

        public List<ICrossroad> GetNextCrossroads(ICrossroad crossroad)
        {
            return GetNextCrossroads(crossroad.Key);
        }

        public List<ICrossroad> GetCrossroads()
        {
            return Graph.GetDatas();
        }

        public List<CampCrossroad> GetCampCrossroads()
        {
            List<CampCrossroad> camps = new List<CampCrossroad>();
            foreach (var crossroad in GetCrossroads())
            {
                if (crossroad is CampCrossroad camp)
                    camps.Add(camp);
            }
            return camps;
        }

        public List<BusStopCrossroad> GetBusStopCrossroads()
        {
            List<BusStopCrossroad> busStops = new List<BusStopCrossroad>();
            foreach (var crossroad in GetCrossroads())
            {
                if (crossroad is BusStopCrossroad busStop)
                    busStops.Add(busStop);
            }
            return busStops;
        }

        public void RemoveCrossroad(string key)
        {
            CrossroadsCount--;
            RoadsCount -= GetNextCrossroads(key).Count;
            Graph.RemoveDataUndirected(key);
        }

        public double GetDistanceBetweenCrossroad(string firstKey, string secondKey)
        {
            ICrossroad firstCrossroad = GetCrossroad(firstKey);
            ICrossroad secondCrossroad = GetCrossroad(secondKey);
            return GetDistanceBetweenCrossroad(firstCrossroad, secondCrossroad);
        }

        public double GetDistanceBetweenCrossroad(ICrossroad firstCrossroad, ICrossroad secondCrossroad)
        {
            double x1 = firstCrossroad.Position.X;
            double x2 = secondCrossroad.Position.X;
            double y1 = firstCrossroad.Position.Y;
            double y2 = secondCrossroad.Position.Y;
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }

        public void AddRoad(string firstKey, string secondKey)
        {
            try
            {
                double weight = GetDistanceBetweenCrossroad(firstKey, secondKey);
                Graph.AddEdgeUndirected(firstKey, secondKey, weight);
                RoadsCount++;
            }
            catch (Exception)
            {

            }
        }

        public void RemoveRoad(string firstKey, string secondKey)
        {
            RoadsCount--;
            Graph.RemoveEdgeUndirected(firstKey, secondKey);
        }

        public double GetRoadWeight(string firstKey, string secondKey)
        {
            return Graph.GetWeight(firstKey, secondKey);
        }

        private string GenerateKey(string predicate, int number)
        {
            return predicate + number;
        }

        internal void Update(ForestPaths newForestPath)
        {
            Graph = newForestPath.Graph;
            crossroadsKeyNumber = newForestPath.crossroadsKeyNumber;
            CrossroadsCount = newForestPath.CrossroadsCount;
            RoadsCount = newForestPath.RoadsCount;
        }
    }
}
