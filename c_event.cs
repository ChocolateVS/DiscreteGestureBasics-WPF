using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.DiscreteGestureBasics
{
    public class c_event
    {
        public event EventHandler<points> SearchResultSelected = delegate { };
    }

    public class points : EventArgs
    {
        public int left_x { get; set; }
        public int left_y { get; set; }
        public int right_x { get; set; }
        public int right_y { get; set; }
    }
}
