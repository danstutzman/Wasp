using System;

namespace Wasp {
    class TopModel {
        public event EventHandler PinnedChange;

        public TopModel() {
        }

        private bool pinned;
        public bool Pinned {
            get { return this.pinned; }
            set {
                this.pinned = value;
                this.PinnedChange(this, new EventArgs());
            }
        }
    }
}