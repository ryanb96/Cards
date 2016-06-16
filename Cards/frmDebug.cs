﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Linq;

namespace JoePitt.Cards
{
    public partial class frmDebug : Form
    {
        public frmDebug()
        {
            InitializeComponent();
            KeyDown += FrmDebug_KeyDown;
            treDebug.KeyDown += FrmDebug_KeyDown;
            treDebug.AfterCheck += TreDebug_AfterCheck;
        }

        private void TreDebug_AfterCheck(object sender, TreeViewEventArgs e)
        {
            CheckChildren(e.Node, e.Node.Checked);
        }

        private void FrmDebug_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.S)
            {
                Export();
            }
        }

        private void frmDebug_Load(object sender, EventArgs e)
        {
            Game game = Program.CurrentGame;

            treDebug.Nodes[0].Nodes["Playable"].Text = "Playable: " + game.Playable;
            treDebug.Nodes[0].Nodes["GameType"].Text = "Game Type: " + game.GameType;

            if (game.GameType != 'J')
            {
                treDebug.Nodes[0].Nodes["ServerNetworking"].Nodes.Add("UPnP_IPv4", "UPnP (IPv4): " + game.HostNetwork.IPv4UPnP);
                treDebug.Nodes[0].Nodes["ServerNetworking"].Nodes.Add("UPnP_IPv6", "UPnP (IPv6): " + game.HostNetwork.IPv6UPnP);
                treDebug.Nodes[0].Nodes["ServerNetworking"].Nodes.Add("Port", "Port: " + game.HostNetwork.Port);
            }
            else
            {
                treDebug.Nodes[0].Nodes["ServerNetworking"].Text = "Server Networking: N/A";
            }
            // Client Networking
            int i = 0;
            foreach (ClientNetworking client in game.LocalPlayers)
            {
                treDebug.Nodes[0].Nodes["ClientNetworking"].Nodes.Add("Client_" + i, client.Owner.Name);
                string command = client.NextCommand;
                if (client.NewCommand)
                {
                    command = "(PENDING)" + command;
                }
                else
                {
                    command = "(SENT) " + command;
                }
                treDebug.Nodes[0].Nodes["ClientNetworking"].Nodes[i].Nodes.Add("Command", command);

                string response = client.LastResponse;
                if (client.NewResponse)
                {
                    response = "(New)" + response;
                }
                else
                {
                    response = "(Old) " + response;
                }
                treDebug.Nodes[0].Nodes["ClientNetworking"].Nodes[i].Nodes.Add("Response", response);
                i++;
            }

            treDebug.Nodes[0].Nodes["NetworkUp"].Text = "Network Up: " + game.NetworkUp;
            treDebug.Nodes[0].Nodes["CardsPerUser"].Text = "Cards Per User: " + game.CardsPerUser;
            treDebug.Nodes[0].Nodes["Rounds"].Text = "Rounds: " + game.Rounds;
            treDebug.Nodes[0].Nodes["NeverHaveI"].Text = "Never Have I: " + game.NeverHaveI;
            treDebug.Nodes[0].Nodes["RebootingTheUniverse"].Text = "Rebooting The Universe: " + game.RebootingTheUniverse;
            treDebug.Nodes[0].Nodes["PlayerCount"].Text = "Player Count: " + game.PlayerCount;
            treDebug.Nodes[0].Nodes["BotCount"].Text = "Bot Count: " + game.BotCount;

            i = 0;
            foreach (Player player in game.Players)
            {
                treDebug.Nodes[0].Nodes["Players"].Nodes.Add("Player_" + i, player.Name);
                treDebug.Nodes[0].Nodes["Players"].Nodes[i].Nodes.Add("IsBot", "Is Bot: " + player.IsBot);
                treDebug.Nodes[0].Nodes["Players"].Nodes[i].Nodes.Add("Score", "Score: " + player.Score);
                treDebug.Nodes[0].Nodes["Players"].Nodes[i].Nodes.Add("Submitted", "Submitted: " + player.Submitted);
                treDebug.Nodes[0].Nodes["Players"].Nodes[i].Nodes.Add("Voted", "Voted: " + player.Voted);
                treDebug.Nodes[0].Nodes["Players"].Nodes[i].Nodes.Add("WhiteCards", "White Cards");
                foreach (Card card in player.WhiteCards)
                {
                    treDebug.Nodes[0].Nodes["Players"].Nodes[i].Nodes[4].Nodes.Add(card.ID, card.Text);
                }
                i++;
            }

            i = 0;
            foreach (CardSet cardSet in game.CardSets)
            {
                treDebug.Nodes[0].Nodes["CardSets"].Nodes.Add("CardSet_" + i, cardSet.Name);
                treDebug.Nodes[0].Nodes["CardSets"].Nodes[i].Nodes.Add("GUID", "GUID: " + cardSet.GUID);
                treDebug.Nodes[0].Nodes["CardSets"].Nodes[i].Nodes.Add("Version", "Version " + cardSet.Version);
                treDebug.Nodes[0].Nodes["CardSets"].Nodes[i].Nodes.Add("BlackCardCount", "Black Card Count: " + cardSet.BlackCardCount);
                treDebug.Nodes[0].Nodes["CardSets"].Nodes[i].Nodes.Add("WhiteCardCount", "White Card Count: " + cardSet.WhiteCardCount);
                i++;
            }

            treDebug.Nodes[0].Nodes["Stage"].Text = "Stage: " + game.Stage;

            i = 0;
            foreach (Card card in game.GameSet.BlackCards.Values)
            {
                treDebug.Nodes[0].Nodes["GameSet"].Nodes["GameBlackCards"].Nodes.Add("Card_" + i, card.Text);
                treDebug.Nodes[0].Nodes["GameSet"].Nodes["GameBlackCards"].Nodes[i].Nodes.Add("ID", "ID: " + card.ID);
                treDebug.Nodes[0].Nodes["GameSet"].Nodes["GameBlackCards"].Nodes[i].Nodes.Add("Needs", "Needs: " + card.Needs);
                i++;
            }
            i = 0;
            foreach (Card card in game.GameSet.WhiteCards.Values)
            {
                treDebug.Nodes[0].Nodes["GameSet"].Nodes["GameWhiteCards"].Nodes.Add("Card_" + i, card.Text);
                treDebug.Nodes[0].Nodes["GameSet"].Nodes["GameWhiteCards"].Nodes[i].Nodes.Add("ID", "ID: " + card.ID);
                i++;
            }

            treDebug.Nodes[0].Nodes["CurrentBlackCard"].Text = "Current Black Card: " + game.CurrentBlackCard;
            treDebug.Nodes[0].Nodes["CurrentWhiteCard"].Text = "Current White Card: " + game.CurrentWhiteCard;
            treDebug.Nodes[0].Nodes["Round"].Text = "Round: " + game.Round;

            i = 0;
            foreach (Answer answer in game.Answers)
            {
                treDebug.Nodes[0].Nodes["Answers"].Nodes.Add("Answer_" + i, answer.Submitter.Name);
                treDebug.Nodes[0].Nodes["Answers"].Nodes[i].Nodes.Add("Text", answer.Text);
                i++;
            }

            i = 0;
            foreach (Vote vote in game.Votes)
            {
                treDebug.Nodes[0].Nodes["Votes"].Nodes.Add("Vote_" + i, vote.Voter.Name);
                treDebug.Nodes[0].Nodes["Votes"].Nodes[i].Nodes.Add("Text", vote.Choice.Text);
                i++;
            }

            i = 0;
            foreach (Answer winner in game.Winners)
            {
                treDebug.Nodes[0].Nodes["Winners"].Nodes.Add("Winner_" + i, winner.Submitter.Name);
            }

            treDebug.ExpandAll();
            treDebug.Nodes[0].Nodes["GameSet"].Nodes["GameBlackCards"].Collapse();
            treDebug.Nodes[0].Nodes["GameSet"].Nodes["GameWhiteCards"].Collapse();
            treDebug.SelectedNode = treDebug.Nodes[0];
            CheckAllNodes(treDebug.Nodes, true);
        }

        public void CheckAllNodes(TreeNodeCollection nodes, bool Check)
        {
            foreach (TreeNode node in nodes)
            {
                node.Checked = Check;
                CheckChildren(node, Check);
            }
        }

        private void CheckChildren(TreeNode rootNode, bool Check)
        {
            foreach (TreeNode node in rootNode.Nodes)
            {
                CheckChildren(node, Check);
                node.Checked = Check;
            }
        }

        private bool Export()
        {
            XElement rootElement = new XElement("Game", CreateXmlElement(treDebug.Nodes[0].Nodes));
            XDocument document = new XDocument(rootElement);
            SaveFileDialog xmlSave = new SaveFileDialog();
            xmlSave.AddExtension = true;
            xmlSave.DefaultExt = ".xml";
            xmlSave.FileName = DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss") + "_CardsDebug.xml";
            xmlSave.Filter = "XML File (*.xml)|*.xml";
            xmlSave.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            xmlSave.Title = "Save Debug Report";
            if (xmlSave.ShowDialog() == DialogResult.OK)
            {
                document.Save(xmlSave.FileName);
                return false;
            }
            return false;
        }

        private static List<XElement> CreateXmlElement(TreeNodeCollection treeViewNodes)
        {
            List<XElement> elements = new List<XElement>();
            foreach (TreeNode treeViewNode in treeViewNodes)
            {
                if (treeViewNode.Checked)
                {
                    XElement element = new XElement(treeViewNode.Name.Replace('/', '_'));
                    if (treeViewNode.GetNodeCount(true) == 0)
                    {
                        element.Value = treeViewNode.Text;
                        if (element.Value.Contains(": "))
                        {
                            element.Value = element.Value.Substring(element.Value.IndexOf(":") + 2);
                        }
                    }
                    else
                    {
                        element.Add(CreateXmlElement(treeViewNode.Nodes));
                    }
                    elements.Add(element);
                }
                else
                {
                    XElement element = new XElement(treeViewNode.Name.Replace('/', '_'));
                    element.Value = "REMOVED";
                    elements.Add(element);
                }
            }
            return elements;
        }
    }
}