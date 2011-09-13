using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace MutableObject.Tests
{
    [TestFixture]
    public class MutableObjectTests
    {
        [Test]
        public void DefaultObjectsShouldntBeMutableObjects()
        {
            Interface1 obj1 = new Class1();

            Assert.IsFalse(Mutate.IsMutable(obj1));
        }

        [Test]
        public void MutableObjectsShouldBeMutable()
        {
            Interface1 obj1 = Mutate.CreateMutableObject<Interface1>();

            Assert.IsTrue(Mutate.IsMutable(obj1));
        }

        [Test]
        public void MutableObjectsWithoutBaseShouldntBeReadable()
        {
            Interface1 obj1 = Mutate.CreateMutableObject<Interface1>();

            try
            {
                string name = obj1.Name;
                Assert.Fail("Did not throw.");
            }
            catch (InvalidOperationException e)
            {
                Assert.IsNotNull(e);
            }
        }

        [Test]
        public void MutableObjectsWithBaseShouldBeReadable()
        {
            Interface1 obj = new Class1();
            obj.Name = Guid.NewGuid().ToString();

            Interface1 obj1 = Mutate.CreateMutableObject<Interface1>(obj);

            Assert.AreEqual(obj.Name, obj1.Name);
        }

        [Test]
        public void SetPropertiesShouldBeReadable()
        {
            String name = Guid.NewGuid().ToString();
            Interface1 obj1 = Mutate.CreateMutableObject<Interface1>();
            obj1.Name = name;

            Assert.AreEqual(name, obj1.Name);
        }

        [Test]
        public void SetPropertiesWithBaseShouldPropagate()
        {
            Interface1 obj = new Class1();
            obj.Name = Guid.NewGuid().ToString();
            String newName = Guid.NewGuid().ToString();

            Interface1 obj1 = Mutate.CreateMutableObject<Interface1>(obj);
            obj1.Name = newName;

            Assert.AreEqual(newName, obj.Name);
        }

        [Test]
        public void ChangedPropertiesShouldBeGettableAsDict()
        {
            Interface1 obj = new Class1();
            Interface1 obj1 = Mutate.CreateMutableObject<Interface1>();
            Interface1 obj2 = Mutate.CreateMutableObject<Interface1>(obj);

            IDictionary<string, object> d1 = Mutate.GetChangedProperties(obj1);
            IDictionary<string, object> d2 = Mutate.GetChangedProperties(obj2);

            Assert.AreEqual(0, d1.Count);
            Assert.AreEqual(0, d2.Count);

            string newName = Guid.NewGuid().ToString();

            obj1.Name = obj2.Name = newName;

            d1 = Mutate.GetChangedProperties(obj1);
            d2 = Mutate.GetChangedProperties(obj2);

            Assert.AreEqual(1, d1.Count);
            Assert.AreEqual(1, d2.Count);

            Assert.IsTrue(d1.ContainsKey("Name"));
            Assert.IsTrue(d2.ContainsKey("Name"));

            Assert.AreEqual(newName, d1["Name"]);
            Assert.AreEqual(newName, d2["Name"]);
        }

        [Test]
        public void BoundObjectsShouldBeBound()
        {
            Interface1 obj = new Class1();
            Interface1 obj1 = Mutate.CreateMutableObject<Interface1>();
            Interface1 obj2 = Mutate.CreateMutableObject<Interface1>(obj);

            bool isBound1 = Mutate.IsBound(obj1);
            bool isBound2 = Mutate.IsBound(obj2);

            Assert.IsFalse(isBound1);
            Assert.IsTrue(isBound2);
        }

        [Test]
        public void PropertiesIsSetTest()
        {
            Interface1 obj = new Class1();
            Interface1 obj1 = Mutate.CreateMutableObject<Interface1>();
            Interface1 obj2 = Mutate.CreateMutableObject<Interface1>(obj);

            Assert.IsTrue(Mutate.PropertiesIsSet(obj2));
            Assert.IsTrue(Mutate.PropertiesIsSet(obj1));

            Assert.IsTrue(Mutate.PropertiesIsSet(obj2, "nonexisting property"));
            Assert.IsFalse(Mutate.PropertiesIsSet(obj1, "nonexisting property"));

            Assert.IsFalse(Mutate.PropertiesIsSet(obj1, "Name"));

            obj1.Name = "set";

            Assert.IsTrue(Mutate.PropertiesIsSet(obj1, "Name"));
        }
    }

    public interface Interface1
    {
        string Name { get; set; }
    }

    public class Class1 : Interface1
    {
        public string Name { get; set; }
    }
}
