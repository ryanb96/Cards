namespace Card_Set_Builder
{
    partial class frmCAHCSB
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCAHCSB));
            this.lblProduct = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblGUID = new System.Windows.Forms.Label();
            this.txtGUID = new System.Windows.Forms.TextBox();
            this.btnNewGUID = new System.Windows.Forms.Button();
            this.lblVersion = new System.Windows.Forms.Label();
            this.txtVersionMajor = new System.Windows.Forms.NumericUpDown();
            this.txtVersionMinor = new System.Windows.Forms.NumericUpDown();
            this.grpBlack = new System.Windows.Forms.GroupBox();
            this.txtBlackCards2 = new System.Windows.Forms.TextBox();
            this.lblBlackCards2 = new System.Windows.Forms.Label();
            this.txtBlackCards1 = new System.Windows.Forms.TextBox();
            this.lblBlackCards1 = new System.Windows.Forms.Label();
            this.grpWhiteCards = new System.Windows.Forms.GroupBox();
            this.txtWhiteCards = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnQuit = new System.Windows.Forms.Button();
            this.picLogo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.txtVersionMajor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtVersionMinor)).BeginInit();
            this.grpBlack.SuspendLayout();
            this.grpWhiteCards.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // lblProduct
            // 
            this.lblProduct.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProduct.Location = new System.Drawing.Point(115, 13);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(96, 96);
            this.lblProduct.TabIndex = 1;
            this.lblProduct.Text = "Card Set Builder";
            this.lblProduct.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.Location = new System.Drawing.Point(217, 15);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(38, 14);
            this.lblName.TabIndex = 2;
            this.lblName.Text = "Name";
            // 
            // txtName
            // 
            this.txtName.AccessibleName = "Card Set Name";
            this.txtName.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.Location = new System.Drawing.Point(265, 12);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(242, 20);
            this.txtName.TabIndex = 3;
            // 
            // lblGUID
            // 
            this.lblGUID.AutoSize = true;
            this.lblGUID.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGUID.Location = new System.Drawing.Point(217, 41);
            this.lblGUID.Name = "lblGUID";
            this.lblGUID.Size = new System.Drawing.Size(32, 14);
            this.lblGUID.TabIndex = 4;
            this.lblGUID.Text = "GUID";
            // 
            // txtGUID
            // 
            this.txtGUID.AccessibleName = "Globally Unique Identifier (GUID)";
            this.txtGUID.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtGUID.Location = new System.Drawing.Point(265, 38);
            this.txtGUID.Name = "txtGUID";
            this.txtGUID.ReadOnly = true;
            this.txtGUID.Size = new System.Drawing.Size(242, 20);
            this.txtGUID.TabIndex = 5;
            this.txtGUID.Text = "00000000-0000-0000-0000-000000000000";
            // 
            // btnNewGUID
            // 
            this.btnNewGUID.AccessibleName = "Generate new GUID";
            this.btnNewGUID.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNewGUID.Location = new System.Drawing.Point(513, 37);
            this.btnNewGUID.Name = "btnNewGUID";
            this.btnNewGUID.Size = new System.Drawing.Size(75, 23);
            this.btnNewGUID.TabIndex = 6;
            this.btnNewGUID.Text = "&Generate";
            this.btnNewGUID.UseVisualStyleBackColor = true;
            this.btnNewGUID.Click += new System.EventHandler(this.btnNewGUID_Click);
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.Location = new System.Drawing.Point(217, 66);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(50, 14);
            this.lblVersion.TabIndex = 7;
            this.lblVersion.Text = "Version";
            // 
            // txtVersionMajor
            // 
            this.txtVersionMajor.AccessibleName = "Major Version";
            this.txtVersionMajor.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtVersionMajor.Location = new System.Drawing.Point(265, 64);
            this.txtVersionMajor.Name = "txtVersionMajor";
            this.txtVersionMajor.Size = new System.Drawing.Size(105, 20);
            this.txtVersionMajor.TabIndex = 8;
            // 
            // txtVersionMinor
            // 
            this.txtVersionMinor.AccessibleName = "Minor Version";
            this.txtVersionMinor.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtVersionMinor.Location = new System.Drawing.Point(376, 64);
            this.txtVersionMinor.Name = "txtVersionMinor";
            this.txtVersionMinor.Size = new System.Drawing.Size(104, 20);
            this.txtVersionMinor.TabIndex = 9;
            this.txtVersionMinor.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // grpBlack
            // 
            this.grpBlack.BackColor = System.Drawing.Color.Black;
            this.grpBlack.Controls.Add(this.txtBlackCards2);
            this.grpBlack.Controls.Add(this.lblBlackCards2);
            this.grpBlack.Controls.Add(this.txtBlackCards1);
            this.grpBlack.Controls.Add(this.lblBlackCards1);
            this.grpBlack.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpBlack.ForeColor = System.Drawing.Color.White;
            this.grpBlack.Location = new System.Drawing.Point(13, 115);
            this.grpBlack.Name = "grpBlack";
            this.grpBlack.Size = new System.Drawing.Size(285, 244);
            this.grpBlack.TabIndex = 10;
            this.grpBlack.TabStop = false;
            this.grpBlack.Text = "Black Cards";
            // 
            // txtBlackCards2
            // 
            this.txtBlackCards2.AccessibleName = "Black Cards which need 2 White Cards (1 per line)";
            this.txtBlackCards2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBlackCards2.Location = new System.Drawing.Point(6, 146);
            this.txtBlackCards2.Multiline = true;
            this.txtBlackCards2.Name = "txtBlackCards2";
            this.txtBlackCards2.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtBlackCards2.Size = new System.Drawing.Size(273, 92);
            this.txtBlackCards2.TabIndex = 3;
            this.txtBlackCards2.WordWrap = false;
            // 
            // lblBlackCards2
            // 
            this.lblBlackCards2.AutoSize = true;
            this.lblBlackCards2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBlackCards2.Location = new System.Drawing.Point(6, 130);
            this.lblBlackCards2.Name = "lblBlackCards2";
            this.lblBlackCards2.Size = new System.Drawing.Size(121, 14);
            this.lblBlackCards2.TabIndex = 2;
            this.lblBlackCards2.Text = "Needs 2 White Cards";
            // 
            // txtBlackCards1
            // 
            this.txtBlackCards1.AccessibleName = "Black Cards which need 1 White Card (1 per line)";
            this.txtBlackCards1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBlackCards1.Location = new System.Drawing.Point(6, 36);
            this.txtBlackCards1.Multiline = true;
            this.txtBlackCards1.Name = "txtBlackCards1";
            this.txtBlackCards1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtBlackCards1.Size = new System.Drawing.Size(273, 91);
            this.txtBlackCards1.TabIndex = 1;
            this.txtBlackCards1.WordWrap = false;
            // 
            // lblBlackCards1
            // 
            this.lblBlackCards1.AutoSize = true;
            this.lblBlackCards1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBlackCards1.Location = new System.Drawing.Point(7, 20);
            this.lblBlackCards1.Name = "lblBlackCards1";
            this.lblBlackCards1.Size = new System.Drawing.Size(114, 14);
            this.lblBlackCards1.TabIndex = 0;
            this.lblBlackCards1.Text = "Needs 1 White Card";
            // 
            // grpWhiteCards
            // 
            this.grpWhiteCards.BackColor = System.Drawing.Color.White;
            this.grpWhiteCards.Controls.Add(this.txtWhiteCards);
            this.grpWhiteCards.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpWhiteCards.Location = new System.Drawing.Point(304, 115);
            this.grpWhiteCards.Name = "grpWhiteCards";
            this.grpWhiteCards.Size = new System.Drawing.Size(284, 244);
            this.grpWhiteCards.TabIndex = 11;
            this.grpWhiteCards.TabStop = false;
            this.grpWhiteCards.Text = "White Cards";
            // 
            // txtWhiteCards
            // 
            this.txtWhiteCards.AccessibleName = "White Cards";
            this.txtWhiteCards.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtWhiteCards.Location = new System.Drawing.Point(7, 20);
            this.txtWhiteCards.Multiline = true;
            this.txtWhiteCards.Name = "txtWhiteCards";
            this.txtWhiteCards.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtWhiteCards.Size = new System.Drawing.Size(271, 218);
            this.txtWhiteCards.TabIndex = 0;
            this.txtWhiteCards.WordWrap = false;
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(12, 365);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 12;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoad.Location = new System.Drawing.Point(93, 365);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 13;
            this.btnLoad.Text = "&Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnReset
            // 
            this.btnReset.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReset.Location = new System.Drawing.Point(174, 365);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 14;
            this.btnReset.Text = "&Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnQuit
            // 
            this.btnQuit.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnQuit.Location = new System.Drawing.Point(513, 365);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(75, 23);
            this.btnQuit.TabIndex = 15;
            this.btnQuit.Text = "&Quit";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // picLogo
            // 
            this.picLogo.Image = global::Card_Set_Builder.Properties.Resources.Logo_v2;
            this.picLogo.Location = new System.Drawing.Point(13, 13);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(96, 96);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogo.TabIndex = 0;
            this.picLogo.TabStop = false;
            // 
            // frmCAHCSB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 400);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.grpWhiteCards);
            this.Controls.Add(this.grpBlack);
            this.Controls.Add(this.txtVersionMinor);
            this.Controls.Add(this.txtVersionMajor);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.btnNewGUID);
            this.Controls.Add(this.txtGUID);
            this.Controls.Add(this.lblGUID);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.lblProduct);
            this.Controls.Add(this.picLogo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCAHCSB";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Card Set Builder";
            this.Load += new System.EventHandler(this.frmCAHCSB_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtVersionMajor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtVersionMinor)).EndInit();
            this.grpBlack.ResumeLayout(false);
            this.grpBlack.PerformLayout();
            this.grpWhiteCards.ResumeLayout(false);
            this.grpWhiteCards.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.Label lblProduct;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblGUID;
        private System.Windows.Forms.TextBox txtGUID;
        private System.Windows.Forms.Button btnNewGUID;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.NumericUpDown txtVersionMajor;
        private System.Windows.Forms.NumericUpDown txtVersionMinor;
        private System.Windows.Forms.GroupBox grpBlack;
        private System.Windows.Forms.TextBox txtBlackCards2;
        private System.Windows.Forms.Label lblBlackCards2;
        private System.Windows.Forms.TextBox txtBlackCards1;
        private System.Windows.Forms.Label lblBlackCards1;
        private System.Windows.Forms.GroupBox grpWhiteCards;
        private System.Windows.Forms.TextBox txtWhiteCards;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnQuit;
    }
}

