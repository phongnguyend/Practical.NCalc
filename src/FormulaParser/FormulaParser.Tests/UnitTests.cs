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

        var context = new Dictionary<string, object>
        {
            ["ReceivedDate"] = new DateTime(2018, 5, 10),
            ["Total"] = 1000.0,
            ["VAT"] = 0.2
        };

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

        var context = new Dictionary<string, object>
        {
            ["ReceivedDate"] = new DateTime(2018, 5, 10),
            ["Total"] = 1000.0,
            ["VAT"] = 0.2
        };

        var result = tree.Evaluate(context);

        Assert.Equal(2103.0, result);
    }
}