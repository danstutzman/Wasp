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
        public event EventHandler ClockTick;

        public TopModel() {
            //this.pinned = true;
            this.time = DateTime.Now.ToLongTimeString();
        }

        public void InitTimer() {
            ScheduleTimer alarm = new ScheduleTimer();
            DateTime alarmTime = new DateTime(2008, 12, 28, 15, 42, 30);
            alarm.AddJob(new TimerJob(new SingleEvent(alarmTime),
                new DelegateMethodCall(new NoArgsDelegate(TriggerAlarm))));
            alarm.Start();

            Timer clock = new Timer();
            clock.Interval = 1000;
            clock.Tick += Tick;
            clock.Start();
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
        public bool Alarmed { get { return this.alarmed; } }

        private String time;
        public String Time { get { return this.time; } }

        public void SnoozeAlarm() {
            this.alarmed = false;
            this.AlarmChange(this, new EventArgs());
        }

        private void TriggerAlarm() {
            this.alarmed = true;
            this.AlarmChange(this, new EventArgs());
        }

        private void Tick(Object sender, EventArgs e) {
            this.time = DateTime.Now.ToLongTimeString();
            this.ClockTick(this, new EventArgs());
        }
    }
}