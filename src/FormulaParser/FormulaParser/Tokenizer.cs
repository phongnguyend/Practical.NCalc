public class Tokenizer
{
    private readonly string _text;
    private int _pos;

    public Tokenizer(string text) => _text = text;

    public List<Token> Tokenize()
    {
        var tokens = new List<Token>();

        while (_pos < _text.Length)
        {
            char c = _text[_pos];

            if (char.IsWhiteSpace(c)) { _pos++; continue; }

            if (char.IsLetter(c))
            {
                var start = _pos;
                while (_pos < _text.Length && (char.IsLetterOrDigit(_text[_pos]) || _text[_pos] == '_'))
                    _pos++;
                tokens.Add(new Token(TokenType.Identifier, _text[start.._pos]));
            }
            else if (char.IsDigit(c) || c == '.')
            {
                var start = _pos;
                while (_pos < _text.Length && (char.IsDigit(_text[_pos]) || _text[_pos] == '.'))
                    _pos++;
                tokens.Add(new Token(TokenType.Number, _text[start.._pos]));
            }
            else
            {
                switch (c)
                {
                    case ',': tokens.Add(new Token(TokenType.Comma, ",")); break;
                    case '(': tokens.Add(new Token(TokenType.OpenParen, "(")); break;
                    case ')': tokens.Add(new Token(TokenType.CloseParen, ")")); break;
                    case '>':
                    case '<':
                    case '=':
                    case '!':
                        string op = c.ToString();
                        if (_pos + 1 < _text.Length && _text[_pos + 1] == '=')
                        {
                            op += "=";
                            _pos++;
                        }
                        tokens.Add(new Token(TokenType.Operator, op));
                        break;

                    case '*':
                    case '+':
                    case '-':
                    case '/':
                        tokens.Add(new Token(TokenType.Operator, c.ToString()));
                        break;

                    default:
                        throw new Exception($"Invalid character: {c}");
                }
                _pos++;
            }
        }

        tokens.Add(new Token(TokenType.EOF, ""));
        return tokens;
    }
}