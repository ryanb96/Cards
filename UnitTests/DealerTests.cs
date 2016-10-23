using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace JoePitt.Cards.UnitTests
{
    [TestClass]
    public class DealerTests
    {
        [TestMethod]
        public void GetPlayerName()
        {
            Program.Init();
            if (string.IsNullOrEmpty(Dealer.GetPlayerName()))
            {
                Assert.Fail("Player Name Not Found");
            }
        }

        [TestMethod]
        public void GetCardSets()
        {
            Program.Init();
            Dealer.GetCardSets();
        }

        [TestMethod]
        public void Shuffle()
        {
            Program.Init();
            List<string> GUIDs = new List<string>();
            foreach (string[] set in Dealer.GetCardSets())
            {
                GUIDs.Add(set[0]);
            }
            CardSet testCS = new CardSet(GUIDs[0]);
            GUIDs.RemoveAt(0);
            testCS.Merge(GUIDs);
        }

        [TestMethod]
        public void Deal()
        {
            Program.Init();
            List<string> GUIDs = new List<string>();
            foreach (string[] set in Dealer.GetCardSets())
            {
                GUIDs.Add(set[0]);
            }
            CardSet testCS = new CardSet(GUIDs[0]);
            GUIDs.RemoveAt(0);
            testCS.Merge(GUIDs);
            Dealer.Deal(testCS, 10, 10);
        }
    }
}
