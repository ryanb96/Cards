using System.Threading;
using System.Windows.Forms;

namespace JoePitt.Cards
{
    public partial class frmWaiting : Form
    {
        bool waiting = true;
        delegate void SetTextCallback(string text);
        delegate void CloseCallback();

        public frmWaiting()
        {
            InitializeComponent();
            FormClosing += FrmWaiting_FormClosing;
        }

        private void frmWaiting_Load(object sender, System.EventArgs e)
        {
            Thread updater = new Thread(KeepUpdated);
            updater.Start();
        }

        private void FrmWaiting_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && waiting)
            {
                Program.CurrentGame.Stage = 'E';
                Program.CurrentGame.Playable = false;
                DialogResult = DialogResult.Abort;
                Close();
            }
        }

        private void KeepUpdated()
        {
            while (waiting)
            {
                Program.CurrentPlayer = Program.CurrentGame.LocalPlayers[0];
                Program.CurrentPlayer.NextCommand = "GAMEUPDATE";
                Program.CurrentPlayer.NewCommand = true;
                while (!Program.CurrentPlayer.NewResponse)
                {
                    Application.DoEvents();
                }
                string[] response = Program.CurrentPlayer.LastResponse.Split(' ');
                Program.CurrentPlayer.NewResponse = false;
                switch (response[0])
                {
                    case "WAITING":
                        Program.CurrentGame.Stage = 'W';
                        break;
                    case "PLAYING":
                        Program.CurrentGame.Stage = 'P';
                        break;
                    case "VOTING":
                        Program.CurrentGame.Stage = 'V';
                        break;
                    case "END":
                        Program.CurrentGame.Stage = 'E';
                        break;
                    default:
                        MessageBox.Show("Unexpected Error! Unknown Game State, Application will exit!", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        Program.CurrentGame.Stop();
                        Application.Exit();
                        break;
                }

                switch (Program.CurrentGame.Stage)
                {
                    case 'W':
                        Update("Waiting for other players to join the game...");
                        break;
                    case 'P':
                        foreach (ClientNetworking player in Program.CurrentGame.LocalPlayers)
                        {
                            Program.CurrentPlayer = player;
                            Program.CurrentPlayer.NextCommand = "NEED ANSWER";
                            Program.CurrentPlayer.NewCommand = true;
                            while (!Program.CurrentPlayer.NewResponse)
                            {
                                Application.DoEvents();
                            }
                            string[] responseP = Program.CurrentPlayer.LastResponse.Split(' ');
                            Program.CurrentPlayer.NewResponse = false;
                            switch (responseP[0])
                            {
                                case "YES":
                                    waiting = false;
                                    break;
                                case "NO":
                                    break;
                                default:
                                    MessageBox.Show("Unexpected Error! Unknown NEED ANSWER Response, Application will exit!", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    Program.CurrentGame.Stop();
                                    Application.Exit();
                                    break;
                            }
                        }
                        Update("Waiting for other players to submit their answers...");
                        break;
                    case 'V':
                        foreach (ClientNetworking player in Program.CurrentGame.LocalPlayers)
                        {
                            Program.CurrentPlayer = player;
                            Program.CurrentPlayer.NextCommand = "NEED VOTE";
                            Program.CurrentPlayer.NewCommand = true;
                            while (!Program.CurrentPlayer.NewResponse)
                            {
                                Application.DoEvents();
                            }
                            string[] responseP = Program.CurrentPlayer.LastResponse.Split(' ');
                            Program.CurrentPlayer.NewResponse = false;
                            switch (responseP[0])
                            {
                                case "YES":
                                    waiting = false;
                                    break;
                                case "NO":
                                    break;
                                default:
                                    MessageBox.Show("Unexpected Error! Unknown NEED VOTE Response, Application will exit!", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    Program.CurrentGame.Stop();
                                    Application.Exit();
                                    break;
                            }
                        }
                        Update("Waiting for other players to submit their votes...");
                        break;
                    default:
                        waiting = false;
                        return;
                }
                Thread.Sleep(1000);
            }
            CloseCallback d = new CloseCallback(Close);
            this.Invoke(d, new object[] { });
            return;
        }

        public void Update(string Message)
        {
            if (this.txtMessage.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(Update);
                this.Invoke(d, new object[] { Message });
            }
            else
            {
                txtMessage.Text = Message;
            }
        }
    }
}
