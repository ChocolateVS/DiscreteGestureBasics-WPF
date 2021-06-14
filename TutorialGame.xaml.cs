using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Microsoft.Samples.Kinect.DiscreteGestureBasics
{
    /// <summary>
    /// Interaction logic for TutorialGame.xaml
    /// </summary>
    public partial class TutorialGame : UserControl
    {
        public TutorialGame()
        {
            InitializeComponent();
        }

        Point LeftHand;
        Point RightHand;
        public void updateHands(Point lh, Point rh)
        {
            LeftHand = lh;
            RightHand = rh;
        }

        public void start()
        {
            
        }
    }
}
