using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numerical_Methods_Lab_3
{
    internal class Polynomial
    {
        public List<PolynomialMember> polynomialMembers;

        public Polynomial()
        {
            polynomialMembers= new List<PolynomialMember>();
        }
        public Polynomial(List<PolynomialMember> polynomial)
        {
            this.polynomialMembers = polynomial;
        }
        public void AddPolynomialMember(double coefficient, double degree)
        {
            polynomialMembers.Add(new PolynomialMember(coefficient, degree));
        }
        //рахує базовий поліном
        public Polynomial Multiply(Polynomial polynomial)
        {
            Polynomial result = new Polynomial();
            foreach(var firstPMember in polynomialMembers)
            {
                foreach(var secondPmember in polynomial.polynomialMembers)
                {
                    result.AddPolynomialMember(firstPMember.Сoefficient * secondPmember.Сoefficient, firstPMember.Grade + secondPmember.Grade);
                }
            }
            return result;
        }

        public void RefactorAllPolynomialMembers(int nodesCount)
        {
            List<PolynomialMember> result = new List<PolynomialMember>();
            for(int i = 0; i< nodesCount; i++)
            {
                var membersWithIDegreeCoefficient = this.polynomialMembers.Where(x => x.Grade == i).Select(x => x.Сoefficient).Sum();
                result.Add(new PolynomialMember(membersWithIDegreeCoefficient, i));
            }
            this.polynomialMembers = result;
        }
        public void Add(Polynomial polynomial)
        {
            foreach(var pm in polynomial.polynomialMembers)
            {
                this.polynomialMembers.Add(pm);
            }
        }

        public void SortByDegree()
        {
            this.polynomialMembers = this.polynomialMembers.OrderByDescending(x => x.Grade).ToList();
        }

    }
}
