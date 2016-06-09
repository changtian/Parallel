using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangTianShuiYue.DataStructure.Node
{
    public abstract class Node<T> where T:class
    {
        private T data;
        private NodeList<T> neighbors;

        public Node()
        { }

        public Node(T data, NodeList<T> neighbors)
        {
            this.data = data;
            this.neighbors = neighbors;
        }

        public T Value
        {
            get 
            {
                return data;
            }
            set 
            {
                data = value;
            }
        }

        public NodeList<T> Neighbors
        {
            get
            {
                return neighbors;
            }
            set
            {
                neighbors = value;
            }
        }
    }
}
