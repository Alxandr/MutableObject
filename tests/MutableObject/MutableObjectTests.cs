﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            Assert.IsFalse(MutableObject.IsMutable(obj1));
        }

        [Test]
        public void MutableObjectsShouldBeMutable()
        {
            Interface1 obj1 = MutableObject.CreateMutableObject<Interface1>();

            Assert.IsTrue(MutableObject.IsMutable(obj1));
        }

        [Test]
        public void MutableObjectsWithoutBaseShouldntBeReadable()
        {
            Interface1 obj1 = MutableObject.CreateMutableObject<Interface1>();

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

            Interface1 obj1 = MutableObject.CreateMutableObject<Interface1>(obj);

            Assert.AreEqual(obj.Name, obj1.Name);
        }

        [Test]
        public void SetPropertiesShouldBeReadable()
        {
            String name = Guid.NewGuid().ToString();
            Interface1 obj1 = MutableObject.CreateMutableObject<Interface1>();
            obj1.Name = name;

            Assert.AreEqual(name, obj1.Name);
        }

        [Test]
        public void SetPropertiesWithBaseShouldPropagate()
        {
            Interface1 obj = new Class1();
            obj.Name = Guid.NewGuid().ToString();
            String newName = Guid.NewGuid().ToString();

            Interface1 obj1 = MutableObject.CreateMutableObject<Interface1>(obj);
            obj1.Name = newName;

            Assert.AreEqual(newName, obj.Name);
        }

        [Test]
        public void ChangedPropertiesShouldBeGettableAsDict()
        {
            Interface1 obj = new Class1();
            Interface1 obj1 = MutableObject.CreateMutableObject<Interface1>();
            Interface1 obj2 = MutableObject.CreateMutableObject<Interface1>(obj);

            IDictionary<string, object> d1 = MutableObject.GetChangedProperties(obj1);
            IDictionary<string, object> d2 = MutableObject.GetChangedProperties(obj2);

            Assert.AreEqual(0, d1.Count);
            Assert.AreEqual(0, d2.Count);

            obj1.Name = obj2.Name = Guid.NewGuid().ToString();

            d1 = MutableObject.GetChangedProperties(obj1);
            d2 = MutableObject.GetChangedProperties(obj2);

            Assert.AreEqual(1, d1.Count);
            Assert.AreEqual(1, d2.Count);

            Assert.IsTrue(d1.ContainsKey("Name"));
            Assert.IsTrue(d2.ContainsKey("Name"));
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
