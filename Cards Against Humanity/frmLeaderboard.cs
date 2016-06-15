using System;
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
    public partial class frmLeaderboard : Form
    {
        public frmLeaderboard()
        {
            InitializeComponent();
            FormClosing += FrmLeaderboard_FormClosing;
        }

        private void FrmLeaderboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.ApplicationExitCall)
            {
                Hide();
                e.Cancel = true;
            }
        }

        public void update()
        {
            int i = 1;
            lstPosition.Items.Clear();
            lstName.Items.Clear();
            lstPts.Items.Clear();
            foreach (Player p in Program.CurrentGame.Players.ToList().OrderByDescending(p => p.Score))
            {
                lstPosition.Items.Add(i);
                lstName.Items.Add(p.Name);
                lstPts.Items.Add(p.Score);
                i++;
            }
        }

        private void lstPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstName.SelectedIndex = lstPosition.SelectedIndex;
            lstPts.SelectedIndex = lstPosition.SelectedIndex;
        }

        private void lstName_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstPosition.SelectedIndex = lstName.SelectedIndex;
            lstPts.SelectedIndex = lstName.SelectedIndex;
        }

        private void lstPts_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstPosition.SelectedIndex = lstPts.SelectedIndex;
            lstName.SelectedIndex = lstPts.SelectedIndex;
        }
    }
}
