using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Wasp {
    class AlarmController : IDisposable {
        private AlarmModel model;
        private AlarmView view;
        private Timer flashTimer;

        public AlarmController(AlarmModel model, AlarmView view) {
            this.model = model;
            this.view = view;

            this.view.timeTextBox.Text = this.model.when.ToString("HH:mm");
            this.view.nameTextBox.Text = this.model.name;

            this.view.offButton.Click += OnClickOffButton;

            this.model.FiringChange += OnFiringChange;

            this.flashTimer = new Timer();
            this.flashTimer.Interval = 200;
            this.flashTimer.Tick += ToggleBackground;
        }

        private void OnFiringChange(Object sender, EventArgs e) {
            this.view.BeginInvoke(new MethodInvoker(delegate() {
                if (model.IsFiring) {
                    this.flashTimer.Start();
                }
                else {
                    this.flashTimer.Stop();
                    view.BackColor = System.Drawing.SystemColors.Control;
                    view.timeTextBox.BackColor = System.Drawing.SystemColors.Control;
                    view.nameTextBox.BackColor = System.Drawing.SystemColors.Control;
                }
            }));
        }

        private void ToggleBackground(Object sender, EventArgs e) {
            this.view.BeginInvoke(new MethodInvoker(delegate() {
                if (model.IsFiring) {
                    if (view.BackColor != System.Drawing.Color.Red) {
                        view.BackColor = System.Drawing.Color.Red;
                        view.timeTextBox.BackColor = System.Drawing.Color.Red;
                        view.nameTextBox.BackColor = System.Drawing.Color.Red;
                    }
                    else {
                        view.BackColor = System.Drawing.SystemColors.Control;
                        view.timeTextBox.BackColor = System.Drawing.SystemColors.Control;
                        view.nameTextBox.BackColor = System.Drawing.SystemColors.Control;
                    }
                }
            }));
        }

        private void OnClickOffButton(Object sender, EventArgs e) {
            this.model.TurnOff();
        }

        public void Dispose() {
        }
    }
}
