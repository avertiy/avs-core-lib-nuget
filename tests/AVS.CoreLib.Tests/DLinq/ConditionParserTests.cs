using AVS.CoreLib.DLinq;
using AVS.CoreLib.DLinq.Conditions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AVS.CoreLib.Tests.DLinq;

[TestClass]
public class ConditionParserTests
{
    [DataTestMethod]
    [DataRow(1, "A > 1", "A > 1")]
    [DataRow(2, "(A > 1 OR B > 2)", "(A > 1 OR B > 2)")]
    [DataRow(3, "(A > 1 AND B > 2)", "(A > 1 AND B > 2)")]
    [DataRow(4, "(A > 1 AND B > 2) OR (C < 4)", "((A > 1 AND B > 2) OR C < 4)")]
    [DataRow(5, "A > 1 AND (B > 2 OR C < 4)", "(A > 1 AND (B > 2 OR C < 4))")]
    [DataRow(6, "(A > 1 OR (B < 1 AND C ==2)) OR (B < 2)", "((A > 1 OR (B < 1 AND C ==2)) OR B < 2)")]
    [DataRow(7, "(A > 1 AND B < 2) OR (A < 1 AND B > 20)", "((A > 1 AND B < 2) OR (A < 1 AND B > 20))")]
    [DataRow(8, "(A > 1 AND (B < 2 OR C > 3)) OR A > 10", "((A > 1 AND (B < 2 OR C > 3)) OR A > 10)")]
    public void Parser_Should_Parse_Expressions(int i, string input, string expectedResult)
    {
        // Act
        var condition = ConditionParser.Parse(input);
        var result = condition.ToString();

        // Assert
        Assert.AreEqual(expectedResult, result, $"DataRow[{i}]");
    }

    [DataTestMethod]
    [DataRow(1, "(bag[\"SMA(21)\"] > 1 OR bag[SMA(12)] > 2)", "(bag[\"SMA(21)\"] > 1 OR bag[SMA(12)] > 2)")]
    public void Parser_Should_Parse_Expressions_With_Brackets_Inside_Key(int i, string input, string expectedResult)
    {
        // Act
        var condition = ConditionParser.Parse(input);
        var result = condition.ToString();

        // Assert
        Assert.AreEqual(expectedResult, result, $"DataRow[{i}]");
    }

    [DataTestMethod]
    [DataRow(1, "(A > 1", "Closing bracket `)` is missing")]
    [DataRow(2, "((A > 1)", "Closing bracket `)` is missing")]
    [DataRow(3, "A > 1)", "Opening bracket `(` is missing")]
    [DataRow(4, "(A > 1))", "Opening bracket `(` is missing")]
    public void Parser_Should_Throw_Exception(int i, string input, string message)
    {
        var exception = Assert.ThrowsException<InvalidExpression>(() => ConditionParser.Parse(input));

        // Assert
        exception.Message.Should().StartWith(message);
    }

    [DataTestMethod]
    [DataRow(1, "(A > 1) AND AND", "A > 1")]
    [DataRow(2, "OR (A > 1) AND", "A > 1")]
    [DataRow(3, "OR (A > 1) AND ()", "A > 1")]
    public void Parser_Should_Ignore_Incomplete_Expression_Parts(int i, string input, string expected)
    {
        // Act
        var condition = ConditionParser.Parse(input);
        var result = condition.ToString();

        // Assert
        Assert.AreEqual(expected, result, $"DataRow[{i}]");
    }
}
