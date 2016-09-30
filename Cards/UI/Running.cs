using System.Windows.Forms;

namespace JoePitt.Cards.UI
{
    /// <summary>
    /// A UI Control containing the Cards Logo, to ensure the game maintains focus.
    /// </summary>
    public partial class Running : Form
    {
        /// <summary>
        /// Intalies the Base UI Control.
        /// </summary>
        public Running()
        {
            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keys)
        {
            switch (keys)
            {
                case Keys.Control | Keys.Shift | Keys.F1:
                    if (Program.CurrentGame != null)
                    {
                        UI.Debug debugUI = new Debug();
                        debugUI.ShowDialog();
                        return false;
                    }
                    break;
            }
            return false;
        }

    }
}
