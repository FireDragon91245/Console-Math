
namespace Tests
{
    [TestClass]
    public class TypeTest
    {
        [TestMethod]
        public void TypeTestsNormal ()
        {
            var objT = typeof(object);
            var intT = typeof(int);
            var longT = typeof(long);
            var floatT = typeof(float);
            var doubleT = typeof(double);

            Assert.IsTrue(intT.IsConvertible(objT), "int -> obj");
            Assert.IsTrue(intT.IsConvertible(longT), "int -> long");
            Assert.IsTrue(floatT.IsConvertible(doubleT), "float -> double");
            Assert.IsTrue(intT.IsConvertible(doubleT), "int -> double");
        }

        [TestMethod]
        public void TypeTestsArray()
        {
            var objT = typeof(object[]);
            var intT = typeof(int[]);
            var longT = typeof(long[]);
            var floatT = typeof(float[]);
            var doubleT = typeof(double[]);

            Assert.IsTrue(intT.IsConvertible(objT), "int -> obj");
            Assert.IsTrue(intT.IsConvertible(longT), "int -> long");
            Assert.IsTrue(floatT.IsConvertible(doubleT), "float -> double");
            Assert.IsTrue(intT.IsConvertible(doubleT), "int -> double");
        }

        [TestMethod]
        public void TypeTestsGeneric()
        {
            var objListT = typeof(List<object>);
            var intListT = typeof(List<int>);
            var longListT = typeof(List<long>);

            Assert.IsTrue(intListT.IsConvertible(objListT), "int -> obj");
            Assert.IsTrue(intListT.IsConvertible(longListT), "int -> long");
        }

        [TestMethod]
        public void TypeTestsTouple()
        {
            var objT = typeof((object, object));
            var intT = typeof((int, int));
            var longT = typeof((long, long));

            Assert.IsTrue(intT.IsConvertible(objT), "int -> obj");
            Assert.IsTrue(intT.IsConvertible(longT), "int -> long");
        }

        [TestMethod]
        public void InterfaceTest()
        {
            var interfaceT = typeof(ITest);
            var implementationT = typeof(TestInterface);

            Assert.IsTrue(implementationT.IsConvertible(interfaceT), "impl -> interface");
            Assert.IsFalse(interfaceT.IsConvertible(implementationT), "interface -> impl");
        }

        [TestMethod]
        public void InheritenceTest()
        {
            var aT = typeof(ATest);
            var bT = typeof(BTest);

            Assert.IsTrue(bT.IsConvertible(aT), "b -> a");
            Assert.IsFalse(aT.IsConvertible(bT), "a -> b");
        }

        private interface ITest
        {
            
        }

        private class TestInterface : ITest
        {

        }

        private class ATest
        {

        }

        private class BTest : ATest
        {

        }
    }
}