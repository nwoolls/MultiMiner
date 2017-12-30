using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiMiner.Xgminer.Data;

namespace MultiMiner.Xgminer.Tests
{
    [TestClass]
    public class PoolFeaturesTests
    {
        [TestMethod]
        public void UpdatePoolFeature_EnableExisingFeature_Succeeds()
        {
            // arrange
            string host = "google.com/#xnsub";
            string fragment = PoolFeatures.XNSubFragment;
            bool enabled = true;

            // act
            string result = PoolFeatures.UpdatePoolFeature(host, fragment, enabled);

            // assert
            Assert.AreEqual("google.com/#xnsub", result);
        }

        [TestMethod]
        public void UpdatePoolFeature_EnableMissingFeature_Succeeds()
        {
            // arrange
            string host = "google.com";
            string fragment = PoolFeatures.XNSubFragment;
            bool enabled = true;

            // act
            string result = PoolFeatures.UpdatePoolFeature(host, fragment, enabled);

            // assert
            Assert.AreEqual("google.com/#xnsub", result);
        }

        [TestMethod]
        public void UpdatePoolFeature_EnableFeature_RemovesDupeSlashes()
        {
            // arrange
            string host = "google.com/";
            string fragment = PoolFeatures.XNSubFragment;
            bool enabled = true;

            // act
            string result = PoolFeatures.UpdatePoolFeature(host, fragment, enabled);

            // assert
            Assert.AreEqual("google.com/#xnsub", result);
        }

        [TestMethod]
        public void UpdatePoolFeature_EnableMultipleFeatures_A_Succeeds()
        {
            // arrange
            string host = "google.com";
            bool enabled = true;

            // act
            string result = PoolFeatures.UpdatePoolFeature(host, PoolFeatures.XNSubFragment, enabled);
            result = PoolFeatures.UpdatePoolFeature(result, PoolFeatures.SkipCBCheckFragment, enabled);

            // assert
            Assert.AreEqual("google.com/#xnsub/#skipcbcheck", result);
        }

        [TestMethod]
        public void UpdatePoolFeature_EnableMultipleFeatures_B_Succeeds()
        {
            // arrange
            string host = "google.com/#xnsub/";
            bool enabled = true;

            // act
            string result = PoolFeatures.UpdatePoolFeature(host, PoolFeatures.XNSubFragment, enabled);
            result = PoolFeatures.UpdatePoolFeature(result, PoolFeatures.SkipCBCheckFragment, enabled);

            // assert
            Assert.AreEqual("google.com/#xnsub/#skipcbcheck", result);
        }

        [TestMethod]
        public void UpdatePoolFeature_EnableMultipleFeatures_C_Succeeds()
        {
            // arrange
            string host = "google.com/#xnsub/#skipcbcheck";
            bool enabled = true;

            // act
            string result = PoolFeatures.UpdatePoolFeature(host, PoolFeatures.XNSubFragment, enabled);
            result = PoolFeatures.UpdatePoolFeature(result, PoolFeatures.SkipCBCheckFragment, enabled);

            // assert
            Assert.AreEqual("google.com/#xnsub/#skipcbcheck", result);
        }

        [TestMethod]
        public void UpdatePoolFeature_EnableMultipleFeatures_D_Succeeds()
        {
            // arrange
            string host = "google.com/#skipcbcheck/#xnsub";
            bool enabled = true;

            // act
            string result = PoolFeatures.UpdatePoolFeature(host, PoolFeatures.XNSubFragment, enabled);
            result = PoolFeatures.UpdatePoolFeature(result, PoolFeatures.SkipCBCheckFragment, enabled);

            // assert
            Assert.AreEqual("google.com/#skipcbcheck/#xnsub", result);
        }

        [TestMethod]
        public void UpdatePoolFeature_DisableExistingFeature_Succeeds()
        {
            // arrange
            string host = "google.com/#xnsub";
            string fragment = PoolFeatures.XNSubFragment;
            bool enabled = false;

            // act
            string result = PoolFeatures.UpdatePoolFeature(host, fragment, enabled);

            // assert
            Assert.AreEqual("google.com", result);
        }

        [TestMethod]
        public void UpdatePoolFeature_DisableMissingFeature_Succeeds()
        {
            // arrange
            string host = "google.com";
            string fragment = PoolFeatures.XNSubFragment;
            bool enabled = false;

            // act
            string result = PoolFeatures.UpdatePoolFeature(host, fragment, enabled);

            // assert
            Assert.AreEqual("google.com", result);
        }
    }
}
