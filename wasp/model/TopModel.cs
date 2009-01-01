using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using Schedule;
using System.Globalization;
using System.Collections.Generic;

namespace Wasp {
    class TopModel {
        private delegate void NoArgsDelegate();
        private delegate void OneArgDelegate(AlarmModel alarm);

        public event EventHandler PinnedChange;
        public event EventHandler ClockTick;
        public event EventHandler ScheduleChange;

        private ScheduleTimer scheduleTimer;

        private List<AlarmModel> alarms;
        public List<AlarmModel> Alarms { get { return this.alarms; } }

        private bool pinned;
        public bool Pinned {
            get { return this.pinned; }
            set {
                this.pinned = value;
                this.PinnedChange(this, new EventArgs());
            }
        }

        private String time;
        public String Time { get { return this.time; } }

        private List<AlarmModel> alarmsToFireAsap;

        private ScheduleService service;

        public TopModel() {
            this.time = DateTime.Now.ToLongTimeString();
            this.alarmsToFireAsap = new List<AlarmModel>();
            this.service = new ScheduleService();
        }

        public void InitTimer() {
            this.scheduleTimer = new ScheduleTimer();

            Timer clock = new Timer();
            clock.Interval = 1000;
            clock.Tick += Tick;
            clock.Start();

            Timer scheduleRetriever = new Timer();
            scheduleRetriever.Interval = 3000;
            scheduleRetriever.Tick += TimeToCheckSchedule;
            scheduleRetriever.Start();
        }

        public void ReadyForAlarms() {
            this.scheduleTimer.Start();
            foreach (AlarmModel alarm in this.alarmsToFireAsap)
                alarm.Fire();
            this.alarmsToFireAsap.Clear();
        }

        private void FireAlarm(AlarmModel alarm) {
            alarm.Fire();
        }

        private void Tick(Object sender, EventArgs e) {
            this.time = DateTime.Now.ToLongTimeString();
            this.ClockTick(this, new EventArgs());
        }

        private void TimeToCheckSchedule(Object sender, EventArgs args) {
            this.service.CheckForNewAlarms(InstallNewAlarms);
        }

        public void InstallNewAlarms(List<AlarmModel> newAlarms) {
            this.alarms = new List<AlarmModel>();
            this.scheduleTimer.Stop();
            this.scheduleTimer.ClearJobs();

            foreach (AlarmModel alarm in newAlarms) {
                this.alarms.Add(alarm);
                if (alarm.IsArmed) {
                    if (alarm.When > DateTime.Now)
                        this.scheduleTimer.AddJob(new TimerJob(new SingleEvent(alarm.When),
                            new DelegateMethodCall(new OneArgDelegate(FireAlarm), alarm)));
                    else
                        this.alarmsToFireAsap.Add(alarm);
                }
            }

            this.ScheduleChange(this, new EventArgs());
        }
    }

 
    class AlarmEventArgs : EventArgs {
        public AlarmModel alarm;
        public AlarmEventArgs() {
        }
        public AlarmEventArgs(AlarmModel alarm) {
            this.alarm = alarm;
        }
    }
}