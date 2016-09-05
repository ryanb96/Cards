using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JoePitt.Cards;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    internal class DealerTests
    {
        [TestMethod]
        internal void GetCardSets()
        {
            Dealer.GetCardSets();
        }

        [TestMethod]
        internal void GetCardSetInfo()
        {
            Dealer.GetCardSetInfo(new Guid("dd12fa05-a7a7-4f6c-8467-4711abbcb16b"));
        }

        [TestMethod]
        internal void Shuffle()
        {
            List<Guid> GUIDs = new List<Guid>();
            GUIDs.Add(new Guid("dd12fa05-a7a7-4f6c-8467-4711abbcb16b"));
            GUIDs.Add(new Guid("f4af220d-ab63-4e64-a8b3-1bbae1454a66"));
            GUIDs.Add(new Guid("9ee1f4a7-3fa6-4fe6-9ff3-517b50a39638"));
            CardSet testCS = new CardSet(GUIDs[0]);
            GUIDs.RemoveAt(0);
            testCS.Merge(GUIDs);
        }

        [TestMethod]
        internal void Deal()
        {
            List<Guid> GUIDs = new List<Guid>();
            GUIDs.Add(new Guid("dd12fa05-a7a7-4f6c-8467-4711abbcb16b"));
            GUIDs.Add(new Guid("f4af220d-ab63-4e64-a8b3-1bbae1454a66"));
            GUIDs.Add(new Guid("9ee1f4a7-3fa6-4fe6-9ff3-517b50a39638"));
            CardSet testCS = new CardSet(GUIDs[0]);
            GUIDs.RemoveAt(0);
            testCS.Merge(GUIDs);
            Dealer.Deal(testCS, 10, 10);
        }
    }
}
