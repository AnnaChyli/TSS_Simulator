using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecretSharingLogic;
using System.Linq;
using System.Windows;

namespace Tests.SecretSharingLogic
{
    [TestClass]
    public class TssLogicTests
    {
        [TestMethod]
        public void ShareTest()
        {
            var algorithm = new TSSAlgorithm();
            string testString = "Hello world";
            string[] shares = algorithm.Share(testString, 3, 3);
            string result = algorithm.Recover(new[] {shares[0], shares[1], shares[2]}).RecoveredText;

            Assert.AreEqual(testString, result);
        }

        [TestMethod]
        public void ShareTestWithLongInput()
        {
            var algorithm = new TSSAlgorithm();
            string testString = "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz" +
                                "abcdefghijklmnopqrstuvwxyz";
            string[] shares = algorithm.Share(testString, 3, 3);
            string result = algorithm.Recover(new[] {shares[0], shares[1], shares[2]}).RecoveredText;

            Assert.AreEqual(testString, result);
        }

        [TestMethod]
        public void Recover_DuplicationsTest()
        {
            var algorithm = new TSSAlgorithm();
            string testString = "Hello world";
            string[] shares = algorithm.Share(testString, 2, 3);
            var result = algorithm.Recover(new[] { shares[0], shares[0] });

            //If a share has at least 1 duplicate - return null
            Assert.IsNull(result);
        }

    }
}
