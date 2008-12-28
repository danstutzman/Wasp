using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Schedule;

namespace Wasp {
    class TopModel {
        private delegate void NoArgsDelegate();

        public event EventHandler PinnedChange;
        public event EventHandler AlarmChange;

        public TopModel() {
            //this.pinned = true;
        }

        public void InitTimer() {
            ScheduleTimer timer = new ScheduleTimer();
            DateTime time = new DateTime(2008, 12, 28, 15, 42, 30);
            timer.AddJob(new TimerJob(new SingleEvent(time),
                new DelegateMethodCall(new NoArgsDelegate(Tick))));
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

        public void Tick() {
            this.alarmed = true;
            this.AlarmChange(this, new EventArgs());
        }
    }
}