using ForestTrails.Paths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ForestTrails
{
    public class TwoDRangeTree<TData> where TData : IPoint 
    {
        private IComparer<TData> comparerByX;
        private IComparer<TData> comparerByY;

        public int Count { private get; set; } = 0;


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
            else
            {
                Add(data, RootNode, true);
            }
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
                else 
                {
                    CommonNode pointer;
                    if (leaf == parent.LeftChild)
                    {
                        parent.LeftChild = BuildSubtree(datas, parent, xDimension);
                        pointer = parent.LeftChild as CommonNode;
                    }
                    else
                    {
                        parent.RightChild = BuildSubtree(datas, parent, xDimension);
                        pointer = parent.RightChild as CommonNode;
                    }
                    Leaf rightChild = pointer.RightChild as Leaf;
                    Leaf rightSibling = GetRightSibling(rightChild);
                    rightSibling.LeftSibling = rightChild;
                    rightChild.RightSibling = rightSibling;
                }

                if (xDimension)
                {
                    Count++;
                }
            }
        }

        

        public TData Find(Point point)
        {
            return FindLeaf(point).Data;
        }

        private Leaf FindLeaf(Point point)
        {
            double queryX = point.X;
            double queryY = point.Y;
            if(RootNode != null && RootNode is Leaf root)
            {
                if(root.Data.point.X == queryX &&
                    root.Data.point.Y == queryY)
                {
                    return root;
                }
                else
                {
                    return null;
                }
            }
            CommonNode pointer = RootNode as CommonNode;
            while (true)
            {
                if (pointer == null) return null;
                Node child = pointer.LeftChild;
                if (HasMatch(point, child))
                {
                    return child as Leaf;
                }
                child = pointer.RightChild;
                if (HasMatch(point, child))
                {
                    return child as Leaf;
                }

                CommonNode nextPointer = null;
                if (IsInsideRange(queryX, pointer.LeftChild))
                {
                    nextPointer = pointer.LeftChild as CommonNode;
                }
                else if (IsInsideRange(queryX, pointer.RightChild))
                {
                    nextPointer = pointer.RightChild as CommonNode;
                }
                pointer = nextPointer;
            }
        }

        private bool IsInsideRange(double queryX, Node node)
        {
            return node != null &&
                node is CommonNode commonNode &&
                commonNode.Min.point.X <= queryX &&
                queryX <= commonNode.Max.point.X;
        }

        private bool HasMatch(Point point, Node node)
        {
            return node != null &&
                node is Leaf leaf &&
                leaf.Data.point.X == point.X &&
                leaf.Data.point.Y == point.Y;
        }

        public CommonNode Build(TData[] datas)
        {
            if (datas.Length == 0 || datas == null) return null;
            Count = datas.Length;
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
            return LeftSibGoUp(leaf);
        }

        private Leaf LeftSibGoUp(Node node)
        {
            if (node.Parent != null)
            {
                CommonNode parent = node.Parent;
                if (parent.LeftChild != node)
                {
                    return LeftSibGoLeft(parent);
                }
                else
                {
                    return LeftSibGoUp(parent);
                }
            }
            else
            {
                return null;
            }
            
        }

        private Leaf LeftSibGoLeft(CommonNode parent)
        {
            Node node = parent.LeftChild;
            if(node is Leaf leaf)
            {
                return leaf;
            }
            else
            {
                return LeftSibGoRight(node as CommonNode);
            }
        }

        private Leaf LeftSibGoRight(CommonNode parent)
        {
            Node node = parent.RightChild;
            if(node is Leaf leaf)
            {
                return leaf;
            }
            else
            {
                return LeftSibGoRight(node as CommonNode);
            }
        }

        private Leaf GetRightSibling(Leaf leaf)
        {
            return RightSibGoUp(leaf);
        }

        private Leaf RightSibGoUp(Node node)
        {
            if (node.Parent != null)
            {
                CommonNode parent = node.Parent;
                if (parent.RightChild != node)
                {
                    return RightSibGoRight(parent);
                }
                else
                {
                    return RightSibGoUp(parent);
                }
            }
            else
            {
                return null;
            }
        }

        private Leaf RightSibGoRight(CommonNode parent)
        {
            Node node = parent.RightChild;
            if (node is Leaf leaf)
            {
                return leaf;
            }
            else
            {
                return RightSibGoLeft(node as CommonNode);
            }
        }

        private Leaf RightSibGoLeft(CommonNode parent)
        {
            Node node = parent.LeftChild;
            if (node is Leaf leaf)
            {
                return leaf;
            }
            else
            {
                return RightSibGoLeft(node as CommonNode);
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
