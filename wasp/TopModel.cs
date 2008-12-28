﻿using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Wasp {
    class TopModel {
        public event EventHandler PinnedChange;
        public event EventHandler AlarmChange;

        public TopModel() {
            //this.pinned = true;
        }

        public void InitTimer() {
            this.alarmed = true;
            Timer timer = new Timer();
            timer.Tick += delegate(Object sender, EventArgs e) {
                this.alarmed = true;
                this.AlarmChange(sender, e);
            };
            timer.Interval = 5000;
            timer.Start();
        }

        private bool pinned;
        public bool Pinned {
            get { return this.pinned; }
            set {
                this.pinned = value;
                this.PinnedChange(this, new EventArgs());
            }
        }

        private bool alarmed;
        public bool Alarmed {
            get { return this.alarmed; }
        }

        public void SnoozeAlarm() {
            this.alarmed = false;
            this.AlarmChange(this, new EventArgs());
        }
    }
}