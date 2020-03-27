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
        private Leaf referenceLeftSibling = null;


        CommonNode RootNode { get; set; }

        public TwoDRangeTree(TData[] datas, IComparer<TData> x, IComparer<TData> y)
        {
            comparerByX = x;
            comparerByY = y;
            RootNode = Build(datas);
        }

        private CommonNode Build(TData[] datas)
        {
            if (datas.Length == 0 || datas == null) return null;
            bool xDimension = true;
            return (CommonNode) BuildTree(datas, null, xDimension);
        }

        private Node BuildTree(TData[] datas, CommonNode otherDimensionNode, bool xDimension)
        {
            Node pointer = BuildSubtree(datas, null, xDimension);
            if(otherDimensionNode != null)
            {
                (pointer as CommonNode).OtherDimensionNode = otherDimensionNode;
            }
            return pointer;
        }

        private Node BuildSubtree(TData[] datas, CommonNode parent, bool xDimension)
        {
            Node pointer = null;
            if(datas.Length > 1)
            {
                CommonNode node = new CommonNode();
                if (xDimension)
                {
                    Array.Sort(datas, comparerByX);
                }
                else
                {
                    Array.Sort(datas, comparerByY);
                }
                node.Min = datas.First();
                node.Max = datas.Last();

                TData[] datasL = datas.Take(datas.Length / 2).ToArray();
                TData[] datasR = datas.Skip(datas.Length / 2).ToArray();

                node.LeftChild = BuildSubtree(datasL, node, xDimension);
                node.RightChild = BuildSubtree(datasR, node, xDimension);
                pointer = node;
            }
            else
            {
                Leaf leaf = new Leaf();
                leaf.Data = datas.First();
                if(referenceLeftSibling != null)
                {
                    referenceLeftSibling.RightSibling = leaf;
                    leaf.LeftSibling = referenceLeftSibling;
                }
                referenceLeftSibling = leaf;
                pointer = leaf;
            }
            
            if(xDimension && pointer is CommonNode n)
            {
                n.OtherDimensionNode = (CommonNode) BuildTree(datas, n, !xDimension);
            }

            pointer.XDimension = xDimension;
            pointer.Parent = parent;
            return pointer;
        }

        private abstract class Node
        {
            public bool XDimension { get; set; }
            public CommonNode Parent { get; set; }
        }

        private class CommonNode : Node
        {
            public TData Min { get; set; }
            public TData Max { get; set; }
            
            public Node LeftChild { get; set; }
            public Node RightChild { get; set; }
            public CommonNode OtherDimensionNode { get; set; }
            
        }

        private class Leaf : Node
        {
            public TData Data { get; set; }
            public Leaf LeftSibling { get; set; }
            public Leaf RightSibling { get; set; }
        }

    }
}
