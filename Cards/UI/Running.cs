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

        /// <summary>
        /// Handles Key Presses and Launch Debug UI if CTRL+SHIFT+F1 is pressed.
        /// </summary>
        /// <param name="msg">System-Generated.</param>
        /// <param name="keyData">The keys that are pressed.</param>
        /// <returns>If the keypress was handled.</returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Control | Keys.Shift | Keys.F1:
                    UI.Debug debugUI = new Debug();
                    debugUI.ShowDialog();
                    return true;
            }
            return false;
        }
    }
}
