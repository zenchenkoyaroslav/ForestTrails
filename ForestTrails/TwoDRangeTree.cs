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

        public TwoDRangeTree(IComparer<TData> comparerByX, IComparer<TData> comparerByY)
        {
            this.comparerByX = comparerByX;
            this.comparerByY = comparerByY;
        }

        public TwoDRangeTree(TData[] datas, IComparer<TData> x, IComparer<TData> y)
            : this(x, y)
        {
            RootNode = Build(datas);
        }

        public void Add(TData data)
        {
            if(RootNode == null)
            {
                RootNode = new Leaf();
                (RootNode as Leaf).Data = data;
            }
            Add(data, RootNode, true);
        }

        private void Add(TData data, Node node, bool xDimension)
        {
            if(node is CommonNode commonNode)
            {
                if (xDimension)
                {
                    Add(data, commonNode.OtherDimensionNode, !xDimension);
                    if(commonNode.RightChild is CommonNode rightChild &&
                        comparerByX.Compare(rightChild.Min, data) <= 0)
                    {
                        Add(data, rightChild, xDimension);
                    }
                    else
                    {
                        Add(data, commonNode.LeftChild, xDimension);
                    }
                }
                else
                {
                    if(commonNode.RightChild is CommonNode rightChild &&
                        comparerByY.Compare(rightChild.Min, data) <= 0)
                    {
                        Add(data, rightChild, xDimension);
                    }
                    else
                    {
                        Add(data, commonNode.LeftChild, xDimension);
                    }
                }
            }
            else
            {
                Leaf leaf = node as Leaf;
                CommonNode parent = leaf.Parent;
                TData[] datas = { data, leaf.Data };
                if (parent == null)
                {
                    RootNode = BuildSubtree(datas, parent, xDimension);
                }
                else if (leaf == parent.LeftChild) 
                {
                    parent.LeftChild = BuildSubtree(datas, parent, xDimension);
                }
                else
                {
                    parent.RightChild = BuildSubtree(datas, parent, xDimension);
                }
                if (xDimension)
                {
                    // TODO: increase elements number
                }
            }
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

                Leaf leftSibling = GetLeftSibling(leaf);
                if(leftSibling != null)
                {
                    leftSibling.RightSibling = leaf;
                    leaf.LeftSibling = leftSibling;
                }
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

        private Leaf GetLeftSibling(Leaf leaf)
        {
            return GoUp(leaf);
        }

        private Leaf GoUp(Node node)
        {
            if (node.Parent != null)
            {
                CommonNode parent = node.Parent;
                if (parent.LeftChild != node)
                {
                    return GoLeft(parent);
                }
                else
                {
                    return GoUp(parent);
                }
            }
            else
            {
                return null;
            }
            
        }

        private Leaf GoLeft(CommonNode parent)
        {
            Node node = parent.LeftChild;
            if(node is Leaf leaf)
            {
                return leaf;
            }
            else
            {
                return GoRight(node as CommonNode);
            }
        }

        private Leaf GoRight(CommonNode parent)
        {
            Node node = parent.RightChild;
            if(node is Leaf leaf)
            {
                return leaf;
            }
            else
            {
                return GoRight(node as CommonNode);
            }
        }

        public abstract class Node
        {
            public bool XDimension { get; set; }
            public CommonNode Parent { get; set; }
        }

        public class CommonNode : Node
        {
            public TData Min { get; set; }
            public TData Max { get; set; }
            
            public Node LeftChild { get; set; }
            public Node RightChild { get; set; }
            public CommonNode OtherDimensionNode { get; set; }
            
        }

        public class Leaf : Node
        {
            public TData Data { get; set; }
            public Leaf LeftSibling { get; set; }
            public Leaf RightSibling { get; set; }
        }

    }
}
