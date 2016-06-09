using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangTianShuiYue.DataStructure.Node
{
    public class BinaryTreeNode<T>:Node<T> where T:class
    {
        public BinaryTreeNode() : base() { }

        public BinaryTreeNode(T data, BinaryTreeNode<T> left, BinaryTreeNode<T> right)
        {
            base.Value = data;
            NodeList<T> children = new NodeList<T>(2);
            children[0] = left;
            children[1] = right;
            base.Neighbors = children;
        }

        public BinaryTreeNode<T> Left
        {
            get
            {
                if (Neighbors == null)
                {
                    return null;
                }
                return (BinaryTreeNode<T>)Neighbors[0];
            }
            set 
            {
                if (Neighbors == null)
                {
                    Neighbors = new NodeList<T>(2);
                }
                Neighbors[0] = value;
            }
        }

        public BinaryTreeNode<T> Right
        {
            get
            {
                if (Neighbors == null)
                {
                    return null;
                }
                return (BinaryTreeNode<T>)Neighbors[1];
            }
            set 
            {
                if (Neighbors == null)
                {
                    Neighbors = new NodeList<T>(2);
                }
                Neighbors[1] = value;
            }
        }
    }
}
