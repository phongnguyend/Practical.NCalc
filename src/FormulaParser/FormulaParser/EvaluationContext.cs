namespace FormulaParser;

public class EvaluationContext
{
    public Dictionary<string, object> Variables { get; } = new();

    private readonly Dictionary<string, Func<object[], object>> _functions = new();

    public EvaluationContext()
    {
        RegisterBuiltInFunctions();
    }

    public void RegisterFunction(string name, Func<object[], object> impl)
    {
        _functions[name.ToUpperInvariant()] = impl;
    }

    public object GetVariable(string name)
    {
        return Variables.TryGetValue(name, out var value)
            ? value
            : throw new Exception($"Variable '{name}' not found.");
    }

    public object CallFunction(string name, object[] args)
    {
        if (_functions.TryGetValue(name.ToUpperInvariant(), out var func))
        {
            return func(args);
        }

        throw new Exception($"Function '{name}' not defined.");
    }

    private void RegisterBuiltInFunctions()
    {
        RegisterFunction("IF", args =>
        {
            if (args.Length != 3) throw new ArgumentException("IF requires 3 arguments.");
            return Convert.ToBoolean(args[0]) ? args[1] : args[2];
        });

        RegisterFunction("ISBLANK", args => args[0] == null);

        RegisterFunction("NOT", args =>
        {
            if (args.Length != 1) throw new ArgumentException("NOT requires 1 argument.");
            return !Convert.ToBoolean(args[0]);
        });

        RegisterFunction("AND", args => args.All(Convert.ToBoolean));

        RegisterFunction("OR", args => args.Any(Convert.ToBoolean));

        RegisterFunction("DATE", args =>
        {
            if (args.Length != 3) throw new ArgumentException("DATE requires 3 arguments.");
            return new DateTime(
                Convert.ToInt32(args[0]),
                Convert.ToInt32(args[1]),
                Convert.ToInt32(args[2])
            );
        });

        RegisterFunction("LEN", args => args[0]?.ToString().Length ?? 0);

        RegisterFunction("UPPER", args => args[0]?.ToString().ToUpperInvariant());

        RegisterFunction("LOWER", args => args[0]?.ToString().ToLowerInvariant());

        RegisterFunction("TRIM", args => args[0]?.ToString().Trim());

        RegisterFunction("CONTAINS", args =>
        {
            if (args.Length != 2) throw new ArgumentException("CONTAINS requires 2 arguments.");
            return args[0]?.ToString().Contains(args[1]?.ToString() ?? "", StringComparison.OrdinalIgnoreCase) ?? false;
        });

        RegisterFunction("NOW", args =>
        {
            if (args.Length != 0) throw new ArgumentException("NOW requires no arguments.");
            return DateTime.Now;
        });
    }

}
