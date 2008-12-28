namespace Wasp
{
    partial class TopForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.exitButton = new System.Windows.Forms.Button();
            this.pinCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // exitButton
            // 
            this.exitButton.Location = new System.Drawing.Point(327, 15);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(75, 23);
            this.exitButton.TabIndex = 0;
            this.exitButton.Text = "Exit";
            this.exitButton.UseVisualStyleBackColor = true;
            // 
            // pinCheckBox
            // 
            this.pinCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
            this.pinCheckBox.AutoSize = true;
            this.pinCheckBox.Location = new System.Drawing.Point(186, 12);
            this.pinCheckBox.Name = "pinCheckBox";
            this.pinCheckBox.Size = new System.Drawing.Size(32, 23);
            this.pinCheckBox.TabIndex = 1;
            this.pinCheckBox.Text = "Pin";
            this.pinCheckBox.UseVisualStyleBackColor = true;
            // 
            // TopForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 50);
            this.Controls.Add(this.pinCheckBox);
            this.Controls.Add(this.exitButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "TopForm";
            this.ShowInTaskbar = false;
            this.Text = "TopForm";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Button exitButton;
        public System.Windows.Forms.CheckBox pinCheckBox;

    }
}

