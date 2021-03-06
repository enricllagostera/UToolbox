﻿/* Enric Llagostera <http://enric.llagostera.com.br> */

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UToolbox.SmartBagSystem;
using NUnit.Framework;

namespace UToolbox.Tests.SmartBagSystem
{
    #region Mockup classes

    public class CItemTest : ConditionedItem
    {
        public CItemTest(string id, float weight, int lockedInterval, List<Condition> preconditions, List<Condition> effects)
            : base(id, weight, lockedInterval, preconditions, effects)
        {
        }
    }

    #endregion

    public class SmartBag_Tests
    {
        [Test]
        public void SmartBag_Constructor()
        {
            // Empty constructor
            var sb = new SmartBag<CItemTest>();
            Assert.True(sb.Items.Count == 0);
            Assert.True(sb.State.Count == 0);
            // Sample data constructor
            var state = Condition.Group("test1", "!test2");
            var item = new CItemTest("test", 10, 2, null, null);
            var allItems = new List<CItemTest>();
            allItems.Add(item);
            sb = new SmartBag<CItemTest>(allItems, state);
            Assert.AreEqual(sb.Items.Count, 1);
            Assert.AreEqual(sb.State.Count, 2);
        }

        [Test]
        public void SmartBag_GetAndSetCondition()
        {
            // Sample data
            var post = Condition.Group("test1", "!test2");
            var item = new CItemTest("test", 10, 2, null, post);
            var allItems = new List<CItemTest>();
            allItems.Add(item);
            var sb = new SmartBag<CItemTest>(allItems, post);
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
            var allItems = new List<CItemTest>();
            allItems.Add(new CItemTest("test1", 10, 1, null, null));
            allItems.Add(new CItemTest("test2", 10, 2, null, null));
            var sb = new SmartBag<CItemTest>(allItems);
            sb.Items[0].Use();
            sb.Items[1].Use();
            Assert.True(sb.Items[0].IsLocked());
            Assert.True(sb.Items[1].IsLocked());
            sb.TickAll();
            Assert.False(sb.Items[0].IsLocked());
            Assert.True(sb.Items[1].IsLocked());
            sb.TickAll();
            Assert.False(sb.Items[1].IsLocked());
        }

        [Test]
        public void SmartBag_FilterOnState_UnusedReqs()
        {
            // Sample data
            var reqs = Condition.Group("test1");
            var allItems = new List<CItemTest>();
            allItems.Add(new CItemTest("test1", 10, 1, reqs, null));
            allItems.Add(new CItemTest("test2", 10, 2, null, null));
            var sb = new SmartBag<CItemTest>(allItems);
            var state = Condition.Group("test3");
            Assert.AreEqual(1, sb.FilterOnState(state).Count);
        }

        [Test]
        public void SmartBag_FilterOnState_OneReqSatisfied()
        {
            // Sample data
            var reqs = Condition.Group("test1");
            var allItems = new List<CItemTest>();
            allItems.Add(new CItemTest("test1", 10, 1, reqs, null));
            allItems.Add(new CItemTest("test2", 10, 2, null, null));
            var sb = new SmartBag<CItemTest>(allItems);
            var query = new List<Condition>();
            var state = Condition.Group("test1");
            Assert.AreEqual(sb.FilterOnState(state).Count, 2);
        }

        [Test]
        public void SmartBag_FilterOnState_NoState()
        {
            // Sample data
            var reqs = Condition.Group("test1");
            var allItems = new List<CItemTest>();
            allItems.Add(new CItemTest("test1", 10, 1, reqs, null));
            allItems.Add(new CItemTest("test2", 10, 2, null, null));
            var sb = new SmartBag<CItemTest>(allItems);
            var query = new List<Condition>();
            var state = Condition.Group();
            Assert.AreEqual(sb.FilterOnState(query).Count, 1);
        }

        [Test]
        public void SmartBag_FilterOnState_AllReqs()
        {
            // Sample data
            var reqs = Condition.Group("test1");
            var allItems = new List<CItemTest>();
            allItems.Add(new CItemTest("test1", 10, 1, reqs, null));
            allItems.Add(new CItemTest("test2", 10, 2, reqs, null));
            var sb = new SmartBag<CItemTest>(allItems);
            var state = Condition.Group("test1", "!test2");
            Assert.AreEqual(sb.FilterOnState(state).Count, 2);
        }


        [Test]
        public void SmartBag_FindRandom_ZeroedWeights()
        {
            // Sample data
            var allItems = new List<CItemTest>();
            allItems.Add(new CItemTest("item1", 0, 1, null, null));
            allItems.Add(new CItemTest("item2", 0, 2, null, null));
            allItems.Add(new CItemTest("item3", 100, 2, null, null));
            var sb = new SmartBag<CItemTest>(allItems);
            var query = Condition.Group("cond1", "cond3");
            Assert.True(sb.FilterOnState(query).Count == 3);
            Assert.True(sb.FindRandom(query).Id == "item3");
        }

        [Test]
        public void SmartBag_FindRandom_TwoReqs()
        {
            // Sample data
            var allItems = new List<CItemTest>();
            allItems.Add(new CItemTest("item1", 10, 1, Condition.Group(), Condition.Group("!cond1")));
            allItems.Add(new CItemTest("item2", 10, 2, Condition.Group("cond2"), null));
            allItems.Add(new CItemTest("item3", 10, 2, Condition.Group("cond1", "cond3"), null));
            var sb = new SmartBag<CItemTest>(allItems);
            var query = Condition.Group("cond1", "cond3");
            Assert.False(sb.FindRandom(query).Id == "item2");
            Assert.False(sb.FindRandom(query).Id == "item2");
            Assert.False(sb.FindRandom(query).Id == "item2");
        }

        [Test]
        public void SmartBag_FindRandom_OneReq()
        {
            // Sample data
            var allItems = new List<CItemTest>();
            allItems.Add(new CItemTest("item1", 10, 1, Condition.Group("cond1"), null));
            allItems.Add(new CItemTest("item2", 10, 2, Condition.Group("cond2"), null));
            var sb = new SmartBag<CItemTest>(allItems);
            var query = Condition.Group("cond1");
            Assert.AreEqual(sb.FindRandom(query).Id, "item1");
        }

        [Test]
        public void SmartBag_FindRandom_Proportions()
        {
            // Sample data
            var allItems = new List<CItemTest>();
            allItems.Add(new CItemTest("item1", 5, 1, null, null));
            allItems.Add(new CItemTest("item2", 20, 2, null, null));
            allItems.Add(new CItemTest("item3", 75, 2, null, null));
            var sb = new SmartBag<CItemTest>(allItems);
            var query = new List<Condition>();

            int c1 = 0, c2 = 0, c3 = 0;
            var draw = sb.FindRandom(query);
            for (int i = 0; i < 100; i++)
            {
                draw = sb.FindRandom(query);
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
            Debug.Log("FindRandom_Proportions test: " + c1 + " / " + c2 + " / " + c3);
            Assert.True(c1 < c2 && c2 < c3);
        }

        [Test]
        public void SmartBag_Use_NotFound()
        {
            // Sample data
            var allItems = new List<CItemTest>();
            allItems.Add(new CItemTest("item1", 5, 1, null, null));
            allItems.Add(new CItemTest("item2", 20, 2, null, null));
            var sb = new SmartBag<CItemTest>(allItems);
            Assert.Null(sb.Use("item5"));
        }

        [Test]
        public void SmartBag_Use_IdFoundAndTick()
        {
            // Sample data
            var allItems = new List<CItemTest>();
            allItems.Add(new CItemTest("item1", 5, 2, null, null));
            allItems.Add(new CItemTest("item2", 20, 4, null, null));
            var sb = new SmartBag<CItemTest>(allItems);
            var pick = sb.Use("item1");
            Assert.True(pick.Id == "item1");
            sb.TickAll();
            Assert.True(pick.IsLocked());
            sb.TickAll();
            Assert.False(pick.IsLocked());
        }

        [Test]
        public void SmartBag_Use_ForcePick()
        {
            // Sample data
            var allItems = new List<CItemTest>();
            allItems.Add(new CItemTest("item1", 5, 2, null, null));
            allItems.Add(new CItemTest("item2", 20, 4, null, null));
            var sb = new SmartBag<CItemTest>(allItems);
            var pick = sb.Use("item1");
            Assert.True(pick.Id == "item1");
            Assert.True(pick.IsLocked());
            pick = sb.Use("item1", true);
            Assert.True(pick.Id == "item1");
        }

        [Test]
        public void SmartBag_Use_IdFoundAndStateChanges()
        {
            // Sample data
            var allItems = new List<CItemTest>();
            allItems.Add(new CItemTest("item1", 5, 2, null, Condition.Group("post1")));
            allItems.Add(new CItemTest("item2", 20, 4, null, null));
            var sb = new SmartBag<CItemTest>(allItems);
            Assert.Null(sb.GetCondition("post1"));
            var pick = sb.Use("item1");
            Assert.True(sb.GetCondition("post1").Status);
        }

        [Test]
        public void SmartBag_UseRandom()
        {
            // Sample data
            var allItems = new List<CItemTest>();
            allItems.Add(new CItemTest("item1", 5, 4, null, new List<Condition>(){ new Condition("post1") }));
            allItems.Add(new CItemTest("item2", 20, 4, null, null));
            var sb = new SmartBag<CItemTest>(allItems);
            var pick1 = sb.UseRandom(null);
            var pick2 = sb.UseRandom(null);
            var pick3 = sb.UseRandom(null);
            Assert.True(pick1.Id != pick2.Id);
            Assert.Null(pick3);
        }

        [Test]
        public void SmartBag_UnlockAll()
        {
            // Sample data
            var allItems = new List<CItemTest>();
            allItems.Add(new CItemTest("item1", 5, 2, null, null));
            allItems.Add(new CItemTest("item2", 20, 2, null, null));
            var sb = new SmartBag<CItemTest>(allItems);
            var p1 = sb.Use("item1");
            var p2 = sb.Use("item2");
            Assert.True(p1.IsLocked());
            Assert.True(p1.IsLocked());
            sb.UnlockAll();
            Assert.False(p1.IsLocked());
            Assert.False(p2.IsLocked());
        }

    }

    public class Condition_Tests
    {
        [Test]
        public void Condition_Parse_True()
        {
            Condition cond = Condition.Parse("teste");
            Assert.True(cond.Status);
        }

        [Test]
        public void Condition_Parse_False()
        {
            Condition cond = Condition.Parse("!teste");
            Assert.False(cond.Status);
        }

        [Test]
        public void Condition_Constructor_Parse()
        {
            var cond = new Condition("!teste");
            Assert.True(cond.Status == false && cond.Id == "teste");
        }

        [Test]
        public void Condition_Check_DiffIds()
        {
            var cond1 = new Condition("teste1");
            var cond2 = new Condition("teste2");
            Assert.False(Condition.Check(cond1, cond2));
        }

        [Test]
        public void Condition_Check_DiffValues()
        {
            var cond1 = new Condition("teste");
            var cond2 = new Condition("!teste");
            Assert.False(Condition.Check(cond1, cond2));
        }


        [Test]
        public void Condition_Group()
        {
            //Arrange
            var g = Condition.Group();
            Assert.AreEqual(0, g.Count);
            g = Condition.Group("test1", "test2");
            Assert.AreEqual(2, g.Count);
        }

        [Test]
        public void Condition_CheckReqs_1to1Query()
        {
            List<Condition> reqs = Condition.Group("test1");
            List<Condition> state = Condition.Group("test1");
            Assert.True(Condition.CheckRequirements(reqs, state));
            state[0] = Condition.Parse("!test1");
            Assert.False(Condition.CheckRequirements(reqs, state));
        }

        [Test]
        public void Condition_CheckReqs_Nulls()
        {
            List<Condition> reqs = null;
            List<Condition> state = Condition.Group("test1");
            Assert.True(Condition.CheckRequirements(reqs, state));
            reqs = Condition.Group("test1");
            state = null;
            Assert.False(Condition.CheckRequirements(reqs, state));
        }

        [Test]
        public void Condition_CheckReqs_2to1Query()
        {
            List<Condition> reqs = Condition.Group("test1", "test2");
            List<Condition> state = Condition.Group("test1", "!test2", "test3");
            Assert.False(Condition.CheckRequirements(reqs, state));
            reqs = Condition.Group("test1", "!test2");
            Assert.True(Condition.CheckRequirements(reqs, state));
        }
    }

    public class ConditionedItem_Tests
    {

        [Test]
        public void ConditionedItem_Constructor()
        {
            var item = new CItemTest("test", 10, 2, null, null);
            Assert.False(item.IsLocked());
            Assert.True(item.Id == "test");
            Assert.AreEqual(0, item.Requirements.Count);
        }

        [Test]
        public void ConditionedItem_Use_LockedAfter()
        {
            var item = new CItemTest("teste", 10, 2, null, null);
            var effects = item.Use();
            Assert.True(item.IsLocked());
        }

        [Test]
        public void ConditionedItem_Use_GetEffects()
        {
            var reqs = Condition.Group("test1", "test2");
            var post = Condition.Group("test1", "!test2");
            var item = new CItemTest("teste", 10, 2, null, post);
            var afterEffects = item.Use();
            Assert.False(Condition.CheckRequirements(reqs, afterEffects));
        }

        [Test]
        public void ConditionedItem_Ticks()
        {
            var item = new CItemTest("teste", 10, 2, null, null);
            item.Use();
            item.Tick();
            Assert.True(item.IsLocked());
            item.Tick();
            Assert.False(item.IsLocked());
        }
    }
}


