namespace FormulaParser;

public class Parser
{
    private readonly List<Token> _tokens;
    private int _pos;

    public Parser(List<Token> tokens) => _tokens = tokens;

    private Token Peek() => _tokens[_pos];

    private Token Advance() => _tokens[_pos++];

    private bool Match(TokenType type)
    {
        if (Peek().Type == type)
        {
            Advance();
            return true;
        }
        return false;
    }

    public Expr ParseExpression()
    {
        return ParseBinaryExpression();
    }

    private Expr ParsePrimary()
    {
        Token token = Peek();

        if (Match(TokenType.OpenParen))
        {
            var inner = ParseExpression();
            if (!Match(TokenType.CloseParen))
            {
                throw new Exception("Expected closing )");
            }

            return inner;
        }

        if (token.Type == TokenType.Identifier)
        {
            string name = token.Value;
            Advance();

            // Function call
            if (Match(TokenType.OpenParen))
            {
                var args = new List<Expr>();
                if (Peek().Type != TokenType.CloseParen)
                {
                    do
                    {
                        args.Add(ParseExpression());
                    }
                    while (Match(TokenType.Comma));
                }

                if (!Match(TokenType.CloseParen))
                {
                    throw new Exception("Expected closing ) after function arguments");
                }

                return new FunctionExpr(name, args);
            }

            // Variable
            return new VariableExpr(name);
        }

        if (token.Type == TokenType.Number)
        {
            Advance();
            return new NumberExpr(double.Parse(token.Value));
        }

        throw new Exception($"Unexpected token: {token}");
    }


    // New: Parse binary expressions like a * b or x >= y
    private Expr ParseBinaryExpression(int parentPrecedence = 0)
    {
        var left = ParsePrimary();

        while (true)
        {
            var opToken = Peek();
            if (opToken.Type != TokenType.Operator)
            {
                break;
            }

            int precedence = GetPrecedence(opToken.Value);
            if (precedence < parentPrecedence)
            {
                break;
            }

            Advance(); // consume operator

            var right = ParseBinaryExpression(precedence + 1);
            left = new BinaryExpr(opToken.Value, left, right);
        }

        return left;
    }

    private int GetPrecedence(string op)
    {
        return op switch
        {
            "*" or "/" => 2,
            "+" or "-" => 1,
            ">" or "<" or ">=" or "<=" or "==" or "!=" => 0,
            _ => -1
        };
    }
}
