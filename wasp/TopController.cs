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
                this.form.pinCheckBox.CheckState = CheckState.Checked;
            this.form.pinCheckBox.CheckedChanged += delegate(Object sender, EventArgs e) {
                this.model.Pinned = this.form.pinCheckBox.Checked;
            };

            this.appBar = new AppBar(this.form, this.model.Pinned);
            this.model.PinnedChange += delegate(Object sender, EventArgs e) {
                this.appBar.Pinned = this.model.Pinned;
            };
        }

        public void Show() {
            this.appBar.Show();
        }

        /// Releases the screen area held by the appBar back to the OS
        public void Dispose() {
            //AutoHide = false;
            this.appBar.Hide();
        }
    }
}