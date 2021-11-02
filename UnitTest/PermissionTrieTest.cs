using IceCoffee.Common.Structures.PermissionTrie;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace UnitTest
{
    [TestClass]
    public class PermissionTrieTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            Trie trie = new Trie();

            trie.InsertRange(new string[] 
            {
                "/Home/Index/",
                "/Home/About/",
                "/Home/In/"
            }, true);

            trie.InsertRange(new string[]
            {
                "/Home/Index/2/",
                "/Home/About/"
            }, false);

            Assert.AreEqual(trie.Validate("/HoME/"), false);
            Assert.AreEqual(trie.Validate("/home/Index/"), true);
            Assert.AreEqual(trie.Validate("/home/Index1/"), false);
            Assert.AreEqual(trie.Validate("/home/Index/1/2"), true);
            Assert.AreEqual(trie.Validate("/home/Index/1/"), true);
            Assert.AreEqual(trie.Validate("/home/Index/2/"), false);
            Assert.AreEqual(trie.Validate("/home/About/"), false);
            Assert.AreEqual(trie.Validate("/home/Abou/"), false);
            Assert.AreEqual(trie.Validate("/home/About/1/"), false);
            Assert.AreEqual(trie.Validate("/home/in/1/"), true);
        }
    }
}
