using Numerical_Methods_Lab_3;
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
        Console.Write($" {polynomial.polynomialMembers[0].Сoefficient.ToString("F15")}*x^{polynomial.polynomialMembers[0].Grade}");
        for (int i = 1; i < polynomial.polynomialMembers.Count; i++)
        {
            Console.Write($" + {polynomial.polynomialMembers[i].Сoefficient.ToString("F15")}*x^{polynomial.polynomialMembers[i].Grade}");
        }
        Console.WriteLine();
    }

    private static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var chebishovNulls = ChebeshovPoints(-1, 0, 10);
        var functionInChebishovNulls = new List<double>();
        foreach(var point in chebishovNulls)
        {
            functionInChebishovNulls.Add(Function(point));
        }
        Console.WriteLine("Вузли:");
        PrintAllNodes(chebishovNulls, functionInChebishovNulls);
        Console.WriteLine("Поліном Лагранжа:");
        Polynomial lagrangePolynomial = GetLagrangePolynomial(chebishovNulls, functionInChebishovNulls, 10);
        ShowPolynomial(lagrangePolynomial);
        Console.WriteLine("Поліном Ньютона:");
    }

}