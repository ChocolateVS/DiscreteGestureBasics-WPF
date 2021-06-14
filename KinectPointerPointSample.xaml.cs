//------------------------------------------------------------------------------
// KINECT POINTER VIEW - RETURNS A CANVAS WITH THE P+HAND POINTERS
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.ControlsBasics
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;
    using Microsoft.Kinect;
    using Microsoft.Kinect.Input;
    using Microsoft.Samples.Kinect.DiscreteGestureBasics;  

    public sealed partial class KinectPointerPointSample : UserControl
    {
        MainWindow parent;

        KinectCoreWindow kinectCoreWindow;

        //Active user Id so we only show the hands of the active user
        ulong active_user_id;

        private const double HandHeight = 60;
        private const double HandWidth = 60;

        // Keeps track of last time, so we know when we get a new set of pointers. Pointer events fire multiple times per timestamp, based on how
        private TimeSpan lastTime;

        //Hand Pointer File Paths
        readonly Uri lhoc = new Uri(@"D:/School/University/COMPX241/Group Project/DiscreteGestureBasics-WPF/Images/hands/lhoc.png");
        readonly Uri lhcc = new Uri(@"D:/School/University/COMPX241/Group Project/DiscreteGestureBasics-WPF/Images/hands/lhcc.png");
        readonly Uri rhoc = new Uri(@"D:/School/University/COMPX241/Group Project/DiscreteGestureBasics-WPF/Images/hands/rhoc.png");
        readonly Uri rhcc = new Uri(@"D:/School/University/COMPX241/Group Project/DiscreteGestureBasics-WPF/Images/hands/rhcc.png");
        readonly Uri none = new Uri(@"D:/School/University/COMPX241/Group Project/DiscreteGestureBasics-WPF/Images/hands/none.png");

        //Hand States
        HandState rh_state;
        HandState lh_state;

        ///Parent window updtes hand_state
        public void update_hand_state(HandState l, HandState r)
        {
            lh_state = l;
            rh_state = r;
        }

        ///Request to create from parent window
        public KinectPointerPointSample(MainWindow par)
        {
           
            this.InitializeComponent();
            parent = par;
            this.Loaded += KinectPointerPointSample_Loaded;
            kinectCoreWindow = KinectCoreWindow.GetForCurrentThread();
        }

        ///Update active user id
        public void update_active_user(ulong id)
        {
            active_user_id = id;
        }

        ///Initialize
        void KinectPointerPointSample_Loaded(object sender, RoutedEventArgs e)
        {
            kinectCoreWindow.PointerMoved += kinectCoreWindow_PointerMoved;
        }

        /// Handles kinect pointer events
        private void kinectCoreWindow_PointerMoved(object sender, KinectPointerEventArgs args)
        {
            KinectPointerPoint kinectPointerPoint = args.CurrentPoint;
            if (lastTime == TimeSpan.Zero || lastTime != kinectPointerPoint.Properties.BodyTimeCounter)
            {
                lastTime = kinectPointerPoint.Properties.BodyTimeCounter;
                mainScreen.Children.Clear();
            }
            RenderPointer(
                kinectPointerPoint.Position,
                kinectPointerPoint.Properties.BodyTrackingId,
                kinectPointerPoint.Properties.HandType
            );                
        }

        ///Renders hands on the canvas
        private void RenderPointer(
            PointF position,
            ulong trackingId,
            HandType handType)
        {
            //If the hands belong to the active user
            if (trackingId == active_user_id)
            {
                //Clear current cusors
                StackPanel cursor = null;
                if (cursor == null)
                {
                    cursor = new StackPanel();
                    mainScreen.Children.Add(cursor);
                }

                cursor.Children.Clear();                
                             
                //Create new panel for the cursors
                StackPanel sp = new StackPanel()
                {
                    Margin = new Thickness(-5, -5, 0, 0),
                    Orientation = Orientation.Horizontal
                };

                //Source for hand pointers
                Uri c_source = none;

                //Set Hand Image based on state
                if (handType.ToString() == "LEFT")
                {
                    if (lh_state == HandState.Open) c_source = lhoc; //Left Hand is open
                    else if (lh_state == HandState.Closed) c_source = lhcc; //Left hand is closed
                }
                   
                else if (handType.ToString() == "RIGHT")
                {
                    if (rh_state == HandState.Open) c_source = rhoc; //Right hand is open
                    else if (rh_state == HandState.Closed) c_source = rhcc; //Right hand is closed
                }
                
                BitmapImage clamped = new BitmapImage(c_source); //Creates Bitmap image with the set source

                Image clampedCursor = new Image //Creates a displayable image with the bitmap image
                {
                    Source = clamped
                };

                //The cursor to the stack panel
                sp.Children.Add(clampedCursor);
                cursor.Children.Add(sp);

                cursor.Children.Add(new TextBlock() { Text = handType.ToString() });

                //Sets hand locations on canvas
                Canvas.SetLeft(cursor, position.X * mainScreen.ActualWidth - HandWidth / 2);
                Canvas.SetTop(cursor, position.Y * mainScreen.ActualHeight - HandHeight / 2);               

                //Gives the parent form the pointer locations
                parent.Coordinates(position, trackingId, handType);
            }
        }
    }
}
