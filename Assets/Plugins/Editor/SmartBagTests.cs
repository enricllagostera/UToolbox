/* Enric Llagostera <http://enric.llagostera.com.br> */

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UToolbox.SmartBagSystem;
using NUnit.Framework;

namespace UToolbox.Tests
{
    public class SmartBagTests
    {
        #region SmartBag

        [Test]
        public void SmartBag_Constructor()
        {
            // Empty constructor
            var sb = new SmartBag();
            Assert.True(sb.Items.Count == 0);
            Assert.True(sb.State.Count == 0);

            // Sample data constructor
            var post = new List<Condition>();
            post.Add(new Condition("test1"));
            post.Add(new Condition("!test2"));
            var item = new CItemTest("test", 10, 2, null, post);
            var allItems = new List<ConditionedItem>();
            allItems.Add(item);
            sb = new SmartBag(allItems, post);
            Assert.AreEqual(sb.Items.Count, 1);
            Assert.AreEqual(sb.State.Count, 2);
        }

        [Test]
        public void SmartBag_GetAndSetCondition()
        {
            // Sample data
            var post = new List<Condition>();
            post.Add(new Condition("test1"));
            post.Add(new Condition("!test2"));
            var item = new CItemTest("test", 10, 2, null, post);
            var allItems = new List<ConditionedItem>();
            allItems.Add(item);
            var sb = new SmartBag(allItems, post);
            // new condition through set
            sb.SetCondition(new Condition("test4"));
            Assert.True(sb.State[2].Status);
            // update condition
            sb.SetCondition(new Condition("!test1"));
            Assert.False(sb.State[0].Status);
            // get existing condition
            var res = sb.GetCondition("test2");
            Assert.False(res.Status);
            // attempt to get non-existing condition
            res = sb.GetCondition("test5");
            Assert.IsNull(res);
        }

        [Test]
        public void SmartBag_TickAll()
        {
            // Sample data
            var allItems = new List<ConditionedItem>();
            allItems.Add(new CItemTest("test1", 10, 1, null, null));
            allItems.Add(new CItemTest("test2", 10, 2, null, null));
            var sb = new SmartBag(allItems);
            sb.Items[0].Use();
            sb.Items[1].Use();
            Assert.True(sb.Items[0].IsLocked());
            Assert.True(sb.Items[1].IsLocked());
            sb.Tick();
            Assert.False(sb.Items[0].IsLocked());
            Assert.True(sb.Items[1].IsLocked());
            sb.Tick();
            Assert.False(sb.Items[1].IsLocked());
        }

        [Test]
        public void SmartBag_Filter()
        {
            // Sample data
            var precond = new List<Condition>();
            precond.Add(new Condition("test1"));
            var allItems = new List<ConditionedItem>();
            allItems.Add(new CItemTest("test1", 10, 1, precond, null));
            allItems.Add(new CItemTest("test2", 10, 2, null, null));
            var sb = new SmartBag(allItems);
            var query = new List<Condition>();
            query.Add(new Condition("test3"));
            Assert.AreEqual(sb.Filter(query).Count, 0);

            query = new List<Condition>();
            query.Add(new Condition("test1"));
            Assert.AreEqual(sb.Filter(query).Count, 1);

            query = new List<Condition>();
            query.Add(new Condition("test1"));
            query.Add(new Condition("test2"));
            Assert.AreEqual(sb.Filter(query).Count, 0);

            query = new List<Condition>();
            Assert.AreEqual(sb.Filter(query).Count, 2);
        }

        #endregion

        #region ConditionedItem

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

        #region Condition

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


