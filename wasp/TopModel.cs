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
using System.Net;
using System.IO;

namespace Wasp {
    class TopModel {
        private delegate void NoArgsDelegate();
        private delegate void OneArgDelegate(Alarm alarm);

        public event EventHandler PinnedChange;
        public event EventHandler<AlarmEventArgs> AlarmChange;
        public event EventHandler ClockTick;

        private ScheduleTimer scheduleTimer;
        private List<Alarm> alarms;
        private DateTime scheduleLastModified;

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

            Timer scheduleRetriever = new Timer();
            scheduleRetriever.Interval = 3000;
            scheduleRetriever.Tick += TimeToCheckSchedule;
            scheduleRetriever.Start();
        }

        public void InstallSchedule(String scheduleXml) {
            this.alarms = new List<Alarm>();
            this.scheduleTimer.Stop();
            this.scheduleTimer.ClearJobs();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(scheduleXml);

            foreach (XmlNode eventNode in doc.SelectSingleNode("schedule").ChildNodes) {
                String eventName = eventNode.Attributes.GetNamedItem("short_name").Value;
                //Console.WriteLine("event: {0}", eventName);
                foreach (XmlNode alarmNode in eventNode.ChildNodes) {
                    String alarmWhenString = alarmNode.Attributes.GetNamedItem("datetime").Value;
                    DateTime alarmWhen = DateTime.ParseExact(alarmWhenString, "yyyy-MM-dd HH:mm:ss",
                        CultureInfo.InvariantCulture);
                    Console.WriteLine("when: {0}", alarmWhen);

                    Alarm alarm = new Alarm();
                    alarm.eventName = eventName;
                    this.scheduleTimer.AddJob(new TimerJob(new SingleEvent(alarmWhen),
                        new DelegateMethodCall(new OneArgDelegate(TriggerAlarm), alarm)));
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

        private void TriggerAlarm(Alarm alarm) {
            this.alarmed = true;
            this.AlarmChange(this, new AlarmEventArgs(alarm));
        }

        private void Tick(Object sender, EventArgs e) {
            this.time = DateTime.Now.ToLongTimeString();
            this.ClockTick(this, new EventArgs());
        }

        private void TimeToCheckSchedule(Object sender, EventArgs args) {
            HttpWebRequest request = WebRequest.Create("http://localhost:3000/schedule.xml") as HttpWebRequest;
            request.IfModifiedSince = this.scheduleLastModified;
            request.BeginGetResponse(delegate(IAsyncResult result) {
                WebResponse response = null;
                try {
                    response = request.EndGetResponse(result);
                    string xml = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    this.scheduleLastModified = DateTime.Parse(response.Headers.Get("Last-Modified"));
                    this.InstallSchedule(xml);
                }
                catch (WebException e) {
                    HttpWebResponse response2 = e.Response as HttpWebResponse;
                    if (response2.StatusCode == HttpStatusCode.NotModified)
                        ; // no problem
                    else
                        throw e;
                }
                finally {
                    if (response != null)
                        response.Close();
                }
            }, null);
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