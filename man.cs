//------------------------------------------------------------------------------
// MAN ANIMATION - Switches the hand raising images with a time delay
//------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Microsoft.Samples.Kinect.DiscreteGestureBasics
{
    class Man
    {
        //Image Paths
        String[] paths =
        {
            "D:/School/University/COMPX241/Group Project/DiscreteGestureBasics-WPF/Images/raise_no_hand.png",
            "D:/School/University/COMPX241/Group Project/DiscreteGestureBasics-WPF/Images/raise_right_hand.png",
            "D:/School/University/COMPX241/Group Project/DiscreteGestureBasics-WPF/Images/raise_no_hand.png",
            "D:/School/University/COMPX241/Group Project/DiscreteGestureBasics-WPF/Images/raise_left_hand.png"
        };

        ///Asynchronus Function - Can be delayed before switching image
        async Task Main(MainWindow main)
        {
            //Forever iterates through each image with a 1.5 second delay
            while (true)
            {
                foreach (String path in paths)
                {
                    main.man_img.Source = new BitmapImage(new Uri(path));
                    await Task.Delay(1500);
                }
            }
        }

        /// Start the asynchronus function, called by parent
        public void start(MainWindow main)
        {
            Main(main);
        }
    }
}
