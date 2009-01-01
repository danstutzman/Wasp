using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Wasp {
    class TopController : IDisposable {
        private TopModel model;
        private TopView form;
        private AppBar appBar;
        public event EventHandler FormDestroyed;
        private List<AlarmController> alarmControllers;

        public TopController(TopModel model) {
            this.model = model;

            this.form = new TopView();
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

            this.form.timeLabel.Text = this.model.Time;

            this.appBar = new AppBar(this.form, !this.model.Pinned);
            this.model.PinnedChange += OnModelPinnedChange;
            this.model.ClockTick += OnClockTick;
            this.model.ScheduleChange += OnScheduleChange;
        }

        public void Show() {
            this.appBar.Show();
        }

        private void OnModelPinnedChange(Object sender, EventArgs e) {
            this.form.BeginInvoke(new MethodInvoker(delegate() {
                this.appBar.Pinned = this.model.Pinned;
            }));
        }

        private void OnClockTick(Object sender, EventArgs e) {
            this.form.BeginInvoke(new MethodInvoker(delegate() {
                this.form.timeLabel.Text = this.model.Time;
            }));
        }

        private void OnScheduleChange(Object sender, EventArgs e) {
            this.form.BeginInvoke(new MethodInvoker(delegate() {
                if (this.alarmControllers != null)
                    foreach (AlarmController alarmController in this.alarmControllers)
                        alarmController.Dispose();

                this.alarmControllers = new List<AlarmController>();
                List<AlarmModel> todaysAlarms = new List<AlarmModel>();
                todaysAlarms.AddRange(this.model.Alarms.Where(delegate(AlarmModel alarm) {
                    // Can't arm an alarm if we have nothing listening to it
                    return true; // alarm.when.Date == DateTime.Now.Date;
                }));
                for (int i = 0; i < todaysAlarms.Count; i++) {
                    AlarmModel alarm = todaysAlarms[i];
                    AlarmView control = new AlarmView();
                    AlarmController controller = new AlarmController(alarm, control);
                    this.form.tableLayoutPanel.Controls.Add(control);
                    this.alarmControllers.Add(controller);

                    alarm.FiringChange += this.AlarmFiringChange;
                }

                this.model.ArmAlarms();
            }));
        }

        private void AlarmFiringChange(Object sender, EventArgs e) {
            bool noAlarmsFiring = true;
            foreach (AlarmModel alarm in this.model.Alarms)
                if (alarm.IsFiring)
                    noAlarmsFiring = false;

            this.form.BeginInvoke(new MethodInvoker(delegate() {
                this.appBar.KeepOpen = !noAlarmsFiring;
            }));
        }

        /// Releases the screen area held by the appBar back to the OS
        public void Dispose() {
            if (this.alarmControllers != null)
                foreach (AlarmController alarmController in this.alarmControllers)
                    alarmController.Dispose();
            this.appBar.Pinned = false;
            this.appBar.Hide();
        }
    }
}