using Numerical_Methods_Lab_3;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Xml;

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

    private static double[] FindHi(List<double> x)
    {
        var result = new double[x.Count - 1];
        for(int i = 0; i < x.Count - 1; i++)
        {
            result[i] = x[i+1] - x[i];
        }
        return result;
    }
    public static double[,] FindMatrixH(double[] hiVector)
    {
        double[,] matrix = new double[hiVector.Length - 1, hiVector.Length + 1];
        for (int i = 0; i < hiVector.Length - 1; i++)
        {
            for (int j = 0; j < hiVector.Length + 1; j++)
            {
                if (i == j)
                {
                    matrix[i, j] = 1 / hiVector[i];
                }
                else if (i == j - 1)
                {
                    matrix[i, j] = -((1 / hiVector[i]) + (1 / hiVector[j]));
                }
                else if (i == j - 2)
                {
                    matrix[i, j] = 1 / hiVector[i + 1];
                }
                else
                {
                    matrix[i, j] = 0;
                }
            }
        }
        return matrix;
    }

    public static double[,] FindMatrixA(double[] hiVector)
    {
        double[,] matrix = new double[hiVector.Length - 1, hiVector.Length - 1];
        for (int i = 0; i < hiVector.Length - 1; i++)
        {
            for (int j = 0; j < hiVector.Length - 1; j++)
            {
                if (i == j)
                {
                    matrix[i, j] = (hiVector[i] + hiVector[i + 1] / 3);
                }
                else if (i == j + 1)
                {
                    matrix[i, j] = hiVector[i] / 6;
                }
                else if (i == j - 1)
                {
                    matrix[i, j] = hiVector[j] / 6;
                }
                else
                {
                    matrix[i, j] = 0;
                }
            }
        }
        return matrix;
    }
    private static void ShowVector(double[] x, char c)
    {
        for(int i = 0; i < x.Length; i++)
        {
            Console.Write($"{c}{i + 1} = {x[i]:F3} ");
        }
    }
    private static void ShowMatrix(double[,] matrix)
    {
        for(int i = 0; i < matrix.GetLength(0); i++)
        {
            for(int j = 0; j < matrix.GetLength(1); j++)
            {
                Console.Write($"{matrix[i, j],10:F3}");
            }
            Console.WriteLine();
        }
    }
    private static double[,] MultipleMatrix(double[,] a, double[,] b)
    {
        int size = a.GetLength(0);
        double temp = 0;
        double[,] m = new double[size, size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                temp = 0;
                for (int k = 0; k < size; k++)
                {
                    temp += a[i, k] * b[k, j];
                }
                m[i, j] = temp;
            }
        }
        return m;
    }

    public static double[] MultipleMatrixVector(double[,] matrix, double[] vector)
    {
        double[] vectorRes = new double[vector.Length];
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            vectorRes[i] = 0;
            for (int j = 0; j < matrix.GetLength(0); j++)
            {
                vectorRes[i] += matrix[i, j] * vector[j];
            }
        }
        return vector;
    }

    public static double[] FindRes(double[,] m, double[] b)
    {

        double[] resVector = new double[m.GetLength(0)];
        double[] aVector = new double[m.GetLength(0)];
        double[] bVector = new double[m.GetLength(0)];
        double[] zVector = new double[m.GetLength(0)];

        aVector[1] = -(m[0, 1] / m[0, 0]);
        bVector[1] = b[0] / m[0, 0];
        zVector[1] = -m[1, 1] - aVector[1] * m[1, 0];

        for (int i = 2; i < aVector.Length; i++)
        {
            aVector[i] = m[i - 1, i] / zVector[i - 1];
            bVector[i] = (-b[i] + m[i, i - 1]) / zVector[i - 1];
            zVector[i] = -m[i, i] - aVector[i] * m[i, i - 1];
        }
        resVector[resVector.Length - 1] = (-b[resVector.Length - 1] + aVector[resVector.Length - 1] * bVector[resVector.Length - 1]) / zVector[resVector.Length - 1];
        for (int i = resVector.Length - 2; i >= 0; i--)
        {
            resVector[i] = aVector[i + 1] * resVector[i + 1] + bVector[i + 1];
        }
        return resVector;
    }
    public static Polynomial GetSplinePart(double[] x, double[] y, double[] miVector, double[] hiVector, int a, int b, int nodesCount)
    {
        Polynomial s1 = new Polynomial();
        s1.AddPolynomialMember(miVector[a], 0);
        Polynomial polynomial = new Polynomial();
        polynomial.AddPolynomialMember(x[b], 0);
        polynomial.AddPolynomialMember(-1, 1);
        for (int i = 0; i < 3; i++)
        {
            s1 = s1.Multiply(polynomial);
        }
        Polynomial temp = new Polynomial();
        temp.AddPolynomialMember(1 / (6 * hiVector[b - 1]), 0);
        s1 = s1.Multiply(temp);

        Polynomial s2 = new Polynomial();
        s2.AddPolynomialMember(miVector[b], 0);
        polynomial = new Polynomial();
        polynomial.AddPolynomialMember(-x[a], 0);
        polynomial.AddPolynomialMember(1, 1);
        for (int i = 0; i < 3; i++)
        {
            s2 = s2.Multiply(polynomial);
        }
        temp = new Polynomial();
        temp.AddPolynomialMember(1 / (6 * hiVector[b - 1]), 0);
        s2 = s2.Multiply(temp);

        Polynomial s3 = new Polynomial();
        s3.AddPolynomialMember((y[a] - (miVector[a] * hiVector[b - 1] * hiVector[b - 1]) / 6), 0);
        polynomial = new Polynomial();
        polynomial.AddPolynomialMember(x[b], 0);
        polynomial.AddPolynomialMember(-1, 1);
        temp = new Polynomial();
        temp.AddPolynomialMember(1 / hiVector[b - 1], 0);
        polynomial = polynomial.Multiply(temp);
        s3 = s3.Multiply(polynomial);

        Polynomial s4 = new Polynomial();
        s4.AddPolynomialMember((y[b] - (miVector[b] * hiVector[b - 1] * hiVector[b - 1]) / 6), 0);
        polynomial = new Polynomial();
        polynomial.AddPolynomialMember(-x[a], 0);
        polynomial.AddPolynomialMember(1, 1);
        temp = new Polynomial();
        temp.AddPolynomialMember(1 / hiVector[b - 1], 0);
        polynomial = polynomial.Multiply(temp);
        s4 = s4.Multiply(polynomial);

        Polynomial s = new Polynomial();
        s.Add(s1);
        s.Add(s2);
        s.Add(s3);
        s.Add(s4);

        s.RefactorAllPolynomialMembers(nodesCount);

        return s;
    }

    public static void ShowPolynomial(Polynomial polynomial, string varLetter)
    {
        int i = 0;
        polynomial.SortByDegree();
        for(int k = 0; k < polynomial.polynomialMembers.Count; k++)
        {
            var item = polynomial.polynomialMembers[k];
            if (k != 0 && item.Сoefficient > 0)
            {
                if (item.Grade != 0)
                {
                    Console.Write($"+{item.Сoefficient:f6}*({varLetter}^{item.Grade})");
                }
                else
                {
                    Console.Write($"+{item.Сoefficient:f6}");
                }

            }
            else
            {
                if (item.Grade != 0)
                {
                    Console.Write($"{item.Сoefficient:f6}*({varLetter}^{item.Grade})");
                }
                else
                {
                    Console.Write($"{item.Сoefficient:f6}");
                }

            }
            k++;
        }
    }

    public static void ShowSpline(List<Polynomial> spline, double[] nodes)
    {
        int i = 0;
        foreach (var s in spline)
        {
            int j = i;
            ShowPolynomial(s, "x");
            Console.Write(",    ");
            Console.Write($"[{nodes[i - j]:f4}; {nodes[i - j + 1]:f4}]");
            Console.WriteLine();
            i++;
        }
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
        Console.WriteLine("_____________________________________________________________________________________________________________________________________________________________________________________________________________________________________________");
        Console.WriteLine("Природній кубічний інтерполяційний сплайн:");
        var hi = FindHi(chebishovNulls.OrderBy(x => x).ToList());
        ShowVector(hi, 'h');
        var fi = functionInChebishovNulls.ToArray();
        var H = FindMatrixH(hi);
        var A = FindMatrixA(hi);
        Console.WriteLine("Матриця H:");
        ShowMatrix(H);
        Console.WriteLine("Матриця A:");
        ShowMatrix(A);
        var b = MultipleMatrixVector(H, fi);
        var result = FindRes(A, b);
        var m = new double[nodes];
        m[0] = 0;
        m[m.Length - 1] = 0;
        Console.WriteLine("Метод прогонки:");
        Console.WriteLine("Am = Hf:");
        Console.WriteLine("Результат: ");
        for (int i = 0; i < m.Length - 2; i++)
        {
            m[i + 1] = result[i];
        }
        ShowVector(result, 'm');
        List<Polynomial> spline = new List<Polynomial>();
        for (int i = 1; i < nodes; i++)
        {
            Polynomial s = GetSplinePart(chebishovNulls.ToArray(), functionInChebishovNulls.ToArray(), m, hi, i - 1, i, nodes);
            spline.Add(s);
        }
        Console.WriteLine("Сплайн: ");
        ShowSpline(spline, chebishovNulls.ToArray());
    }

}