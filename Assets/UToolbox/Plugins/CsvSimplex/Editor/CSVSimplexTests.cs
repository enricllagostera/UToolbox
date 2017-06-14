/* Enric Llagostera <http://enric.llagostera.com.br> */

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UToolbox.CsvSimplexSystem;
using NUnit.Framework;

namespace UToolbox.Tests
{
    public class CsvSimplex_Tests
    {
        private class Info {
            public string id;
            public float weight;
        }

        [Test]
        public void CsvSimplex_BasicRead()
        {
            // Empty constructor
            var csv = new CsvSimplex<Info>();
            //LOG Debug.Log(Application.dataPath + "/UToolbox/Examples/ExampleCSV.csv");
            var list = csv.ReadList(Application.dataPath + "/UToolbox/Plugins/CsvSimplex/Editor/TestCSV.csv");
            //LOG Debug.Log(list[0].id + " : " + list[0].weight);
            Assert.AreEqual(3, list.Count);
        }
    }
}