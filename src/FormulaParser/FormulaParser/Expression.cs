namespace FormulaParser;

public abstract class Expr
{
    public abstract object Evaluate(EvaluationContext context);

    public abstract override string ToString();
}

public class NumberExpr : Expr
{
    public double Value;

    public NumberExpr(double value) => Value = value;

    public override object Evaluate(EvaluationContext context) => Value;

    public override string ToString() => Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
}

public class StringExpr : Expr
{
    public string Value;
    public StringExpr(string value) => Value = value;

    public override object Evaluate(EvaluationContext context) => Value;

    public override string ToString() => $"\"{Value}\"";
}

public class BoolExpr : Expr
{
    public bool Value;
    public BoolExpr(bool value) => Value = value;

    public override object Evaluate(EvaluationContext context) => Value;

    public override string ToString() => Value.ToString().ToLower();
}


public class VariableExpr : Expr
{
    public string Name;

    public VariableExpr(string name) => Name = name;

    public override object Evaluate(EvaluationContext context)
    {
        return context.GetVariable(Name);
    }


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

    public override object Evaluate(EvaluationContext context)
    {
        var left = Left.Evaluate(context);
        var right = Right.Evaluate(context);

        return Operator switch
        {
            "+" => EvaluateAdd(left, right),
            "-" => Convert.ToDouble(left) - Convert.ToDouble(right),
            "*" => Convert.ToDouble(left) * Convert.ToDouble(right),
            "/" => Convert.ToDouble(left) / Convert.ToDouble(right),
            "==" => Equals(left, right),
            "!=" => !Equals(left, right),
            ">" => Compare(left, right) > 0,
            "<" => Compare(left, right) < 0,
            ">=" => Compare(left, right) >= 0,
            "<=" => Compare(left, right) <= 0,
            _ => throw new Exception($"Unsupported operator: {Operator}")
        };
    }

    private static object EvaluateAdd(object left, object right)
    {
        if (left is string || right is string)
        {
            return $"{left}{right}"; // string concat
        }

        return Convert.ToDouble(left) + Convert.ToDouble(right); // numeric add
    }

    private static int Compare(object left, object right)
    {
        if (left == null || right == null)
        {
            throw new Exception("Cannot compare null values.");
        }

        if (left is IComparable l && right is IComparable r)
        {
            // Convert strings and numbers to compatible types
            if (left.GetType() != right.GetType())
            {
                if (left is string || right is string)
                {
                    return string.Compare(left.ToString(), right.ToString(), StringComparison.Ordinal);
                }

                if (left is double || right is double)
                {
                    return Convert.ToDouble(left).CompareTo(Convert.ToDouble(right));
                }

                throw new Exception($"Cannot compare values of different types: {left.GetType()} and {right.GetType()}");
            }

            return l.CompareTo(r);
        }

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

    public override object Evaluate(EvaluationContext context)
    {
        var argValues = Args.Select(arg => arg.Evaluate(context)).ToArray();
        return context.CallFunction(Name, argValues);
    }

    public override string ToString() => $"{Name}({string.Join(", ", Args)})";
}

