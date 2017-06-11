using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UToolbox.SmartBag;
using NUnit.Framework;

public class SmartBagTests
{
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
