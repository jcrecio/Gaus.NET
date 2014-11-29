namespace GausNET.Implementation
{
    using Contract;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Polynomic equation. It establishes the equation formed by a polynomial equals to zero.
    /// </summary>
    [Serializable]
    public class PolynomicEquation : Operable, IEquation
    {
        /// <summary>
        /// Polynomial is to match to zero 
        /// </summary>
        public Polynomial Content { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PolynomicEquation"/> class.
        /// </summary>
        public PolynomicEquation() :
            this(new Polynomial(0))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PolynomicEquation"/> class.
        /// </summary>
        /// <param name="polynomial"></param>
        public PolynomicEquation(Polynomial polynomial)
        {
            this.Content = polynomial;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PolynomicEquation"/> class.
        /// </summary>
        /// <param name="polynomial"></param>
        public PolynomicEquation(PolynomialRoots polynomial)
            : this(polynomial.GetExplicitPolynomial())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PolynomicEquation"/> class.
        /// </summary>
        /// <param name="polynomicEquationString"></param>
        public PolynomicEquation(string polynomicEquationString)
            :this(new Polynomial(polynomicEquationString))
        {
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing"></param>
        protected void Dispose(bool disposing)
        {
            if (this.Disposed)
                return;

            if (disposing)
            {
                this.Content.Dispose();
                base.Dispose();
            }

            this.Disposed = true;
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public override void Dispose()
        {
            this.Dispose(false);
            base.Dispose();
        }

        /// <summary>
        /// Gets the numbers that fulfill the equation for one variable
        /// </summary>
        /// <returns></returns>
        public IEnumerable<double> GetSolutions()
        {
            switch ((Int32)this.Content.GetGrade())
            {
                case 0:
                    {
                        return new double[0];
                    }
                case 1:
                    {
                        // Linear equation => mx + n = 0
                        return new [] { -this.Content.Coefficient / this.Content.Monomials.Values.ElementAt(0).Coefficient };
                    }
                case 2:
                    {
                        // Cuadratic equation => ax2 + bx + c = 0 => x = (-b +- sqrt(b2 - 4ac))/2a

                        IEnumerable<Monomial> monomialsGrade2 = this.Content.Monomials.Values.Where(mon => mon.GetGrade().Equals(2)).ToArray();

                        double a = monomialsGrade2.ElementAt(0).Coefficient;
                        double twoA = 2 * a;
                        double b = this.Content.Monomials.Values.Except(monomialsGrade2).ElementAt(0).Coefficient;
                        double c = this.Content.Coefficient;

                        double sqrt = Math.Sqrt(Math.Pow(b, 2) - 4 * a * c);

                        return new[]
                                {
                                    (-b+sqrt)/twoA, 
                                    (-b-sqrt)/twoA
                                };
                    }
                default:
                    {
                        return new double[0];
                    }
            }
        }

        /// <summary>
        /// Returns the instance of the object as a string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Concat(this.Content.ToString(), "=0");
        }

        /// <summary>
        /// Gets the variables compounding the equation
        /// </summary>
        /// <returns></returns>
        public IEnumerable<char> GetVariables()
        {
            var variables = this.Content.GetDistinctVariables();
            return variables.Select(v => v.Literal);
        }

        /// <summary>
        /// Gets the number of the different literal variables in the equation
        /// </summary>
        /// <returns></returns>
        public int GetNumberDisctinctVariables()
        {
            var variables = this.Content.GetDistinctVariables();
            return variables.Count();
        }

        /// <summary>
        /// Gets a value indicating whether the equation has solution/s
        /// </summary>
        public bool HasSolution
        {
            get
            {
                return this.GetSolutions().Any();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the equation doesn't have any solution/s
        /// </summary>
        public override bool IsZero
        {
            get
            {
                return !this.GetSolutions().Any();
            }
        }

        /// <summary>
        /// Sets the indicating content to the current object.
        /// </summary>
        /// <param name="operable">
        /// The new operable.
        /// </param>
        public override void SetContent(IOperable operable)
        {
            var polynomicEquation = operable as Polynomial;
            this.Content = polynomicEquation;
        }

        /// <summary>
        /// Returns a clone of the equation.
        /// </summary>
        /// <returns></returns>
        public PolynomicEquation Clone()
        {
            return new PolynomicEquation(this.Content);
        }
    }
}
