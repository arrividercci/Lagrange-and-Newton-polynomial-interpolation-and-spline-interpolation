using Numerical_Methods_Lab_3;
using System.Diagnostics;
using System.Runtime.InteropServices;

internal class Program
{
    
    //функція
    private static double Function(double x)
    {
        return 3 * x + Math.Cos(x) + 1;
    }
    
    //нулі чебишова
    private static List<double> ChebeshovPoints(double a, double b, int numberOfNodes)
    {
        int n = numberOfNodes - 1;
        double x;
        List<double> points = new List<double>();
        for(int k = 0; k <= n; k++)
        {
            x = ((a + b) / 2) + ((b - a) / 2) * (Math.Cos((Math.PI * (2 * k + 1)) / (2 * (n + 1))));
            points.Add(x);
        }
        return points;
    }
    
    //прінтить всі вузли
    private static void PrintAllNodes(List<double> points, List<double> functionInPoints)
    {
        for(int i = 0; i < points.Count; i++)
        {
            Console.WriteLine($"X{i} = {points[i]}\tF(X{i}) = {functionInPoints[i]}");
        }
    }
    
    private static Polynomial GetLagrangePolynomial(List<double> x, List<double> y, int nodes) 
    {
        Polynomial resPolynomial = new Polynomial();

        for(int i = 0; i < x.Count; i++)
        {
            double devider = 1;
            for(int j = 0; j < x.Count; j++)
            {
                if (i != j)
                {
                    devider *= (x[i] - x[j]);
                }
            }
            Polynomial polynomial = new Polynomial();
            polynomial.AddPolynomialMember(1, 0);
            for(int k = 0; k < x.Count; k++)
            {
                if (k != i) 
                {
                    var testPolynomial = new Polynomial();
                    testPolynomial.AddPolynomialMember(1,1);
                    testPolynomial.AddPolynomialMember(-x[k],0);
                    polynomial = polynomial.Multiply(testPolynomial);
                }
            }
            for(int l = 0; l < polynomial.polynomialMembers.Count; l++)
            {
                polynomial.polynomialMembers[l].MultiplyCof(y[i] / devider);
            }
            resPolynomial.Add(polynomial);
        }
        resPolynomial.RefactorAllPolynomialMembers(nodes);
        resPolynomial.SortByDegree();
        return resPolynomial;
    }
    
    private static void ShowPolynomial(Polynomial polynomial)
    {
        Console.Write("F(x)=");
        Console.Write($"{polynomial.polynomialMembers[0].Сoefficient.ToString("F15")}*x^{polynomial.polynomialMembers[0].Grade}");
        for (int i = 1; i < polynomial.polynomialMembers.Count; i++)
        {
            Console.Write($"+{polynomial.polynomialMembers[i].Сoefficient.ToString("F15")}*x^{polynomial.polynomialMembers[i].Grade}");
        }
        Console.WriteLine();
    }
    private static double DividedDifference(double f1, double f2, double x1, double x2)
    {
        return (f2 - f1) / (x2 - x1);
    }
    // return (Function(x2) - Function(x1)) / (x2 - x1);
    private static List<List<double>> CountDividedDifferences(List<double> x, int nodesCount)
    {
        List<List<double>> result = new List<List<double>>();
        List<double> dividedDifference = new List<double>();
        for(int i = 0; i < x.Count - 1; i++)
        {
            dividedDifference.Add(DividedDifference(Function(x[i]), Function(x[i+1]),x[i], x[i+1]));
        }
        result.Add(dividedDifference);
        int k = 2;
        for(int i = 0; i < nodesCount - 2; i++)
        {
            dividedDifference = new List<double>();
            for(int j = 0; j < result[i].Count - 1; j++)
            {
                dividedDifference.Add(DividedDifference(result[i][j], result[i][j + 1], x[j], x[j + k]));
            }
            k++;
            result.Add(dividedDifference);
        }
        return result;
    }
    private static void ShowDividedDifferences(List<List<double>> dividedDifferences)
    {
        int i = 1;
        foreach(var dividedDifference in dividedDifferences)
        {
            Console.Write($"р.р.{i}п. ");
            foreach(var difference in dividedDifference)
            {
                Console.Write($"{difference.ToString("F15")} ");
            }
            Console.WriteLine();
            i++;
        }
    }

    private static Polynomial GetNewtonPolynomial(List<double> x, List<List<double>> dividedDiffirences ,int nodes)
    {
        Polynomial resultPol = new Polynomial();
        resultPol.AddPolynomialMember(Function(x[0]), 0);
        for(int i = 1; i < x.Count; i++)
        {
            Polynomial polynomial = new Polynomial();
            polynomial.AddPolynomialMember(1, 0);
            for (int j = 0; j < i; j++)
            {
                Polynomial testPoltnomial = new Polynomial();
                testPoltnomial.AddPolynomialMember(1, 1);
                testPoltnomial.AddPolynomialMember(-x[j], 0);
                polynomial = polynomial.Multiply(testPoltnomial);
            }
            foreach(var pm in polynomial.polynomialMembers)
            {
                pm.MultiplyCof(dividedDiffirences[i - 1][0]);
            }
            resultPol.Add(polynomial);
        }
        resultPol.RefactorAllPolynomialMembers(nodes);
        resultPol.SortByDegree();
        return resultPol;
    }

    private static void Main(string[] args)
    {
        var nodes = 10;
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var chebishovNulls = ChebeshovPoints(-1, 0, nodes);
        var functionInChebishovNulls = new List<double>();
        foreach(var point in chebishovNulls)
        {
            functionInChebishovNulls.Add(Function(point));
        }
        Console.WriteLine("_____________________________________________________________________________________________________________________________________________________________________________________________________________________________________________");
        Console.WriteLine("Вузли:");
        PrintAllNodes(chebishovNulls, functionInChebishovNulls);
        Console.WriteLine("_____________________________________________________________________________________________________________________________________________________________________________________________________________________________________________");
        Console.WriteLine("Поліном Лагранжа(L(x)):");
        Polynomial lagrangePolynomial = GetLagrangePolynomial(chebishovNulls, functionInChebishovNulls, nodes);
        ShowPolynomial(lagrangePolynomial);
        Console.WriteLine("_____________________________________________________________________________________________________________________________________________________________________________________________________________________________________________");
        var dividedDifferences = CountDividedDifferences(chebishovNulls, nodes);
        Console.WriteLine("Урізані різниці");
        ShowDividedDifferences(dividedDifferences);
        Console.WriteLine("_____________________________________________________________________________________________________________________________________________________________________________________________________________________________________________");
        Console.WriteLine("Поліном Ньютона:");
        var newtonPolynomial = GetNewtonPolynomial(chebishovNulls, dividedDifferences, nodes);
        ShowPolynomial(newtonPolynomial);
        Console.WriteLine("_____________________________________________________________________________________________________________________________________________________________________________________________________________________________________________");
        Console.WriteLine("Невідоме буду шукати оберненою інтерполяцію за поліномом Лагранжа:");
        Console.WriteLine("Поліном Лагранжа(L(y)):");
        Polynomial lagrangePolynomialY = GetLagrangePolynomial(functionInChebishovNulls, chebishovNulls,  nodes);
        ShowPolynomial(lagrangePolynomialY);
        Console.WriteLine("Рівняння L(0) = x");
        Console.WriteLine($"x = {lagrangePolynomialY.polynomialMembers.Where(pm => pm.Grade == 0).Select(pm => pm.Сoefficient).FirstOrDefault()}");
    }

}