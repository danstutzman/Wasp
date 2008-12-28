using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Wasp {
    class AppBar {
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

        private bool autoHide;
        private AppBarEdges _Edge = AppBarEdges.Top;
        private Form form;
        private UInt32 callbackMessageID;
        private Form _EdgeForm;
        private ShellApi.RECT formSize;
        private ShellApi.RECT edgeFormSize;

        public AppBar(Form form, bool autoHide) {
            this.form = form;
            this.callbackMessageID = RegisterCallbackMessage();
            this.form.FormClosing += new FormClosingEventHandler(Form_Closing);
            this.autoHide = autoHide;

            if (this.form.FormBorderStyle != FormBorderStyle.None)
                throw new Exception("Only forms with a FormBorderStyle of None are supported as an ApplicationDesktopToolbar. This is because the system does not support resizable and draggable AppBars");
            if (this.form.TopMost == false)
                throw new Exception("An AppBar must be topmost to work properly");

            this.formSize = PickAppropriateSize();
            this.edgeFormSize = GenerateRECTForEdgeForm(1);

            _EdgeForm = new Form();
            _EdgeForm.TopMost = true;
            _EdgeForm.FormBorderStyle = FormBorderStyle.None;
            _EdgeForm.ShowInTaskbar = false;
            _EdgeForm.MouseEnter += new EventHandler(UnhideAppbar);
            _EdgeForm.MouseHover += new EventHandler(UnhideAppbar);
            _EdgeForm.MouseMove += new MouseEventHandler(UnhideAppbar);

            Timer = new Timer();
            Timer.Tick += new EventHandler(Timer_Tick);
            Timer.Interval = 500;

            this.MakeNew();
        }

        public bool Pinned {
            get { return !this.autoHide; }
            set {
                if (this.autoHide && value) {
                    this.form.BeginInvoke(new MethodInvoker(delegate() {
                        AppbarSetAutoHideBar(false);
                        SizeAppBar(ref this.formSize);
                        ResizeAppBar(ref this.formSize);
                        //this._EdgeForm.Close();
                        //this._EdgeForm = null;
                        this._EdgeForm.Visible = false;
                        this.form.Visible = true;
                        Timer.Stop();
                    }));
                }
                else if (!this.autoHide && !value) {
                    this.form.BeginInvoke(new MethodInvoker(delegate() {
                        this.autoHide = true;
                        this.Show();
                        Timer.Start();
                    }));
                }
                this.autoHide = !value;
            }
        }

        public void Show() {
            if (this.autoHide) {
                if (IntPtr.Zero != AppbarGetAutoHideBar(_Edge))
                    return;

                SizeAppBar(ref edgeFormSize);
                ResizeAppBar(ref this.formSize);

                AppbarQueryPos(ref edgeFormSize);
                Application.DoEvents();
                AppbarSetPos(ref edgeFormSize);
                Application.DoEvents();

                _EdgeForm.Show();
                _EdgeForm.Focus();

                // Keep Bill Gates Happy
                Application.DoEvents();

                // Set up location of EdgeForm
                _EdgeForm.LocationChanged += new EventHandler(_EdgeForm_FixLocationAndSize);
                _EdgeForm.SizeChanged += new EventHandler(_EdgeForm_FixLocationAndSize);
                _EdgeForm_FixLocationAndSize(this, new EventArgs());

                //Form.MouseLeave += new EventHandler(Form_MouseLeave);

                this.form.TopMost = true;
            }
            else {
                this.form.Visible = true;
                SizeAppBar(ref this.formSize);
                ResizeAppBar(ref this.formSize);
            }
        }

        private void MakeNew() {
            ShellApi.APPBARDATA msgData = new ShellApi.APPBARDATA();
            msgData.cbSize = (UInt32)Marshal.SizeOf(msgData);
            msgData.hWnd = this.form.Handle;
            msgData.uCallbackMessage = callbackMessageID;
            UInt32 retVal = ShellApi.SHAppBarMessage((UInt32)AppBarMessages.New, ref msgData);
            if (retVal == 0)
                throw new Exception("Got zero from AppBarMessages.New");
        }

        private ShellApi.RECT PickAppropriateSize() {
            ShellApi.RECT rt = new ShellApi.RECT();

            if ((_Edge == AppBarEdges.Left) || (_Edge == AppBarEdges.Right)) {
                rt.Top = 0;
                rt.Bottom = SystemInformation.PrimaryMonitorSize.Height;
                if (_Edge == AppBarEdges.Left) {
                    rt.Right = this.form.Width;
                    rt.Left = 0;
                }
                else {
                    rt.Right = SystemInformation.PrimaryMonitorSize.Width;
                    rt.Left = rt.Right - this.form.Width;
                }
            }
            else {
                rt.Left = 0;
                rt.Right = SystemInformation.PrimaryMonitorSize.Width;
                if (_Edge == AppBarEdges.Top) {
                    rt.Bottom = this.form.Height;
                }
                else {
                    rt.Bottom = SystemInformation.PrimaryMonitorSize.Height;
                    rt.Top = rt.Bottom - this.form.Height;
                }
            }

            return rt;
        }

        /// Register the AppBar with the OS and sets its appropriate size
        private void SizeAppBar(ref ShellApi.RECT rt) {
            AppbarQueryPos(ref rt);
            Application.DoEvents();

            AppbarSetPos(ref rt);
            Application.DoEvents();
        }

        private void ResizeAppBar(ref ShellApi.RECT rt) {
            Point desiredLocation = new Point(rt.Left, rt.Top);
            if (!this.form.Location.Equals(desiredLocation))
                this.form.Location = new Point(rt.Left, rt.Top);

            Size desiredSize = new Size(rt.Right - rt.Left, rt.Bottom - rt.Top);
            if (!this.form.Size.Equals(desiredSize))
                this.form.Size = desiredSize;
        }

        private UInt32 RegisterCallbackMessage() {
            String uniqueMessageString = Guid.NewGuid().ToString();
            return ShellApi.RegisterWindowMessage(uniqueMessageString);
        }

        /// Passes a proposed AppBar location to the OS to verify that it's ok to set
        private void AppbarQueryPos(ref ShellApi.RECT appRect) {
            ShellApi.APPBARDATA msgData = new ShellApi.APPBARDATA();
            msgData.cbSize = (UInt32)Marshal.SizeOf(msgData);
            msgData.hWnd = this.form.Handle;
            msgData.uEdge = (UInt32)_Edge;
            msgData.rc = appRect;
            ShellApi.SHAppBarMessage((UInt32)AppBarMessages.QueryPos, ref msgData);
            appRect = msgData.rc;
        }

        private void AppbarSetPos(ref ShellApi.RECT appRect) {
            ShellApi.APPBARDATA msgData = new ShellApi.APPBARDATA();
            msgData.cbSize = (UInt32)Marshal.SizeOf(msgData);
            msgData.hWnd = this.form.Handle;
            msgData.uEdge = (UInt32)_Edge;
            msgData.rc = appRect;
            ShellApi.SHAppBarMessage((UInt32)AppBarMessages.SetPos, ref msgData);
            appRect = msgData.rc;
        }

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

        private IntPtr AppbarGetAutoHideBar(AppBarEdges edge) {
            ShellApi.APPBARDATA msgData = new ShellApi.APPBARDATA();
            msgData.cbSize = (UInt32)Marshal.SizeOf(msgData);
            msgData.uEdge = (UInt32)edge;
            IntPtr retVal = (IntPtr)ShellApi.SHAppBarMessage((UInt32)AppBarMessages.GetAutoHideBar, ref msgData);
            return retVal;
        }

        protected Boolean AppbarSetAutoHideBar(Boolean hideValue) {
            ShellApi.APPBARDATA msgData = new ShellApi.APPBARDATA();
            msgData.cbSize = (UInt32)Marshal.SizeOf(msgData);
            msgData.hWnd = this.form.Handle;
            msgData.uEdge = (UInt32)_Edge;
            msgData.lParam = (hideValue) ? 1 : 0;
            UInt32 retVal = ShellApi.SHAppBarMessage((UInt32)AppBarMessages.SetAutoHideBar, ref msgData);
            return (retVal != 0) ? true : false;
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
            this.form.Visible = true;
            this.form.BringToFront();
            this.form.Focus();
            Timer.Start();
        }

        void Timer_Tick(object sender, EventArgs e) {
            // Stop watch if the form becomes visible
            /*if (!AutoHide) {
                Timer.Stop();
                return;
            }*/

            // Do not hide if there is a model form displayed
            if (!this.form.CanFocus) {
                return;
            }

            ShellApi.POINT point = new ShellApi.POINT();

            ShellApi.GetCursorPos(ref point);

            // Make sure that the mouse cursor is within the form
            if (point.x >= this.form.Left)
                if (point.x <= this.form.Right)
                    if (point.y >= this.form.Top)
                        if (point.y <= this.form.Bottom)
                            return;

            this.form.Visible = false;
            _EdgeForm.TopMost = true;
            _EdgeForm.Show();
        }

        /// Timer used to watch the mouse when the hidden form is displayed
        private Timer Timer;

        /// Displays the docked AppBar when AutoHide is on
        void UnhideAppbar(object sender, MouseEventArgs e) {
            UnhideAppbar();
        }


        private void Form_Closing(object sender, CancelEventArgs e) {
            //Timer.Stop();
            Hide();
        }

        public void Hide() {
            ShellApi.APPBARDATA msgData = new ShellApi.APPBARDATA();
            msgData.cbSize = (UInt32)Marshal.SizeOf(msgData);
            msgData.hWnd = this.form.Handle;
            UInt32 retVal = ShellApi.SHAppBarMessage((UInt32)AppBarMessages.Remove, ref msgData);
            // always returns true
        }
    }
}