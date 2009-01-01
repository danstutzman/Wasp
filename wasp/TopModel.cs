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
        private delegate void OneArgDelegate(AlarmModel alarm);

        public event EventHandler PinnedChange;
        public event EventHandler ClockTick;
        public event EventHandler ScheduleChange;

        private ScheduleTimer scheduleTimer;

        private List<AlarmModel> alarms;
        public List<AlarmModel> Alarms { get { return this.alarms; } }

        private DateTime scheduleLastModified;

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

        List<AlarmModel> alarmsToFireAsap;

        public void ReadyForAlarms() {
            this.scheduleTimer.Start();
            foreach (AlarmModel alarm in this.alarmsToFireAsap)
                alarm.Fire();
            this.alarmsToFireAsap.Clear();
        }

        public TopModel() {
            this.time = DateTime.Now.ToLongTimeString();
            this.alarmsToFireAsap = null;
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

        private void FireAlarm(AlarmModel alarm) {
            alarm.Fire();
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
                    if (response2.StatusCode != HttpStatusCode.NotModified)
                        throw e;
                }
                finally {
                    if (response != null)
                        response.Close();
                }
            }, null);
        }

        private void InstallSchedule(String scheduleXml) {
            this.alarms = new List<AlarmModel>();
            this.scheduleTimer.Stop();
            this.scheduleTimer.ClearJobs();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(scheduleXml);

            this.alarmsToFireAsap = new List<AlarmModel>();
            foreach (XmlNode alarmNode in doc.SelectSingleNode("schedule").ChildNodes) {
                String alarmWhenString = alarmNode.Attributes.GetNamedItem("datetime").Value;
                DateTime alarmWhen = DateTime.ParseExact(alarmWhenString, "yyyy-MM-dd HH:mm:ss",
                    CultureInfo.InvariantCulture);
                Console.WriteLine("when: {0}", alarmWhen);

                String id = alarmNode.Attributes.GetNamedItem("id").Value;
                String name = alarmNode.Attributes.GetNamedItem("name").Value;
                bool isArmed = alarmNode.Attributes.GetNamedItem("armed").Value.Equals("true");
                AlarmModel alarm = new AlarmModel(id, name, alarmWhen, isArmed);

                this.alarms.Add(alarm);
                if (alarm.IsArmed) {
                    if (alarm.When > DateTime.Now)
                        this.scheduleTimer.AddJob(new TimerJob(new SingleEvent(alarmWhen),
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