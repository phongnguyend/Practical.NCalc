using NCalc;
using NCalc.Exceptions;
using NCalc.Handlers;

namespace Practical.NCalc.Tests;

public class UnitTests
{
    [Fact]
    public void Test1()
    {
        var e = new Expression("if (And(NOT(ISBLANK(ReceivedDate)), ReceivedDate >= Date(2017,01,01), ReceivedDate <= Date(2018,12,31)), Total * 0.1, Total * VAT)");

        e.Parameters["ReceivedDate"] = DateTime.Parse("2018/01/01");
        e.Parameters["VAT"] = (decimal)12 / 100;
        e.Parameters["Total"] = 1000;

        e.EvaluateFunction += delegate (string name, FunctionArgs args)
        {
            var operatorName = name.ToUpper();
            if (operatorName == "DATE")
            {
                args.Result = new DateTime((int)args.Parameters[0].Evaluate(), (int)args.Parameters[1].Evaluate(), (int)args.Parameters[2].Evaluate());
            }
            if (operatorName == "AND")
            {
                args.Result = args.Parameters.All(x => (bool)x.Evaluate());
            }
            if (operatorName == "ISBLANK")
            {
                var temp = args.Parameters[0].Evaluate();
                args.Result = temp == null || string.IsNullOrWhiteSpace(temp.ToString());
            }
            if (operatorName == "NOT")
            {
                args.Result = !(bool)args.Parameters[0].Evaluate();
            }
        };

        var rs = e.Evaluate().ToString();

        Assert.Equal(100.ToString(), rs);
    }

    [Fact]
    public void Test2()
    {
        var e = new Expression("if(PostCode = UPPERCASE('A1'), Total * 0.1, if(PostCode = 'A2', Total * 0.2, -1))");

        e.Parameters["PostCode"] = "A1";
        e.Parameters["Total"] = 1000;

        e.EvaluateFunction += delegate (string name, FunctionArgs args)
        {
            var operatorName = name.ToUpper();
            if (operatorName == "UPPERCASE")
            {
                args.Result = args.Parameters[0].Evaluate().ToString().ToUpper();
            }
        };

        var rs = e.Evaluate().ToString();

        Assert.Equal(100.ToString(), rs);
    }

    [Fact]
    public void Test3()
    {
        var e = new Expression("Total * 0.5");

        e.Parameters["Total"] = 1000;

        e.EvaluateFunction += delegate (string name, FunctionArgs args)
        {
            var operatorName = name.ToUpper();
            if (operatorName == "UPPERCASE")
            {
                args.Result = args.Parameters[0].Evaluate().ToString().ToUpper();
            }
        };

        var rs = e.Evaluate().ToString();

        Assert.Equal(500.ToString(), rs);
    }

    [Fact]
    public void Test4()
    {
        var e = new Expression("Total * 50%"); // not support

        e.Parameters["Total"] = 1000;

        e.EvaluateFunction += delegate (string name, FunctionArgs args)
        {
            var operatorName = name.ToUpper();
            if (operatorName == "UPPERCASE")
            {
                args.Result = args.Parameters[0].Evaluate().ToString().ToUpper();
            }
        };

        Assert.Throws<NCalcParserException>(() => e.Evaluate());
    }
}