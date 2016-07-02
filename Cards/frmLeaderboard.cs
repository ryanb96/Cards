using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace JoePitt.Cards
{
    /// <summary>
    /// Leaderboard UI.
    /// </summary>
    public partial class frmLeaderboard : Form
    {
        /// <summary>
        /// Initalises the Leaderboard UI.
        /// </summary>
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

        /// <summary>
        /// Update the Leaderboard UI.
        /// </summary>
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
