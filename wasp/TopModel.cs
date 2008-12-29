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
        private delegate void OneArgDelegate(Alarm alarm);

        public event EventHandler PinnedChange;
        public event EventHandler<AlarmEventArgs> AlarmChange;
        public event EventHandler ClockTick;

        private ScheduleTimer scheduleTimer;
        private List<Alarm> alarms;

        public TopModel() {
            //this.pinned = true;
            this.time = DateTime.Now.ToLongTimeString();
        }

        public void InitTimer() {
            this.scheduleTimer = new ScheduleTimer();
            //            this.scheduleTimer.Start();

            Timer clock = new Timer();
            clock.Interval = 1000;
            clock.Tick += Tick;
            clock.Start();
        }

        public void InitSchedule() {
            this.alarms = new List<Alarm>();

            XmlDocument doc = new XmlDocument();
            doc.Load("../../schedule.xml");
            foreach (XmlNode eventNode in doc.SelectSingleNode("schedule").ChildNodes) {
                String eventName = eventNode.Attributes.GetNamedItem("name").Value;
                //Console.WriteLine("event: {0}", eventName);
                foreach (XmlNode alarmNode in eventNode.ChildNodes) {
                    String alarmWhenString = alarmNode.Attributes.GetNamedItem("when").Value;
                    DateTime alarmWhen = DateTime.ParseExact(alarmWhenString, "yyyy-MM-dd HH:mm:ss",
                        CultureInfo.InvariantCulture);
                    Console.WriteLine("when: {0}", alarmWhen);

                    Alarm alarm = new Alarm();
                    alarm.eventName = eventName;
                    this.scheduleTimer.AddJob(new TimerJob(new SingleEvent(alarmWhen),
                        new DelegateMethodCall(new OneArgDelegate(TriggerAlarm2), alarm)));
                    this.alarms.Add(alarm);
                }
            }
            this.scheduleTimer.Start();
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
            this.AlarmChange(this, new AlarmEventArgs());
        }

        //private void TriggerAlarm() {
        //    this.alarmed = true;
        //    this.AlarmChange(this, new EventArgs());
        //}

        private void TriggerAlarm2(Alarm alarm) {
            this.alarmed = true;
            this.AlarmChange(this, new AlarmEventArgs(alarm));
        }

        private void Tick(Object sender, EventArgs e) {
            this.time = DateTime.Now.ToLongTimeString();
            this.ClockTick(this, new EventArgs());
        }
    }

    struct Alarm {
        public String eventName;
        public DateTime when;
    }

    class AlarmEventArgs : EventArgs {
        public Alarm alarm;
        public AlarmEventArgs() {
        }
        public AlarmEventArgs(Alarm alarm) {
            this.alarm = alarm;
        }
    }
}