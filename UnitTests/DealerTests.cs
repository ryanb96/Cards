using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cards_Against_Humanity;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class DealerTests
    {
        [TestMethod]
        public void GetCardSets()
        {
            Dealer.GetCardSets();
        }

        [TestMethod]
        public void GetCardSetInfo()
        {
            Dealer.GetCardSetInfo(new Guid("dd12fa05-a7a7-4f6c-8467-4711abbcb16b"));
        }

        [TestMethod]
        public void Shuffle()
        {
            List<Guid> GUIDs = new List<Guid>();
            GUIDs.Add(new Guid("dd12fa05-a7a7-4f6c-8467-4711abbcb16b"));
            GUIDs.Add(new Guid("f4af220d-ab63-4e64-a8b3-1bbae1454a66"));
            GUIDs.Add(new Guid("9ee1f4a7-3fa6-4fe6-9ff3-517b50a39638"));
            Dealer.ShuffleCards(GUIDs, 'W');
            Dealer.ShuffleCards(GUIDs, 'B');
        }

        [TestMethod]
        public void Deal()
        {
            List<Guid> GUIDs = new List<Guid>();
            GUIDs.Add(new Guid("dd12fa05-a7a7-4f6c-8467-4711abbcb16b"));
            GUIDs.Add(new Guid("f4af220d-ab63-4e64-a8b3-1bbae1454a66"));
            GUIDs.Add(new Guid("9ee1f4a7-3fa6-4fe6-9ff3-517b50a39638"));
            List<string> W = Dealer.ShuffleCards(GUIDs, 'W');
            Dealer.Deal(W, 5, 10);
        }
    }
}
