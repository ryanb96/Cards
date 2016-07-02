using System;
using System.Windows.Forms;

namespace JoePitt.Cards
{
    /// <summary>
    /// Licence UI Control.
    /// </summary>
    public partial class frmLicense : Form
    {
        /// <summary>
        /// Initalises the License UI.
        /// </summary>
        public frmLicense()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void frmLicense_Load(object sender, EventArgs e)
        {
            txtTerms.Select(0, 0);
            txtTerms.AccessibleName = txtTerms.Text;
        }
    }
}
