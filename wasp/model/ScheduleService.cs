using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Globalization;

namespace Wasp {
    class ScheduleService {
        public delegate void InstallNewAlarms(List<AlarmModel> newAlarms);

        private DateTime scheduleLastModified;

        public ScheduleService() {
        }

        public void CheckForNewAlarms(InstallNewAlarms installer) {
            HttpWebRequest request = WebRequest.Create("http://localhost:3000/schedule.xml") as HttpWebRequest;
            request.IfModifiedSince = this.scheduleLastModified;
            request.BeginGetResponse(delegate(IAsyncResult result) {
                WebResponse response = null;
                try {
                    response = request.EndGetResponse(result);
                    string xml = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    this.scheduleLastModified = DateTime.Parse(response.Headers.Get("Last-Modified"));

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);

                    List<AlarmModel> newAlarms = new List<AlarmModel>();
                    foreach (XmlNode alarmNode in doc.SelectSingleNode("schedule").ChildNodes) {
                        String alarmWhenString = alarmNode.Attributes.GetNamedItem("datetime").Value;
                        DateTime alarmWhen = DateTime.ParseExact(alarmWhenString, "yyyy-MM-dd HH:mm:ss",
                            CultureInfo.InvariantCulture);
                        //Console.WriteLine("when: {0}", alarmWhen);

                        String id = alarmNode.Attributes.GetNamedItem("id").Value;
                        String name = alarmNode.Attributes.GetNamedItem("name").Value;
                        bool isArmed = alarmNode.Attributes.GetNamedItem("armed").Value.Equals("true");
                        AlarmModel alarm = new AlarmModel(id, name, alarmWhen, isArmed);

                        alarm.FiringChange += this.OnAlarmFiringChange;

                        newAlarms.Add(alarm);
                    }

                    installer(newAlarms);
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

        private void OnAlarmFiringChange(Object sender, EventArgs e) {
            AlarmModel alarm = (sender as AlarmModel);

            string postData = "firing=" + alarm.IsFiring + "&armed=" + alarm.IsArmed;
            byte[] postDataBytes = Encoding.UTF8.GetBytes(postData);
            HttpWebRequest request = WebRequest.Create("http://localhost:3000/alarm/service_update/" + alarm.Id) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postDataBytes.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(postDataBytes, 0, postDataBytes.Length);
            dataStream.Close();
            request.BeginGetResponse(delegate(IAsyncResult result) {
                WebResponse response = request.EndGetResponse(result);
                response.Close();
            }, null);

            // todo: update the last-modified so you don't have to reload every time
        }
    }
}