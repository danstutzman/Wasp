using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Wasp {
    class AlarmModel {
        public event EventHandler FiringChange;

        private String id;
        public String Id { get { return this.id; } }

        private String name;
        public String Name { get { return this.name; } }

        private DateTime when;
        public DateTime When { get { return this.when; } }

        private bool isFiring;
        public bool IsFiring { get { return this.isFiring; } }

        private bool isArmed;
        public bool IsArmed { get { return this.isArmed; } }

        public AlarmModel(String id, String name, DateTime when, bool isArmed) {
            this.id = id;
            this.name = name;
            this.when = when;
            this.isArmed = isArmed;
        }

        public void Fire() {
            this.isFiring = true;
            FiringChange(this, new EventArgs());
        }



        public void TurnOff() {
            this.isFiring = false;
            FiringChange(this, new EventArgs());

            byte[] byteArray = Encoding.UTF8.GetBytes("");
            HttpWebRequest request = WebRequest.Create("http://localhost:3000/alarm/turn_off/" + this.id) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            request.BeginGetResponse(delegate(IAsyncResult result) {
                WebResponse response = request.EndGetResponse(result);
                response.Close();
            }, null);
        }
    }
}
