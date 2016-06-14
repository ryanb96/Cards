namespace Cards_Against_Humanity
{
    partial class frmRules
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRules));
            this.btnClose = new System.Windows.Forms.Button();
            this.txtRules = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(12, 501);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(526, 25);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // txtRules
            // 
            this.txtRules.BackColor = System.Drawing.SystemColors.Control;
            this.txtRules.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtRules.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtRules.ForeColor = System.Drawing.SystemColors.ControlText;
            this.txtRules.Location = new System.Drawing.Point(15, 10);
            this.txtRules.Multiline = true;
            this.txtRules.Name = "txtRules";
            this.txtRules.ReadOnly = true;
            this.txtRules.Size = new System.Drawing.Size(523, 485);
            this.txtRules.TabIndex = 0;
            this.txtRules.Text = resources.GetString("txtRules.Text");
            // 
            // frmRules
            // 
            this.AcceptButton = this.btnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(550, 538);
            this.Controls.Add(this.txtRules);
            this.Controls.Add(this.btnClose);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmRules";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Rules";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmRules_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox txtRules;
    }
}