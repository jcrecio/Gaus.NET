namespace GausNET.Implementation
{
    using System;
    using System.Globalization;
    using Contract;

    using GausNET.Exception;
    using GausNET.Extension;

    /// <summary>
    /// The variable class is in charge to stand for a typical math variable.
    /// </summary>
    public class Variable: IDisposable, IVariable
    {
        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Literal.GetHashCode()*397) ^ this.Exponent.GetHashCode();
            }
        }

        /// <summary>
        /// Gets or sets the literal of the variable.
        /// </summary>
        public char Literal { get; set; }

        /// <summary>
        /// Gets or sets the exponent applied on the variable.
        /// </summary>
        public double Exponent { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Variable"/> class.
        /// </summary>
        /// <param name="variableString">
        /// The variable math expression.
        /// </param>
        /// <exception cref="GausGenericException">
        /// </exception>
        public Variable(string variableString)
        {
            if (variableString.IsNumeric()) throw new GausGenericException("A variable must be build with a character");
            try
            {
                string[] parts = variableString.Split('^');
                this.Literal = Convert.ToChar(parts[0]);

                if (parts.Length > 1)
                {
                    this.Exponent = Convert.ToDouble(parts[1]);
                }
            }
            catch
            {
                throw new GausGenericException("Variable bad initialized. It should be var^exponent, f.i.:  x^5");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Variable"/> class.
        /// </summary>
        /// <param name="literal">
        /// The variable literal.
        /// </param>
        /// <param name="exponent">
        /// The exponent.
        /// </param>
        public Variable(char literal, double exponent)
        {
            this.Literal = literal;
            this.Exponent = exponent;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Variable"/> class.
        /// </summary>
        /// <param name="literal">
        /// The variable literal.
        /// </param>
        /// <param name="exponent">
        /// The exponent.
        /// </param>
        public Variable(string literal, double exponent)
        {
            this.Literal = literal[0];
            this.Exponent = exponent;
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Returns whether the variable is the same.
        /// </summary>
        /// <param name="variable">
        /// The v.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Equals(IVariable variable)
        {
            return this.Literal.Equals(variable.Literal) && this.Exponent.Equals(variable.Exponent);
        }

        /// <summary>
        /// Returns whether the variable is the same as the object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var variable = obj as Variable;
            if (variable == null) throw new GausGenericException("The object is not a variable");

            return this.Equals(variable);
        }

        /// <summary>
        /// Returns the variable literal to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string LiteralToString()
        {
            return this.Literal.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// The clone.
        /// </summary>
        /// <returns>
        /// The <see cref="Variable"/>.
        /// </returns>
        public Variable Clone()
        {
            return new Variable(this.Literal, this.Exponent);
        }

        /// <summary>
        /// Returns the instance as a string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Concat(this.Literal, this.Exponent.Equals(1) ? this.Exponent.ToString(CultureInfo.InvariantCulture) : string.Empty);
        }
    }
}