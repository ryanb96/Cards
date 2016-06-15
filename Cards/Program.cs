using System;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace JoePitt.Cards
{
    static class Program
    {
        //static public frmMain iFace;
        static public Game CurrentGame;
        static public ClientNetworking CurrentPlayer;
        static public frmLeaderboard LeaderBoard;
        static internal string SessionKey;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        /*static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }*/

        static void Main()
        {
            try
            {
                if (AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData != null)
                {
                    string arg = Uri.UnescapeDataString(AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData[0].Substring(8));
                    if (File.Exists(arg))
                    {
                        if (arg.EndsWith(".cahc"))
                        {
                            Dealer.InstallCardSet(arg);
                        }
                    }
                }
            }
            catch { }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            RNGCryptoServiceProvider PassGen = new RNGCryptoServiceProvider();
            byte[] PassBytes = new byte[32];
            PassGen.GetBytes(PassBytes);
            SessionKey = Convert.ToBase64String(PassBytes);

            frmNew newGame = new frmNew();
            newGame.ShowDialog();
            newGame.Dispose();
            if (CurrentGame != null)
            {
                KeyboardHook hook = new KeyboardHook();
                hook.KeyPressed += Hook_KeyPressed;
                hook.RegisterHotKey(ModifierKeys.Control | ModifierKeys.Shift, Keys.F1);

                LeaderBoard = new frmLeaderboard();
                while (CurrentGame.Playable)
                {
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
                            break;
                        case "VOTING":
                            CurrentGame.Stage = 'V';
                            break;
                        case "END":
                            CurrentGame.Stage = 'E';
                            break;
                        default:
                            MessageBox.Show("Unexpected Error! Unknown Game State, Application will exit!", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            CurrentGame.Stop();
                            Application.Exit();
                            break;
                    }

                    switch (CurrentGame.Stage)
                    {
                        case 'W':
                            MessageBox.Show("WAITING...");
                            break;
                        case 'P':
                            foreach (ClientNetworking player in CurrentGame.LocalPlayers)
                            {
                                CurrentPlayer = player;
                                int i = 0;
                                if (CurrentPlayer.Owner.Submitted != CurrentGame.Round)
                                {
                                    frmGameplay game = new frmGameplay();
                                    if (game.ShowDialog() == DialogResult.Abort)
                                    {
                                        break;
                                    }
                                    CurrentPlayer.Owner.Submitted = CurrentGame.Round;
                                    i++;
                                }
                                if (i == 0)
                                {
                                    MessageBox.Show("Waiting on remote players answers...");
                                }
                            }
                            break;
                        case 'V':
                            foreach (ClientNetworking player in CurrentGame.LocalPlayers)
                            {
                                CurrentPlayer = player;
                                int i = 0;
                                if (CurrentPlayer.Owner.Voted != CurrentGame.Round)
                                {
                                    frmVote vote = new frmVote();
                                    if (vote.ShowDialog() == DialogResult.Abort)
                                    {
                                        break;
                                    }
                                    CurrentPlayer.Owner.Voted = CurrentGame.Round;
                                    i++;
                                }
                                if (i == 0)
                                {
                                    MessageBox.Show("Waiting on remote players votes...");
                                }
                            }
                            break;
                        case 'E':
                            CurrentGame.Playable = false;
                            MessageBox.Show("Final Results Not Implemented, Exiting!");
                            CurrentGame.Stop();
                            Application.Exit();
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