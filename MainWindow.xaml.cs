//---------------------------------------------------------------------------------------------------
//
// <Description>
// This program tracks up to 6 people simultaneously.
// If a person is tracked, the associated gesture detector will determine if that person is seated or not.
// If any of the 6 positions are not in use, the corresponding gesture detector(s) will be paused
// and the 'Not Tracked' image will be displayed in the UI.
// </Description>
//----------------------------------------------------------------------------------------------------
using Microsoft.Kinect;
using Microsoft.Kinect.Input;
using Microsoft.Samples.Kinect.ControlsBasics;
using Microsoft.Speech.Recognition;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace Microsoft.Samples.Kinect.DiscreteGestureBasics
{
    /// <summary> Interaction logic for the MainWindow </summary>
    public partial class MainWindow : Window
    {
        /// <summary> Active Kinect sensor </summary>
        private KinectSensor kinectSensor = null;

        //SpeechSynthesizer speechSynthesizerObj;
        private readonly Messages messages = new Messages();
        /// <summary> Array for the bodies (Kinect will track up to 6 people simultaneously) </summary>
        private Body[] bodies = null;

        /// <summary> Reader for body frames </summary>
        private BodyFrameReader bodyFrameReader = null;

        List<SpeechCommands.term> term_objects = new List<SpeechCommands.term>();

        /// <summary> KinectBodyView object which handles drawing the Kinect bodies to a View box in the UI </summary>
        private KinectBodyView kinectBodyView = null;

        /// <summary> List of gesture detectors, there will be one detector created for each potential body (max of 6) </summary>
        private List<GestureDetector> gestureDetectorList = null;

        /// <summary> Id for the currently active user</summary>
        private ulong active_user_id;

        /// <summary> This </summary>
        MainWindow main;

        /// <summary> Are any users active? </summary>
        public bool user_active = false;

        /// <summary> Vairable to check if the active user is still present</summary>
        bool current_found = false;

        /// <summary> New instance of the hand pointers </summary>
        KinectPointerPointSample kinectPointerTest;

        /// <summary> New Speech Recognition instance </summary>
        SpeechBasics.MainWindow speechRecognition;

        /// <summary> Instance of mal's pathfinding </summary>
        PathFinding pathFinding = new PathFinding();

        /// <summary> Search Terms </summary>
        Choices search_terms;

        /// <summary> Menu States 
        /// 0 = Welcome Message
        /// 1 = Main Menu
        /// 2 = Map Menu
        /// 3 = Calender Menu
        /// 4 = Notice Board
        /// 5 = Map Room List
        /// 6 = Map Paper Location List
        /// </summary>
        int menuState = 0;

        //Hand States
        HandState lh_state, rh_state;
       
        /// <summary> Initializes a new instance of the MainWindow class </summary>
        public MainWindow()
        {
            //Sets main to this
            main = this;           

            //Creates new hand pointer instance
            kinectPointerTest = new KinectPointerPointSample(main);
        
            // only one sensor is currently supported
            this.kinectSensor = KinectSensor.GetDefault();

            // set IsAvailableChanged event notifier
            this.kinectSensor.IsAvailableChanged += this.Sensor_IsAvailableChanged;

            // open the sensor
            this.kinectSensor.Open();

            // open the reader for the body frames
            this.bodyFrameReader = this.kinectSensor.BodyFrameSource.OpenReader();

            // set the BodyFramedArrived event notifier
            this.bodyFrameReader.FrameArrived += this.Reader_BodyFrameArrived;

            // initialize the BodyViewer object for displaying tracked bodies in the UI
            this.kinectBodyView = new KinectBodyView(this.kinectSensor, this);

            // initialize the gesture detection objects for our gestures
            this.gestureDetectorList = new List<GestureDetector>();

            // initialize the MainWindow
            this.InitializeComponent();

            // set our data context objects for display in UI
            this.DataContext = this;
            this.kinectBodyViewbox.DataContext = this.kinectBodyView;

            // create a gesture detector for each body (6 bodies => 6 detectors) and create content controls to display results in the UI
            int maxBodies = this.kinectSensor.BodyFrameSource.BodyCount;

            for (int i = 0; i < maxBodies; ++i)
            {
                GestureResultView result = new GestureResultView(i, false, false, 0.0f);
                GestureDetector detector = new GestureDetector(this, this.kinectSensor, result);
                this.gestureDetectorList.Add(detector);
            }

            //Initiliaze on load
            Loaded += Load;
        }

        /// <summary> When Window is loaded </summary>
        public void Load(Object sender, RoutedEventArgs e)
        {
            //Creates an object for each of the menu items
            LoadMenuItems();

            //Disables JS ERRORS
            //webBrowser.ScriptErrorsSuppressed = true;

            //Initialize cycling messages
            messages.start(main);

            //Initialize Man animation
            Man m = new Man();
            m.start(main);

            //Adds og map to the location view grid
            l_view.Children.Add(pathFinding);

            //Gets Speech Commands
            SpeechCommands speechCommands = new SpeechCommands();
            search_terms = speechCommands.get_search_terms();
            term_objects = speechCommands.get_term_objects();

            this.speechRecognition = new SpeechBasics.MainWindow(this, search_terms);

            ShowMenu(0);
        }
        /// <summary> Execute shutdown tasks </summary>
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (this.bodyFrameReader != null)
            {
                // BodyFrameReader is IDisposable
                this.bodyFrameReader.FrameArrived -= this.Reader_BodyFrameArrived;
                this.bodyFrameReader.Dispose();
                this.bodyFrameReader = null;
            }

            if (this.gestureDetectorList != null)
            {
                // The GestureDetector contains disposable members (VisualGestureBuilderFrameSource and VisualGestureBuilderFrameReader)
                foreach (GestureDetector detector in this.gestureDetectorList)
                {
                    detector.Dispose();
                }

                this.gestureDetectorList.Clear();
                this.gestureDetectorList = null;
            }

            if (this.kinectSensor != null)
            {
                this.kinectSensor.IsAvailableChanged -= this.Sensor_IsAvailableChanged;
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }
        /// <summary> Handles the event when the sensor becomes unavailable (e.g. paused, closed, unplugged). </summary>     
        private void Sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            // on failure, set the status text
            /*this.StatusText = this.kinectSensor.IsAvailable ? Properties.Resources.RunningStatusText
                                                            : Properties.Resources.SensorNotAvailableStatusText;*/
        }

        /// <summary> Hand State for each hand is updated by Body View class </summary>
        public void updateHandState(HandState left_hand_state, HandState right_hand_state, ulong id)
        {
            kinectPointerTest.update_hand_state(left_hand_state, right_hand_state);
            lh_state = left_hand_state;
            rh_state = right_hand_state;
        }

        /// <summary> Handles the body frame data arriving from the sensor and updates the associated gesture detector object for each body </summary>
        private void Reader_BodyFrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            bool dataReceived = false;
            
            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    if (this.bodies == null)
                    {
                        // creates an array of 6 bodies, which is the max number of bodies that Kinect can track simultaneously
                        this.bodies = new Body[bodyFrame.BodyCount];
                    }

                    // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                    // As long as those body objects are not disposed and not set to null in the array,
                    // those body objects will be re-used.
                    bodyFrame.GetAndRefreshBodyData(this.bodies);
                    dataReceived = true;
                }
            }

            if (dataReceived)
            {
                // visualize the new body data
                this.kinectBodyView.UpdateBodyFrame(this.bodies, active_user_id);

                // we may have lost/acquired bodies, so update the corresponding gesture detectors
                if (this.bodies != null)
                {
                    // loop through all bodies to see if any of the gesture detectors need to be updated
                    int maxBodies = this.kinectSensor.BodyFrameSource.BodyCount;
                    current_found = false;
                    for (int i = 0; i < maxBodies; ++i)
                    {
                        Body body = this.bodies[i];
                        ulong trackingId = body.TrackingId;
                        //Console.WriteLine("Active User ID: " + active_user_id + ", Tracking ID: " + trackingId);
                        if (trackingId == active_user_id)
                        {
                            current_found = true;
                        }

                        // if the current body TrackingId changed, update the corresponding gesture detector with the new value
                        if (trackingId != this.gestureDetectorList[i].TrackingId)
                        {
                            this.gestureDetectorList[i].TrackingId = trackingId;
                            // if the current body is tracked, unpause its detector to get VisualGestureBuilderFrameArrived events
                            // if the current body is not tracked, pause its detector so we don't waste resources trying to get invalid gesture results
                            this.gestureDetectorList[i].IsPaused = trackingId == 0;
                        }
                    }
                    
                    if (!current_found) ClearUser();                      
                }
            }
        }

        /// <summary> If Speech Term Recognized, find the type and do neccessary action </summary>
        public void SpeechRecognized(String result)
        {
            SpeechCommands.term command = new SpeechCommands.term(); 
            //TYPES//
            //0 = Global Menu Item      
            //1 = Asking map for room location directions (map)
            //2 = Asking map for paper location (map)
            //3 = Asking for paper time (calender)
            //4 = Asking for paper details / schedule (calender)
            //5 = lone paper code
            //6 = Map Menu Item

            //Finds the term object that has been said
            foreach (SpeechCommands.term term_object in term_objects)
            {
                if (term_object.name == result)
                {
                    command = term_object;
                    break;
                }
            }

            //Prints what user has said to the console
            Console.WriteLine("USER SAID: " + result + " " + command.type);

            //For each type of command, do the appropriate action, passing necessary varibles
            if (command.type == 0) //Menu Item Command - Show appropriate menu
            {
                if (result == "MAP") ShowMenu(2);                   
                if (result == "NOTICES") ShowMenu(4);
                if (result == "CALENDAR") ShowMenu(3);
                if (result == "EXIT") ClearUser();
                if (result == "TUTORIAL")
                {
                    
                }
                if (result == "BACK")
                {
                    if (menuState == 1) ClearUser();
                    else ShowMenu(1);
                }
            }
            if (command.type == 1)
            {
                //Need Mal to return me an image / directions to the room
                Console.WriteLine("USER NEEDS DIRECTIONS TO ROOM: " + result);
                ShowMenu(5);
                pathFinding.pathFind(result);
            }
            if (command.type == 2)
            {
                //Need Jason to return me location of the next availiable paper, and mal to return me location of the room
                Console.WriteLine("USER NEEDS LOCATION OF PAPER: " + command.p_name);
            }
            if (command.type == 3)
            {
                //Need Jason to return me the time for the next avaible paper
                Console.WriteLine("USER NEEDS TIME OF PAPER: " + command.p_name);
            }
            if (command.type == 4 || command.type == 5)
            {
                //Display Paper all paper information and show next avaliable time / location for the paper
                Console.WriteLine("USER NEEDS PAPER INFORMATION FOR: " + command.name);
            }
            if (command.type == 6) //Map Menu Item
            {
                if (result == "RL") //Find Room Location
                {
                    //Display Locations of each room
                    Console.WriteLine("DISPLAY LOCATION OF EACH ROOM");
                }
                if (result == "PL") //Find Paper Location
                {
                    //Display Locations of each paper
                    Console.WriteLine("DISPLAY LOCATION OF EACH PAPER");
                }
            }
        }

        /// <summary> Attempts to create a new user on hand raise event from the gesture tracking class, called by gesture tracking form </summary>
        public void HandRaised(ulong id)
        {
            //If the hand has been raised by the current user, 
            if (id != active_user_id && !user_active)
            {
                //A user is now active, show the menu and create the new user
                user_active = true;
                ShowMenu(1);
                NewUser(id);
            }  
        }

        /// <summary> Creates new user </summary>
        /// <param name="id"></param>
        public void NewUser(ulong id)
        {
            //Sets active user id to the new users id
            active_user_id = id;

            //Update the hand pointer classes active user
            kinectPointerTest.update_active_user(active_user_id);

            //Adds nw user
            main.menuBorder.Children.Add(kinectPointerTest);
        }

        /// <summary> User has left or wants to exit, clear </summary>
        public void ClearUser()
        {
            active_user_id = 0; //Clears Active user id
            user_active = false; //There is no longer a active user
            current_found = false; //The current user is not found - they have left
            ShowMenu(0); //Show first menu
            kinectPointerTest.update_active_user(0); //Clears the hand pointer classes active user
        }

        //Hands
        Hand left_hand = new Hand();
        Hand right_hand = new Hand();

        /// <summary> MenuItem has boolean for Left and right hand entered, state, and hand states on enter</summary>
        public class MenuItem
        {
            public Grid item;
            public bool l_entered, r_entered, state; //Hands hovering on item?
            public HandState l_state_on_enter, r_state_on_enter; //Hand states when first entering
        }

        //Array of menu_items
        public MenuItem[] menu_items = new MenuItem[7];

        /// <summary> Creates a class for each menu item and adds it the menu_items array</summary>
        public void LoadMenuItems()
        {            
            //Make array of menu items
            Grid[] items = { map, cal, not, exit, voice, paper_l, room_l };

            //For each menu item, assign values and add to the array of menu items
            for (int i = 0; i < items.Length; i++)
            {
                MenuItem menuItem = new MenuItem();
                menuItem.item = items[i];
                menuItem.l_entered = false;
                menuItem.r_entered = false;
                menuItem.state = false;

                menu_items[i] = menuItem;
            }
        }

        public class Hand
        { 
            public double x; //Left hand hovering on item
            public double y; //Right Hand hovering on item
            public HandState state; //Left hand state when first entering 
        }

        ///<summary> Coordinates updated by KinectPointer class
        /// Tracks hand hover and select events on all menu items, for both hands 
        ///</summary>
        public void Coordinates(PointF points, ulong id, HandType handType)
        {
            //If hands to be updated belong to the active user
            if (id == active_user_id)
            {                
                //Gets Position of each hand
                if (handType.ToString() == "LEFT")
                {
                     left_hand.x = points.X * main.ActualWidth;
                     left_hand.y = points.Y * main.ActualHeight;
                }
                else if (handType.ToString() == "RIGHT")
                {
                    right_hand.x = points.X * main.ActualWidth;
                    right_hand.y = points.Y * main.ActualHeight;
                }

                //Sets Hand States
                left_hand.state = lh_state;
                right_hand.state = rh_state;

                //For all the menu items
                foreach (MenuItem thing in menu_items) {

                    //Get Menu Item Position
                    Point position = thing.item.PointToScreen(new Point(0d, 0d));

                    //If the right hand is hovering on the menu item
                    if (
                        right_hand.x > position.X &&
                        right_hand.x < position.X + thing.item.ActualWidth &&
                        right_hand.y > position.Y &&
                        right_hand.y < position.Y + thing.item.ActualHeight)
                    {
                        //Hovering on the current thing is true
                        hover(thing, true);

                        //If hand is hovering and hand was already hovering previously
                        if (thing.r_entered == true)
                        {
                            //If the user has closed thier hand while hovering
                            if (rh_state == HandState.Closed && thing.r_state_on_enter == HandState.Open)
                            {
                                //Menu Item has been selected
                                Select(thing, true);
                                //Set
                                thing.r_entered = false;
                            }
                        }
                        //Hand has just entered check if its open while entering
                        else if (rh_state == HandState.Open)
                        {
                            //Its open and it has just entered, its ready to select
                            thing.r_entered = true;
                            thing.r_state_on_enter = rh_state;
                        }
                        else
                        {
                            //Otherwise hand was closed on entering
                            thing.r_entered = false;
                        }
                    }
                    else if (
                         left_hand.x > position.X &&
                         left_hand.x < position.X + thing.item.ActualWidth &&
                         left_hand.y > position.Y &&
                         left_hand.y < position.Y + thing.item.ActualHeight
                    )
                    {
                        //Hovering on the current thing is true
                        hover(thing, true);

                        //If hand is hovering and hand was already hovering previously
                        if (thing.l_entered == true)
                        {
                            //If the user has closed thier hand while hovering
                            if (lh_state == HandState.Closed && thing.l_state_on_enter == HandState.Open)
                            {
                                //Menu Item has been selected
                                Select(thing, true);
                                //Set
                                thing.l_entered = false;
                            }
                        }
                        //Hand has just entered check if its open while entering
                        else if (lh_state == HandState.Open)
                        {
                            //Its open and it has just entered, its ready to select
                            thing.l_entered = true;
                            thing.l_state_on_enter = lh_state;
                        }
                        else
                        {
                            //Otherwise hand was closed on entering
                            thing.l_entered = false;
                        }
                    }
                    //Otherwise the thing is not currently being hovered
                    else
                    {                   
                        //Hovering on the current thing is false
                        hover(thing, false);

                        //Nothing has been enters
                        thing.l_entered = false;
                        thing.r_entered = false;
                    }                  
                }        
            }
        }
        ///<summary> Hover event, change properties of each menu item on hover </summary>
        public void hover(MenuItem thing, bool on)
        {
            if (on)
            {
                if (thing.item.Name == "map" || thing.item.Name == "cal" || thing.item.Name == "not") thing.item.Margin = new Thickness(-10);
                if (thing.item.Name == "paper_l") paper_marker_img.Source = new BitmapImage(new Uri("D:/School/University/COMPX241/Group Project/DiscreteGestureBasics-WPF/Images/paper_marker.png"));
                if (thing.item.Name == "room_l") room_marker_img.Source = new BitmapImage(new Uri("D:/School/University/COMPX241/Group Project/DiscreteGestureBasics-WPF/Images/room_marker.png"));
                if (thing.item.Name == "exit") exit_text.Foreground = Brushes.Red;
            }
            else
            {
                if (thing.item.Name != "voice") thing.item.Margin = new Thickness(0);
                if (thing.item.Name == "paper_l") paper_marker_img.Source = new BitmapImage(new Uri("D:/School/University/COMPX241/Group Project/DiscreteGestureBasics-WPF/Images/paper_marker-active.png"));
                if (thing.item.Name == "room_l") room_marker_img.Source = new BitmapImage(new Uri("D:/School/University/COMPX241/Group Project/DiscreteGestureBasics-WPF/Images/room_marker-active.png"));
                if (thing.item.Name == "exit") exit_text.Foreground = Brushes.White;
            }
        }

        ///<summary> Select event for menu items</summary>
        public void Select(MenuItem thing, bool h)
        {
            Console.WriteLine(thing.item.Name);
            //If User selects voice - switch voice state and image
            if (thing.item.Name == "voice")
            {
                if (thing.state)
                {
                    thing.state = false;                   
                    VoiceStatus(false);
                }
                else if (!thing.state)
                {
                    thing.state = true;                    
                    VoiceStatus(true);
                }
            }
            //If user selects exit/back go back or exit
            if (thing.item.Name == "exit")
            {
                if (menuState == 1) ClearUser();
                else ShowMenu(1);
            }
            //If User Selects menu item, show respective menu
            Console.Write(thing.item.Name);
            if (thing.item.Name == "cal") ShowMenu(3);
            if (thing.item.Name == "not") ShowMenu(4);
            if (thing.item.Name == "map") ShowMenu(2);
            //if (thing.item.Name == "room_l") ShowMenu(5);
            //if (thing.item.Name == "paper_l") ShowMenu(5);
        }

        ///<summary> Switches Voice Status </summary>
        public void VoiceStatus(bool act)
        {
            if (!act) 
            {
                voice_img.Source = new BitmapImage(new Uri("D:/School/University/COMPX241/Group Project/DiscreteGestureBasics-WPF/Images/voice_off.png"));
                voice_on_text.Visibility = Visibility.Hidden;
                voice_off_text.Visibility = Visibility.Visible;
                map_text.Text = "Map";
                not_text.Text = "Notices";
                cal_text.Text = "Calendar";

                this.speechRecognition.speechEngine.RequestRecognizerUpdate();
                this.speechRecognition.speechEngine.RecognizeAsyncStop();
            }
            else
            {
                voice_img.Source = new BitmapImage(new Uri("D:/School/University/COMPX241/Group Project/DiscreteGestureBasics-WPF/Images/voice_on.png"));
                voice_off_text.Visibility = Visibility.Hidden;
                voice_on_text.Visibility = Visibility.Visible;

                map_text.Text = "Say 'Map'";
                not_text.Text = "Say 'Notices'";
                cal_text.Text = "Say 'Calendar'";

                this.speechRecognition.speechEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
        }

        ///<summary> Shows selected menu </summary>
        public void ShowMenu(int state)
        {
            menuState = state;
            HideAll();
            if (state == 0)
            {
                welcomeMsg.Visibility = Visibility.Visible;
                VoiceStatus(false);
            }
            else
            {
                exit.Visibility = Visibility.Visible;
                voice.Visibility = Visibility.Visible;
                text.Visibility = Visibility.Visible;
            }
            if (state == 1)
            {
                Menu.Visibility = Visibility.Visible;
                exit_text.Text = "EXIT";
            }
            else exit_text.Text = "BACK";
            if (state == 2) map_view.Visibility = Visibility.Visible;
            if (state == 3) cal_view.Visibility = Visibility.Visible;
            if (state == 4) not_view.Visibility = Visibility.Visible;   
            if (state == 5) l_view.Visibility = Visibility.Visible;
        }

        ///<summary> Hides all menu items </summary>
        public void HideAll()
        {
            welcomeMsg.Visibility = Visibility.Hidden;
            Menu.Visibility = Visibility.Hidden;
            map_view.Visibility = Visibility.Hidden;
            cal_view.Visibility = Visibility.Hidden;
            not_view.Visibility = Visibility.Hidden;
            exit.Visibility = Visibility.Hidden;
            voice.Visibility = Visibility.Hidden;
            text.Visibility = Visibility.Hidden;
            l_view.Visibility = Visibility.Hidden;
        }
    }
}











