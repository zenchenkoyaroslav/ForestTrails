using System;
using System.Collections.Generic;

namespace ForestTrails
{
    [Serializable]
    public class Graph<TKey, TData, TWeight> where TWeight : IComparable
    {
        private Dictionary<TKey, Vertex> Vertices { get; set; }

        public Graph()
        {
            Vertices = new Dictionary<TKey, Vertex>();
        }


        private void AddVertex(TKey key, Vertex vertex)
        {
            Vertices.Add(key, vertex);
        }

        public void AddData(TKey key, TData data)
        {
            Vertex vertex = new Vertex(data);
            AddVertex(key, vertex);
        }
        public void EditData(TKey key, TData data)
        {
            Vertices[key].Data = data;
        }

        internal void Clear()
        {
            Vertices.Clear();
        }

        public void RemoveDataUndirected(TKey key)
        {
            if (!Vertices.ContainsKey(key))
            {
                throw new ArgumentException();
            }
            List<TKey> keys = new List<TKey>();
            foreach (var edge in Vertices[key].Edges)
            {
                keys.Add(edge.Key);
            }
            for(int i = 0; i < keys.Count; i++)
            {
                RemoveEdgeUndirected(keys[i], key);
            }
            
            Vertices.Remove(key);
        }

        public void RemoveDataDirected(TKey key)
        {
            if (!Vertices.ContainsKey(key))
            {
                throw new ArgumentException();
            }
            List<TKey> keys = new List<TKey>();
            foreach (var vertex in Vertices)
            {
                if (vertex.Value.HasEdge(key))
                {
                    keys.Add(vertex.Key);
                }
            }
            for (int i = 0; i < keys.Count; i++)
            {
                RemoveEdgeDirected(keys[i], key);
            }
            Vertices.Remove(key);
        }

        public TData GetData(TKey key)
        {
            return Vertices[key].Data;
        }

        public List<TKey> GetKeys()
        {
            return new List<TKey>(Vertices.Keys);
        }

        public List<TData> GetDatas()
        {
            List<TData> datas = new List<TData>();
            foreach (var vertex in Vertices)
            {
                datas.Add(vertex.Value.Data);
            }
            return datas;
        }

        public List<TData> GetNextDatas(TKey dataKey)
        {
            List<TData> datas = new List<TData>();
            foreach (var edge in Vertices[dataKey].Edges)
            {
                TData data = GetData(edge.Key);
                datas.Add(data);
            }
            return datas;
        }

        public void AddEdgeDirected(TKey from, TKey to, TWeight weight)
        {
            if(!Vertices.ContainsKey(from) || !Vertices.ContainsKey(to))
            {
                throw new ArgumentException();
            }
            Vertices[from].AddEdge(to, weight);
        }

        public void AddEdgeUndirected(TKey firstKey, TKey secondKey, TWeight weight)
        {
            if (!Vertices.ContainsKey(firstKey) || !Vertices.ContainsKey(secondKey))
            {
                throw new ArgumentException();
            }
            Vertices[firstKey].AddEdge(secondKey, weight);
            Vertices[secondKey].AddEdge(firstKey, weight);
        }

        public void RemoveEdgeDirected(TKey from, TKey to)
        {
            Vertices[from].RemoveEdge(to);
        }

        public void RemoveEdgeUndirected(TKey firstKey, TKey secondKey)
        {
            Vertices[firstKey].RemoveEdge(secondKey);
            Vertices[secondKey].RemoveEdge(firstKey);
        }

        public TWeight GetWeight(TKey firstKey, TKey secondKey)
        {
            return Vertices[firstKey].Edges[secondKey];
        }


        [Serializable]
        private class Vertex
        {
            public TData Data { get; set; }
            public Dictionary<TKey, TWeight> Edges { get; set; }

            public Vertex(TData data)
            {
                Data = data;
                Edges = new Dictionary<TKey, TWeight>();
            }

            public void AddEdge(TKey nextVertex, TWeight weight)
            {
                Edges.Add(nextVertex, weight);
            }

            public void RemoveEdge(TKey nextVertex)
            {
                Edges.Remove(nextVertex);
            }

            public bool HasEdge(TKey vertex)
            {
                return Edges.ContainsKey(vertex);
            }
        }
    }
}
