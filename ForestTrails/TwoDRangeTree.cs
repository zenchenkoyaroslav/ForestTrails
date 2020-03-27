using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForestTrails
{
    public class TwoDRangeTree<TData>
    {
        private IComparer<TData> comparerByX;
        private IComparer<TData> comparerByY;


        Node RootNode { get; set; }

        public TwoDRangeTree(TData[] datas, IComparer<TData> x, IComparer<TData> y)
        {
            comparerByX = x;
            comparerByY = y;
            RootNode = Build(datas);
        }

        
        private abstract class Node
        {
            public bool XDimension { get; set; }
            public Node Parent { get; set; }
        }

        private class CommonNode : Node
        {
            public TData Min { get; set; }
            public TData Max { get; set; }
            
            public Node LeftChild { get; set; }
            public Node RightChild { get; set; }
            public Node OtherDimensionNode { get; set; }
            
        }

        private class Leaf : Node
        {
            public TData Data { get; set; }
            public Node LeftSibling { get; set; }
            public Node RightSibling { get; set; }
        }

    }
}
