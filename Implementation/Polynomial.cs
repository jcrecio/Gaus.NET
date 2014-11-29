namespace GausNET.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;

    using GausNET.Contract;
    using GausNET.Regex;
    using GausNET.Services;

    /// <summary>
    /// The polynomial stands out for mathematical polynomials composed by monomials.
    /// </summary>
    [Serializable]
    public class Polynomial : Operable
    {
        /// <summary>
        /// Disposing object.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected void Dispose(bool disposing)
        {
            if (this.Disposed)
            {
                return;
            }

            if (disposing)
            {
                var disposableItems = new List<Operable>(this.Monomials.Values);
                disposableItems.ForEach(m => m.Dispose());
                base.Dispose();
            }

            this.Disposed = true;
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public override void Dispose()
        {
            this.Dispose(true);
            base.Dispose();
        }

        /// <summary>
        /// The operation.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        protected delegate Polynomial Operation(IEnumerable<Polynomial> args);

        /// <summary>
        /// The add.
        /// </summary>
        protected readonly Operation _Add;

        /// <summary>
        /// The substract.
        /// </summary>
        protected readonly Operation _Substract;

        /// <summary>
        /// The multiply.
        /// </summary>
        protected readonly Operation _Multiply;

        /// <summary>
        /// The divide.
        /// </summary>
        protected readonly Operation _Divide;

        /// <summary>
        /// Gets or sets the monomials.
        /// </summary>
        public Dictionary<string, Monomial> Monomials { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polynomial"/> class.
        /// </summary>
        public Polynomial()
        {
            this._Add = BaseAdd;
            this._Substract = BaseSubstract;
            this._Multiply = BaseMultiply;
            this._Divide = BaseDivide;

            this.Monomials = new Dictionary<string, Monomial>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polynomial"/> class.
        /// </summary>
        /// <param name="polynomialString">
        /// The polynomial math expression.
        /// </param>
        public Polynomial(string polynomialString)
        {
            this.Monomials = new Dictionary<string, Monomial>();

            polynomialString = polynomialString.Replace(" ", string.Empty);
            if (polynomialString[0] != '+' && polynomialString[0] != '-')
            {
                polynomialString = string.Concat('+', polynomialString);
            }

            const string splitPattern = Patterns.Polynomial;

            MatchCollection ms = new Regex(splitPattern).Matches(polynomialString);

            foreach (Match mo in ms)
            {
                this.AddOrSetMonomial(new Monomial(mo.Value));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polynomial"/> class.
        /// </summary>
        /// <param name="coefficient">
        /// The coefficient.
        /// </param>
        public Polynomial(double coefficient)
        {
            this.Coefficient = coefficient;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polynomial"/> class.
        /// </summary>
        /// <param name="monomial">
        /// The monomial.
        /// </param>
        public Polynomial(Monomial monomial)
            : this()
        {
            this.AddOrSetMonomial(monomial);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polynomial"/> class.
        /// </summary>
        /// <param name="coefficient">
        /// The coefficient.
        /// </param>
        /// <param name="monomial">
        /// The monomial.
        /// </param>
        public Polynomial(double coefficient, Monomial monomial)
            : this(coefficient)
        {
            this.AddOrSetMonomial(monomial);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polynomial"/> class.
        /// </summary>
        /// <param name="coefficient">
        /// The coefficient.
        /// </param>
        /// <param name="monomialCollection">
        /// The monomials.
        /// </param>
        public Polynomial(double coefficient, IEnumerable<Monomial> monomialCollection)
            : this(coefficient)
        {
            foreach (Monomial m in monomialCollection)
            {
                this.AddOrSetMonomial(m);
            }
        }

        /// <summary>
        /// Applies the operation indicated between the current element and the one passed by argument
        /// </summary>
        /// <param name="argument1">
        /// The polynomial argument.
        /// </param>
        /// <param name="argument2">
        /// The double argument.
        /// </param>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <returns>
        /// The <see cref="Polynomial"/>.
        /// </returns>
        private static Polynomial Operate(Polynomial argument1, double argument2, Operation operation)
        {
            return operation(new[] { argument1, new Polynomial(argument2) });
        }

        /// <summary>
        /// Applies the operation indicated between the current element and the one passed by argument
        /// </summary>
        /// <param name="argument1">
        /// The polynomial argument 1.
        /// </param>
        /// <param name="argument2">
        /// The operable argument.
        /// </param>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <returns>
        /// The <see cref="Polynomial"/>.
        /// </returns>
        private static Polynomial Operate(Polynomial argument1, IOperable argument2, Operation operation)
        {
            return operation(new[] { argument1, new Polynomial(argument2.Coefficient) });
        }

        /// <summary>
        /// Applies the operation indicated between the current element and the one passed by argument
        /// </summary>
        /// <param name="argument1">
        /// The polynomial argument.
        /// </param>
        /// <param name="argument2">
        /// The monomial argument.
        /// </param>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <returns>
        /// The <see cref="Polynomial"/>.
        /// </returns>
        private static Polynomial Operate(Polynomial argument1, Monomial argument2, Operation operation)
        {
            return operation(new[] { argument1, new Polynomial(0, argument2) });
        }

        /// <summary>
        /// Applies the operation indicated between the current element and the one passed by argument
        /// </summary>
        /// <param name="argument1">
        /// The argument 1.
        /// </param>
        /// <param name="argument2">
        /// The argument 2.
        /// </param>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <returns>
        /// The <see cref="Polynomial"/>.
        /// </returns>
        private static Polynomial Operate(Polynomial argument1, Polynomial argument2, Operation operation)
        {
            return operation(new[] { argument1, argument2 });
        }

        /// <summary>
        /// Applies the operation indicated among the current element and the ones passed by argument
        /// </summary>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <returns>
        /// The <see cref="Polynomial"/>.
        /// </returns>
        private static Polynomial Operate(IEnumerable<Polynomial> arguments, Operation operation)
        {
            return operation(arguments);
        }

        /// <summary>
        /// Adds a polynomial and an operable.
        /// </summary>
        /// <param name="polynomial">
        /// The polynomial.
        /// </param>
        /// <param name="operable">
        /// The operable.
        /// </param>
        /// <returns>
        /// Polynomial
        /// </returns>
        public static Polynomial operator +(Polynomial polynomial, IOperable operable)
        {
            return new Polynomial(polynomial.Coefficient + operable.Coefficient, polynomial.Monomials.Values);
        }

        /// <summary>
        /// Adds a polynomial and an monomial.
        /// </summary>
        /// <param name="polynomial">
        /// The polynomial.
        /// </param>
        /// <param name="monomial">
        /// The operable.
        /// </param>
        /// <returns>
        /// Monomial
        /// </returns>
        public static Polynomial operator +(Polynomial polynomial, Monomial monomial)
        {
            Polynomial p = polynomial.Clone();
            Monomial m = monomial.Clone();
            string key = m.VariablesToString();
            if (p.Monomials.ContainsKey(key))
            {
                Monomial monomialRef = p.Monomials[key];

                double val = monomialRef.Coefficient;
                val += m.Coefficient;

                if (!val.Equals(0))
                {
                    monomialRef.Coefficient = val;
                }
                else
                {
                    p.Monomials.Remove(key);
                }
            }
            else
            {
                if (m.Coefficient.Equals(0))
                {
                    if (m.Variables.Count == 0)
                    {
                        p.Coefficient += m.Coefficient;
                    }
                    else
                    {
                        p.Monomials.Add(m.VariablesToString(), m.Clone());
                    }
                }
            }

            return p;
        }

        /// <summary>
        /// Adds a polynomial and an monomial.
        /// </summary>
        /// <param name="polynomial1">
        /// The polynomial.
        /// </param>
        /// <param name="polynomial2">
        /// The polynomial.
        /// </param>
        /// <returns>
        /// Polynomial
        /// </returns>
        public static Polynomial operator +(Polynomial polynomial1, Polynomial polynomial2)
        {
            Polynomial p = polynomial1.Clone();
            p.Coefficient += polynomial2.Coefficient;

            return polynomial2.Monomials.Aggregate(p, (current, kvp) => current + kvp.Value);
        }

        /// <summary>
        /// Operator to add a polynomial and an polynomial defined by its roots.
        /// </summary>
        /// <param name="polynomial">
        /// The polynomial.
        /// </param>
        /// <param name="polynomialRoots">
        /// The polynomial defined by its roots.
        /// </param>
        /// <returns>
        /// Polynomial
        /// </returns>
        public static Polynomial operator +(Polynomial polynomial, PolynomialRoots polynomialRoots)
        {
            return polynomial + polynomialRoots.GetExplicitPolynomial();
        }

        /// <summary>
        /// Function to support the public operator for the addition of a list of polynomials.
        /// </summary>
        /// <param name="operands">
        /// The operands.
        /// </param>
        /// <returns>
        /// The <see cref="Polynomial"/>.
        /// </returns>
        private static Polynomial BaseAdd(IEnumerable<Polynomial> operands)
        {
            var p = new Polynomial(0);

            return operands.Aggregate(p, (current, operand) => current + operand);
        }

        /// <summary>
        /// Substracts a polynomial and an operable.
        /// </summary>
        /// <param name="polynomial">
        /// The polynomial.
        /// </param>
        /// <param name="operable">
        /// The polynomial expressed by its roots.
        /// </param>
        /// <returns>
        /// Polynomial
        /// </returns>
        public static Polynomial operator -(Polynomial polynomial, IOperable operable)
        {
            return new Polynomial(polynomial.Coefficient - operable.Coefficient, polynomial.Monomials.Values);
        }

        /// <summary>
        /// Returns the opposite polynomial
        /// </summary>
        /// <param name="polynomial">
        /// The polynomial.
        /// </param>
        /// <returns>
        /// Polynomial
        /// </returns>
        public static Polynomial operator -(Polynomial polynomial)
        {
            Polynomial p = polynomial.Clone();
            p.Coefficient = -p.Coefficient;

            foreach (var mon in p.Monomials)
            {
                mon.Value.Coefficient = -mon.Value.Coefficient;
            }

            return p;
        }

        /// <summary>
        /// Substracts a polynomial and an monomial.
        /// </summary>
        /// <param name="polynomial">
        /// The polynomial.
        /// </param>
        /// <param name="monomial">
        /// The monomial.
        /// </param>
        /// <returns>
        /// Polynomial
        /// </returns>
        public static Polynomial operator -(Polynomial polynomial, Monomial monomial)
        {
            Polynomial p = polynomial.Clone();
            Monomial m = monomial.Clone();
            string key = m.VariablesToString();
            if (p.Monomials.ContainsKey(key))
            {
                Monomial monomialRef = p.Monomials[key];

                double val = monomialRef.Coefficient - m.Coefficient;

                if (!val.Equals(0))
                {
                    monomialRef.Coefficient = val;
                }
                else
                {
                    p.Monomials.Remove(key);
                }
            }
            else
            {
                if (!m.Coefficient.Equals(0))
                {
                    if (m.Variables.Count == 0)
                    {
                        p.Coefficient -= m.Coefficient;
                    }
                    else
                    {
                        p.Monomials.Add((-m).VariablesToString(), m.Clone());
                    }
                }
            }

            return p;
        }

        /// <summary>
        /// Subtracts two polynomials.
        /// </summary>
        /// <param name="polynomial1">
        /// The polynomial 1.
        /// </param>
        /// <param name="polynomial2">
        /// The polynomial 2.
        /// </param>
        /// <returns>
        /// Polynomial
        /// </returns>
        public static Polynomial operator -(Polynomial polynomial1, Polynomial polynomial2)
        {
            Polynomial polynomial = polynomial1.Clone();
            polynomial.Coefficient -= polynomial2.Coefficient;

            return polynomial2.Monomials.Aggregate(polynomial, (current, kvp) => current - kvp.Value);
        }

        /// <summary>
        /// Function to support the public operator for the substraction of a list of polynomials.
        /// </summary>
        /// <param name="operands">
        /// The operands.
        /// </param>
        /// <returns>
        /// The <see cref="Polynomial"/>.
        /// </returns>
        private static Polynomial BaseSubstract(IEnumerable<Polynomial> operands)
        {
            Polynomial polynomial = new Polynomial(0);

            return operands.Aggregate(polynomial, (current, operand) => current - operand);
        }

        /// <summary>
        /// Operator to multiply a polynomial per an scalar.
        /// </summary>
        /// <param name="polynomial">
        /// The polynomial.
        /// </param>
        /// <param name="scalar">
        /// The polynomial defined by its roots.
        /// </param>
        /// <returns>
        /// Polynomial
        /// </returns>
        public static Polynomial operator *(Polynomial polynomial, double scalar)
        {
            Polynomial p = polynomial.Clone();
            if (scalar.Equals(0))
            {
                p.Monomials.Clear();
                p.Coefficient = 0;
            }
            else
            {
                p.Coefficient *= scalar;
                foreach (var m in p.Monomials.Values)
                {
                    m.Coefficient *= scalar;
                }
            }

            return p;
        }

        /// <summary>
        /// Operator to multiply a polynomial per an operable.
        /// </summary>
        /// <param name="polynomial">
        /// The polynomial.
        /// </param>
        /// <param name="operable">
        /// The polynomial defined by its roots.
        /// </param>
        /// <returns>
        /// Polynomial
        /// </returns>
        public static Polynomial operator *(Polynomial polynomial, IOperable operable)
        {
            Double coefficient = operable.Coefficient;

            Polynomial p = polynomial.Clone();

            if (coefficient.Equals(0))
            {
                p.Monomials.Clear();
                p.Coefficient = 0;
            }
            else
            {
                p.Coefficient *= coefficient;
                foreach (var m in p.Monomials.Values)
                {
                    m.Coefficient *= coefficient;
                }
            }

            return p;
        }

        /// <summary>
        /// Operator to multiply a polynomial per an monomial.
        /// </summary>
        /// <param name="polynomial">
        /// The polynomial.
        /// </param>
        /// <param name="monomial">
        /// The monomial.
        /// </param>
        /// <returns>
        /// Polynomial
        /// </returns>
        public static Polynomial operator *(Polynomial polynomial, Monomial monomial)
        {
            Polynomial p = polynomial.Clone();
            if (monomial.Coefficient.Equals(0))
            {
                p.Monomials.Clear();
                p.Coefficient = 0;
            }
            else
            {
                var arrayController = ArrayController.GetInstance();
                Dictionary<string, Monomial> monomialCollection = arrayController.CloneDictionary(p.Monomials);

                foreach (var monomialInCollection in monomialCollection.Values)
                {
                    string oldKey = monomialInCollection.VariablesToString();

                    monomialInCollection.Multiply(monomial);
                    string key = monomialInCollection.VariablesToString();

                    if (!p.Monomials.ContainsKey(key))
                    {
                        p.Monomials.Add(key, monomialInCollection);
                    }
                    else
                    {
                        p.Monomials[key] = monomialInCollection;
                    }

                    p.Monomials.Remove(oldKey);
                }

                if (!p.Coefficient.Equals(0))
                {
                    Monomial maux = monomial.Clone();
                    maux.Multiply(p.Coefficient);
                    string key = monomial.VariablesToString();

                    if (!p.Monomials.ContainsKey(key))
                    {
                        p.Monomials.Add(key, maux);
                    }
                    else
                    {
                        p.Monomials[key] = maux;
                    }
                }

                p.Coefficient = 0;
            }

            return p;
        }

        /// <summary>
        /// Operator to multiply a polynomial per an polynomial.
        /// </summary>
        /// <param name="polynomial1">
        /// The polynomial 1.
        /// </param>
        /// <param name="polynomial2">
        /// The polynomial 2.
        /// </param>
        /// <returns>
        /// Polynomial
        /// </returns>
        public static Polynomial operator *(Polynomial polynomial1, Polynomial polynomial2)
        {
            var polynomialAux = new Polynomial(0);
            Polynomial polynomialCopy = null;

            foreach (var kvp in polynomial1.Monomials)
            {
                polynomialCopy = polynomial1.Clone();
                polynomialCopy.Multiply(kvp.Value);
                polynomialAux.Add(polynomialCopy);
            }

            polynomialCopy = polynomial1.Clone();
            polynomialCopy.Multiply(polynomial1.Coefficient);
            polynomialAux.Add(polynomialCopy);
            polynomial1.Coefficient = polynomialAux.Coefficient;
            polynomial1.Monomials = polynomialAux.Monomials;

            return polynomial1;
        }

        /// <summary>
        /// Function to support the public operator for the multiplication of a list of polynomials.
        /// </summary>
        /// <param name="operands">
        /// The operands.
        /// </param>
        /// <returns>
        /// The <see cref="Polynomial"/>.
        /// </returns>
        private static Polynomial BaseMultiply(IEnumerable<Polynomial> operands)
        {
            Polynomial polynomial = null;

            foreach (var operand in operands)
            {
                if (polynomial == null)
                {
                    polynomial = new Polynomial(operand.Coefficient, operand.Monomials.Values);
                }
                else
                {
                    polynomial *= operand;
                }
            }

            return polynomial;
        }

        /// <summary>
        /// Divides a polynomial per an scalar.
        /// </summary>
        /// <param name="polynomial">
        /// The polynomial.
        /// </param>
        /// <param name="scalar">
        /// The scalar.
        /// </param>
        /// <returns>
        /// Polynomial
        /// </returns>
        public static Polynomial operator /(Polynomial polynomial, double scalar)
        {
            Polynomial p1 = polynomial.Clone();

            if (scalar.Equals(0))
            {
                p1.Coefficient /= scalar;
                foreach (var m in p1.Monomials.Values)
                {
                    m.Coefficient /= scalar;
                }
            }

            return p1;
        }

        /// <summary>
        /// Divides a polynomial per an operable.
        /// </summary>
        /// <param name="polynomial">
        /// The polynomial.
        /// </param>
        /// <param name="operable">
        /// The operable.
        /// </param>
        /// <returns>
        /// Polynomial
        /// </returns>
        public static Polynomial operator /(Polynomial polynomial, IOperable operable)
        {
            Polynomial p = polynomial.Clone();

            if (operable.Coefficient.Equals(0))
            {
                p.Coefficient /= operable.Coefficient;
                foreach (var m in p.Monomials.Values)
                {
                    m.Coefficient /= operable.Coefficient;
                }
            }

            return p;
        }

        /// <summary>
        /// Divides a polynomial per a monomial.
        /// </summary>
        /// <param name="polynomial">
        /// The polynomial.
        /// </param>
        /// <param name="monomial">
        /// The monomial.
        /// </param>
        /// <returns>
        /// Polynomial
        /// </returns>
        public static Polynomial operator /(Polynomial polynomial, Monomial monomial)
        {
            Polynomial p1 = polynomial.Clone();
            if (monomial.Coefficient.Equals(0))
            {
                foreach (var monomialDividend in p1.Monomials)
                {
                    p1.Monomials[monomialDividend.Key].Divide(monomial);
                }

                Monomial mn = monomial.Clone();
                foreach (var v in monomial.Variables)
                {
                    monomial.Variables[v.Key].Exponent = -monomial.Variables[v.Key].Exponent;
                }

                monomial.Coefficient = mn.Coefficient / monomial.Coefficient;
                p1.Coefficient = 0;

                p1.Monomials.Add(monomial.ToString(), monomial);
            }

            return p1;
        }

        /// <summary>
        /// Divides a polynomial per a monomial.
        /// </summary>
        /// <param name="polynomial1">
        /// The dividend.
        /// </param>
        /// <param name="polynomial2">
        /// The divisor.
        /// </param>
        /// <returns>
        /// Polynomial
        /// </returns>
        public static Polynomial operator /(Polynomial polynomial1, Polynomial polynomial2)
        {
            Polynomial[] quasiRestAndQuotionAndRest = DivisionProcess(polynomial1, polynomial2);

            if (quasiRestAndQuotionAndRest[1].Monomials.Values.First().GetGrade().Equals(polynomial2.GetGrade()))
            {
                quasiRestAndQuotionAndRest[0].Add(new Monomial(1));
            }

            return quasiRestAndQuotionAndRest[0];
        }

        /// <summary>
        /// Gets the rest of the division between the polynomials.
        /// </summary>
        /// <param name="dividend">
        /// The dividend.
        /// </param>
        /// <param name="divisor">
        /// The divisor.
        /// </param>
        /// <returns>
        /// Polynomial
        /// </returns>
        public static Polynomial operator %(Polynomial dividend, Polynomial divisor)
        {
            Polynomial[] quasiRestAndQuotionAndRest = DivisionProcess(dividend, divisor);

            if (quasiRestAndQuotionAndRest[0].Monomials.Values.First().GetGrade().Equals(divisor.GetGrade()))
            {
                quasiRestAndQuotionAndRest[1].Substract(divisor);
            }

            return quasiRestAndQuotionAndRest[1]; // Returns the rest
        }

        /// <summary>
        /// Gets a 2 items tuple with the quotient and the dividend
        /// </summary>
        /// <param name="dividend">
        /// The dividend.
        /// </param>
        /// <param name="divisor">
        /// The divisor.
        /// </param>
        /// <returns>
        /// The quotient and dividend within a tuple
        /// </returns>
        public static Polynomial[] DivisionProcess(Polynomial dividend, Polynomial divisor)
        {
            Polynomial dividendContainer = dividend.Clone();
            Polynomial quotient = new Polynomial(0);

            double dividerGrade = divisor.GetGrade();

            while (dividendContainer.Monomials.Values.First().GetGrade() > dividerGrade)
            {
                dividendContainer.Monomials.Values.First().Divide(divisor.Monomials.Values.First());
                Monomial quotientItem = dividendContainer.Monomials.Values.First();

                quotient.Add(quotientItem);

                Polynomial productoQuotientPerDivider = divisor.Clone();
                productoQuotientPerDivider.Multiply(quotientItem);

                dividend.Substract(productoQuotientPerDivider);
                dividendContainer = dividend.Clone();
            }

            return new[] { quotient, dividend };
        }

        /// <summary>
        /// Function to support the public operator for the division of a list of polynomials.
        /// </summary>
        /// <param name="operands">
        /// The operands.
        /// </param>
        /// <returns>
        /// The <see cref="Polynomial"/>.
        /// </returns>
        private static Polynomial BaseDivide(IEnumerable<Polynomial> operands)
        {
            Polynomial p = null;
            foreach (var operand in operands)
            {
                if (p == null)
                {
                    p = new Polynomial(operand.Coefficient, operand.Monomials.Values);
                }
                else
                {
                    p /= operand;
                }
            }

            return p;
        }

        /// <summary>
        /// Sets the indicating content to the current object.
        /// </summary>
        /// <param name="operable">
        /// The new operable.
        /// </param>
        public override void SetContent(IOperable operable)
        {
            var p = operable as Polynomial;

            if (p != null)
            {
                this.Coefficient = p.Coefficient;
                this.Monomials = p.Monomials;
            }
        }

        /// <summary>
        /// Adds a real number to the polynomial
        /// </summary>
        /// <param name="real"></param>
        public void Add(double real)
        {
            this.SetContent(Operate(this, real, this._Add));
        }

        /// <summary>
        /// Adds a operable to the polynomial
        /// </summary>
        /// <param name="operable"></param>
        public void Add(IOperable operable)
        {
            this.SetContent(Operate(this, operable, this._Add));
        }

        /// <summary>
        /// Adds a monomial to the polynomial
        /// </summary>
        /// <param name="monomial"></param>
        public void Add(Monomial monomial)
        {
            this.SetContent(Operate(this, monomial, this._Add));
        }

        /// <summary>
        /// Adds a polynomial to the polynomial
        /// </summary>
        /// <param name="polynomial"></param>
        public void Add(Polynomial polynomial)
        {
            this.SetContent(Operate(this, polynomial, this._Add));
        }

        /// <summary>
        /// Adds some polynomials to the polynomial
        /// </summary>
        /// <param name="polynomials"></param>
        public void Add(IEnumerable<Polynomial> polynomials)
        {
            this.SetContent(Operate(polynomials.Union(new[] { this }), this._Add));
        }

        /// <summary>
        /// Substract a real number to the polynomial
        /// </summary>
        /// <param name="number"></param>
        public void Substract(double number)
        {
            this.SetContent(Operate(this, number, this._Substract));
        }

        /// <summary>
        /// Substract a operable to the polynomial
        /// </summary>
        /// <param name="operable"></param>
        public void Substract(IOperable operable)
        {
            this.SetContent(Operate(this, operable, this._Substract));
        }

        /// <summary>
        /// Substract a monomial to the polynomial
        /// </summary>
        /// <param name="monomial"></param>
        public void Substract(Monomial monomial)
        {
            this.SetContent(Operate(this, monomial, this._Substract)); ;
        }

        /// <summary>
        /// Substract a polynomial to the polynomial
        /// </summary>
        /// <param name="polynomial"></param>
        public void Substract(Polynomial polynomial)
        {
            this.SetContent(Operate(this, polynomial, this._Substract));
        }

        /// <summary>
        /// Substract some polynomials to the polynomial
        /// </summary>
        /// <param name="polynomials"></param>
        public void Substract(IEnumerable<Polynomial> polynomials)
        {
            this.SetContent(Operate(polynomials.Union(new [] { this }), this._Substract));
        }

        /// <summary>
        /// Multiplies a real number to the polynomial
        /// </summary>
        /// <param name="number"></param>
        public void Multiply(double number)
        {
            this.SetContent(Operate(this, number, this._Multiply));
        }

        /// <summary>
        /// Multiplies the polynomial per a real number
        /// </summary>
        /// <param name="operable"></param>
        public void Multiply(IOperable operable)
        {
            this.SetContent(Operate(this, operable, this._Multiply));
        }

        /// <summary>
        /// Multiplies the polynomial per a monomial
        /// </summary>
        /// <param name="monomial"></param>
        public void Multiply(Monomial monomial)
        {
            this.SetContent(Operate(this, monomial, this._Multiply));
        }

        /// <summary>
        /// Multiplies the polynomial per a polynomial
        /// </summary>
        /// <param name="polynomial"></param>
        public void Multiply(Polynomial polynomial)
        {
            this.SetContent(Operate(this, polynomial, this._Multiply));
        }

        /// <summary>
        /// Multiplies the polynomial per some polynomials
        /// </summary>
        /// <param name="polynomials"></param>
        public void Multiply(IEnumerable<Polynomial> polynomials)
        {
            this.SetContent(Operate(polynomials.Union(new [] { this }), this._Multiply));
        }

        /// <summary>
        /// Divides the polynomial per a number
        /// </summary>
        /// <param name="number"></param>
        public void Divide(double number)
        {
            this.SetContent(Operate(this, number, this._Divide));
        }

        /// <summary>
        /// Divides the polynomial per a operable
        /// </summary>
        /// <param name="operable"></param>
        public void Divide(IOperable operable)
        {
            this.SetContent(Operate(this, operable, this._Divide));
        }

        /// <summary>
        /// Divides the polynomial per a monomial
        /// </summary>
        /// <param name="monomial"></param>
        public void Divide(Monomial monomial)
        {
            this.SetContent(Operate(this, monomial, this._Divide));
        }

        /// <summary>
        /// Divides the polynomial per a polynomial
        /// </summary>
        /// <param name="polynomial"></param>
        public void Divide(Polynomial polynomial)
        {
            this.SetContent(Operate(this, polynomial, this._Divide));
        }

        /// <summary>
        /// Divides the polynomial per some polynomials
        /// </summary>
        /// <param name="polynomials"></param>
        public void Divide(IEnumerable<Polynomial> polynomials)
        {
            this.SetContent(Operate(polynomials.Union(new[] { this }), this._Divide));
        }

        /// <summary>
        /// Gets the polynomial defined by its roots
        /// </summary>
        /// <returns>
        /// PolynomialRoots
        /// </returns>
        public PolynomialRoots GetRootsPolynomial()
        {
            var polynomialRoots = new PolynomialRoots();

            var polynomicEquation = new PolynomicEquation(this);
            foreach (double root in polynomicEquation.GetSolutions())
            {
                polynomialRoots.InsertRoot(root);
            }

            return polynomialRoots;
        }

        /// <summary>
        /// Pow Operation between a polynomial and a given integer exponent.
        /// </summary>
        /// <param name="exponent">
        /// The exponent.</param>
        public void Pow(Int32 exponent)
        {
            this.SetContent(this.Pow(this, exponent));
        }

        /// <summary>
        /// Pow Operator to pow a polynomial and a given integer exponent.
        /// </summary>
        /// <param name="polynomial">
        /// </param>
        /// <param name="exponent">
        /// The exponent.
        /// </param>
        public static Polynomial operator ^(Polynomial polynomial, Int32 exponent)
        {
            using (var polynomialCopyToMultiply = polynomial.Clone())
            {
                Polynomial polynomialToStoreResult = polynomial.Clone();

                for (int i = 0; i < exponent; i++)
                {
                    polynomialToStoreResult = polynomialToStoreResult * polynomialCopyToMultiply;
                }

                return polynomialToStoreResult;
            }
        }

        /// <summary>
        /// Pow Operator to pow a polynomial and a given integer exponent.
        /// </summary>
        /// <param name="polynomial">
        /// </param>
        /// <param name="exponent">
        /// The exponent.
        /// </param>
        public Polynomial Pow(Polynomial polynomial, Int32 exponent)
        {
            return polynomial ^ exponent;
        }

        /// <summary>
        /// Gets the grade of the polynomial.
        /// </summary>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public override double GetGrade()
        {
            return (from monomialGrade in this.Monomials.Values select monomialGrade.GetGrade()).Max();
        }

        /// <summary>
        /// Gets a value indicating whether the polynomial is zero.
        /// </summary>
        public override bool IsZero
        {
            get { return Coefficient.Equals(0) && Monomials.Values.All(m => m.IsZero); }
        }

        /// <summary>
        /// Determines whether the polynomial is equal to the object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>bool</returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as Polynomial);
        }

        /// <summary>
        /// Determines whether the polynomial is equal.
        /// </summary>
        /// <param name="polynomial"></param>
        /// <returns>bool</returns>
        public bool Equals(Polynomial polynomial)
        {
            int n = this.Monomials.Count;
            bool isEqual = polynomial.Monomials.Count == n;

            if (isEqual && (polynomial.Coefficient.Equals(this.Coefficient)))
            {
                int i = 0;
                while (isEqual && (i < n))
                {
                    string key = this.Monomials.ElementAt(i).Key;
                    if (polynomial.Monomials.ContainsKey(key))
                    {
                        isEqual = polynomial.Monomials[key].ToString().Equals(this.Monomials[key].ToString());
                    }

                    i++;
                }

                return isEqual;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the polynomal is equal to the one represent by its roots.
        /// </summary>
        /// <param name="polynomial"></param>
        /// <returns>bool</returns>
        public bool Equals(PolynomialRoots polynomial)
        {
            return this.Equals(polynomial.GetExplicitPolynomial());
        }

        /// <summary>
        /// Get a copy of a polynomial with an polynomial sign investment
        /// </summary>
        /// <returns></returns>
        public Polynomial GetInvestSign()
        {
            var copy = this.Clone();
            this.InvestSign(copy);

            return copy;
        }

        /// <summary>
        /// Invest the polynomial sign
        /// </summary>
        /// <param name="polynomial"></param>
        private void InvestSign(Polynomial polynomial)
        {
            this.Coefficient = -this.Coefficient;
            foreach (var monomial in polynomial.Monomials.Values)
            {
                monomial.Coefficient = -monomial.Coefficient;
            }
        }

        /// <summary>
        /// Gets a clone of the polynomial
        /// </summary>
        /// <returns></returns>
        public Polynomial Clone()
        {
            var arrayController = ArrayController.GetInstance();
            return new Polynomial(this.Coefficient, arrayController.CloneDictionary(this.Monomials).Values.ToArray());
        }

        /// <summary>
        /// Gets the literal variables of the polynomial
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Variable> GetDistinctVariables()
        {
            return this.Monomials.Select(m => m.Value).SelectMany(m => m.Variables.Values).Distinct();
        }

        /// <summary>
        /// Returns a generated string from the members's polynomial
        /// </summary>
        /// <returns></returns>
        public string GetPolynomialStringIdentification()
        {
            return string.Concat((from m in this.Monomials select m.Value).ToString(), this.Coefficient.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Returns a string of the polynomials's instance
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Concat(
                string.Join("+", from m in this.Monomials select m.Value), "+", this.Coefficient.ToString())
                .Replace("+-", "-")
                .Replace("-+", "-");
        }

        /// <summary>
        /// Add internally the monomial to the polynomial taking care of whether there's already a key with same base monomial,
        /// in that case the new monomial will be added to the previous one, otherwise will be added normally
        /// </summary>
        /// <param name="monomial"></param>
        private void AddOrSetMonomial(Monomial monomial)
        {
            String monomialKeyString = monomial.VariablesToString();

            if (monomialKeyString.Equals(string.Empty))
            {
                this.Coefficient += monomial.Coefficient;

            }
            else if (this.Monomials.ContainsKey(monomialKeyString))
            {
                this.Monomials[monomialKeyString].Coefficient += monomial.Coefficient;
            }
            else
            {
                this.Monomials.Add(monomial.VariablesToString(), monomial);
            }
        }
    }
}
