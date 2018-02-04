using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiMiner.Engine.Extensions;

namespace MultiMiner.Engine.Tests
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        public void ToSpaceDelimitedWords_WithOneHour_Returns_One_Hour()
        {
            // arrange
            var unformattedText = "OneHour";

            // act
            var formattedText = unformattedText.ToSpaceDelimitedWords();

            // assert
            Assert.AreEqual("One Hour", formattedText);
        }

        [TestMethod]
        public void ToSpaceDelimitedWords_WithTwentyFourHours_Returns_Twenty_Four_Hours()
        {
            // arrange
            var unformattedText = "TwentyFourHours";

            // act
            var formattedText = unformattedText.ToSpaceDelimitedWords();

            // assert
            Assert.AreEqual("Twenty Four Hours", formattedText);
        }

        [TestMethod]
        public void ToSpaceDelimitedWords_WithNeoScrypt_Returns_Neo_Scrypt()
        {
            // arrange
            var unformattedText = "NeoScrypt";

            // act
            var formattedText = unformattedText.ToSpaceDelimitedWords();

            // assert
            Assert.AreEqual("Neo Scrypt", formattedText);
        }

        [TestMethod]
        public void ToSpaceDelimitedWords_WithLyra2REv2_Returns_Lyra2RE_v2()
        {
            // arrange
            var unformattedText = "Lyra2REv2";

            // act
            var formattedText = unformattedText.ToSpaceDelimitedWords();

            // assert
            Assert.AreEqual("Lyra2REv2", formattedText);
        }

        [TestMethod]
        public void ToSpaceDelimitedWords_WithLyra2RE_Returns_Lyra2RE_v2()
        {
            // arrange
            var unformattedText = "Lyra2RE";

            // act
            var formattedText = unformattedText.ToSpaceDelimitedWords();

            // assert
            Assert.AreEqual("Lyra2RE", formattedText);
        }

        [TestMethod]
        public void ToSpaceDelimitedWords_WithBlake256r8_Returns_Lyra2RE_v2()
        {
            // arrange
            var unformattedText = "Blake256r8";

            // act
            var formattedText = unformattedText.ToSpaceDelimitedWords();

            // assert
            Assert.AreEqual("Blake256r8", formattedText);
        }
    }
}
