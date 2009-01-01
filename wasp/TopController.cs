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
        private TopForm form;
        private AppBar appBar;
        public event EventHandler FormDestroyed;
        private Timer flashTimer;

        public TopController(TopModel model) {
            this.model = model;

            this.form = new TopForm();
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

            /*this.form.snoozeButton.Click += delegate(Object sender, EventArgs e) {
                this.model.SnoozeAlarm();
            };
            this.form.snoozeButton.Enabled = this.model.Alarmed;*/

            this.form.timeLabel.Text = this.model.Time;

            this.appBar = new AppBar(this.form, !this.model.Pinned);
            this.model.PinnedChange += OnModelPinnedChange;
            this.model.AlarmChange += OnModelAlarmChange;
            this.model.ClockTick += OnClockTick;
            this.model.ScheduleChange += OnScheduleChange;

            this.flashTimer = new Timer();
            this.flashTimer.Tick += FlashBackground;
        }

        public void Show() {
            this.appBar.Show();
        }

        private void OnModelPinnedChange(Object sender, EventArgs e) {
            this.form.BeginInvoke(new MethodInvoker(delegate() {
                this.appBar.Pinned = this.model.Pinned;
            }));
        }

        private void OnModelAlarmChange(Object sender, AlarmEventArgs e) {
            this.form.BeginInvoke(new MethodInvoker(delegate() {
                List<Alarm> todaysAlarms = new List<Alarm>();
                todaysAlarms.AddRange(this.model.Alarms.Where(delegate(Alarm alarm) { return alarm.when.Date == DateTime.Now.Date; }));
                int flashingAlarms = 0;
                for (int i = 0; i < todaysAlarms.Count; i++) {
                    Alarm alarm = todaysAlarms[i];
                    AlarmControl control = this.form.tableLayoutPanel.Controls[i] as AlarmControl;
                    if (alarm.IsGoingOff)
                        flashingAlarms += 1;
                    else {
                        control.BackColor = System.Drawing.SystemColors.Control;
                        control.timeTextBox.BackColor = System.Drawing.SystemColors.Control;
                        control.nameTextBox.BackColor = System.Drawing.SystemColors.Control;
                    }
                }

                if (flashingAlarms > 0) {
                    this.flashTimer.Start();
                    this.appBar.KeepOpen = true;
                }
                else {
                    this.flashTimer.Stop();
                    this.appBar.KeepOpen = false;
                }
            }));
        }
        
        private void OnClockTick(Object sender, EventArgs e) {
            this.form.BeginInvoke(new MethodInvoker(delegate() {
                this.form.timeLabel.Text = this.model.Time;
            }));
        }

        private void FlashBackground(Object sender, EventArgs e) {
            List<Alarm> todaysAlarms = new List<Alarm>();
            todaysAlarms.AddRange(this.model.Alarms.Where(delegate(Alarm alarm) { return alarm.when.Date == DateTime.Now.Date; }));
            for (int i = 0; i < todaysAlarms.Count; i++) {
                Alarm alarm = todaysAlarms[i];
                AlarmControl control = this.form.tableLayoutPanel.Controls[i] as AlarmControl;
                if (alarm.IsGoingOff) {
                    if (control.BackColor != System.Drawing.Color.Red) {
                        control.BackColor = System.Drawing.Color.Red;
                        control.timeTextBox.BackColor = System.Drawing.Color.Red;
                        control.nameTextBox.BackColor = System.Drawing.Color.Red;
                    }
                    else {
                        control.BackColor = System.Drawing.SystemColors.Control;
                        control.timeTextBox.BackColor = System.Drawing.SystemColors.Control;
                        control.nameTextBox.BackColor = System.Drawing.SystemColors.Control;
                    }
                }
            }
        }

        private void OnScheduleChange(Object sender, EventArgs e) {
            this.form.BeginInvoke(new MethodInvoker(delegate() {
                List<Alarm> todaysAlarms = new List<Alarm>();
                todaysAlarms.AddRange(this.model.Alarms.Where(delegate(Alarm alarm) { return alarm.when.Date == DateTime.Now.Date; }));
                for (int i = 0; i < todaysAlarms.Count; i++) {
                    Alarm alarm = todaysAlarms[i];
                    AlarmControl control;
                    if (i >= this.form.tableLayoutPanel.Controls.Count) {
                        control = new AlarmControl();
                        this.form.tableLayoutPanel.Controls.Add(control);
                    }
                    else
                        control = this.form.tableLayoutPanel.Controls[i] as AlarmControl;
                    control.timeTextBox.Text = alarm.when.ToString("HH:mm");
                    control.nameTextBox.Text = alarm.name;
                }
                for (int i = todaysAlarms.Count; i < this.form.tableLayoutPanel.Controls.Count; i++)
                    this.form.tableLayoutPanel.Controls.RemoveAt(i);
            }));
        }

        /// Releases the screen area held by the appBar back to the OS
        public void Dispose() {
            //AutoHide = false;
            this.appBar.Hide();
        }
    }
}