using ChangTianShuiYue.DataStructure.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangTianShuiYue.DataStructure.Tree.BinaryTree
{
    public abstract class BinaryTree<T> where T:class
    {
        private BinaryTreeNode<T> root;

        public BinaryTree() 
        {
            this.root = null;
        }

        public BinaryTree(BinaryTreeNode<T> data)
        {
            this.root = data;
        }

        public void Clear()
        {
            this.root = null;
        }

        public BinaryTreeNode<T> Root
        {
            get 
            {
                return root;
            }
            set 
            {
                root = value;
            }
        }
    }
}
