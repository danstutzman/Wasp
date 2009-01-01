﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wasp {
    class Alarm {
        public event EventHandler FiringChange;

        public String name;
        public DateTime when;

        private bool isFiring;
        public bool IsFiring { get { return this.isFiring; } }

        public Alarm() {
        }

        public void Fire() {
            this.isFiring = true;
            FiringChange(this, new EventArgs());
        }

        public void TurnOff() {
            this.isFiring = false;
            FiringChange(this, new EventArgs());
        }
    }
}
