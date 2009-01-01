namespace Wasp {
    partial class AlarmView {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.timeTextBox = new System.Windows.Forms.MaskedTextBox();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.offButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // timeTextBox
            // 
            this.timeTextBox.Font = new System.Drawing.Font("Arial Narrow", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeTextBox.Location = new System.Drawing.Point(0, 0);
            this.timeTextBox.Name = "timeTextBox";
            this.timeTextBox.ReadOnly = true;
            this.timeTextBox.Size = new System.Drawing.Size(61, 38);
            this.timeTextBox.TabIndex = 0;
            this.timeTextBox.Text = "12:88";
            // 
            // nameTextBox
            // 
            this.nameTextBox.Font = new System.Drawing.Font("Arial Narrow", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nameTextBox.Location = new System.Drawing.Point(67, 0);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.ReadOnly = true;
            this.nameTextBox.Size = new System.Drawing.Size(72, 38);
            this.nameTextBox.TabIndex = 1;
            this.nameTextBox.Text = "desc";
            // 
            // offButton
            // 
            this.offButton.Location = new System.Drawing.Point(141, 4);
            this.offButton.Name = "offButton";
            this.offButton.Size = new System.Drawing.Size(44, 32);
            this.offButton.TabIndex = 2;
            this.offButton.Text = "OFF";
            this.offButton.UseVisualStyleBackColor = true;
            // 
            // AlarmControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.offButton);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.timeTextBox);
            this.Name = "AlarmControl";
            this.Size = new System.Drawing.Size(189, 37);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.MaskedTextBox timeTextBox;
        public System.Windows.Forms.TextBox nameTextBox;
        public System.Windows.Forms.Button offButton;

    }
}
