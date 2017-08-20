namespace PGSolutions.Monads.TrafficLightDemo {

    partial class Form1 {
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
                Source.Dispose();
              }
          base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.UpTownLeftTurnLight = new System.Windows.Forms.PictureBox();
            this.DownTownLeftTurnLight = new System.Windows.Forms.PictureBox();
            this.CrossTownLight = new System.Windows.Forms.PictureBox();
            this.UpDownTownLight = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.UpTownLeftTurnLight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DownTownLeftTurnLight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CrossTownLight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UpDownTownLight)).BeginInit();
            this.SuspendLayout();
            // 
            // UpTownLeftTurnLight
            // 
            this.UpTownLeftTurnLight.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.UpTownLeftTurnLight.Image = global::PGSolutions.Monads.TrafficLightDemo.Properties.Resources.YellowLight;
            this.UpTownLeftTurnLight.InitialImage = null;
            this.UpTownLeftTurnLight.Location = new System.Drawing.Point(0, 25);
            this.UpTownLeftTurnLight.Name = "UpTownLeftTurnLight";
            this.UpTownLeftTurnLight.Size = new System.Drawing.Size(187, 395);
            this.UpTownLeftTurnLight.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.UpTownLeftTurnLight.TabIndex = 0;
            this.UpTownLeftTurnLight.TabStop = false;
            this.UpTownLeftTurnLight.Click += new System.EventHandler(this.ResetLights);
            // 
            // DownTownLeftTurnLight
            // 
            this.DownTownLeftTurnLight.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.DownTownLeftTurnLight.Image = global::PGSolutions.Monads.TrafficLightDemo.Properties.Resources.GreenLight;
            this.DownTownLeftTurnLight.InitialImage = null;
            this.DownTownLeftTurnLight.Location = new System.Drawing.Point(188, 25);
            this.DownTownLeftTurnLight.Name = "DownTownLeftTurnLight";
            this.DownTownLeftTurnLight.Size = new System.Drawing.Size(187, 395);
            this.DownTownLeftTurnLight.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.DownTownLeftTurnLight.TabIndex = 1;
            this.DownTownLeftTurnLight.TabStop = false;
            this.DownTownLeftTurnLight.Click += new System.EventHandler(this.ResetLights);
            // 
            // CrossTownLight
            // 
            this.CrossTownLight.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.CrossTownLight.Image = global::PGSolutions.Monads.TrafficLightDemo.Properties.Resources.YellowLight;
            this.CrossTownLight.InitialImage = null;
            this.CrossTownLight.Location = new System.Drawing.Point(564, 25);
            this.CrossTownLight.Name = "CrossTownLight";
            this.CrossTownLight.Size = new System.Drawing.Size(187, 395);
            this.CrossTownLight.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.CrossTownLight.TabIndex = 3;
            this.CrossTownLight.TabStop = false;
            this.CrossTownLight.Click += new System.EventHandler(this.ResetLights);
            // 
            // UpDownTownLight
            // 
            this.UpDownTownLight.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.UpDownTownLight.Image = global::PGSolutions.Monads.TrafficLightDemo.Properties.Resources.RedLight;
            this.UpDownTownLight.InitialImage = null;
            this.UpDownTownLight.Location = new System.Drawing.Point(376, 25);
            this.UpDownTownLight.Name = "UpDownTownLight";
            this.UpDownTownLight.Size = new System.Drawing.Size(187, 395);
            this.UpDownTownLight.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.UpDownTownLight.TabIndex = 2;
            this.UpDownTownLight.TabStop = false;
            this.UpDownTownLight.Click += new System.EventHandler(this.ResetLights);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(18, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(158, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Up-Town Left Turn";
            this.label1.Click += new System.EventHandler(this.ResetLights);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(192, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(180, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "Down-Town Left Turn";
            this.label2.Click += new System.EventHandler(this.ResetLights);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(605, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "Cross-Town";
            this.label3.Click += new System.EventHandler(this.ResetLights);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(397, 4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(146, 20);
            this.label4.TabIndex = 6;
            this.label4.Text = "Up- / Down-Town";
            this.label4.Click += new System.EventHandler(this.ResetLights);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(751, 421);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CrossTownLight);
            this.Controls.Add(this.UpDownTownLight);
            this.Controls.Add(this.DownTownLeftTurnLight);
            this.Controls.Add(this.UpTownLeftTurnLight);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Click += new System.EventHandler(this.ResetLights);
            ((System.ComponentModel.ISupportInitialize)(this.UpTownLeftTurnLight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DownTownLeftTurnLight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CrossTownLight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UpDownTownLight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox UpTownLeftTurnLight;
        private System.Windows.Forms.PictureBox DownTownLeftTurnLight;
        private System.Windows.Forms.PictureBox CrossTownLight;
        private System.Windows.Forms.PictureBox UpDownTownLight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}

