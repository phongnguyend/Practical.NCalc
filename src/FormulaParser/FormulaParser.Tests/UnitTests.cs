namespace FormulaParser.Tests;

public class UnitTests
{
    [Fact]
    public void Test1()
    {
        string formula = "IF(And(NOT(ISBLANK(ReceivedDate)), ReceivedDate >= Date(2017,1,1), ReceivedDate <= Date(2018,12,31)), Total * 0.1, Total * VAT)";

        var tokenizer = new Tokenizer(formula);
        var tokens = tokenizer.Tokenize();

        var parser = new Parser(tokens);
        Expr tree = parser.ParseExpression();

        var context = new EvaluationContext();
        context.Variables["ReceivedDate"] = new DateTime(2018, 5, 10);
        context.Variables["Total"] = 1000.0;
        context.Variables["VAT"] = 0.2;

        var result = tree.Evaluate(context);

        Assert.Equal(100.0, result);
    }

    [Fact]
    public void Test2()
    {
        string formula = "IF(And(NOT(ISBLANK(ReceivedDate)), ReceivedDate >= Date(2017,1,1), ReceivedDate <= Date(2018,12,31)), Total * (0.1 + 2) + 3, (Total + 3) * VAT)";

        var tokenizer = new Tokenizer(formula);
        var tokens = tokenizer.Tokenize();

        var parser = new Parser(tokens);
        Expr tree = parser.ParseExpression();

        var context = new EvaluationContext();
        context.Variables["ReceivedDate"] = new DateTime(2018, 5, 10);
        context.Variables["Total"] = 1000.0;
        context.Variables["VAT"] = 0.2;

        var result = tree.Evaluate(context);

        Assert.Equal(2103.0, result);
    }

    [Fact]
    public void Test3()
    {
        string formula = "IF(ISBLANK(Name), \"No name\", \"Hello \" + Name)";
        var tokenizer = new Tokenizer(formula);
        var parser = new Parser(tokenizer.Tokenize());
        Expr expr = parser.ParseExpression();

        var context = new EvaluationContext();
        context.Variables["Name"] = "Phong";

        var result = expr.Evaluate(context);

        Assert.Equal("Hello Phong", result);
    }
}