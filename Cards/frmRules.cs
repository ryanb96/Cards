﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JoePitt.Cards
{
    public partial class frmRules : Form
    {
        public frmRules()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void frmRules_Load(object sender, EventArgs e)
        {
            txtRules.Select(0, 0);
            txtRules.AccessibleName = txtRules.Text;
        }
    }
}