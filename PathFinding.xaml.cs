using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Hardcoded_path_finding;

namespace Microsoft.Samples.Kinect.DiscreteGestureBasics
{
    /// <summary>
    /// Interaction logic for PathFinding.xaml
    /// </summary>
    /// 

    public partial class PathFinding : UserControl
    {
        List<Node> nodeList = new List<Node>();
        Brush pathline = Brushes.Red;

        public PathFinding()
        {
            

            InitializeComponent();
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = new BitmapImage(new Uri("D:/School/University/COMPX241/Group Project/DiscreteGestureBasics-WPF/Images/r_block.png"));
            mainScreen.Background = ib;

            Node h = new Node("Home", "", 464, 577, null);
            Node r1 = new Node("R.G.01", "Lab 1 is directly behind you!", 339, 428, h);
            Node r2 = new Node("R.G.02", "Lab 2 is directly behind you!", 424, 428, h);
            Node r7 = new Node("R.G.07", "Lab 7 is to your left!", 511, 505, h);
            Node hs = new Node("Hall Start", "Behind you, ", 464, 470, h);
            Node he = new Node("Hall End", "", 830, 469, hs);
            Node br = new Node("Bathroom", "The Bathrooms are to your right", 284, 524, h);
            Node exit = new Node("Exit", "The Exit is to your right", 280, 590, h);
            Node r3 = new Node("R.G.03", "down the hall and to your left", 840, 430, he);
            Node r4 = new Node("R.G.04", "down the hall and to your left", 922, 420, he);
            Node r5 = new Node("R.G.05", "at the end of the hall ", 977, 504, he);
            Node r6 = new Node("R.G.06", "down the hall and to the right", 750, 504, he);

            //Add each node to the list
            nodeList.Add(h);
            nodeList.Add(he);
            nodeList.Add(hs);
            nodeList.Add(r1);
            nodeList.Add(r2);
            nodeList.Add(r3);
            nodeList.Add(r4);
            nodeList.Add(r5);
            nodeList.Add(r6);
            nodeList.Add(r7);
            nodeList.Add(br);
            nodeList.Add(exit);
        }

        public void pathFind(string room)
        {
            Node temp = nodeList[0];
            foreach (Node n in nodeList)
            {
                Console.WriteLine("Current: " + n.name + "Looking For: " + room);
                if (n.name.Equals(room))
                {
                    temp = n;
                    Console.WriteLine("Found");
                }
            }

            List<string> directions = new List<string>(); //List of directions between each point on the path

            List<Node> DrawList = new List<Node>(); //List of nodes on the path

            String direction = ""; //Direction string

            /// The selected room is the final location on the search path
            /// We use the previous node pointer and iterate backwards through each node
            /// The starting point has no previous node and thus we can iterate 
            /// until the previos node doesn't exist

            while (temp != null)
            {
                //Add the direction between current nodes to the array
                directions.Add(temp.directions);
                DrawList.Add(temp);
                temp = temp.prev;
            }

            //Since we iterate backwards through points, need to reverse directions
            directions.Reverse();
            //Concantenate each direction into a direction string
            foreach (String d in directions)
            {
                direction += d;
            }
            
            //Set direction label text
            Console.WriteLine(direction);

            //Draw the list of directions
            drawList(DrawList);
            direction_output.Text = direction;
        }

        private void drawList(List<Node> list)
        {           
            //Clear previous path
            mainScreen.Children.Clear();
            //For each node in the path, draw a line
            for (int i = 0; i < list.Count - 1; i++) { 
                //Start and end points
                Node start = list[i];
                Node end = list[i + 1];

                //Creates a Line to draw on the canvas
                Line line = new Line
                {
                    Stroke = Brushes.Red,
                    Fill = Brushes.Red,
                    StrokeThickness = 5,
                    X1 = start.x,
                    X2 = end.x,
                    Y1 = start.y,
                    Y2 = end.y
                };
                mainScreen.Children.Add(line);
            }
        }
    }
}
