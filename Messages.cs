using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.DiscreteGestureBasics
{
    class Messages
    {
        public string[] message = {
            "Use either hand to hover over an item, make a fist to select!",
            "Try asking 'Where is R.G.02' (or any room / paper / lab)",
            "You can ask 'When is COMPX223' (or any paper)",
            "Try asking for 'notices'",
            "Try asking 'Lab 7' (or any room / paper / lab)",
            "Try asking 'How do I get to L.G.05'",
            "End your session by walking away, selecting or saying 'EXIT'",
            "The active user will appear GREEN in the bottom right",
            "Try asking 'What time is COMPX241'",
            "Say 'Help' for a tutorial game :)",
            "Try saying 'Find Paper Location'",
        };
        async Task Main(MainWindow main)
        {
            while (true)
            {     
                foreach(String m in message)
                {
                    main.message.Text = m;
                    await Task.Delay(5000);
                }                       
            }
        }

        public void start(MainWindow main)
        {
            Main(main);
        }
    }
}
