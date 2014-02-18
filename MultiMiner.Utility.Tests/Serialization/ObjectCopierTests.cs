using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MultiMiner.Utility.Serialization.Tests
{
    [TestClass]
    public class ObjectCopierTests
    {
        enum EnumA
        {
            None = 0,
            A = 1 << 0,
            B = 1 << 1,
            C = 1 << 2
        }

        enum EnumB
        {
            None = 0,
            A = 1 << 0,
            B = 1 << 1,
            C = 1 << 2
        }

        class ClassA
        {
            public string PropertyA { get; set; }
            public int PropertyB { get; set; }
            public bool PropertyC { get; set; }
            public DateTime? PropertyD { get; set; }
            public EnumA PropertyE { get; set; }
        }

        class ClassB
        {
            public string PropertyA { get; set; }
            public int PropertyB { get; set; }
            public bool PropertyC { get; set; }
            public DateTime? PropertyD { get; set; }
            public EnumA PropertyE { get; set; }
        }

        [TestMethod]
        public void ObjectCopier_DifferentClasses_Succeeds()
        {
            ClassA objA = new ClassA
            {
                PropertyA = "1",
                PropertyB = 1,
                PropertyC = true,
                PropertyD = DateTime.UtcNow,
                PropertyE = EnumA.B
            };

            ClassB objB = new ClassB();

            ObjectCopier.CopyObject(objA, objB);

            Assert.IsTrue(objB.PropertyA.Equals(objA.PropertyA));
            Assert.IsTrue(objB.PropertyB.Equals(objA.PropertyB));
            Assert.IsTrue(objB.PropertyC.Equals(objA.PropertyC));
            Assert.IsTrue(objB.PropertyD.Equals(objA.PropertyD));
            Assert.IsTrue(objB.PropertyE.Equals(objA.PropertyE));
        }

        [TestMethod]
        public void ObjectCopier_SameClasses_Succeeds()
        {
            ClassA objA = new ClassA
            {
                PropertyA = "1",
                PropertyB = 1,
                PropertyC = true,
                PropertyD = DateTime.UtcNow,
                PropertyE = EnumA.B
            };

            ClassA objB = new ClassA();

            ObjectCopier.CopyObject(objA, objB);

            Assert.IsTrue(objB.PropertyA.Equals(objA.PropertyA));
            Assert.IsTrue(objB.PropertyB.Equals(objA.PropertyB));
            Assert.IsTrue(objB.PropertyC.Equals(objA.PropertyC));
            Assert.IsTrue(objB.PropertyD.Equals(objA.PropertyD));
            Assert.IsTrue(objB.PropertyE.Equals(objA.PropertyE));
        }
    }
}
