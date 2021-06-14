using System;

namespace Hardcoded_path_finding
{
    class Node
    {
        public string name;
        public string directions;
        public int x;
        public int y;
        public Node prev;

        public Node(String _name, String _directions, int _x, int _y, Node _prev)
        {
            name = _name;
            x = _x;
            y = _y;
            directions = _directions;
            prev = _prev;
        }                          

        public override string ToString()
        {
            return name;// + " , " + x.ToString() + " , " + y.ToString();
        }
    }
}
