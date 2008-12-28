using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Wasp {
    class TopController : IDisposable {
        private TopModel model;
        private TopForm form;
        private AppBar appBar;
        public event EventHandler FormDestroyed;

        public TopController(TopModel model) {
            this.model = model;

            this.form = new TopForm();
            this.form.Visible = false;
            this.form.HandleDestroyed += delegate(Object sender, EventArgs e) {
                this.FormDestroyed(sender, e);
            };
            this.form.exitButton.Click += delegate(Object sender, EventArgs e) {
                this.form.Close();
            };
            this.form.pinButton.Click += delegate(Object sender, EventArgs e) {
                this.Pinned = !this.Pinned;
            };

            Boolean autoHide = false;
            this.appBar = new AppBar(this.form, autoHide);
        }

        public void Show() {
            this.appBar.Show();
        }

        /// Releases the screen area held by the appBar back to the OS
        public void Dispose() {
            //AutoHide = false;
            this.appBar.Hide();
        }

        public bool Pinned {
            get { return this.appBar.Pinned; }
            set { this.appBar.Pinned = value; }
        }
    }
}