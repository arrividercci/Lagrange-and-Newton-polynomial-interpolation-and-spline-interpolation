using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numerical_Methods_Lab_3
{
    internal class PolynomialMember
    {
        public int Grade { get; set; }
        public double Сoefficient { get; set; }
        public PolynomialMember() {}
        public PolynomialMember(double сoefficient, int grade)
        {
            Grade = grade;
            Сoefficient = сoefficient;
        }
        public void MultiplyCof(double coff)
        {
            this.Сoefficient = this.Сoefficient * coff;
        }
    }
}
