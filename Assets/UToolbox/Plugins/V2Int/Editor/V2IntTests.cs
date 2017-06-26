/* Enric Llagostera <http://enric.llagostera.com.br> */

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UToolbox.SmartBagSystem;
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
    }
}