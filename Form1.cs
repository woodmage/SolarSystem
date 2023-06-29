using Accessibility;
using SolarSystem.Properties;

namespace SolarSystem
{
    public partial class Form1 : Form
    {
        readonly List<SpaceObject> spaceObjects = new() {
            new SpaceObject("Sol", 1.989e30, 1391000, 2.5e17, 0, 0, Resources.sol, -1), //0
            new SpaceObject("Mercury", 3.3011e23, 4879, 5.79e7, 47.87, 0, Resources.mercury, 0), //1
            new SpaceObject("Venus", 4.8675e24, 12104, 1.082e8, 35.02, 0, Resources.venus, 0), //2
            new SpaceObject("Earth", 5.972e24, 12742, 1.496e8, 29.78, 0, Resources.earth, 0), //3
            new SpaceObject("Moon", 7.35e22, 3474, 3.844e5, 1.02, 0, Resources.moon, 3), //4
            new SpaceObject("Mars", 6.39e23, 6779, 2.279e8, 24.07, 0, Resources.mars, 0), //5
            new SpaceObject("Jupiter", 1.898e27, 139820, 7.785e8, 13.07, 0, Resources.jupiter, 0), //6
            new SpaceObject("Io", 8.9319e22, 3643, 4.217e5, 17.33, 0, Resources.io, 6), //7
            new SpaceObject("Europa", 4.7998e22, 3121, 6.709e5, 13.74, 0, Resources.europa, 6), //8
            new SpaceObject("Ganymede", 1.4819e23, 5268, 1.0704e6, 10.88, 0, Resources.ganymede, 6), //9
            new SpaceObject("Callisto", 1.0759e23, 4820, 1.8827e6, 8.2, 0, Resources.callisto, 6), //10
            new SpaceObject("Saturn", 5.683e26, 116460, 1.4e9, 9.69, 0, Resources.saturn, 0), //11
            new SpaceObject("Titan", 1.35e23, 5149, 1.22187e6, 5.57, 0, Resources.titan, 11), //12
            new SpaceObject("Rhea", 2.31e21, 1527, 5.2704e5, 3.52, 0, null, 11), //13
            new SpaceObject("Iapetus", 1.81e21, 1470, 3.5613e6, 3.26, 0, null, 11), //14
            new SpaceObject("Dione", 1.09e21, 1123, 3.7742e5, 3.02, 0, null, 11), //15
            new SpaceObject("Tethys", 6.17e20, 1066, 1.8552e5, 2.73, 0, null, 11), //16
            new SpaceObject("Enceladus", 1.08e20, 504, 2.3802e5, 2.31, 0, null, 11), //17
            new SpaceObject("Uranus", 8.681e25, 50724, 2.9e9, 6.81, 0, Resources.uranus, 0), //18
            new SpaceObject("Titania", 3.52e21, 1578, 4.3591e5, 3.64, 0, null, 18), //19
            new SpaceObject("Oberon", 3.01e21, 1522, 5.8352e5, 3.15, 0, null, 18), //20
            new SpaceObject("Umbriel", 1.27e21, 1169, 2.663e5, 2.67, 0, null, 18), //21
            new SpaceObject("Ariel", 1.35e21, 1157, 1.9102e5, 2.59, 0, null, 18), //22
            new SpaceObject("Miranda", 6.59e19, 471, 1.2939e5, 1.45, 0, null, 18), //23
            new SpaceObject("Neptune", 1.024e26, 49244, 4.5e9, 5.43, 0, Resources.neptune, 0), //24
            new SpaceObject("Triton", 2.14e22, 2706, 3.548e5, 4.39, 0, null, 24), //25
            new SpaceObject("Proteus", 4.4e19, 420, 1.17646e5, 1.12, 0, null, 24), //26
            new SpaceObject("Nereid", 3.1e19, 340, 5.513818e6, 0.37, 0, null, 24), //27
            new SpaceObject("Larissa", 4.2e16, 194, 7.3548e5, 0.23, 0, null, 24), //28
            new SpaceObject("Galatea", 2.12e16, 158, 6.1953e5, 0.31, 0, null, 24), //29
            //new SpaceObject("asteroid", 1.1e10, 50, 4.1e8, 0.5, 100, null, -1),
        };
        PictureBox pictureBox = new PictureBox();
        Bitmap screen = new(1, 1);
        System.Windows.Forms.Timer timer = new();
        int follow = -1;
        public Form1()
        {
            InitializeComponent();
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            Size = new(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height); //set to desktop size
            Location = new Point(0, 0); //put in upper left corner of screen
            int widehigh = ClientSize.Height; //set widehigh to height
            if (ClientSize.Width < widehigh) widehigh = ClientSize.Width; //change to width if that's smaller
            pictureBox.Size = new Size(widehigh, widehigh); //picturebox fills form - sorta
            int x = 0; //x position = 0
            if (widehigh < ClientSize.Width) x = (ClientSize.Width - widehigh) / 2; //if not, then centered
            int y = 0; //y position = 0
            if (widehigh < ClientSize.Height) y = (ClientSize.Height - widehigh) / 2; //if not, then centered
            pictureBox.Location = new Point(x, y); //set location of picturebox
            pictureBox.BackColor = Color.Black; //set it to black (for space)
            Boundaries.ScreenHeight = widehigh; //give horizontal size to boundaries
            Boundaries.ScreenWidth = widehigh; //give vertical size to boundaries
            pictureBox.MouseMove += HandleMouse;
            pictureBox.MouseUp += HandleMouseUp;
            pictureBox.MouseWheel += HandleMouse;
            pictureBox.MouseDown += HandleMouseDown;
            Controls.Add(pictureBox); //add picturebox to form
            screen.Dispose(); //get rid of screen
            screen = new(widehigh, widehigh); //make new screen, size of picturebox
            foreach (SpaceObject sp in spaceObjects) //for every space object
            {
                int o = sp.GetOrbits(); //get which object it orbits
                if (o == -1) //if it doesn't orbit
                    sp.SetPosition(0, 0); //set position to middle
                else //otherwise
                    sp.SetParent(spaceObjects[o].GetPos()); //set parent position
                sp.ComputePosition(); //compute its position
            }
            timer.Interval = 10; //every hundredth of a second
            timer.Tick += Timer_Tick; //function to run
            timer.Start(); //begin timer
        }

        PointD lastpos = PointD.Empty;

        private void HandleMouse(object? sender, MouseEventArgs e)
        {
            Point pos = e.Location; //get mouse location
            if (e.Delta > 0) Boundaries.ZoomIn(); //if we scrolled one way, zoom in
            if (e.Delta < 0) Boundaries.ZoomOut(); //if we scrolled the other way, zoom out
        }
        private void HandleMouseDown(object? sender, MouseEventArgs e)
        {
            Point pos = e.Location; //get mouse location
            if (e.Button == MouseButtons.Right) //if right mouse button clicked
                Boundaries.SetCenter(Boundaries.FromScreenPos(pos)); //center on that spot
            if (e.Button == MouseButtons.Left) //if left mouse button clicked
            {
                if (lastpos == PointD.Empty) //if last position is not filled,
                    lastpos = Boundaries.FromScreenPos(pos); //get the last position from the mouse position
            }
        }
        private void HandleMouseUp(object? sender, MouseEventArgs e)
        {
            Point pos = e.Location; //get mouse position
            if (e.Button == MouseButtons.Left) //if we are releasing left mouse button
            {
                PointD p1 = lastpos; //set first point to position gotten earlier
                PointD p2 = Boundaries.FromScreenPos(pos); //get the other point in the screen
                if (p1.X < p2.X) //if first point less than second one
                {
                    Boundaries.Left = p1.X; //set left to be the first point
                    Boundaries.Right = p2.X; //set right to the the second point
                }
                else //otherwise,
                {
                    Boundaries.Left = p2.X; //set left to be the second point
                    Boundaries.Right = p1.X; //set right to be the first point
                }
                if (p1.Y < p2.Y) //if first point less than the second one
                {
                    Boundaries.Top = p1.Y; //set top to be the first point
                    Boundaries.Bottom = p2.Y; //set bottom to be the second point
                }
                else //otherwise
                {
                    Boundaries.Top = p2.Y; //set top to be the second point
                    Boundaries.Bottom = p1.Y; //set bottom to be the first point
                }
                Boundaries.Width = Math.Abs(Boundaries.Right - Boundaries.Left); //compute width
                Boundaries.Height = Math.Abs(Boundaries.Bottom - Boundaries.Top); //compute height
                double widehigh = Boundaries.Width; //set widehigh to width
                if (Boundaries.Height < widehigh) widehigh = Boundaries.Height; //if height is smaller, set to height
                p1 = Boundaries.GetCenter(); //get center position
                Boundaries.Left = p1.X - widehigh / 2; //set positions
                Boundaries.Right = p1.X + widehigh / 2;
                Boundaries.Top = p1.Y - widehigh / 2;
                Boundaries.Bottom = p1.Y + widehigh / 2;
                Boundaries.Width = Boundaries.Height = widehigh;
                lastpos = PointD.Empty; //reset last position
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            timer.Stop(); //stop the timer
            foreach (SpaceObject sp in spaceObjects) //for each object
            {
                sp.Move(); //move the object
                int o = sp.GetOrbits(); //get which object it orbits
                if (o != -1) //if it orbits another object
                {
                    sp.SetParent(spaceObjects[o].GetPos()); //set parent position
                    sp.ComputePosition(); //compute its position
                }
            }
            if (follow >= 0) //if we are following an object
            {
                PointD pos = spaceObjects[follow].GetPos(); //get the position of that object
                Boundaries.SetCenter(pos); //set it as the center of our screen
            }
            using Graphics g = Graphics.FromImage(screen); //get graphics object
            {
                g.Clear(Color.Black); //clear the screen
                foreach (SpaceObject sp in spaceObjects) sp.Paint(g); //paint all the objects
            }
            pictureBox.Image = screen; //set the picture box to use the screen bitmap
            pictureBox.Invalidate(); //tell windows to redraw the picture box
            timer.Start(); //start the timer again
        }
        string helptext = "Solar System version 1.0.0\nCopyright © 2023 by John Worthington.\n(email: woodmage@gmail.com)\nAll rights reserved.\n\nKeys: Actions\nEsc: Quits program.\n? or F1: Get this help.\nEnter: Get info on objects or the object we are following.\nLeft: Shift left.\nRight: Shift right.\nUp: Shift up.\nDown: Shift down.\nHome: Reset boundaries.\nEnd: Select an object to follow.\nPgUp or numeric keypad -: Zoom out.\nPgDn or numeric keypad +: Zoom in.\n\nRight clicking somewhere on the screen will center the screen on that point.\nClicking with the left mouse button and dragging to another spot will make the rectangle selected the screen.\nAnd mouse wheel up and down zoom in and out.\n";
        private void OnFormKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode) //switch according to key pressed
            {
                case Keys.Escape: //escape key
                    if (MessageBox.Show("Exit?  Are you sure?", "Quit?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) //if user verifies
                        Close(); //close the form (which will exit the program)
                    break; //move on
                case Keys.OemQuestion: //? key
                case Keys.F1: //F1 key
                    ShowHelp();
                    break;
                case Keys.Enter: //Enter key
                    if (follow == -1)
                        ListSpaceObjects();
                    else
                        MessageBox.Show(spaceObjects[follow].ToString(), spaceObjects[follow].GetName(), MessageBoxButtons.OK);
                    break;
                case Keys.Left: //left arrow
                    Boundaries.ShiftLeft(); //shift left
                    break; //move on
                case Keys.Right: //right arrow
                    Boundaries.ShiftRight(); //shift right
                    break; //move on
                case Keys.Up: //up arrow
                    Boundaries.ShiftUp(); //shift up
                    break; //move on
                case Keys.Down: //down arrow
                    Boundaries.ShiftDown(); //shift down
                    break;
                case Keys.Home: //home key
                    follow = -1;
                    Boundaries.ResetAll();
                    break; //move on
                case Keys.End: //end key
                    if (follow < spaceObjects.Count - 1)
                    {
                        follow++;
                        MessageBox.Show("Now following " + spaceObjects[follow].GetName(), "Following", MessageBoxButtons.OK);
                    }
                    else
                        MessageBox.Show("This is the last possible object to follow!", "Info", MessageBoxButtons.OK);
                    break; //move on
                case Keys.Add: //numeric keypad + key
                case Keys.PageDown: //pgdn key
                    Boundaries.ZoomIn(); //zoom in on system
                    break; //move on
                case Keys.Subtract: //numeric keypad - key
                case Keys.PageUp: //pgup key
                    Boundaries.ZoomOut(); //zoom out from system
                    break; //move on

                default: //any other key
                    return; //exit function
            }
            e.Handled = true; //tell windows we handled the keystroke
        }
        private void ListSpaceObjects()
        {
            Form f = new Form();
            f.ClientSize = new Size(520, 580);
            ListBox lb = new ListBox();
            lb.Size = new Size(500, 500);
            lb.Location = new Point(10, 10);
            lb.Items.Clear();
            lb.BeginUpdate();
            foreach (SpaceObject sp in spaceObjects)
                lb.Items.Add(sp.ToString());
            lb.EndUpdate();
            lb.HorizontalScrollbar = true;
            f.Controls.Add(lb);
            Button b = new Button();
            b.Size = new Size(100, 50);
            b.Location = new Point(205, 520);
            b.Text = "Continue";
            f.Controls.Add(b);
            f.AcceptButton = f.CancelButton = b;
            f.ShowDialog();
        }
        private void ShowHelp()
        {
            Form f = new Form();
            f.ClientSize = new Size(520, 580);
            RichTextBox rtb = new RichTextBox();
            rtb.Location = new Point(10, 10);
            rtb.Size = new Size(500, 500);
            rtb.Text = helptext;
            f.Controls.Add(rtb);
            Button b = new Button();
            b.Location = new Point(205, 520);
            b.Size = new Size(100, 50);
            b.Text = "Continue";
            f.Controls.Add(b);
            f.AcceptButton = f.CancelButton = b;
            f.ShowDialog();
        }
    }
}