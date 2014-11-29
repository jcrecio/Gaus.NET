namespace GausNET.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Contract;

    using GausNET.Exception;
    using GausNET.Regex;
    using Services;

    /// <summary>
    /// The algebraic fraction provides the functionality to work with fractions compound by algebraic elements in the numerator and denominator as well as it does with the pertinent operations related to it.
    /// </summary>
    [Serializable]
    public class AlgebraicFraction : Operable
    {
        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">
        /// Disposing.
        /// </param>
        protected void Dispose(bool disposing)
        {
            if (this.Disposed)
            {
                return;
            }

            if (disposing)
            {
                this.Numerator.Dispose();
                this.Denominator.Dispose();
                base.Dispose();
            }

            this.Disposed = true;
        }

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
        private delegate AlgebraicFraction Operation(IEnumerable<AlgebraicFraction> args);

        /// <summary>
        /// The add, substract, multiply, divide.
        /// </summary>
        private readonly Operation add, substract, multiply, divide;

        /// <summary>
        /// Gets or sets the numerator.
        /// </summary>
        public Polynomial Numerator { get; set; }

        /// <summary>
        /// Gets or sets the denominator.
        /// </summary>
        public Polynomial Denominator { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlgebraicFraction"/> class.
        /// </summary>
        public AlgebraicFraction()
        {
            this.add = BaseAdd;
            this.substract = BaseSubstract;
            this.multiply = BaseMultiply;
            this.divide = BaseDivide;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlgebraicFraction"/> class.
        /// </summary>
        /// <param name="n">
        /// The numerator as a double.
        /// </param>
        public AlgebraicFraction(double n)
            : this()
        {
            this.Numerator = new Polynomial(n);
            this.Denominator = new Polynomial(1);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlgebraicFraction"/> class.
        /// </summary>
        /// <param name="numerator">
        /// The numerator as polynomial.
        /// </param>
        public AlgebraicFraction(Polynomial numerator)
            : this(numerator, new Polynomial(1))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlgebraicFraction"/> class.
        /// </summary>
        /// <param name="numerator">
        /// The numerator.
        /// </param>
        /// <param name="denominator">
        /// The denominator.
        /// </param>
        public AlgebraicFraction(Polynomial numerator, Polynomial denominator)
            : this()
        {
            if (denominator.IsZero)
            {
                this.Numerator = new Polynomial(0);
                this.Denominator = new Polynomial(1);
            }
            else
            {
                this.Numerator = numerator;
                this.Denominator = denominator;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlgebraicFraction"/> class.
        /// </summary>
        /// <param name="algebraicalFractionString">
        /// The algebraical fraction math expression.
        /// </param>
        /// <exception cref="GausGenericException">
        /// </exception>
        public AlgebraicFraction(string algebraicalFractionString)
            : this()
        {
            var collection = (new Regex(Patterns.AlgebraicalFraction)).Matches(algebraicalFractionString);

            try
            {
                var denominatorNonZero = new Polynomial(collection[1].Value);

                if (!this.Denominator.IsZero)
                {
                    this.Denominator = denominatorNonZero;
                    this.Numerator = new Polynomial(collection[0].Value);
                }
            }
            catch (Exception)
            {
                throw new GausGenericException("Algebraical fraction requires numerator and denominator both being polynomials.");
            }
        }

        /// <summary>
        /// Clones the current object.
        /// </summary>
        /// <returns>
        /// The <see cref="AlgebraicFraction"/>.
        /// </returns>
        public AlgebraicFraction Clone()
        {
            return new AlgebraicFraction(this.Numerator.Clone(), this.Denominator.Clone());
        }

        /// <summary>
        /// Gets a value indicating whether the fraction is zero.
        /// </summary>
        public override bool IsZero
        {
            get
            {
                return this.Denominator.IsZero;
            }
        }

        /// <summary>
        /// Sets the indicating content to the current object.
        /// </summary>
        /// <param name="operable">
        /// The new content.
        /// </param>
        public override void SetContent(IOperable operable)
        {
            var algebraicalFranction = (AlgebraicFraction) operable;

            this.Numerator = algebraicalFranction.Numerator;
            this.Denominator = algebraicalFranction.Denominator;
        }

        /// <summary>
        /// Returns whether the fractions have the same denominator.
        /// </summary>
        /// <param name="af">
        /// The fraction.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasSameDenominator(AlgebraicFraction af)
        {
            return this.Denominator.Equals(af.Denominator);
        }

        /// <summary>
        /// Function to support the public operator for the operation indicated between the current element and the one passed by argument
        /// </summary>
        /// <param name="algebraicFraction1">
        /// The Algebraical Fraction 1.
        /// </param>
        /// <param name="algebraicFraction2">
        /// The Algebraical Fraction 2.
        /// </param>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <returns>
        /// The <see cref="AlgebraicFraction"/>.
        /// </returns>
        private static AlgebraicFraction Operate(AlgebraicFraction algebraicFraction1, AlgebraicFraction algebraicFraction2, Operation operation)
        {
            return operation(new[] { algebraicFraction1, algebraicFraction2 });
        }

        /// <summary>
        /// Function to support the public operator for the operation indicated between the current element and the ones passed by argument
        /// </summary>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <returns>
        /// The <see cref="AlgebraicFraction"/>.
        /// </returns>
        private static AlgebraicFraction Operate(IEnumerable<AlgebraicFraction> arguments, Operation operation)
        {
            return operation(arguments);
        }

        /// <summary>
        /// Adds two fractions.
        /// </summary>
        /// <param name="algebraicFraction">
        /// The fraction 1.
        /// </param>
        /// <param name="algebraicFraction2">
        /// The fraction 2.
        /// </param>
        /// <returns>
        /// </returns>
        public static AlgebraicFraction operator +(AlgebraicFraction algebraicFraction, AlgebraicFraction algebraicFraction2)
        {
            var algebraicalFraction1 = algebraicFraction.Clone();
            if (algebraicalFraction1.HasSameDenominator(algebraicFraction2))
            {
                algebraicalFraction1.Numerator.Add(algebraicFraction2.Numerator);
            }
            else
            {
                Polynomial mcmDenominators = algebraicFraction2.GetMcm(algebraicalFraction1);

                algebraicalFraction1.Numerator = (algebraicalFraction1.Denominator / mcmDenominators) * algebraicalFraction1.Numerator 
                                               + (algebraicalFraction1.Denominator / algebraicFraction.Denominator) * algebraicFraction.Numerator;
            }

            return algebraicalFraction1;
        }

        /// <summary>
        /// Function to support the public operator for the addition between the current element and the ones passed by argument
        /// </summary>
        /// <param name="algebraicFraction">
        /// The algebraicalFraction 1.
        /// </param>
        /// <param name="algebraicFraction2">
        /// The algebraicalFraction 2.
        /// </param>
        /// <returns>
        /// The <see cref="AlgebraicFraction"/>.
        /// </returns>
        private static AlgebraicFraction BaseAdd(AlgebraicFraction algebraicFraction, AlgebraicFraction algebraicFraction2)
        {
            return algebraicFraction + algebraicFraction2;
        }

        /// <summary>
        /// Function to support the public operator for the addition of a list of fractions.
        /// </summary>
        /// <param name="fractions"></param>
        /// <returns></returns>
        private static AlgebraicFraction BaseAdd(IEnumerable<AlgebraicFraction> fractions)
        {
            var af = new AlgebraicFraction(0);
            return fractions.Aggregate(af, (current, fraction) => current + fraction);
        }

        /// <summary>
        /// Subtracts two fractions.
        /// </summary>
        /// <param name="algebraicFraction">
        /// The fraction 1.
        /// </param>
        /// <param name="algebraicFraction2">
        /// The fraction 2.
        /// </param>
        /// <returns>
        /// </returns>
        public static AlgebraicFraction operator -(AlgebraicFraction algebraicFraction, AlgebraicFraction algebraicFraction2)
        {
            var af1 = algebraicFraction.Clone();
            if (af1.HasSameDenominator(algebraicFraction2))
            {
                af1.Numerator.Add(algebraicFraction2.Numerator);
            }
            else
            {
                var mcmDenominators = algebraicFraction2.GetMcm(af1);

                af1.Numerator = (af1.Denominator / mcmDenominators) * af1.Numerator 
                              - (af1.Denominator / algebraicFraction.Denominator) * algebraicFraction.Numerator;
            }

            return af1;
        }

        /// <summary>
        /// Function to support the public operator for the substraction of a couple a fractions
        /// <param name="algebraicFraction"></param>
        /// <param name="algebraicFraction2"></param>
        /// <returns></returns>
        /// </summary>
        private static AlgebraicFraction BaseSubstract(AlgebraicFraction algebraicFraction, AlgebraicFraction algebraicFraction2)
        {
            return algebraicFraction - algebraicFraction2;
        }

        /// <summary>
        /// The base subtraction.
        /// </summary>
        /// <param name="fractions">
        /// The algebraicalFraction 1.
        /// </param>
        /// <returns>AlgebraicalFraction</returns>
        private static AlgebraicFraction BaseSubstract(IEnumerable<AlgebraicFraction> fractions)
        {
            var algebraicalFraction = new AlgebraicFraction(0);
            return fractions.Aggregate(algebraicalFraction, (current, fraction) => current - fraction);
        }

        /// <summary>
        /// Multiplies two fractions.
        /// </summary>
        /// <param name="algebraicFraction">
        /// The fraction 1.
        /// </param>
        /// <param name="algebraicFraction2">
        /// The fraction 2.
        /// </param>
        /// <returns>
        /// </returns>
        public static AlgebraicFraction operator *(AlgebraicFraction algebraicFraction, AlgebraicFraction algebraicFraction2)
        {
            return new AlgebraicFraction(algebraicFraction.Numerator * algebraicFraction2.Numerator, 
                                           algebraicFraction.Denominator * algebraicFraction2.Denominator);
        }

        /// <summary>
        /// Function to support the public operator for the multiplication of a couple of fractions
        /// <param name="algebraicFraction"></param>
        /// <param name="algebraicFraction2"></param>
        /// <returns></returns>
        /// </summary>
        private static AlgebraicFraction BaseMultiply(AlgebraicFraction algebraicFraction, AlgebraicFraction algebraicFraction2)
        {
            return algebraicFraction * algebraicFraction2;
        }

        /// <summary>
        /// Function to support the public operator for the multiplication of a list of fractions
        /// </summary>
        /// <param name="fractions"></param>
        /// <returns></returns>
        private static AlgebraicFraction BaseMultiply(IEnumerable<AlgebraicFraction> fractions)
        {
            var algebraicalFractions = fractions as AlgebraicFraction[] ?? fractions.ToArray();
            var first = algebraicalFractions.First();

            var collection = algebraicalFractions.Skip(1).ToList();

            return collection.Aggregate(first, (current, fraction) => current * fraction);
        }

        /// <summary>
        /// Multiplies two fractions.
        /// </summary>
        /// <param name="algebraicFraction">
        /// The fraction 1.
        /// </param>
        /// <param name="algebraicFraction2">
        /// The fraction 2.
        /// </param>
        /// <returns>
        /// </returns>
        public static AlgebraicFraction operator /(AlgebraicFraction algebraicFraction, AlgebraicFraction algebraicFraction2)
        {
            return new AlgebraicFraction(algebraicFraction.Numerator * algebraicFraction2.Denominator,
                                            algebraicFraction.Denominator * algebraicFraction2.Numerator);
        }

        /// <summary>
        /// Function to support the public operator for the division of a couple of fractions
        /// </summary>
        /// <param name="algebraicFraction">
        /// The fraction 1.
        /// </param>
        /// <param name="algebraicFraction2">
        /// The fraction 2.
        /// </param>
        /// <returns></returns>
        private static AlgebraicFraction BaseDivide(AlgebraicFraction algebraicFraction, AlgebraicFraction algebraicFraction2)
        {
            return algebraicFraction / algebraicFraction2;
        }

        /// <summary>
        /// Function to support the public operator for the division of a list of fractions
        /// </summary>
        /// <param name="fractions">
        /// The fractions.
        /// </param>
        /// <returns></returns>
        private static AlgebraicFraction BaseDivide(IEnumerable<AlgebraicFraction> fractions)
        {
            var algebraicalFractions = fractions as AlgebraicFraction[] ?? fractions.ToArray();
            var first = algebraicalFractions.First();

            var collection = algebraicalFractions.Skip(1).ToList();

            return collection.Aggregate(first, (current, f) => current / f);
        }

        /// <summary>
        /// Adds a algebraical fraction with the numerator provided and denominator = 1
        /// </summary>
        /// <param name="numerator"></param>
        public void Add(Polynomial numerator)
        {
            this.SetContent(Operate(this, new AlgebraicFraction(numerator), this.add));
        }

        /// <summary>
        /// Adds a fraction
        /// </summary>
        /// <param name="algebraicFraction"></param>
        public void Add(AlgebraicFraction algebraicFraction)
        {
            this.SetContent(Operate(this, algebraicFraction, this.add));
        }

        /// <summary>
        /// Adds some fractions
        /// </summary>
        /// <param name="fractions"></param>
        public void Add(IEnumerable<AlgebraicFraction> fractions)
        {
            this.SetContent(Operate(fractions.Union(new[] { this }), this.add));
        }

        /// <summary>
        /// Substract a algebraical fraction with the numerator provided and denominator = 1
        /// </summary>
        /// <param name="numerator"></param>
        public void Substract(Polynomial numerator)
        {
            this.SetContent(Operate(this, new AlgebraicFraction(numerator), this.substract));
        }

        /// <summary>
        /// Substracts a fraction
        /// </summary>
        /// <param name="algebraicFraction"></param>
        public void Substract(AlgebraicFraction algebraicFraction)
        {
            this.SetContent(Operate(this, algebraicFraction, this.substract));
        }

        /// <summary>
        /// Substracts some fractions
        /// </summary>
        /// <param name="fractions"></param>
        public void Substract(IEnumerable<AlgebraicFraction> fractions)
        {
            this.SetContent(Operate(fractions.Union(new[] { this }), this.substract));
        }

        /// <summary>
        /// Multiplies a algebraical fraction with the numerator provided and denominator = 1
        /// </summary>
        /// <param name="numerator"></param>
        public void Multiply(Polynomial numerator)
        {
            this.SetContent(Operate(this, new AlgebraicFraction(numerator), this.multiply));
        }

        /// <summary>
        /// Multiplies a fraction
        /// </summary>
        /// <param name="algebraicFraction"></param>
        public void Multiply(AlgebraicFraction algebraicFraction)
        {
            this.SetContent(Operate(this, algebraicFraction, this.multiply));
        }

        /// <summary>
        /// Multiplies some fractions
        /// </summary>
        /// <param name="fractions"></param>
        public void Multiply(IEnumerable<AlgebraicFraction> fractions)
        {
            this.SetContent(Operate(fractions.Union(new[] { this }), this.multiply));
        }

        /// <summary>
        /// Divides a algebraical fraction with the numerator provided and denominator = 1
        /// </summary>
        /// <param name="numerator"></param>
        public void Divide(Polynomial numerator)
        {
            this.SetContent(Operate(this, new AlgebraicFraction(numerator), this.divide));
        }

        /// <summary>
        /// Divides a fraction
        /// </summary>
        /// <param name="algebraicFraction"></param>
        public void Divide(AlgebraicFraction algebraicFraction)
        {
            this.SetContent(Operate(this, algebraicFraction, this.divide));
        }

        /// <summary>
        /// Divides some fractions
        /// </summary>
        /// <param name="fractions"></param>
        public void Divide(IEnumerable<AlgebraicFraction> fractions)
        {
            this.SetContent(Operate(fractions.Union(new[] { this }), this.divide));
        }

        /// <summary>
        /// Gets the mcm of the denominators's fractions
        /// </summary>
        /// <param name="af1"></param>
        /// <returns></returns>
        private Polynomial GetMcm(AlgebraicFraction af1)
        {
            // Calculate the mcm of the denominators obtaining a new denominator
            Polynomial oldDenominator = af1.Denominator;

            var arrayController = ArrayController.GetInstance();

            af1.Denominator = arrayController.GetMcm(
                    new[]
                    {
                        af1.Denominator,
                        this.Denominator
                    }
                ).GetExplicitPolynomial();

            return oldDenominator;
        }

        /// <summary>
        /// Returns the instance of the object as a string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Concat("(", this.Numerator.ToString(), ")/(", this.Denominator.ToString(), ")");
        }
    }
}
