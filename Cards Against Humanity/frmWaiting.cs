using System.Windows.Forms;

namespace Cards_Against_Humanity
{
    public partial class frmWaiting : Form
    {
        bool quitable = false;

        public frmWaiting()
        {
            InitializeComponent();
            FormClosing += FrmWaiting_FormClosing;
        }

        private void FrmWaiting_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (quitable)
            {
                e.Cancel = true;
            }
        }

        public void Update(string Message)
        {
            txtMessage.Text = Message;
        }
    }
}
