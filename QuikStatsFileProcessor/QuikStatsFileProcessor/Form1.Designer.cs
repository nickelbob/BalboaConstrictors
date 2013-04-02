namespace QuikStatsFileProcessor
{
    partial class Form1
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
            this.buttonLoadOuterField = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.buttonLoadInnerField = new System.Windows.Forms.Button();
            this.buttonLoadGPS = new System.Windows.Forms.Button();
            this.buttonLoadQuikStats = new System.Windows.Forms.Button();
            this.buttonGPSToArrayVar = new System.Windows.Forms.Button();
            this.buttonFlipGPS = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonLoadOuterField
            // 
            this.buttonLoadOuterField.Location = new System.Drawing.Point(81, 79);
            this.buttonLoadOuterField.Name = "buttonLoadOuterField";
            this.buttonLoadOuterField.Size = new System.Drawing.Size(105, 23);
            this.buttonLoadOuterField.TabIndex = 0;
            this.buttonLoadOuterField.Text = "Load Outer Field";
            this.buttonLoadOuterField.UseVisualStyleBackColor = true;
            this.buttonLoadOuterField.Click += new System.EventHandler(this.buttonLoadOuterField_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // buttonLoadInnerField
            // 
            this.buttonLoadInnerField.Location = new System.Drawing.Point(81, 109);
            this.buttonLoadInnerField.Name = "buttonLoadInnerField";
            this.buttonLoadInnerField.Size = new System.Drawing.Size(105, 23);
            this.buttonLoadInnerField.TabIndex = 1;
            this.buttonLoadInnerField.Text = "Load Inner Field";
            this.buttonLoadInnerField.UseVisualStyleBackColor = true;
            this.buttonLoadInnerField.Click += new System.EventHandler(this.buttonLoadInnerField_Click);
            // 
            // buttonLoadGPS
            // 
            this.buttonLoadGPS.Location = new System.Drawing.Point(81, 139);
            this.buttonLoadGPS.Name = "buttonLoadGPS";
            this.buttonLoadGPS.Size = new System.Drawing.Size(105, 23);
            this.buttonLoadGPS.TabIndex = 2;
            this.buttonLoadGPS.Text = "Load GPS";
            this.buttonLoadGPS.UseVisualStyleBackColor = true;
            this.buttonLoadGPS.Click += new System.EventHandler(this.buttonLoadGPS_Click);
            // 
            // buttonLoadQuikStats
            // 
            this.buttonLoadQuikStats.Location = new System.Drawing.Point(81, 169);
            this.buttonLoadQuikStats.Name = "buttonLoadQuikStats";
            this.buttonLoadQuikStats.Size = new System.Drawing.Size(105, 23);
            this.buttonLoadQuikStats.TabIndex = 3;
            this.buttonLoadQuikStats.Text = "Load Quik Stats";
            this.buttonLoadQuikStats.UseVisualStyleBackColor = true;
            this.buttonLoadQuikStats.Click += new System.EventHandler(this.buttonLoadQuikStats_Click);
            // 
            // buttonGPSToArrayVar
            // 
            this.buttonGPSToArrayVar.Location = new System.Drawing.Point(193, 139);
            this.buttonGPSToArrayVar.Name = "buttonGPSToArrayVar";
            this.buttonGPSToArrayVar.Size = new System.Drawing.Size(93, 23);
            this.buttonGPSToArrayVar.TabIndex = 4;
            this.buttonGPSToArrayVar.Text = "GPS to ArrayVar";
            this.buttonGPSToArrayVar.UseVisualStyleBackColor = true;
            this.buttonGPSToArrayVar.Click += new System.EventHandler(this.buttonGPSToArrayVar_Click);
            // 
            // buttonFlipGPS
            // 
            this.buttonFlipGPS.Location = new System.Drawing.Point(293, 138);
            this.buttonFlipGPS.Name = "buttonFlipGPS";
            this.buttonFlipGPS.Size = new System.Drawing.Size(75, 23);
            this.buttonFlipGPS.TabIndex = 5;
            this.buttonFlipGPS.Text = "Flip GPS";
            this.buttonFlipGPS.UseVisualStyleBackColor = true;
            this.buttonFlipGPS.Click += new System.EventHandler(this.buttonFlipGPS_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(433, 366);
            this.Controls.Add(this.buttonFlipGPS);
            this.Controls.Add(this.buttonGPSToArrayVar);
            this.Controls.Add(this.buttonLoadQuikStats);
            this.Controls.Add(this.buttonLoadGPS);
            this.Controls.Add(this.buttonLoadInnerField);
            this.Controls.Add(this.buttonLoadOuterField);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonLoadOuterField;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button buttonLoadInnerField;
        private System.Windows.Forms.Button buttonLoadGPS;
        private System.Windows.Forms.Button buttonLoadQuikStats;
        private System.Windows.Forms.Button buttonGPSToArrayVar;
        private System.Windows.Forms.Button buttonFlipGPS;
    }
}

