using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarbonCube
{
    public class Tree<T>
    {
        private Dictionary<Node, List<Node>> mDico = new Dictionary<Node, List<Node>>();

        public Node this[T item]
        {
            get
            {
                if (this.Contains(item))
                {
                    return new Node(this, item);
                }
                else
                {
                    throw new Exception("L'item " + item + " n'est pas dans l'arbre.");
                }
            }
        }

        public bool Add(T item, T parent)
        {
            if (item == null) return false;
            if (parent == null) return this.Add(item);

            Node nodeItem = new Node(this, item);
            Node nodeParent = new Node(this, parent);

            if (this.Contains(parent)
                && nodeParent != nodeItem
                && !(mDico[nodeParent].Contains(nodeItem)))
            {
                if (!(this.Contains(item)))
                {
                    mDico[nodeParent].Add(nodeItem);
                    mDico.Add(nodeItem, new List<Node>());
                    return true;
                }
                else
                {
                    if ((nodeParent.Level + 1) < nodeItem.Level)
                    {
                        mDico[nodeItem.Parent].Remove(nodeItem);
                        mDico[nodeParent].Add(nodeItem);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool Add(T item)
        {
            if (item != null)
            {
                Node nodeItem = new Node(this, item);

                if (!(this.Contains(item)))
                {
                    mDico.Add(nodeItem, new List<Node>());
                    return true;
                }
                else
                {
                    if (nodeItem.Parent != null && nodeItem.Level > 0)
                    {
                        mDico[nodeItem.Parent].Remove(nodeItem);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool Remove(T item)
        {
            Node nodeItem = new Node(this, item);

            if (item != null && this.Contains(item))
            {
                for (int i = 0; i < mDico[nodeItem].Count; i++)
                {
                    Remove(mDico[nodeItem][i].Value);
                }
                mDico.Remove(nodeItem);

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Contains(T item)
        {
            return mDico.ContainsKey(new Node(this, item));
        }

        public bool IsEmpty()
        {
            foreach (KeyValuePair<Node, List<Node>> pair in mDico)
            {
                if (pair.Value.Count > 0)
                {
                    return false;
                }
            }
            return true;
        }

        // méthode très moche
        public T GetParent(T item, out bool isNull)
        {
            if (!this.Contains(item))
            {
                throw new Exception("L'item " + item + " n'est pas dans l'arbre.");
            }

            Node nodeItem = new Node(this, item);
            Node nodeParent = nodeItem.Parent;
            if (nodeParent != null)
            {
                isNull = false;
                return nodeParent.Value;
            }
            else
            {
                isNull = true;
                return item;
            }
        }

        public List<T> PathTo(T item)
        {
            List<T> path = new List<T>();
            if (this.Contains(item))
            {
                Node current = new Node(this, item);
                while (current != null)
                {
                    path.Add(current.Value);
                    current = current.Parent;
                }
            }
            return path;
        }

        public List<T> ToList()
        {
            List<T> l = new List<T>();
            foreach (Node node in mDico.Keys)
            {
                l.Add(node.Value);
            }
            return l;
        }

        public override string ToString()
        {
            string s = "";
            foreach (KeyValuePair<Node, List<Node>> pair in mDico)
            {
                s += "[" + pair.Key.Value + "] {";
                foreach (Node n in pair.Value)
                {
                    s += n.Value + ", ";
                }
                s += "\b\b}\n";
            }
            return s;
        }

        private Node GetNodeParent(Node node)
        {
            foreach (KeyValuePair<Node, List<Node>> pair in mDico)
            {
                if (pair.Value.Contains(node))
                {
                    return pair.Key;
                }
            }
            return null;
        }

        private int GetNodeLevel(Node node)
        {
            int level = -1;
            while (node != null)
            {
                node = node.Parent;
                level++;
            }
            return level;
        }


        public class Node : IEquatable<Node>
        {
            private Tree<T> mTree;
            private T mValue;

            public Node Parent
            {
                get { return mTree.GetNodeParent(this); }
            }

            public List<Node> Children
            {
                get { return mTree.mDico[this]; }
            }

            public int Level
            {
                get { return mTree.GetNodeLevel(this); }
            }

            public T Value
            {
                get { return mValue; }
            }



            public Node(Tree<T> conteneur, T valeur)
            {
                mTree = conteneur;
                mValue = valeur;
            }

            public override bool Equals(object right)
            {
                if (object.ReferenceEquals(right, null))
                {
                    return false;
                }

                if (object.ReferenceEquals(this, right))
                {
                    return true;
                }

                if (this.GetType() != right.GetType())
                {
                    return false;
                }

                return this.Equals(right as Node);
            }

            public bool Equals(Node other)
            {
                return Value.Equals(other.Value);
            }

            public override int GetHashCode()
            {
                return Value.GetHashCode();
            }

            public static bool operator ==(Node a, Node b)
            {
                if (object.ReferenceEquals(a, null))
                {
                    return object.ReferenceEquals(b, null);
                }

                return a.Equals(b as object);
            }

            public static bool operator !=(Node a, Node b)
            {
                return !(a == b);
            }

        }
    }
}
