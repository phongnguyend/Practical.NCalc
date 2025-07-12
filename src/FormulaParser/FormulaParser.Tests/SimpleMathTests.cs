namespace FormulaParser.Tests;

public class SimpleMathTests
{
    [Theory]
    [InlineData("1 + 2 + 3 + 4 + 5", 1 + 2 + 3 + 4 + 5)]
    [InlineData("1 - 2 - 3 - 4 - 5", 1 - 2 - 3 - 4 - 5)]
    [InlineData("1 * 2 * 3 * 4 * 5", 1 * 2 * 3 * 4 * 5)]
    [InlineData("1 / 2 / 3 / 4 / 5", (double)1 / 2 / 3 / 4 / 5)]
    public void Simple(string formula, double expected)
    {
        var tokenizer = new Tokenizer(formula);
        var tokens = tokenizer.Tokenize();

        var parser = new Parser(tokens);
        Expr tree = parser.ParseExpression();

        var context = new Dictionary<string, object>
        {
        };

        var result = tree.Evaluate(context);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("(1 + 2) * (3 + 4) - 5", (1 + 2) * (3 + 4) - 5)]
    [InlineData("1 + 2 - 3 * 4 / 5", 1 + 2 - 3 * 4 / (double)5)]
    [InlineData("((1 + 2) + 3) * (4 - 5)", ((1 + 2) + 3) * (4 - 5))]
    [InlineData("1 * (2 + 3) / (4 + 5)", 1 * (2 + 3) / (double)(4 + 5))]
    [InlineData("(1 + 2 + 3 + 4 + 5)", (1 + 2 + 3 + 4 + 5))]
    [InlineData("1 + (2 * 3) - (4 / 5)", 1 + (2 * 3) - (4 / (double)5))]
    [InlineData("((1 + 2) * 3) + (4 - 5)", ((1 + 2) * 3) + (4 - 5))]
    [InlineData("1 + 2 + 3 - 4 * 5", 1 + 2 + 3 - 4 * 5)]
    [InlineData("(1 + 2 + 3) / (4 + 5)", (1 + 2 + 3) / (double)(4 + 5))]
    [InlineData("(1 * 2) + (3 / (4 - 5))", (1 * 2) + (3 / (double)(4 - 5)))]
    public void Complex(string formula, double expected)
    {
        var tokenizer = new Tokenizer(formula);
        var tokens = tokenizer.Tokenize();

        var parser = new Parser(tokens);
        Expr tree = parser.ParseExpression();

        var context = new Dictionary<string, object>
        {
        };

        var result = tree.Evaluate(context);

        Assert.Equal(expected, result);
    }
}
