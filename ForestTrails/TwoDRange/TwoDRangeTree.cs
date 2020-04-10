using ForestTrails.Paths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ForestTrails.TwoDRange
{
    public class TwoDRangeTree<TData, T> 
        where TData : ICoordinates<T>
        where T : IComparable
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
            Build(datas);
        }


        public List<TData> FindRange(T x1, T y1, T x2, T y2, ref List<TData> result)
        {
            FindRange(x1, y1, x2, y2, RootNode, true, ref result);
            return result;
        }

        private void FindRange(T x1, T y1, T x2, T y2, Node node, bool xDimension, ref List<TData> result)
        {
            if (node != null)
            {
                if (node is Leaf leaf)
                {
                    if (IsInsideRangeX(x1, x2, leaf) &&
                        IsInsideRangeY(y1, y2, leaf))
                    {
                        result.Add(leaf.Data);
                    }
                }
                else
                {
                    CommonNode common = node as CommonNode;
                    if (xDimension)
                    {
                        if (x1.CompareTo(common.Min.X) <= 0 &&
                            common.Max.X.CompareTo(x2) <= 0)
                        {
                            FindRange(x1, y1, x2, y2, common.OtherDimensionNode, !xDimension, ref result);
                        }
                        else if (common.Min.X.CompareTo(x1) <= 0 ||
                            x2.CompareTo(common.Max.X) <= 0)
                        {
                            FindRange(x1, y1, x2, y2, common.LeftChild, xDimension, ref result);
                            FindRange(x1, y1, x2, y2, common.RightChild, xDimension, ref result);
                        }
                    }
                    else
                    {
                        if (y1.CompareTo(common.Min.Y) <= 0 &&
                            common.Max.Y.CompareTo(y2) <= 0)
                        {
                            SubtreeView(x1, y1, x2, y2, common, ref result);
                        }
                        else if (common.Min.Y.CompareTo(y1) <= 0 &&
                            y2.CompareTo(common.Max.Y) <= 0)
                        {
                            FindRange(x1, y1, x2, y2, common.LeftChild, xDimension, ref result);
                            FindRange(x1, y1, x2, y2, common.RightChild, xDimension, ref result);
                        }
                    }
                }
            }
        }

        private void SubtreeView(T x1, T y1, T x2, T y2, CommonNode node, ref List<TData> result)
        {
            Leaf pointer = GetMostLeftChild(node);
            do
            {
                if (IsInsideRangeX(x1, x2, pointer) &&
                    IsInsideRangeY(y1, y2, pointer))
                {
                    result.Add(pointer.Data);
                }
                pointer = pointer.RightSibling;
            } while (pointer != null);
        }

        private Leaf GetMostLeftChild(CommonNode node)
        {
            Node pointer = node.LeftChild;
            if (pointer is Leaf leaf) return leaf;
            else return GetMostLeftChild(pointer as CommonNode);
        }

        public TData Find(T x, T y)
        {
            return FindLeaf(x, y).Data;
        }

        private Leaf FindLeaf(T x, T y)
        {
            if(RootNode != null && RootNode is Leaf root)
            {
                if(root.Data.X.CompareTo(x) == 0 &&
                    root.Data.Y.CompareTo(y) == 0)
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
                if (HasMatch(x, y, child))
                {
                    return child as Leaf;
                }
                child = pointer.RightChild;
                if (HasMatch(x, y, child))
                {
                    return child as Leaf;
                }

                CommonNode nextPointer = null;
                if (IsInsideRange(x, pointer.LeftChild))
                {
                    nextPointer = pointer.LeftChild as CommonNode;
                }
                else if (IsInsideRange(x, pointer.RightChild))
                {
                    nextPointer = pointer.RightChild as CommonNode;
                }
                pointer = nextPointer;
            }
        }

        private bool IsInsideRange(T x, Node node)
        {
            return node != null &&
                node is CommonNode commonNode &&
                commonNode.Min.X.CompareTo(x) <= 0 &&
                x.CompareTo(commonNode.Max.X) <= 0;
        }

        private bool IsInsideRangeX(T x1, T x2, Leaf leaf)
        {
            return x1.CompareTo(leaf.Data.X) <= 0 &&
                leaf.Data.X.CompareTo(x2) <= 0;
        }

        private bool IsInsideRangeY(T y1, T y2, Leaf leaf)
        {
            return y1.CompareTo(leaf.Data.Y) <= 0 &&
                leaf.Data.Y.CompareTo(y2) <= 0;
        }

        private bool HasMatch(T x, T y, Node node)
        {
            return node != null &&
                node is Leaf leaf &&
                leaf.Data.X.CompareTo(x) == 0 &&
                leaf.Data.Y.CompareTo(y) == 0;
        }

        public void Build(TData[] datas)
        {
            RootNode = BuildFromRoot(datas);
            BuildSiblingLinks(RootNode);
        }

        private CommonNode BuildFromRoot(TData[] datas)
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

        private void BuildSiblingLinks(Node node)
        {
            if(node is CommonNode common)
            {
                if (common.XDimension)
                {
                    BuildSiblingLinks(common.OtherDimensionNode);
                }
                BuildSiblingLinks(common.LeftChild);
            }
            else if(node is Leaf leaf)
            {
                BuildLinks(leaf);
            }
        }

        private void BuildLinks(Leaf leaf)
        {
            Leaf rightSibling = GetRightSibling(leaf);
            if(rightSibling != null)
            {
                leaf.RightSibling = rightSibling;
                rightSibling.LeftSibling = leaf;
                BuildLinks(leaf.RightSibling);
            }
        }

        private Leaf GetRightSibling(Leaf leaf)
        {
            return GoUp(leaf);
        }

        private Leaf GoUp(Node node)
        {
            CommonNode pointer = node.Parent;
            
            if (pointer == null) return null;
            if (node == pointer.RightChild)
            {
                return GoUp(pointer);
            }
            else
            {
                if(pointer.RightChild is CommonNode commonChild)
                {
                    return GetMostLeftChild(commonChild);
                }
                else
                {
                    return pointer.RightChild as Leaf;
                }
                
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
