using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UToolbox.SmartBag;
using NUnit.Framework;

namespace UToolbox.Tests
{
    public class SmartBagTests
    {
        #region ConditionedItem Tests

        private class CItemTest : ConditionedItem
        {
            public CItemTest(string id, float weight, int lockedInterval, List<Condition> preconditions, List<Condition> effects)
                : base(id, weight, lockedInterval, preconditions, effects)
            {
            }
        }

        [Test]
        public void ConditionedItem_Constructor()
        {
            //Arrange
            var item = new CItemTest("teste", 10, 2, null, null);

            //Act

            //Assert
            Assert.False(item.IsLocked());
        }

        [Test]
        public void ConditionedItem_Use_LockedAfter()
        {
            //Arrange
            var item = new CItemTest("teste", 10, 2, null, null);

            //Act
            var effects = item.Use();

            //Assert
            Assert.True(item.IsLocked());
        }

        [Test]
        public void ConditionedItem_Use_GetEffects()
        {
            //Arrange
            var post = new List<Condition>();
            post.Add(new Condition("teste1"));
            post.Add(new Condition("!teste2"));
            var item = new CItemTest("teste", 10, 2, null, post);

            //Act
            var afterEffects = item.Use();

            //Assert
            Assert.True(Condition.CheckAll(afterEffects, post));
        }

        [Test]
        public void ConditionedItem_Ticks()
        {
            //Arrange
            var item = new CItemTest("teste", 10, 2, null, null);

            //Act
            item.Use();
            item.Tick();
            Assert.True(item.IsLocked());
            item.Tick();
            Assert.False(item.IsLocked());
        }

        #endregion

        #region Condition Tests

        [Test]
        public void Condition_Parse_True()
        {
            //Arrange
            var cond = new Condition("teste", false);

            //Act
            cond = Condition.Parse("teste");

            //Assert
            Assert.True(cond.Status);
        }

        [Test]
        public void Condition_Parse_False()
        {
            //Arrange
            var cond = new Condition("teste", true);

            //Act
            cond = Condition.Parse("!teste");

            //Assert
            Assert.False(cond.Status);
        }

        [Test]
        public void Condition_Constructor_Parse()
        {
            //Act
            var cond = new Condition("!teste");

            //Assert
            Assert.True(cond.Status == false && cond.Id == "teste");
        }

        [Test]
        public void Condition_Check_DiffIds()
        {
            //Arrange
            var cond1 = new Condition("teste1");
            var cond2 = new Condition("teste2");

            //Assert
            Assert.False(Condition.Check(cond1, cond2));
        }

        [Test]
        public void Condition_Check_DiffValues()
        {
            //Arrange
            var cond1 = new Condition("teste");
            var cond2 = new Condition("!teste");

            //Assert
            Assert.False(Condition.Check(cond1, cond2));
        }

        [Test]
        public void Condition_CheckAll_ExpectedQuery()
        {
            //Arrange
            var state = new List<Condition>();
            state.Add(new Condition("teste1"));
            state.Add(new Condition("!teste2"));

            var query = new List<Condition>();
            query.Add(new Condition("teste1"));

            //Assert
            Assert.True(Condition.CheckAll(state, query));
        }

        [Test]
        public void Condition_CheckAll_LargeQuery()
        {
            //Arrange
            var state = new List<Condition>();
            state.Add(new Condition("teste1"));
            state.Add(new Condition("!teste2"));

            var query = new List<Condition>();
            query.Add(new Condition("teste3"));

            //Assert
            Assert.False(Condition.CheckAll(state, query));
        }

        [Test]
        public void Condition_CheckAll_DiffValuesQuery()
        {
            //Arrange
            var state = new List<Condition>();
            state.Add(new Condition("teste1"));
            state.Add(new Condition("!teste2"));

            var query = new List<Condition>();
            query.Add(new Condition("!teste1"));

            //Assert
            Assert.False(Condition.CheckAll(state, query));
        }

        [Test]
        public void Condition_CheckAll_OverflowQuery()
        {
            //Arrange
            var state = new List<Condition>();
            state.Add(new Condition("teste1"));
            state.Add(new Condition("!teste2"));

            var query = new List<Condition>();
            query.Add(new Condition("teste1"));
            query.Add(new Condition("!teste2"));
            query.Add(new Condition("teste3"));

            //Assert
            Assert.False(Condition.CheckAll(state, query));
        }

        #endregion
    }
}


