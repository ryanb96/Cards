using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

namespace JoePitt.Cards.Editor
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            FormClosing += FrmMain_FormClosing;
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Quit Cards - Editor?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            { e.Cancel = true; }
        }

        private void btnNewGUID_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Generate new GUID?" + Environment.NewLine + "This will break the version control for any previous versions of this pack, potentially leading to duplicate packs", "Generate new GUID?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                Guid newGUID = Guid.NewGuid();
                txtGUID.Text = newGUID.ToString();
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            Guid newGUID = Guid.NewGuid();
            txtGUID.Text = newGUID.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Regex alphaNumeric = new Regex("^[a-zA-Z0-9]*$");
            if (string.IsNullOrEmpty(txtName.Text))
            { MessageBox.Show("Name cannot be blank", "Bad Name", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            else if (alphaNumeric.IsMatch(txtName.Text))
            {
                SaveFileDialog dlgSave = new SaveFileDialog();
                dlgSave.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dlgSave.FileName = txtName.Text + "_" + txtVersionMajor.Value + "." + txtVersionMinor.Value + "_" + txtGUID.Text + ".cahc";
                dlgSave.Filter = "Cards Against Humanity Cards Set (*.cahc)|*.cahc";
                DialogResult result = dlgSave.ShowDialog();
                while (result == DialogResult.OK && !dlgSave.FileName.EndsWith(".cahc"))
                {
                    MessageBox.Show("You must use the .cahc extension at the end of the file name", "Bad Extension", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    result = dlgSave.ShowDialog();
                }
                if (result == DialogResult.OK && dlgSave.FileName.EndsWith(".cahc"))
                {
                    XmlDocument cardSetDoc = new XmlDocument();
                    XmlElement xmlCardSet = (XmlElement)cardSetDoc.AppendChild(cardSetDoc.CreateElement("CardSet"));
                    xmlCardSet.SetAttribute("GUID", txtGUID.Text);
                    xmlCardSet.SetAttribute("Name", txtName.Text);
                    xmlCardSet.SetAttribute("Version", txtVersionMajor.Value + "." + txtVersionMinor.Value);

                    XmlElement xmlBlackCards = (XmlElement)xmlCardSet.AppendChild(cardSetDoc.CreateElement("BlackCards"));
                    int i = 1;
                    string[] blackCards1 = txtBlackCards1.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    foreach (string blackCard in blackCards1)
                    {
                        if (!string.IsNullOrEmpty(blackCard))
                        {
                            XmlElement xmlBlackCard = (XmlElement)xmlBlackCards.AppendChild(cardSetDoc.CreateElement("Card"));
                            xmlBlackCard.SetAttribute("ID", "B" + i);
                            xmlBlackCard.SetAttribute("Needs", "1");
                            xmlBlackCard.InnerText = blackCard;
                            i++;
                        }
                    }
                    string[] blackCards2 = txtBlackCards2.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    foreach (string blackCard in blackCards2)
                    {
                        if (!string.IsNullOrEmpty(blackCard))
                        {
                            XmlElement xmlBlackCard = (XmlElement)xmlBlackCards.AppendChild(cardSetDoc.CreateElement("Card"));
                            xmlBlackCard.SetAttribute("ID", "B" + i);
                            xmlBlackCard.SetAttribute("Needs", "2");
                            xmlBlackCard.InnerText = blackCard;
                            i++;
                        }
                    }
                    XmlElement xmlWhiteCards = (XmlElement)xmlCardSet.AppendChild(cardSetDoc.CreateElement("WhiteCards"));
                    i = 1;
                    string[] whiteCards = txtWhiteCards.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    foreach (string whiteCard in whiteCards)
                    {
                        if (!string.IsNullOrEmpty(whiteCard))
                        {
                            XmlElement xmlWhiteCard = (XmlElement)xmlWhiteCards.AppendChild(cardSetDoc.CreateElement("Card"));
                            xmlWhiteCard.SetAttribute("ID", "W" + i);
                            xmlWhiteCard.InnerText = whiteCard;
                            i++;
                        }
                    }

                    SHA256CryptoServiceProvider hasher = new SHA256CryptoServiceProvider();
                    byte[] toHash = Encoding.Default.GetBytes(xmlBlackCards.InnerXml + xmlWhiteCards.InnerXml);
                    byte[] hash = hasher.ComputeHash(toHash);
                    xmlCardSet.SetAttribute("Hash", Convert.ToBase64String(hash));

                    cardSetDoc.Save(dlgSave.FileName);
                    MessageBox.Show("Saved to: " + dlgSave.FileName);
                }
            }
            else
            { MessageBox.Show("Name may only contain alphanumeric characters", "Bad Name", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlgOpen = new OpenFileDialog();
            dlgOpen.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dlgOpen.Filter = "Cards Against Humanity Cards Set (*.cahc)|*.cahc";
            dlgOpen.ShowDialog();
            if (File.Exists(dlgOpen.FileName))
            {
                txtName.Text = "";
                txtVersionMajor.Value = 0;
                txtVersionMinor.Value = 1;
                txtBlackCards1.Text = "";
                txtBlackCards2.Text = "";
                txtWhiteCards.Text = "";
                //load
                XmlDocument cardSetDoc = new XmlDocument();
                cardSetDoc.Load(dlgOpen.FileName);
                XmlElement descriptor = (XmlElement)cardSetDoc.GetElementsByTagName("CardSet")[0];
                txtName.Text = descriptor.GetAttribute("Name");
                txtGUID.Text = descriptor.GetAttribute("GUID");
                string[] Version = descriptor.GetAttribute("Version").Split('.');
                txtVersionMajor.Value = Convert.ToInt32(Version[0]);
                txtVersionMinor.Value = Convert.ToInt32(Version[1]) + 1;
                XmlElement blackCards = (XmlElement)cardSetDoc.GetElementsByTagName("BlackCards")[0];
                foreach (XmlElement blackCard in blackCards.GetElementsByTagName("Card"))
                {
                    if (Convert.ToInt32(blackCard.GetAttribute("Needs")) == 1)
                    { txtBlackCards1.Text = txtBlackCards1.Text + blackCard.InnerText + Environment.NewLine; }
                    else
                    { txtBlackCards2.Text = txtBlackCards2.Text + blackCard.InnerText + Environment.NewLine; }
                }
                XmlElement WhiteCards = (XmlElement)cardSetDoc.GetElementsByTagName("WhiteCards")[0];
                foreach (XmlElement WhiteCard in WhiteCards.GetElementsByTagName("Card"))
                {
                    txtWhiteCards.Text = txtWhiteCards.Text + WhiteCard.InnerText + Environment.NewLine;
                }
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Reset all fields?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                txtName.Text = "";
                Guid newGUID = Guid.NewGuid();
                txtGUID.Text = newGUID.ToString();
                txtVersionMajor.Value = 0;
                txtVersionMinor.Value = 1;
                txtBlackCards1.Text = "";
                txtBlackCards2.Text = "";
                txtWhiteCards.Text = "";
            }
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}