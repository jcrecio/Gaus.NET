namespace GausNET.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using GausNET;
    using System.Linq;
    using Contract;
    using GausNET.Regex;

    /// <summary>
    /// The polynomialRoots class represents a polynomial defined by its roots.
    /// </summary>
    [Serializable]
    public class PolynomialRoots : Operable //(x-1)(y-2)^3(z+3)^(1/2)
    {
        /// <summary>
        /// Disposes the object.
        /// </summary>
        protected void Dispose(bool disposing)
        {
            if (this.Disposed)
                return;

            if (disposing)
            {
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
        /// Dictionary key - value representing each root
        /// </summary>
        public Dictionary<Double, Int32> RootAndExponents { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PolynomialRoots"/> class.
        /// </summary>
        public PolynomialRoots()
        {
            this.RootAndExponents = new Dictionary<double, int>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PolynomialRoots"/> class.
        /// </summary>
        /// <param name="rootAndExponentSet"></param>
        public PolynomialRoots(Dictionary<double, int> rootAndExponentSet)
            : this()
        {
            rootAndExponentSet.Keys.ToList().ForEach(key => this.InsertRoot(key, rootAndExponentSet[key]));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PolynomialRoots"/> class.
        /// </summary>
        /// <param name="polynomialRootsString">
        /// The math expression
        /// </param>
        public PolynomialRoots(string polynomialRootsString)
            : this()
        {
            const string pattern = Patterns.PolynomialRoots;
            var re = new Regex(pattern);
            MatchCollection roots = re.Matches(polynomialRootsString);

            foreach (var root in roots)
            {
                string[] rootString = root.ToString().Split('^');

                int exponent = rootString.Length == 1 ? 1 : Convert.ToInt32(rootString[1]);

                string key = new Regex(Patterns.PolynomialRoots).Match(rootString[0]).Value;
                key = key.Substring(0, key.Length - 1);

                this.InsertRoot(new KeyValuePair<double, int>(Convert.ToDouble(key), exponent));
            }
        }

        /// <summary>
        /// Gets the exponent for the given root
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public int GetExponent(double root)
        {
            return this.RootAndExponents[root];
        }

        /// <summary>
        /// Inserts a root to the polynomial
        /// </summary>
        /// <param name="keyValuePair"></param>
        public void InsertRoot(KeyValuePair<double, int> keyValuePair)
        {
            this.InsertRoot(keyValuePair.Key, keyValuePair.Value);
        }

        /// <summary>
        /// Inserts a root to the polynomial
        /// </summary>
        /// <param name="root"></param>
        /// <param name="exponent"></param>
        public void InsertRoot(double root, int exponent = 1)
        {
            if (this.RootAndExponents.ContainsKey(root))
            {
                this.RootAndExponents[root] = exponent;
            }
            else
            {
                this.RootAndExponents.Add(root, exponent);
            }
        }

        /// <summary>
        /// Gets the explicit polynomial form providing the variable literal
        /// </summary>
        /// <returns>
        /// Polynomial
        /// </returns>
        public Polynomial GetExplicitPolynomial(char variableDefault = 'x')
        {
            Polynomial final = null;
            foreach (var keyValuePair in this.RootAndExponents)
            {
                Polynomial polynomial = new Polynomial(-keyValuePair.Key, new Monomial(variableDefault));
                int n = keyValuePair.Value;

                for (var i = 0; i < n; i++)
                {
                    if (final == null)
                    {
                        final = polynomial.Clone();
                    }
                    else
                    {
                        final.Multiply(polynomial);
                    }
                }
            }

            return final;
        }

        /// <summary>
        /// Determines whether the polynomial is equal.
        /// </summary>
        /// <param name="polynomial"></param>
        /// <returns>bool</returns>
        public bool Equal(PolynomialRoots polynomial)
        {
            int count = this.RootAndExponents.Count;
            return ((count == polynomial.RootAndExponents.Count) &&
                    (this.RootAndExponents.Count(pair => polynomial.RootAndExponents.Contains(pair)) == count));
        }

        /// <summary>
        /// Determines whether the polynomial is equal to the provided explicit one.
        /// </summary>
        /// <param name="polynomial"></param>
        /// <returns>bool</returns>
        public bool Equals(Polynomial polynomial)
        {
            return this.Equal(polynomial.GetRootsPolynomial());
        }

        /// <summary>
        /// Returns the instance of the object as a string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            var roots = 
                from root in this.RootAndExponents
                select new {PrintRoot = string.Concat("(x", -root.Key, ")", root.Value)};

            return string.Join(string.Empty, roots.Select(root => root.PrintRoot).ToList());
        }

        /// <summary>
        /// Gets a value indicating whether the polynomial is zero.
        /// </summary>
        public override bool IsZero
        {
            get
            {
                return new PolynomicEquation(this.GetExplicitPolynomial('x')).HasSolution;
            }
        }

        /// <summary>
        /// Sets the indicating content to the current object.
        /// </summary>
        /// <param name="operable"></param>
        public override void SetContent(IOperable operable)
        {
            this.RootAndExponents = ((PolynomialRoots)operable).RootAndExponents;
        }

        /// <summary>
        /// Returns a clone of the object
        /// </summary>
        /// <returns></returns>
        public PolynomialRoots Clone()
        {
            return new PolynomialRoots(this.RootAndExponents);
        }
    }
}