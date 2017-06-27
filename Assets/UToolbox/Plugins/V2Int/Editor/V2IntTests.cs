/* Enric Llagostera <http://enric.llagostera.com.br> */

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UToolbox.V2IntSystem;
using NUnit.Framework;

namespace UToolbox.Tests
{
    public class V2Int_Tests
    {
        [Test]
        public void V2Int_Constructor()
        {
            V2Int n = new V2Int(1, 1);
            Assert.AreEqual(1, n.x);
            Assert.AreEqual(1, n.y);
        }

        [Test]
        public void V2Int_ToV2()
        {
            V2Int n = new V2Int(1, 1);
            var v2 = n.ToV2();
            Assert.AreEqual(1, v2.x);
            Assert.AreEqual(1, v2.y);
        }

        [Test]
        public void V2Int_FromV2()
        {
            var v2 = new Vector2(1, 1);
            V2Int n = V2Int.FromV2(v2);
            Assert.AreEqual(n.x, v2.x);
            Assert.AreEqual(n.y, v2.y);
        }

        [Test]
        public void V2Int_ClampSelf()
        {
            var v2 = new V2Int(-5, 5);
            v2.ClampSelf(0, 4, 0, 4);
            Assert.AreEqual(0, v2.x);
            Assert.AreEqual(4, v2.y);
            v2 = new V2Int(5, -5);
            v2.ClampSelf(0, 4, 0, 4);
            Assert.AreEqual(4, v2.x);
            Assert.AreEqual(0, v2.y);
        }

        [Test]
        public void V2Int_IsValid()
        {
            var v2 = new V2Int(-5, 5);
            Assert.False(v2.IsValid(0, 2, 0, 2));
            Assert.True(v2.IsValid(-10, 10, -10, 10));
        }

        [Test]
        public void V2Int_Equals()
        {
            var a = new V2Int(-5, 5);
            var b = new V2Int(-5, 5);
            Assert.True(a == b);
            b = new V2Int(5, 5);
            Assert.True(a != b);
        }

        [Test]
        public void V2Int_AtDirection()
        {
            var a = new V2Int(0, 0);
            Assert.AreEqual(1, V2Int.AtDirection(a, EDirection.N).y);
            Assert.AreEqual(1, V2Int.AtDirection(a, EDirection.E).x);
            Assert.AreEqual(-1, V2Int.AtDirection(a, EDirection.W).x);
            Assert.AreEqual(-1, V2Int.AtDirection(a, EDirection.S).y);
            Assert.AreEqual(new V2Int(1, 1), V2Int.AtDirection(a, EDirection.NE));
            Assert.AreEqual(new V2Int(1, -1), V2Int.AtDirection(a, EDirection.SE));
            Assert.AreEqual(new V2Int(-1, -1), V2Int.AtDirection(a, EDirection.SW));
            Assert.AreEqual(new V2Int(-1, 1), V2Int.AtDirection(a, EDirection.NW));
        }
    }
}