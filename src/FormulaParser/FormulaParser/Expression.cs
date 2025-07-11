namespace FormulaParser;

public abstract class Expr
{
    public abstract object Evaluate(Dictionary<string, object> context);

    public abstract override string ToString();
}

public class NumberExpr : Expr
{
    public double Value;

    public NumberExpr(double value) => Value = value;

    public override object Evaluate(Dictionary<string, object> context) => Value;

    public override string ToString() => Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
}

public class VariableExpr : Expr
{
    public string Name;

    public VariableExpr(string name) => Name = name;

    public override object Evaluate(Dictionary<string, object> context) =>
        context.TryGetValue(Name, out var value) ? value : throw new Exception($"Variable '{Name}' not found.");

    public override string ToString() => Name;
}

public class BinaryExpr : Expr
{
    public string Operator;

    public Expr Left, Right;

    public BinaryExpr(string op, Expr left, Expr right)
    {
        Operator = op;
        Left = left;
        Right = right;
    }

    public override object Evaluate(Dictionary<string, object> context)
    {
        var left = Left.Evaluate(context);
        var right = Right.Evaluate(context);

        return Operator switch
        {
            "*" => Convert.ToDouble(left) * Convert.ToDouble(right),
            "+" => Convert.ToDouble(left) + Convert.ToDouble(right),
            "-" => Convert.ToDouble(left) - Convert.ToDouble(right),
            "/" => Convert.ToDouble(left) / Convert.ToDouble(right),
            ">=" => Compare(left, right) >= 0,
            "<=" => Compare(left, right) <= 0,
            ">" => Compare(left, right) > 0,
            "<" => Compare(left, right) < 0,
            "==" => Equals(left, right),
            "!=" => !Equals(left, right),
            _ => throw new Exception($"Unsupported operator {Operator}")
        };
    }

    private static int Compare(object left, object right)
    {
        if (left is IComparable l && right is IComparable r)
            return l.CompareTo(r);
        throw new Exception($"Cannot compare values: {left} and {right}");
    }

    public override string ToString() => $"({Left} {Operator} {Right})";
}


public class FunctionExpr : Expr
{
    public string Name;
    public List<Expr> Args;

    public FunctionExpr(string name, List<Expr> args)
    {
        Name = name.ToUpperInvariant();
        Args = args;
    }

    public override object Evaluate(Dictionary<string, object> context)
    {
        switch (Name)
        {
            case "IF":
                if (Args.Count != 3) throw new Exception("IF requires 3 arguments.");
                return Convert.ToBoolean(Args[0].Evaluate(context))
                    ? Args[1].Evaluate(context)
                    : Args[2].Evaluate(context);

            case "AND":
                foreach (var arg in Args)
                    if (!Convert.ToBoolean(arg.Evaluate(context)))
                        return false;
                return true;

            case "NOT":
                if (Args.Count != 1) throw new Exception("NOT requires 1 argument.");
                return !Convert.ToBoolean(Args[0].Evaluate(context));

            case "ISBLANK":
                if (Args.Count != 1) throw new Exception("ISBLANK requires 1 argument.");
                return Args[0].Evaluate(context) == null;

            case "DATE":
                if (Args.Count != 3) throw new Exception("DATE requires 3 arguments.");
                return new DateTime(
                    Convert.ToInt32(Args[0].Evaluate(context)),
                    Convert.ToInt32(Args[1].Evaluate(context)),
                    Convert.ToInt32(Args[2].Evaluate(context))
                );

            default:
                throw new Exception($"Unknown function: {Name}");
        }
    }

    public override string ToString() =>
        $"{Name}({string.Join(", ", Args)})";
}

