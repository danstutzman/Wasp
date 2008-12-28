﻿using System;
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
        private Timer flashTimer;
        private bool backgroundIsRed;

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

            this.form.pinCheckBox.CheckState =
                this.model.Pinned ? CheckState.Checked : CheckState.Unchecked;
            this.form.pinCheckBox.CheckedChanged += delegate(Object sender, EventArgs e) {
                this.model.Pinned = this.form.pinCheckBox.Checked;
            };

            this.form.snoozeButton.Click += delegate(Object sender, EventArgs e) {
                this.model.SnoozeAlarm();
            };
            this.form.snoozeButton.Enabled = this.model.Alarmed;

            this.appBar = new AppBar(this.form, !this.model.Pinned);
            this.model.PinnedChange += delegate(Object sender, EventArgs e) {
                this.appBar.Pinned = this.model.Pinned;
            };
            this.model.AlarmChange += delegate(Object sender, EventArgs e) {
                if (this.model.Alarmed) {
                    this.flashTimer.Start();
                    this.appBar.KeepOpen = true;
                    this.form.snoozeButton.Enabled = true;
                }
                else {
                    this.flashTimer.Stop();
                    this.appBar.KeepOpen = false;
                    this.form.BackColor = System.Drawing.SystemColors.Control;
                    this.form.snoozeButton.Enabled = false;
                }
            };

            this.backgroundIsRed = false;
            this.flashTimer = new Timer();
            this.flashTimer.Tick += FlashBackground;
        }

        public void Show() {
            this.appBar.Show();
        }

        private void FlashBackground(Object sender, EventArgs e) {
            this.backgroundIsRed = !this.backgroundIsRed;
            this.form.BackColor = this.backgroundIsRed ?
                System.Drawing.Color.Red : System.Drawing.SystemColors.Control;
        }

        /// Releases the screen area held by the appBar back to the OS
        public void Dispose() {
            //AutoHide = false;
            this.appBar.Hide();
        }
    }
}