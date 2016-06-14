﻿namespace Cards_Against_Humanity
{
    partial class frmGameplay
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmGameplay));
            this.txtBlackCard = new System.Windows.Forms.TextBox();
            this.picBlackCard = new System.Windows.Forms.PictureBox();
            this.picWhiteCard = new System.Windows.Forms.PictureBox();
            this.txtWhiteCard = new System.Windows.Forms.TextBox();
            this.cmbWhiteCard = new System.Windows.Forms.ComboBox();
            this.picWhiteCard2 = new System.Windows.Forms.PictureBox();
            this.txtWhiteCard2 = new System.Windows.Forms.TextBox();
            this.cmbWhiteCard2 = new System.Windows.Forms.ComboBox();
            this.btnLeaderboard = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.btnRules = new System.Windows.Forms.Button();
            this.btnLicense = new System.Windows.Forms.Button();
            this.lblRound = new System.Windows.Forms.Label();
            this.txtRound = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.picBlackCard)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picWhiteCard)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picWhiteCard2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtBlackCard
            // 
            this.txtBlackCard.AccessibleName = "Black Card";
            this.txtBlackCard.BackColor = System.Drawing.Color.Black;
            this.txtBlackCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBlackCard.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtBlackCard.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBlackCard.ForeColor = System.Drawing.Color.White;
            this.txtBlackCard.Location = new System.Drawing.Point(14, 12);
            this.txtBlackCard.Multiline = true;
            this.txtBlackCard.Name = "txtBlackCard";
            this.txtBlackCard.ReadOnly = true;
            this.txtBlackCard.Size = new System.Drawing.Size(280, 336);
            this.txtBlackCard.TabIndex = 0;
            this.txtBlackCard.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // picBlackCard
            // 
            this.picBlackCard.BackColor = System.Drawing.Color.Black;
            this.picBlackCard.Image = global::Cards_Against_Humanity.Properties.Resources.Stamp_White;
            this.picBlackCard.Location = new System.Drawing.Point(15, 297);
            this.picBlackCard.Name = "picBlackCard";
            this.picBlackCard.Size = new System.Drawing.Size(278, 50);
            this.picBlackCard.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picBlackCard.TabIndex = 4;
            this.picBlackCard.TabStop = false;
            // 
            // picWhiteCard
            // 
            this.picWhiteCard.BackColor = System.Drawing.Color.White;
            this.picWhiteCard.Image = ((System.Drawing.Image)(resources.GetObject("picWhiteCard.Image")));
            this.picWhiteCard.Location = new System.Drawing.Point(322, 297);
            this.picWhiteCard.Name = "picWhiteCard";
            this.picWhiteCard.Size = new System.Drawing.Size(278, 50);
            this.picWhiteCard.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picWhiteCard.TabIndex = 9;
            this.picWhiteCard.TabStop = false;
            // 
            // txtWhiteCard
            // 
            this.txtWhiteCard.AccessibleName = "White Card";
            this.txtWhiteCard.BackColor = System.Drawing.Color.White;
            this.txtWhiteCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtWhiteCard.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtWhiteCard.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtWhiteCard.ForeColor = System.Drawing.Color.Black;
            this.txtWhiteCard.Location = new System.Drawing.Point(321, 12);
            this.txtWhiteCard.Multiline = true;
            this.txtWhiteCard.Name = "txtWhiteCard";
            this.txtWhiteCard.ReadOnly = true;
            this.txtWhiteCard.Size = new System.Drawing.Size(280, 336);
            this.txtWhiteCard.TabIndex = 2;
            this.txtWhiteCard.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // cmbWhiteCard
            // 
            this.cmbWhiteCard.AccessibleName = "Select White Card";
            this.cmbWhiteCard.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWhiteCard.FormattingEnabled = true;
            this.cmbWhiteCard.Location = new System.Drawing.Point(322, 12);
            this.cmbWhiteCard.Name = "cmbWhiteCard";
            this.cmbWhiteCard.Size = new System.Drawing.Size(298, 21);
            this.cmbWhiteCard.TabIndex = 1;
            this.cmbWhiteCard.SelectedIndexChanged += new System.EventHandler(this.cmbWhiteCard_SelectedIndexChanged);
            // 
            // picWhiteCard2
            // 
            this.picWhiteCard2.BackColor = System.Drawing.Color.White;
            this.picWhiteCard2.Image = ((System.Drawing.Image)(resources.GetObject("picWhiteCard2.Image")));
            this.picWhiteCard2.Location = new System.Drawing.Point(629, 297);
            this.picWhiteCard2.Name = "picWhiteCard2";
            this.picWhiteCard2.Size = new System.Drawing.Size(278, 50);
            this.picWhiteCard2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picWhiteCard2.TabIndex = 12;
            this.picWhiteCard2.TabStop = false;
            // 
            // txtWhiteCard2
            // 
            this.txtWhiteCard2.AccessibleName = "Second White Card";
            this.txtWhiteCard2.BackColor = System.Drawing.Color.White;
            this.txtWhiteCard2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtWhiteCard2.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtWhiteCard2.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtWhiteCard2.ForeColor = System.Drawing.Color.Black;
            this.txtWhiteCard2.Location = new System.Drawing.Point(628, 12);
            this.txtWhiteCard2.Multiline = true;
            this.txtWhiteCard2.Name = "txtWhiteCard2";
            this.txtWhiteCard2.ReadOnly = true;
            this.txtWhiteCard2.Size = new System.Drawing.Size(280, 336);
            this.txtWhiteCard2.TabIndex = 4;
            this.txtWhiteCard2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // cmbWhiteCard2
            // 
            this.cmbWhiteCard2.AccessibleName = "Select Second White Card";
            this.cmbWhiteCard2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWhiteCard2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbWhiteCard2.FormattingEnabled = true;
            this.cmbWhiteCard2.Location = new System.Drawing.Point(629, 12);
            this.cmbWhiteCard2.Name = "cmbWhiteCard2";
            this.cmbWhiteCard2.Size = new System.Drawing.Size(298, 21);
            this.cmbWhiteCard2.TabIndex = 3;
            this.cmbWhiteCard2.SelectedIndexChanged += new System.EventHandler(this.cmbWhiteCard2_SelectedIndexChanged);
            // 
            // btnLeaderboard
            // 
            this.btnLeaderboard.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLeaderboard.Location = new System.Drawing.Point(955, 12);
            this.btnLeaderboard.Name = "btnLeaderboard";
            this.btnLeaderboard.Size = new System.Drawing.Size(178, 23);
            this.btnLeaderboard.TabIndex = 6;
            this.btnLeaderboard.Text = "Show/Hide &Leaderboard";
            this.btnLeaderboard.UseVisualStyleBackColor = true;
            this.btnLeaderboard.Click += new System.EventHandler(this.btnLeaderboard_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Cards_Against_Humanity.Properties.Resources.PlaceholderCard;
            this.pictureBox1.Location = new System.Drawing.Point(628, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(280, 337);
            this.pictureBox1.TabIndex = 15;
            this.pictureBox1.TabStop = false;
            // 
            // btnSubmit
            // 
            this.btnSubmit.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSubmit.Location = new System.Drawing.Point(955, 297);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(178, 52);
            this.btnSubmit.TabIndex = 5;
            this.btnSubmit.Text = "&Submit";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // btnRules
            // 
            this.btnRules.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRules.Location = new System.Drawing.Point(955, 41);
            this.btnRules.Name = "btnRules";
            this.btnRules.Size = new System.Drawing.Size(178, 23);
            this.btnRules.TabIndex = 7;
            this.btnRules.Text = "Show &Rules";
            this.btnRules.UseVisualStyleBackColor = true;
            this.btnRules.Click += new System.EventHandler(this.btnRules_Click);
            // 
            // btnLicense
            // 
            this.btnLicense.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLicense.Location = new System.Drawing.Point(955, 70);
            this.btnLicense.Name = "btnLicense";
            this.btnLicense.Size = new System.Drawing.Size(178, 23);
            this.btnLicense.TabIndex = 8;
            this.btnLicense.Text = "Show Li&cense";
            this.btnLicense.UseVisualStyleBackColor = true;
            this.btnLicense.Click += new System.EventHandler(this.btnLicense_Click);
            // 
            // lblRound
            // 
            this.lblRound.AutoSize = true;
            this.lblRound.Location = new System.Drawing.Point(952, 96);
            this.lblRound.Name = "lblRound";
            this.lblRound.Size = new System.Drawing.Size(44, 13);
            this.lblRound.TabIndex = 9;
            this.lblRound.Text = "Round";
            // 
            // txtRound
            // 
            this.txtRound.AccessibleName = "Round";
            this.txtRound.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRound.Location = new System.Drawing.Point(955, 112);
            this.txtRound.Name = "txtRound";
            this.txtRound.ReadOnly = true;
            this.txtRound.Size = new System.Drawing.Size(178, 29);
            this.txtRound.TabIndex = 10;
            this.txtRound.Text = "x/y";
            this.txtRound.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // frmGameplay
            // 
            this.AcceptButton = this.btnSubmit;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1148, 361);
            this.Controls.Add(this.txtRound);
            this.Controls.Add(this.lblRound);
            this.Controls.Add(this.btnLicense);
            this.Controls.Add(this.btnRules);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.btnLeaderboard);
            this.Controls.Add(this.picWhiteCard2);
            this.Controls.Add(this.txtWhiteCard2);
            this.Controls.Add(this.cmbWhiteCard2);
            this.Controls.Add(this.picWhiteCard);
            this.Controls.Add(this.txtWhiteCard);
            this.Controls.Add(this.cmbWhiteCard);
            this.Controls.Add(this.picBlackCard);
            this.Controls.Add(this.txtBlackCard);
            this.Controls.Add(this.pictureBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmGameplay";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cards Against Humanity";
            this.Load += new System.EventHandler(this.frmGameplay_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picBlackCard)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picWhiteCard)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picWhiteCard2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtBlackCard;
        private System.Windows.Forms.PictureBox picBlackCard;
        private System.Windows.Forms.PictureBox picWhiteCard;
        private System.Windows.Forms.TextBox txtWhiteCard;
        private System.Windows.Forms.ComboBox cmbWhiteCard;
        private System.Windows.Forms.PictureBox picWhiteCard2;
        private System.Windows.Forms.TextBox txtWhiteCard2;
        private System.Windows.Forms.ComboBox cmbWhiteCard2;
        private System.Windows.Forms.Button btnLeaderboard;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Button btnRules;
        private System.Windows.Forms.Button btnLicense;
        private System.Windows.Forms.Label lblRound;
        private System.Windows.Forms.TextBox txtRound;
    }
}