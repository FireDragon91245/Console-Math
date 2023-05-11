using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class MethodTest
    {

        [TestMethod]
        public void SimpleMethods()
        {
            var m1 = typeof(MethodTest).GetMethod("TestM1", BindingFlags.Instance | BindingFlags.NonPublic);
            var m2 = typeof(MethodTest).GetMethod("TestM2", BindingFlags.Instance | BindingFlags.NonPublic);
            var m3 = typeof(MethodTest).GetMethod("TestM3", BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.IsFalse(m1 is null || m2 is null || m3 is null, "methods not set up corectly");

            Assert.IsTrue(m1.IsCallableWith(new []{typeof(int)}), "int call int");
            Assert.IsTrue(m2.IsCallableWith(new[] { typeof(int) }), "int call long");
            Assert.IsTrue(m3.IsCallableWith(new[] { typeof(int) }), "int call obj");

            TestM1(1);
            TestM2(1);
            TestM3(1);
        }

        [TestMethod]
        public void GenericMethods()
        {
            var m4 = typeof(MethodTest).GetMethod("TestM4", BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.IsFalse(m4 is null, "methods not set up corectly");

            Assert.IsTrue(m4.IsCallableWithGeneric(new []{typeof(B)}, new []{ typeof(B) }), "B -> where T : class, ITest");
            Assert.IsFalse(m4.IsCallableWithGeneric(new []{typeof(A)}, new []{typeof(A)}), "A -> where T : class, ITest");

            TestM4(new B());
        }

        [TestMethod]
        public void ParamMethods()
        {

        }

        private void TestM1(int a)
        {

        }

        private void TestM2(long a)
        {

        }

        private void TestM3(object a)
        {

        }

        private interface ITest
        {
            
        }

        private class A
        {

        }

        private class B : ITest
        {
            
        }

        private void TestM4<T>(T a) where T : class, ITest
        {
        }
    }
}
