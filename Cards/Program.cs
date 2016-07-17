using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace JoePitt.Cards
{
    /// <summary>
    /// The central class of Cards.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The game that is currently being played.
        /// </summary>
        static public Game CurrentGame { get; set; }
        /// <summary>
        /// The Client Communications Handler of the current player.
        /// </summary>
        static public ClientNetworking CurrentPlayer { get; set; }
        /// <summary>
        /// The Leaderboard window.
        /// </summary>
        static public frmLeaderboard LeaderBoard { get; set; }
        /// <summary>
        /// The Session Key of this instance of Cards.
        /// </summary>
        static internal string SessionKey { get; set; }
        /// <summary>
        /// The Last round that the winners prompt was shown for.
        /// </summary>
        static private int ShownWinners { get; set; } = 0;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                if (AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData != null)
                {
                    string arg = Uri.UnescapeDataString(AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData[0].Substring(8));
                    if (File.Exists(arg))
                    {
                        if (arg.EndsWith(".cardset"))
                        {
                            Dealer.InstallCardSet(arg);
                        }
                    }
                }
            }
            catch { }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

        NewGame:
            RNGCryptoServiceProvider PassGen = new RNGCryptoServiceProvider();
            byte[] PassBytes = new byte[32];
            PassGen.GetBytes(PassBytes);
            SessionKey = Convert.ToBase64String(PassBytes);

            frmNew newGame = new frmNew();
            newGame.ShowDialog();
            newGame.Dispose();
            if (CurrentGame != null)
            {
                try
                {
                    NativeMethods hook = new NativeMethods();
                    hook.KeyboardHook();
                    hook.KeyPressed += Hook_KeyPressed;
                    hook.RegisterHotKey(ModifierKeys.Control | ModifierKeys.Shift, Keys.F1);
                }
                catch { }

                LeaderBoard = new frmLeaderboard();
                while (CurrentGame.Playable)
                {
                    LeaderBoard.update();

                    CurrentPlayer = CurrentGame.LocalPlayers[0];
                    CurrentPlayer.NextCommand = "GAMEUPDATE";
                    CurrentPlayer.NewCommand = true;
                    while (!CurrentPlayer.NewResponse)
                    {
                        Application.DoEvents();
                    }
                    string[] response = CurrentPlayer.LastResponse.Split(' ');
                    CurrentPlayer.NewResponse = false;
                    switch (response[0])
                    {
                        case "WAITING":
                            CurrentGame.Stage = 'W';
                            break;
                        case "PLAYING":
                            CurrentGame.Stage = 'P';
                            CurrentGame.Round = Convert.ToInt32(response[2]);
                            if (CurrentGame.Round > 1 && ShownWinners < (CurrentGame.Round -1))
                            {
                                CurrentPlayer = CurrentGame.LocalPlayers[0];
                                CurrentPlayer.NextCommand = "GETWINNER";
                                CurrentPlayer.NewCommand = true;
                                while (!CurrentPlayer.NewResponse)
                                {
                                    Application.DoEvents();
                                }
                                response = CurrentPlayer.LastResponse.Split(' ');
                                CurrentPlayer.NewResponse = false;
                                try
                                {
                                    BinaryFormatter formatter = new BinaryFormatter();
                                    byte[] test = Convert.FromBase64String(response[0]);
                                    using (MemoryStream stream = new MemoryStream(test))
                                    {
                                        CurrentGame.Winners = (List<Answer>)formatter.Deserialize(stream);
                                    }
                                    string message = "";
                                    if (CurrentGame.Winners.Count > 1)
                                    {
                                        message = "The winning answers are:" + Environment.NewLine;
                                        foreach (Answer winner in CurrentGame.Winners)
                                        {
                                            message = message + Environment.NewLine + winner.Text + " (by " + winner.Submitter.Name + ")";
                                        }
                                    }
                                    else
                                    {
                                        message = "The winning answer is: " + CurrentGame.Winners[0].Text + " (by " + CurrentGame.Winners[0].Submitter.Name + ")";
                                    }
                                    MessageBox.Show(message, "Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    ShownWinners = CurrentGame.Round - 1;
                                }
                                catch
                                {
                                    MessageBox.Show("Unexpected Error! Bad response to GETWINNER, Application will exit!", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    CurrentGame.Stop();
                                    Application.Exit();
                                    break;
                                }
                            }
                            break;
                        case "VOTING":
                            CurrentGame.Stage = 'V';
                            break;
                        case "END":
                            CurrentPlayer = CurrentGame.LocalPlayers[0];
                            CurrentPlayer.NextCommand = "GETWINNER";
                            CurrentPlayer.NewCommand = true;
                            while (!CurrentPlayer.NewResponse)
                            {
                                Application.DoEvents();
                            }
                            response = CurrentPlayer.LastResponse.Split(' ');
                            CurrentPlayer.NewResponse = false;
                            try
                            {
                                BinaryFormatter formatter = new BinaryFormatter();
                                byte[] test = Convert.FromBase64String(response[0]);
                                using (MemoryStream stream = new MemoryStream(test))
                                {
                                    CurrentGame.Winners = (List<Answer>)formatter.Deserialize(stream);
                                }
                                string message = "";
                                if (CurrentGame.Winners.Count > 1)
                                {
                                    message = "The winning answers are:" + Environment.NewLine;
                                    foreach (Answer winner in CurrentGame.Winners)
                                    {
                                        message = message + Environment.NewLine + winner.Text + " (by " + winner.Submitter.Name + ")";
                                    }
                                }
                                else
                                {
                                    message = "The winning answer is: " + CurrentGame.Winners[0].Text + " (by " + CurrentGame.Winners[0].Submitter.Name + ")";
                                }
                                MessageBox.Show(message, "Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                ShownWinners = CurrentGame.Round;
                            }
                            catch
                            {
                                MessageBox.Show("Unexpected Error! Bad response to GETWINNER, Application will exit!", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                CurrentGame.Stop();
                                Application.Exit();
                                break;
                            }
                            CurrentGame.Stage = 'E';
                            break;
                        default:
                            MessageBox.Show("Unexpected Error! Unknown Game State, Application will exit!", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            CurrentGame.Stop();
                            Application.Exit();
                            break;
                    }
                    frmWaiting waiting = new frmWaiting();
                    switch (CurrentGame.Stage)
                    {
                        case 'W':
                            waiting.Update("Waiting for other players to join the game...");
                            waiting.ShowDialog();
                            break;
                        case 'P':
                            int submitted = 0;
                            foreach (ClientNetworking player in CurrentGame.LocalPlayers)
                            {
                                CurrentPlayer = player;
                                CurrentPlayer.NextCommand = "NEED ANSWER";
                                CurrentPlayer.NewCommand = true;
                                while (!CurrentPlayer.NewResponse)
                                {
                                    Application.DoEvents();
                                }
                                string[] responseP = CurrentPlayer.LastResponse.Split(' ');
                                CurrentPlayer.NewResponse = false;
                                switch (responseP[0])
                                {
                                    case "YES":
                                        if (CurrentGame.LocalPlayers.Count > 1)
                                        {
                                            MessageBox.Show("Pass to " + CurrentPlayer.Owner.Name, "Next Player", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                        }
                                        frmGameplay game = new frmGameplay();
                                        if (game.ShowDialog() == DialogResult.Abort)
                                        {
                                            break;
                                        }
                                        submitted++;
                                        break;
                                    case "NO":
                                        break;
                                    default:
                                        MessageBox.Show("Unexpected Error! Unknown NEED ANSWER Response, Application will exit!", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                        CurrentGame.Stop();
                                        Application.Exit();
                                        break;
                                }
                            }
                            if (CurrentGame.Stage == 'P' && submitted < 1)
                            {
                                waiting.Update("Waiting for other players to submit their answers...");
                                waiting.ShowDialog();
                            }
                            break;
                        case 'V':
                            int voted = 0;
                            foreach (ClientNetworking player in CurrentGame.LocalPlayers)
                            {
                                CurrentPlayer = player;
                                CurrentPlayer.NextCommand = "NEED VOTE";
                                CurrentPlayer.NewCommand = true;
                                while (!CurrentPlayer.NewResponse)
                                {
                                    Application.DoEvents();
                                }
                                string[] responseP = CurrentPlayer.LastResponse.Split(' ');
                                CurrentPlayer.NewResponse = false;
                                switch (responseP[0])
                                {
                                    case "YES":
                                        if (CurrentGame.LocalPlayers.Count > 1)
                                        {
                                            MessageBox.Show("Pass to " + CurrentPlayer.Owner.Name, "Next Player", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                        }
                                        frmVote vote = new frmVote();
                                        if (vote.ShowDialog() == DialogResult.Abort)
                                        {
                                            break;
                                        }
                                        voted++;
                                        break;
                                    case "NO":
                                        break;
                                    default:
                                        MessageBox.Show("Unexpected Error! Unknown NEED VOTE Response, Application will exit!", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                        CurrentGame.Stop();
                                        Application.Exit();
                                        break;
                                }
                            }
                            if (CurrentGame.Stage == 'V' && voted < 1)
                            {
                                waiting.Update("Waiting for other players to submit their votes...");
                                waiting.ShowDialog();
                            }
                            break;
                        case 'E':
                            CurrentGame.Playable = false;
                            string FinalScore = "The Final Scores are:" + Environment.NewLine;
                            int position = 1;
                            foreach (Player player in CurrentGame.Players)
                            {
                                FinalScore = FinalScore + Environment.NewLine + position + ") " + player.Name + " with " + player.Score + " points";
                                position++;
                            }
                            FinalScore = FinalScore + Environment.NewLine + Environment.NewLine + "Play Again?";
                            CurrentGame.Stop();
                            if (MessageBox.Show(FinalScore, "Final Results - Cards", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                            {
                                goto NewGame;
                            }
                            else
                            {
                                Application.Exit();
                            }
                            break;
                        default:
                            MessageBox.Show("Unexpected Error! Unknown Game State, Application will exit!", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            CurrentGame.Stop();
                            Application.Exit();
                            break;
                    }
                }

                CurrentGame.Stop();
            }
            Application.Exit();
        }

        private static void Hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            frmDebug debug = new frmDebug();
            debug.ShowDialog();
            debug.Dispose();
        }
    }
}