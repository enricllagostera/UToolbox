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
        public void SmartBag_Filter_UnusedReqs()
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
            Assert.AreEqual(sb.FilterConditions(query).Count, 1);
        }

        [Test]
        public void SmartBag_Filter_OneReqSatisfied()
        {
            // Sample data
            var precond = new List<Condition>();
            precond.Add(new Condition("cond1"));
            var allItems = new List<ConditionedItem>();
            allItems.Add(new CItemTest("item1", 10, 1, precond, null));
            allItems.Add(new CItemTest("item2", 10, 2, null, null));
            var sb = new SmartBag(allItems);
            var query = new List<Condition>();
            query.Add(new Condition("cond1"));
            Debug.Log(sb.FilterConditions(query).Count);
            Assert.AreEqual(sb.FilterConditions(query).Count, 2);
        }

        [Test]
        public void SmartBag_Filter_NoQuery()
        {
            // Sample data
            var precond = new List<Condition>();
            precond.Add(new Condition("cond1"));
            var allItems = new List<ConditionedItem>();
            allItems.Add(new CItemTest("item1", 10, 1, precond, null));
            allItems.Add(new CItemTest("item2", 10, 2, null, null));
            var sb = new SmartBag(allItems);
            var query = new List<Condition>();
            Assert.AreEqual(sb.FilterConditions(query).Count, 1);
        }

        [Test]
        public void SmartBag_Filter_AllReqs()
        {
            // Sample data
            var allItems = new List<ConditionedItem>();
            allItems.Add(new CItemTest("item1", 10, 1, new List<Condition>() { new Condition("cond1") }, null));
            allItems.Add(new CItemTest("item2", 10, 2, new List<Condition>() { new Condition("cond2") }, null));
            var sb = new SmartBag(allItems);
            var query = new List<Condition>();
            query.Add(new Condition("cond1"));
            query.Add(new Condition("cond2"));
            Assert.AreEqual(sb.FilterConditions(query).Count, 2);
        }


        [Test]
        public void SmartBag_Peek_NoReqs()
        {
            // Sample data
            var allItems = new List<ConditionedItem>();
            allItems.Add(new CItemTest("item1", 0, 1, null, null));
            allItems.Add(new CItemTest("item2", 0, 2, null, null));
            allItems.Add(new CItemTest("item3", 100, 2, null, null));
            var sb = new SmartBag(allItems);
            var query = new List<Condition>();
            query.Add(new Condition("cond1"));
            query.Add(new Condition("cond3"));
            Assert.True(sb.FilterConditions(query).Count == 3);
            Assert.True(sb.Peek(query).Id == "item3");
        }

        [Test]
        public void SmartBag_Peek_TwoReqs()
        {
            // Sample data
            var allItems = new List<ConditionedItem>();
            allItems.Add(new CItemTest("item1", 10, 1, new List<Condition>() { new Condition("cond1") }, null));
            allItems.Add(new CItemTest("item2", 10, 2, new List<Condition>() { new Condition("cond2") }, null));
            allItems.Add(new CItemTest("item3", 10, 2, new List<Condition>() { new Condition("cond1"), new Condition("cond3") }, null));
            var sb = new SmartBag(allItems);
            var query = new List<Condition>();
            query.Add(new Condition("cond1"));
            query.Add(new Condition("cond3"));
            Assert.False(sb.Peek(query).Id == "item2");
            Assert.False(sb.Peek(query).Id == "item2");
            Assert.False(sb.Peek(query).Id == "item2");
        }

        [Test]
        public void SmartBag_Peek_OneReq()
        {
            // Sample data
            var allItems = new List<ConditionedItem>();
            allItems.Add(new CItemTest("item1", 10, 1, new List<Condition>() { new Condition("cond1") }, null));
            allItems.Add(new CItemTest("item2", 10, 2, new List<Condition>() { new Condition("cond2") }, null));
            var sb = new SmartBag(allItems);
            var query = new List<Condition>();
            query.Add(new Condition("cond1"));
            Assert.AreEqual(sb.Peek(query).Id, "item1");
        }

        [Test]
        public void SmartBag_Peek_Proportions()
        {
            // Sample data
            var allItems = new List<ConditionedItem>();
            allItems.Add(new CItemTest("item1", 5, 1, null, null));
            allItems.Add(new CItemTest("item2", 20, 2, null, null));
            allItems.Add(new CItemTest("item3", 75, 2, null, null));
            var sb = new SmartBag(allItems);
            var query = new List<Condition>();

            int c1 = 0, c2 = 0, c3 = 0;
            var draw = sb.Peek(query);
            for (int i = 0; i < 100; i++)
            {
                draw = sb.Peek(query);
                switch (draw.Id)
                {
                    case "item1":
                        c1++;
                        break;
                    case "item2":
                        c2++;
                        break;
                    case "item3":
                        c3++;
                        break;
                }
            }
            Debug.Log(c1 + " / " + c2 + " / " + c3);
            Assert.True(c1 < c2 && c2 < c3);
        }

        [Test]
        public void SmartBag_Pick_NotFound()
        {
            // Sample data
            var allItems = new List<ConditionedItem>();
            allItems.Add(new CItemTest("item1", 5, 1, null, null));
            allItems.Add(new CItemTest("item2", 20, 2, null, null));
            var sb = new SmartBag(allItems);
            Assert.Null(sb.Pick("item5"));
        }

        [Test]
        public void SmartBag_Pick_IdFoundAndTick()
        {
            // Sample data
            var allItems = new List<ConditionedItem>();
            allItems.Add(new CItemTest("item1", 5, 2, null, null));
            allItems.Add(new CItemTest("item2", 20, 4, null, null));
            var sb = new SmartBag(allItems);
            var pick = sb.Pick("item1");
            Assert.True(pick.Id == "item1");
            sb.Tick();
            Assert.True(pick.IsLocked());
            sb.Tick();
            Assert.False(pick.IsLocked());
        }

        [Test]
        public void SmartBag_Pick_ForcePick()
        {
            // Sample data
            var allItems = new List<ConditionedItem>();
            allItems.Add(new CItemTest("item1", 5, 2, null, null));
            allItems.Add(new CItemTest("item2", 20, 4, null, null));
            var sb = new SmartBag(allItems);
            var pick = sb.Pick("item1");
            Assert.True(pick.Id == "item1");
            Assert.True(pick.IsLocked());
            pick = sb.Pick("item1", true);
            Assert.True(pick.Id == "item1");
        }

        [Test]
        public void SmartBag_Pick_IdFoundAndStateChanges()
        {
            // Sample data
            var allItems = new List<ConditionedItem>();
            allItems.Add(new CItemTest("item1", 5, 2, null, new List<Condition>(){ new Condition("post1") }));
            allItems.Add(new CItemTest("item2", 20, 4, null, null));
            var sb = new SmartBag(allItems);
            Assert.Null(sb.GetCondition("post1"));
            var pick = sb.Pick("item1");
            Assert.True(sb.GetCondition("post1").Status);
        }

        [Test]
        public void SmartBag_PickRandom()
        {
            // Sample data
            var allItems = new List<ConditionedItem>();
            allItems.Add(new CItemTest("item1", 5, 4, null, new List<Condition>(){ new Condition("post1") }));
            allItems.Add(new CItemTest("item2", 20, 4, null, null));
            var sb = new SmartBag(allItems);
            var pick1 = sb.PickRandom(null);
            var pick2 = sb.PickRandom(null);
            var pick3 = sb.PickRandom(null);
            Assert.True(pick1.Id != pick2.Id);
            Assert.Null(pick3);

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
            var reqs = new List<Condition>();
            reqs.Add(new Condition("teste1"));

            var state = new List<Condition>();
            state.Add(new Condition("teste1"));
            state.Add(new Condition("!teste2"));

            //Assert
            Assert.True(Condition.CheckAll(reqs, state));
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
            var reqs = new List<Condition>();
            reqs.Add(new Condition("teste1"));
            reqs.Add(new Condition("!teste2"));
            reqs.Add(new Condition("teste3"));

            var state = new List<Condition>();
            state.Add(new Condition("teste1"));
            state.Add(new Condition("!teste2"));

            //Assert
            Assert.False(Condition.CheckAll(reqs, state));
        }

        #endregion
    }
}


