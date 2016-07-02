using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace JoePitt.Cards
{
    /// <summary>
    /// Main Gameplay UI Control.
    /// </summary>
    public partial class frmGameplay : Form
    {
        private Card BlackCard;
        private bool Submitted = false;

        /// <summary>
        /// Initalises the main gameplay UI.
        /// </summary>
        public frmGameplay()
        {
            InitializeComponent();
            FormClosing += FrmGameplay_FormClosing;
        }

        /// <summary>
        /// Prevents accidental closing, ends game if closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmGameplay_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && !Submitted)
            {
                if (MessageBox.Show("End game now?", "End Game", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    Program.CurrentGame.Stage = 'E';
                    Program.CurrentGame.Playable = false;
                    DialogResult = DialogResult.Abort;
                }
            }
        }

        private void frmGameplay_Load(object sender, EventArgs e)
        {
            Text = Program.CurrentPlayer.Owner.Name + " - Cards";
            Program.CurrentPlayer.NextCommand = "GAMEUPDATE";
            Program.CurrentPlayer.NewCommand = true;
            while (!Program.CurrentPlayer.NewResponse)
            {
                Application.DoEvents();
            }
            string[] response = Program.CurrentPlayer.LastResponse.Split(' ');
            Program.CurrentPlayer.NewResponse = false;
            if (response[0] == "PLAYING")
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(response[1])))
                {
                    BlackCard = (Card)formatter.Deserialize(stream);
                }
                txtBlackCard.Text = BlackCard.Text;

                Program.CurrentPlayer.NextCommand = "MYCARDS";
                Program.CurrentPlayer.NewCommand = true;
                while (!Program.CurrentPlayer.NewResponse)
                {
                    Application.DoEvents();
                }
                string cards = Program.CurrentPlayer.LastResponse;
                Program.CurrentPlayer.NewResponse = false;
                try
                {
                    using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(cards)))
                    {
                        Program.CurrentPlayer.Owner.WhiteCards = (List<Card>)formatter.Deserialize(stream);
                    }
                }
                catch
                {
                    MessageBox.Show("Unexpected Error! Unable to get White Cards. Application will exit!", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Application.Exit();
                }
                
                foreach (Card playerCard in Program.CurrentPlayer.Owner.WhiteCards)
                {
                    cmbWhiteCard.Items.Add(playerCard.Text);
                    cmbWhiteCard2.Items.Add(playerCard.Text);
                }
                if (BlackCard.Needs == 1)
                {
                    txtWhiteCard2.Visible = false;
                    cmbWhiteCard2.Visible = false;
                    picWhiteCard2.Visible = false;
                }

                cmbWhiteCard.SelectedIndex = 0;
                cmbWhiteCard2.SelectedIndex = 1;

                txtRound.Text = Program.CurrentGame.Round + " of " + Program.CurrentGame.Rounds;
            }
            else
            {
                Submitted = true;
                Close();
            }
        }

        /// <summary>
        /// Keeps the First White card up to date with the selected card.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbWhiteCard_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtWhiteCard.Text = cmbWhiteCard.SelectedItem.ToString();
            if (cmbWhiteCard.SelectedIndex == cmbWhiteCard2.SelectedIndex)
            {
                if (cmbWhiteCard2.SelectedIndex == cmbWhiteCard2.Items.Count -1)
                {
                    cmbWhiteCard2.SelectedIndex = 0;
                }
                else
                {
                    cmbWhiteCard2.SelectedIndex++;
                }
            }
        }

        /// <summary>
        /// Keeps the Second White Card up to date with the selected card
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbWhiteCard2_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtWhiteCard2.Text = cmbWhiteCard2.SelectedItem.ToString();
            if (cmbWhiteCard.SelectedIndex == cmbWhiteCard2.SelectedIndex)
            {
                if (cmbWhiteCard.SelectedIndex == cmbWhiteCard.Items.Count - 1)
                {
                    cmbWhiteCard.SelectedIndex = 0;
                }
                else
                {
                    cmbWhiteCard.SelectedIndex++;
                }
            }
        }

        private void btnLeaderboard_Click(object sender, EventArgs e)
        {
            if (Program.LeaderBoard.Visible)
            {
                Program.LeaderBoard.Hide();
                Program.LeaderBoard.update();
            }
            else
            {
                Program.LeaderBoard.Show();
                Program.LeaderBoard.update();
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            Answer myAnswer;
            if (BlackCard.Needs == 1)
            {
                myAnswer = new Answer(Program.CurrentPlayer.Owner, BlackCard, Program.CurrentPlayer.Owner.WhiteCards[cmbWhiteCard.SelectedIndex]);
            }
            else
            {
                myAnswer = new Answer(Program.CurrentPlayer.Owner, BlackCard, Program.CurrentPlayer.Owner.WhiteCards[cmbWhiteCard.SelectedIndex], Program.CurrentPlayer.Owner.WhiteCards[cmbWhiteCard2.SelectedIndex]);
            }
            if (MessageBox.Show("Submit: " + myAnswer.Text, "Confirm Answer", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //submit 
                BinaryFormatter formatter = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream())
                {
                    formatter.Serialize(stream, myAnswer);
                    byte[] myAnswerBytes = stream.ToArray();
                    Program.CurrentPlayer.NextCommand = "SUBMIT " + Convert.ToBase64String(myAnswerBytes);
                }
                Program.CurrentPlayer.NewCommand = true;
                while (!Program.CurrentPlayer.NewResponse)
                {
                    Application.DoEvents();
                }
                string response = Program.CurrentPlayer.LastResponse;
                Program.CurrentPlayer.NewResponse = false;
                if (response == "SUBMITTED")
                {
                    Submitted = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Unexpected Error! Unable to submit answer. Application will exit!", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Application.Exit();
                }
            }
        }

        private void btnRules_Click(object sender, EventArgs e)
        {
            frmRules rules = new frmRules();
            rules.ShowDialog();
            rules.Dispose();
        }

        private void btnLicense_Click(object sender, EventArgs e)
        {
            frmLicense license = new frmLicense();
            license.ShowDialog();
            license.Dispose();
        }
    }
}
