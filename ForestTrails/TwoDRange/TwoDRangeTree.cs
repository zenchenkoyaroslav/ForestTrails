using System;
using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        ///  Build method
        ///  Public method for tree building
        ///  Call BuildFromRoot method that build the tree
        ///  and BuildSiblingLinks method that do links between leafs
        /// </summary>
        /// <param name="datas">Elements stored in leafs</param>
        public void Build(TData[] datas)
        {
            RootNode = BuildFromRoot(datas);
            BuildSiblingLinks(RootNode);
        }

        /// <summary>
        ///  BuildFromRoot method
        ///  Check if input datas aren't null, 
        ///  set property Count and call BuildTree method
        /// </summary>
        /// <param name="datas">Elements stored in leafs</param>
        /// <returns>Root of tree</returns>
        private NavigationalNode BuildFromRoot(TData[] datas)
        {
            if (datas.Length == 0 || datas == null)
            {
                Count = 0;
                return null;
            }
            Count = datas.Length;
            bool xDimension = true;
            return (NavigationalNode)BuildTree(datas, null, xDimension);
        }

        /// <summary>
        ///  BuildTree method
        ///  Call BuildSubtree method for X dimension, then for Y dimension
        /// </summary>
        /// <param name="datas">Elements stored in leafs</param>
        /// <param name="otherDimensionNode">X node for Y node</param>
        /// <param name="xDimension">Dimension where we are. If it's true, it's X</param>
        /// <returns>Subtree root</returns>
        private Node BuildTree(TData[] datas, NavigationalNode otherDimensionNode, bool xDimension)
        {
            Node pointer = BuildSubtree(datas, null, xDimension);
            if (otherDimensionNode != null)
            {
                (pointer as NavigationalNode).OtherDimensionNode = otherDimensionNode;
            }
            return pointer;
        }

        /// <summary>
        ///  BuildSubtree method
        ///  This method do most of work
        ///  for tree building.
        /// </summary>
        /// <param name="datas">Elements stored in leafs</param>
        /// <param name="parent">Parent of node, that this method must return</param>
        /// <param name="xDimension">Dimension where we are. If it's true, it's X</param>
        /// <returns>Node of tree</returns>
        private Node BuildSubtree(TData[] datas, NavigationalNode parent, bool xDimension)
        {
            Node pointer = null;
            if (datas.Length > 1)
            {
                // Set range for navigational node
                NavigationalNode node = new NavigationalNode();
                if (xDimension)
                {
                    Array.Sort(datas, comparerByX);
                    node.Min = datas.First().X;
                    node.Max = datas.Last().X;
                }
                else
                {
                    Array.Sort(datas, comparerByY);
                    node.Min = datas.First().Y;
                    node.Max = datas.Last().Y;
                }

                // Halve datas. One half of datas go to left subtree
                // the other one go to right subtree
                TData[] datasL = datas.Take(datas.Length / 2).ToArray();
                TData[] datasR = datas.Skip(datas.Length / 2).ToArray();

                node.LeftChild = BuildSubtree(datasL, node, xDimension);
                node.RightChild = BuildSubtree(datasR, node, xDimension);

                pointer = node;
            }
            else
            {
                // if datas parametr has only one element
                // this one will be a leaf
                Leaf leaf = new Leaf();
                leaf.Data = datas.First();
                pointer = leaf;
            }

            // If we are in X dimension and current node is navigational one,
            // go to Y Dimension and build tree for it
            if (xDimension && pointer is NavigationalNode navigational)
            {
                navigational.OtherDimensionNode = (NavigationalNode)BuildTree(datas, navigational, !xDimension);
            }

            // Set parametrs that are common for both leafs and navigational nodes
            pointer.XDimension = xDimension;
            pointer.Parent = parent;
            return pointer;
        }

        /// <summary>
        ///  BuildSiblingLinks method
        ///  Build links between leafs
        /// </summary>
        /// <param name="node">Current node. Root in first call</param>
        private void BuildSiblingLinks(Node node)
        {
            if (node is NavigationalNode navigationalNode)
            {
                // Go build links on second dimension for
                // every navigational nodes
                if (navigationalNode.XDimension)
                {
                    BuildSiblingLinks(navigationalNode.OtherDimensionNode);
                }
                // Go deeper, until get most left leaf.
                BuildSiblingLinks(navigationalNode.LeftChild);
            }
            else if (node is Leaf leaf)
            {
                // Links build is starting here
                BuildLinks(leaf);
            }
        }

        /// <summary>
        ///  BuildLinks method
        ///  Started with most left leaf
        ///  and do links for all of them
        /// </summary>
        /// <param name="leaf">Current leaf</param>
        private void BuildLinks(Leaf leaf)
        {
            // Get right sibling of current node
            Leaf rightSibling = GetRightSibling(leaf);
            // Set mutual links for current node and
            // its right sibling
            if (rightSibling != null)
            {
                leaf.RightSibling = rightSibling;
                rightSibling.LeftSibling = leaf;
                BuildLinks(leaf.RightSibling);
            }
        }

        /// <summary>
        ///  GetRightSibling method
        ///  Find right sibling for
        ///  current node
        /// </summary>
        /// <param name="leaf">Current node</param>
        /// <returns>Right sibling for current node</returns>
        private Leaf GetRightSibling(Leaf leaf)
        {
            return GoUp(leaf);
        }

        /// <summary>
        ///  GoUp method
        ///  Part of GetRightSibling
        /// </summary>
        /// <param name="node">Current node</param>
        /// <returns>Right sibling for current node</returns>
        private Leaf GoUp(Node node)
        {
            NavigationalNode parent = node.Parent;
            // We got root via its right child
            // All leafs are worked out
            if (parent == null) return null;
            // If we came via right child, go upper
            if (node == parent.RightChild)
            {
                return GoUp(parent);
            }
            else
            {
                // If we came via left child go to most left child of right child
                if (parent.RightChild is NavigationalNode navigationalChild)
                {
                    return GetMostLeftChild(navigationalChild);
                }
                else
                {
                    // Or return right child if it is leaf
                    return parent.RightChild as Leaf;
                }

            }


        }

        /// <summary>
        ///  FindRange method
        ///  Public method of range search
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="result">List of searching elements</param>
        public void FindRange(T x1, T y1, T x2, T y2, ref List<TData> result)
        {
            FindRange(x1, y1, x2, y2, RootNode, true, ref result);
        }


        /// <summary>
        ///  FindRange method
        ///  Method for searching elements in concrete range
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="node"></param>
        /// <param name="xDimension">True is first dimension</param>
        /// <param name="result">List of searching elements</param>
        private void FindRange(T x1, T y1, T x2, T y2, Node node, bool xDimension, ref List<TData> result)
        {
            if (node != null)
            {
                // When get leaf, control its coordinates
                // and add to result list if they in the range
                if (node is Leaf leaf)
                {
                    if (IsInsideRangeX(x1, x2, leaf) &&
                        IsInsideRangeY(y1, y2, leaf))
                    {
                        result.Add(leaf.Data);
                    }
                }
                // Actions for navigation nodes
                else
                {
                    NavigationalNode navigational = node as NavigationalNode;
                    // Search in first dimension
                    if (xDimension)
                    {
                        // If whole interval is inside the range go to second dimension
                        if (x1.CompareTo(navigational.Min) <= 0 &&
                            navigational.Max.CompareTo(x2) <= 0)
                        {
                            FindRange(x1, y1, x2, y2, navigational.OtherDimensionNode, !xDimension, ref result);
                        }
                        // If whole interval is out of range do nothing
                        else if (navigational.Max.CompareTo(x1) <= 0 ||
                            navigational.Min.CompareTo(x2) >= 0)
                        {

                        }
                        // If part of interval is inside the range go find to both children
                        else if (navigational.Min.CompareTo(x1) <= 0 ||
                            x2.CompareTo(navigational.Max) <= 0)
                        {
                            FindRange(x1, y1, x2, y2, navigational.LeftChild, xDimension, ref result);
                            FindRange(x1, y1, x2, y2, navigational.RightChild, xDimension, ref result);
                        }
                    }
                    // Search in second dimension
                    else
                    {
                        // If whole interval is inside the range look to all leafs of subtree
                        if (y1.CompareTo(navigational.Min) <= 0 &&
                            navigational.Max.CompareTo(y2) <= 0)
                        {
                            LookAllSubtree(x1, y1, x2, y2, navigational, ref result);
                        }
                        // If part of interval is inside the range go find to both children
                        else if (navigational.Min.CompareTo(y1) <= 0 ||
                            y2.CompareTo(navigational.Max) <= 0)
                        {
                            FindRange(x1, y1, x2, y2, navigational.LeftChild, xDimension, ref result);
                            FindRange(x1, y1, x2, y2, navigational.RightChild, xDimension, ref result);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  LookAllSubtree method
        ///  Take every leaf of subtree and
        ///  control its coordinates. If they
        ///  are inside range add to result list
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="node">Root of subtree</param>
        /// <param name="result">List of searching elements</param>
        private void LookAllSubtree(T x1, T y1, T x2, T y2, NavigationalNode node, ref List<TData> result)
        {
            // Find the most left child in subtree
            Leaf pointer = GetMostLeftChild(node);
            // If something went wrong and not all links was built
            if (pointer.RightSibling == null)
                BuildLinks(pointer);
            do
            {
                // Control leaf coordinates and add to 
                // result list if they are inside range
                if (IsInsideRangeX(x1, x2, pointer) &&
                    IsInsideRangeY(y1, y2, pointer))
                {
                    result.Add(pointer.Data);
                }
                // Go to next leaf
                pointer = pointer.RightSibling;
            // End when all leafs was controlled
            } while (pointer != null);
        }

        /// <summary>
        ///  GetMostLeftChild method
        ///  Returns most left child in subtree
        /// </summary>
        /// <param name="node">Subtree root</param>
        /// <returns>Most left child</returns>
        private Leaf GetMostLeftChild(NavigationalNode node)
        {
            Node pointer = node.LeftChild;
            if (pointer is Leaf leaf) return leaf;
            else return GetMostLeftChild(pointer as NavigationalNode);
        }


        /// <summary>
        ///  Find method
        ///  Return data of leaf in specific coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>TData</returns>
        public TData Find(T x, T y)
        {
            Leaf leaf = FindLeaf(x, y);
            if (leaf == null) return default;
            return leaf.Data;
        }


        /// <summary>
        ///  FindLeaf method
        ///  Returns leaf with specific coordinates of data
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>Leaf</returns>
        private Leaf FindLeaf(T x, T y)
        {
            // If tree have only one element so root is leaf
            // control its coordinate and return if they accepted
            // else return null
            if (RootNode != null && RootNode is Leaf root)
            {
                if (root.Data.X.CompareTo(x) == 0 &&
                    root.Data.Y.CompareTo(y) == 0)
                {
                    return root;
                }
                else
                {
                    return null;
                }
            }
            // Start with root and go down
            NavigationalNode pointer = RootNode as NavigationalNode;
            while (true)
            {
                if (pointer == null) return null;
                // Control if one of children of current node
                // is searching node and return it if yes
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
                // Control interval of children of current node
                // If searching node is inside of one of them
                // go there
                NavigationalNode nextPointer = null;
                if (IsInsideRange(x, pointer.LeftChild))
                {
                    nextPointer = pointer.LeftChild as NavigationalNode;
                }
                else if (IsInsideRange(x, pointer.RightChild))
                {
                    nextPointer = pointer.RightChild as NavigationalNode;
                }
                pointer = nextPointer;
            }
        }

        /// <summary>
        ///  IsInsideRange method
        ///  Control if node is in searching range
        /// </summary>
        /// <param name="x"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool IsInsideRange(T x, Node node)
        {
            return node != null &&
                node is NavigationalNode navigationalNode &&
                navigationalNode.Min.CompareTo(x) <= 0 &&
                x.CompareTo(navigationalNode.Max) <= 0;
        }

        /// <summary>
        ///  IsInsideRangeX method
        ///  Control if current leaf is inside range
        ///  by x dimension
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="leaf">Current leaf</param>
        /// <returns></returns>
        private bool IsInsideRangeX(T x1, T x2, Leaf leaf)
        {
            return x1.CompareTo(leaf.Data.X) <= 0 &&
                leaf.Data.X.CompareTo(x2) <= 0;
        }

        /// <summary>
        ///  IsInsideRangeY method
        ///  Control if current leaf is inside range
        ///  by y dimension
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="leaf">Current leaf</param>
        /// <returns></returns>
        private bool IsInsideRangeY(T y1, T y2, Leaf leaf)
        {
            return y1.CompareTo(leaf.Data.Y) <= 0 &&
                leaf.Data.Y.CompareTo(y2) <= 0;
        }

        /// <summary>
        ///  HasMatch method
        ///  Control if current node is leaf
        ///  and has coordinates that are needed
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool HasMatch(T x, T y, Node node)
        {
            return node != null &&
                node is Leaf leaf &&
                leaf.Data.X.CompareTo(x) == 0 &&
                leaf.Data.Y.CompareTo(y) == 0;
        }



        private abstract class Node
        {
            public bool XDimension { get; set; }
            public NavigationalNode Parent { get; set; }
        }

        private class NavigationalNode : Node
        {
            public T Min { get; set; }
            public T Max { get; set; }

            public Node LeftChild { get; set; }
            public Node RightChild { get; set; }
            public NavigationalNode OtherDimensionNode { get; set; }

        }

        private class Leaf : Node
        {
            public TData Data { get; set; }
            public Leaf LeftSibling { get; set; }
            public Leaf RightSibling { get; set; }
        }

    }
}
