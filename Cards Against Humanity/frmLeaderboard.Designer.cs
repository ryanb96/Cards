namespace Cards_Against_Humanity
{
    partial class frmLeaderboard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLeaderboard));
            this.lstPosition = new System.Windows.Forms.ListBox();
            this.lstName = new System.Windows.Forms.ListBox();
            this.lstPts = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // lstPosition
            // 
            this.lstPosition.FormattingEnabled = true;
            this.lstPosition.ItemHeight = 14;
            this.lstPosition.Location = new System.Drawing.Point(12, 12);
            this.lstPosition.Name = "lstPosition";
            this.lstPosition.Size = new System.Drawing.Size(27, 298);
            this.lstPosition.TabIndex = 0;
            this.lstPosition.SelectedIndexChanged += new System.EventHandler(this.lstPosition_SelectedIndexChanged);
            // 
            // lstName
            // 
            this.lstName.FormattingEnabled = true;
            this.lstName.ItemHeight = 14;
            this.lstName.Location = new System.Drawing.Point(45, 12);
            this.lstName.Name = "lstName";
            this.lstName.Size = new System.Drawing.Size(144, 298);
            this.lstName.TabIndex = 1;
            this.lstName.SelectedIndexChanged += new System.EventHandler(this.lstName_SelectedIndexChanged);
            // 
            // lstPts
            // 
            this.lstPts.FormattingEnabled = true;
            this.lstPts.ItemHeight = 14;
            this.lstPts.Location = new System.Drawing.Point(195, 12);
            this.lstPts.Name = "lstPts";
            this.lstPts.Size = new System.Drawing.Size(27, 298);
            this.lstPts.TabIndex = 2;
            this.lstPts.SelectedIndexChanged += new System.EventHandler(this.lstPts_SelectedIndexChanged);
            // 
            // frmLeaderboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(234, 316);
            this.Controls.Add(this.lstPts);
            this.Controls.Add(this.lstName);
            this.Controls.Add(this.lstPosition);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmLeaderboard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Leaderboard";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lstPosition;
        private System.Windows.Forms.ListBox lstName;
        private System.Windows.Forms.ListBox lstPts;



    }
}