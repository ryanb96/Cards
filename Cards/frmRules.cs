using System;
using System.Windows.Forms;

namespace JoePitt.Cards
{
    /// <summary>
    /// Rules UI.
    /// </summary>
    public partial class frmRules : Form
    {
        /// <summary>
        /// Initalises the Rules UI.
        /// </summary>
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
