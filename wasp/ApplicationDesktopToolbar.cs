using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Wasp
{
    public class ApplicationDesktopToolbar : IDisposable {
        private Form Form;

        #region Enums
        public enum AppBarMessages {
            /// Registers a new appbar and specifies the message identifier
            /// that the system should use to send notification messages to 
            /// the appbar. 
            New = 0x00000000,

            /// Unregisters an appbar, removing the bar from the system's 
            /// internal list.
            Remove = 0x00000001,

            /// Requests a size and screen position for an appbar.
            QueryPos = 0x00000002,

            /// Sets the size and screen position of an appbar. 
            SetPos = 0x00000003,

            /// Retrieves the autohide and always-on-top states of the
            /// Windows taskbar. 
            GetState = 0x00000004,

            /// Retrieves the bounding rectangle of the Windows taskbar. 
            GetTaskBarPos = 0x00000005,

            /// Notifies the system that an appbar has been activated. 
            Activate = 0x00000006,

            /// Retrieves the handle to the autohide appbar associated with
            /// a particular edge of the screen. 
            GetAutoHideBar = 0x00000007,

            /// Registers or unregisters an autohide appbar for an edge of 
            /// the screen. 
            SetAutoHideBar = 0x00000008,

            /// Notifies the system when an appbar's position has changed. 
            WindowPosChanged = 0x00000009,

            /// Sets the state of the appbar's autohide and always-on-top 
            /// attributes.
            SetState = 0x0000000a
        }

        public enum AppBarNotifications {
            /// Notifies an appbar that the taskbar's autohide or 
            /// always-on-top state has change - that is, the user has selected 
            /// or cleared the "Always on top" or "Auto hide" check box on the
            /// taskbar's property sheet. 
            StateChange = 0x00000000,

            /// Notifies an appbar when an event has occurred that may affect 
            /// the appbar's size and position. Events include changes in the
            /// taskbar's size, position, and visibility state, as well as the
            /// addition, removal, or resizing of another appbar on the same 
            /// side of the screen.
            PosChanged = 0x00000001,

            /// Notifies an appbar when a full-screen application is opening or
            /// closing. This notification is sent in the form of an 
            /// application-defined message that is set by the ABM_NEW message. 
            FullScreenApp = 0x00000002,

            /// Notifies an appbar that the user has selected the Cascade, 
            /// Tile Horizontally, or Tile Vertically command from the 
            /// taskbar's shortcut menu.
            WindowArrange = 0x00000003
        }

        [Flags]
        public enum AppBarStates {
            AutoHide = 0x00000001,
            AlwaysOnTop = 0x00000002
        }

        public enum AppBarEdges {
            Left = 0,
            Top = 1,
            Right = 2,
            Bottom = 3
        }

        // Window Messages	
        public enum WM {
            ACTIVATE = 0x0006,
            WINDOWPOSCHANGED = 0x0047,
            NCHITTEST = 0x0084
        }

        public enum MousePositionCodes {
            HTERROR = (-2),
            HTTRANSPARENT = (-1),
            HTNOWHERE = 0,
            HTCLIENT = 1,
            HTCAPTION = 2,
            HTSYSMENU = 3,
            HTGROWBOX = 4,
            HTSIZE = HTGROWBOX,
            HTMENU = 5,
            HTHSCROLL = 6,
            HTVSCROLL = 7,
            HTMINBUTTON = 8,
            HTMAXBUTTON = 9,
            HTLEFT = 10,
            HTRIGHT = 11,
            HTTOP = 12,
            HTTOPLEFT = 13,
            HTTOPRIGHT = 14,
            HTBOTTOM = 15,
            HTBOTTOMLEFT = 16,
            HTBOTTOMRIGHT = 17,
            HTBORDER = 18,
            HTREDUCE = HTMINBUTTON,
            HTZOOM = HTMAXBUTTON,
            HTSIZEFIRST = HTLEFT,
            HTSIZELAST = HTBOTTOMRIGHT,
            HTOBJECT = 19,
            HTCLOSE = 20,
            HTHELP = 21
        }

        #endregion Enums

        #region AppBar Functions

        private Boolean AppbarNew() {
            if (CallbackMessageID == 0)
                throw new Exception("CallbackMessageID is 0");

            if (IsAppbarMode) {
                return true;
            }

            ShellApi.APPBARDATA msgData = new ShellApi.APPBARDATA();
            msgData.cbSize = (UInt32)Marshal.SizeOf(msgData);
            msgData.hWnd = Form.Handle;
            msgData.uCallbackMessage = CallbackMessageID;
            UInt32 retVal = ShellApi.SHAppBarMessage((UInt32)AppBarMessages.New, ref msgData);
            IsAppbarMode = (retVal != 0);
            SizeAppBar();
            return IsAppbarMode;
        }

        private Boolean AppbarRemove() {
            ShellApi.APPBARDATA msgData = new ShellApi.APPBARDATA();
            msgData.cbSize = (UInt32)Marshal.SizeOf(msgData);
            msgData.hWnd = Form.Handle;
            UInt32 retVal = ShellApi.SHAppBarMessage((UInt32)AppBarMessages.Remove, ref msgData);
            IsAppbarMode = false;
            return (retVal != 0) ? true : false;
        }

        /// Passes a proposed AppBar location to the OS to verify that it's ok to set
        private void AppbarQueryPos(ref ShellApi.RECT appRect) {
            ShellApi.APPBARDATA msgData = new ShellApi.APPBARDATA();
            msgData.cbSize = (UInt32)Marshal.SizeOf(msgData);
            msgData.hWnd = Form.Handle;
            msgData.uEdge = (UInt32)_Edge;
            msgData.rc = appRect;
            ShellApi.SHAppBarMessage((UInt32)AppBarMessages.QueryPos, ref msgData);
            appRect = msgData.rc;
        }

        private void AppbarSetPos(ref ShellApi.RECT appRect) {
            ShellApi.APPBARDATA msgData = new ShellApi.APPBARDATA();
            msgData.cbSize = (UInt32)Marshal.SizeOf(msgData);
            msgData.hWnd = Form.Handle;
            msgData.uEdge = (UInt32)_Edge;
            msgData.rc = appRect;
            ShellApi.SHAppBarMessage((UInt32)AppBarMessages.SetPos, ref msgData);
            appRect = msgData.rc;
        }

        private void AppbarActivate() {
            ShellApi.APPBARDATA msgData = new ShellApi.APPBARDATA();
            msgData.cbSize = (UInt32)Marshal.SizeOf(msgData);
            msgData.hWnd = Form.Handle;
            ShellApi.SHAppBarMessage((UInt32)AppBarMessages.Activate, ref msgData);
        }

        protected Boolean AppbarSetAutoHideBar(Boolean hideValue) {
            ShellApi.APPBARDATA msgData = new ShellApi.APPBARDATA();
            msgData.cbSize = (UInt32)Marshal.SizeOf(msgData);
            msgData.hWnd = Form.Handle;
            msgData.uEdge = (UInt32)_Edge;
            msgData.lParam = (hideValue) ? 1 : 0;
            UInt32 retVal = ShellApi.SHAppBarMessage((UInt32)AppBarMessages.SetAutoHideBar, ref msgData);
            return (retVal != 0) ? true : false;
        }

        private IntPtr AppbarGetAutoHideBar(AppBarEdges edge) {
            ShellApi.APPBARDATA msgData = new ShellApi.APPBARDATA();
            msgData.cbSize = (UInt32)Marshal.SizeOf(msgData);
            msgData.uEdge = (UInt32)edge;
            IntPtr retVal = (IntPtr)ShellApi.SHAppBarMessage((UInt32)AppBarMessages.GetAutoHideBar, ref msgData);
            return retVal;
        }

        #endregion AppBar Functions

        #region Private Variables

        private UInt32 CallbackMessageID = 0;

        private Boolean IsAppbarMode = false;

        #endregion Private Variables

        /// Turns the form into an AppBar on the given edge.
        /// It is up to you to make sure that the form can not be moved or resized
        public ApplicationDesktopToolbar(Form form, AppBarEdges edge) {
            Form = form;

            // Make sure that this form can be an appbar
            if (Form.FormBorderStyle != FormBorderStyle.None)
                throw new Exception("Only forms with a FormBorderStyle of None are supported as an ApplicationDesktopToolbar. This is because the system does not support resizable and draggable AppBars");
            if (Form.TopMost == false)
                throw new Exception("An AppBar must be topmost to work properly");

            // Register a unique message as our callback message
            CallbackMessageID = RegisterCallbackMessage();
            if (CallbackMessageID == 0)
                throw new Exception("RegisterCallbackMessage failed");

            _Edge = edge;
            AppbarNew();

            Form.FormClosing += new FormClosingEventHandler(Form_Closing);

            Timer = new Timer();
            Timer.Tick += new EventHandler(Timer_Tick);
            Timer.Interval = 500;
        }

        private UInt32 RegisterCallbackMessage() {
            String uniqueMessageString = Guid.NewGuid().ToString();
            return ShellApi.RegisterWindowMessage(uniqueMessageString);
        }

        /// Register the AppBar with the OS and sets its appropriate size
        private void SizeAppBar()
        {
            ShellApi.RECT rt = new ShellApi.RECT();

            if ((_Edge == AppBarEdges.Left) || (_Edge == AppBarEdges.Right)) {
                rt.Top = 0;
                rt.Bottom = SystemInformation.PrimaryMonitorSize.Height;
                if (_Edge == AppBarEdges.Left) {
                    rt.Right = Form.Width;
                    rt.Left = 0;
                }
                else {
                    rt.Right = SystemInformation.PrimaryMonitorSize.Width;
                    rt.Left = rt.Right - Form.Width;
                }
            }
            else {
                rt.Left = 0;
                rt.Right = SystemInformation.PrimaryMonitorSize.Width;
                if (_Edge == AppBarEdges.Top) {
                    rt.Bottom = Form.Height;
                }
                else {
                    rt.Bottom = SystemInformation.PrimaryMonitorSize.Height;
                    rt.Top = rt.Bottom - Form.Height;
                }
            }

            AppbarQueryPos(ref rt);
            Application.DoEvents();

            AppbarSetPos(ref rt);
            Application.DoEvents();

            Point desiredLocation = new Point(rt.Left, rt.Top);
            if (!Form.Location.Equals(desiredLocation))
                Form.Location = new Point(rt.Left, rt.Top);

            Size desiredSize = new Size(rt.Right - rt.Left, rt.Bottom - rt.Top);
            if (!Form.Size.Equals(desiredSize))
                Form.Size = desiredSize;
        }

        private void Form_Closing(object sender, CancelEventArgs e) {
            Timer.Stop();
            AppbarRemove();
        }

        public AppBarEdges Edge { get { return _Edge; } }
        private AppBarEdges _Edge;

        /// A 1-pixel-wide or 1-pixel-tall form used when the appbar is set to autohide. It makes the edge
        /// sticky so that the appbar is shown when the mouse hits the edge
        public Form EdgeForm { get { return _EdgeForm; } }
        private Form _EdgeForm = null;

        /// Gets/Sets the autohide status. Note: When setting AutoHide to true, it's possible for such a
        /// setting to fail if there's already an autohide bar on the edge. In that case, this property will
        /// remain false
        public bool AutoHide {
            get { return _AutoHide; }
            set {
                // changing to settings that are already set would f*ck things up!!!
                if (value == _AutoHide)
                    return;

                // Autohide actions differ based on what we're doing
                if (value) {
                    // We're setting autohide on

                    // Let's make sure that there already isn't an existing autohide bar
                    if (IntPtr.Zero != AppbarGetAutoHideBar(_Edge))
                        return;

                    // Ok, we can autohide on this edge. Remove the old appbar and add a new one
                    AppbarRemove();
                    Application.DoEvents();

                    ShellApi.RECT edgeFormSize = GenerateRECTForEdgeForm(1);

                    AppbarNew();

                    AppbarQueryPos(ref edgeFormSize);
                    Application.DoEvents();

                    AppbarSetPos(ref edgeFormSize);
                    Application.DoEvents();

                    // If we can't set the autohide bar, return without changing the setting
                    if (!AppbarSetAutoHideBar(true)) {
                        AppbarRemove();
                        AppbarNew();
                        SizeAppBar();
                        return;
                    }

                    // Hide the form
                    Form.Visible = false;

                    // Create edge form
                    _EdgeForm = new Form();
                    _EdgeForm.TopMost = true;
                    _EdgeForm.FormBorderStyle = FormBorderStyle.None;
                    _EdgeForm.ShowInTaskbar = false;
                    _EdgeForm.MouseEnter += new EventHandler(UnhideAppbar);
                    _EdgeForm.MouseHover += new EventHandler(UnhideAppbar);
                    _EdgeForm.MouseMove += new MouseEventHandler(UnhideAppbar);

                    _EdgeForm.Show();
                    _EdgeForm.Focus();

                    // Keep Bill Gates Happy
                    Application.DoEvents();

                    // Set up location of EdgeForm
                    _EdgeForm.LocationChanged += new EventHandler(_EdgeForm_FixLocationAndSize);
                    _EdgeForm.SizeChanged += new EventHandler(_EdgeForm_FixLocationAndSize);
                    _EdgeForm_FixLocationAndSize(this, new EventArgs());

                    //Form.MouseLeave += new EventHandler(Form_MouseLeave);

                    Form.TopMost = true;
                }
                else {
                    // Turn autohide bar off
                    AppbarSetAutoHideBar(false);
                    SizeAppBar();
                    EdgeForm.Close();
                    _EdgeForm = null;
                    Form.Visible = true;
                    Timer.Stop();
                }

                _AutoHide = value;
            }
        }
        private bool _AutoHide = false;

        /// Generates coordinates for the edge form used during autohide
        private ShellApi.RECT GenerateRECTForEdgeForm(int size) {
            // resize appbar
            switch (_Edge) {
                case AppBarEdges.Bottom:
                    return new ShellApi.RECT(
                    0,
                    SystemInformation.PrimaryMonitorSize.Width - 1,
                    SystemInformation.PrimaryMonitorSize.Height - 1 - size,
                    SystemInformation.PrimaryMonitorSize.Height - 1);

                case AppBarEdges.Top:
                    return new ShellApi.RECT(
                    0,
                    SystemInformation.PrimaryMonitorSize.Width / 2 - 1,
                    0,
                    size);

                case AppBarEdges.Left:
                    return new ShellApi.RECT(
                    0,
                    size,
                    0,
                    SystemInformation.PrimaryMonitorSize.Height - 1);

                case AppBarEdges.Right:
                    return new ShellApi.RECT(
                    SystemInformation.PrimaryMonitorSize.Width - 1 - size,
                    SystemInformation.PrimaryMonitorSize.Width - 1,
                    0,
                    SystemInformation.PrimaryMonitorSize.Height - 1);

                default:
                    throw new Exception("Unrecognized edge");
            }
        }

        /// Handles sizing for the edge form used during autohide
        void _EdgeForm_FixLocationAndSize(object sender, EventArgs e) {
            ShellApi.RECT edgeFormSize = GenerateRECTForEdgeForm(0);
            _EdgeForm.Location = new Point(edgeFormSize.Left, edgeFormSize.Top);
            _EdgeForm.Size = new Size(edgeFormSize.Right - edgeFormSize.Left, edgeFormSize.Bottom - edgeFormSize.Top);
        }

        /// Displays the docked AppBar when AutoHide is on
        void UnhideAppbar(object sender, EventArgs e) {
            UnhideAppbar();
        }

        /// Displays the docked AppBar when AutoHide is on
        void UnhideAppbar() {
            _EdgeForm.Hide();
            Application.DoEvents();
            Form.Visible = true;
            Form.BringToFront();
            Form.Focus();
            Timer.Start();
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            // Stop watch if the form becomes visible
            if (!AutoHide) {
                Timer.Stop();
                return;
            }

            // Do not hide if there is a model form displayed
            if (!Form.CanFocus) {
                return;
            }

            ShellApi.POINT point = new ShellApi.POINT();

            ShellApi.GetCursorPos(ref point);

            // Make sure that the mouse cursor is within the form
            if (point.x >= Form.Left)
                if (point.x <= Form.Right)
                    if (point.y >= Form.Top)
                        if (point.y <= Form.Bottom)
                            return;

            Form.Visible = false;
            _EdgeForm.TopMost = true;
            _EdgeForm.Show();
        }

        /// Timer used to watch the mouse when the hidden form is displayed
        private Timer Timer;

        /// Displays the docked AppBar when AutoHide is on
        void UnhideAppbar(object sender, MouseEventArgs e) {
            UnhideAppbar();
        }

        /// Releases the screen area held by the appBar back to the OS
        public void Dispose() {
            AutoHide = false;
            AppbarRemove();
        }

        /// Base class for all exceptions generated by the ApplicationDesktopToolbar
        public class Exception : System.Exception {
            internal Exception(string message) : base(message) { }
        }
    }
}