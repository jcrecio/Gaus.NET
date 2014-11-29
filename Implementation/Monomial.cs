namespace GausNET.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Contract;

    using GausNET.Exception;
    using GausNET.Regex;
    using Services;

    /// <summary>
    /// The monomial represents the mathematical object with the same name.
    /// </summary>
    [Serializable]
    public class Monomial : Operable
    {
        protected delegate Monomial Operation(IEnumerable<Monomial> args);

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
        /// Gets the variables compounding the monomial.
        /// </summary>
        public Dictionary<string, Variable> Variables { get; private set; }

        /// <summary>
        /// Disposes the object.
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
                foreach (var v in this.Variables.Values)
                {
                    v.Dispose();
                }

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
        /// Initializes a new instance of the <see cref="Monomial"/> class.
        /// </summary>
        public Monomial()
        {
            this._Add = this.BaseAdd;
            this._Substract = this.BaseSubstract;
            this._Multiply = this.BaseMultiply;
            this._Divide = this.BaseDivide;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Monomial"/> class.
        /// </summary>
        /// <param name="n">
        /// The coefficient of the monomial.
        /// </param>
        public Monomial(double n)
            : this()
        {
            this.Coefficient = n;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Monomial"/> class.
        /// </summary>
        /// <param name="monomialString">
        /// The monomial math expression.
        /// </param>
        /// <exception cref="GausGenericException">
        /// </exception>
        public Monomial(string monomialString)
            : this()
        {
            this.Variables = new Dictionary<string, Variable>();

            const string pattern = Patterns.Monomial;

            var re = new Regex(pattern);
            var ms = re.Matches(monomialString);

            var coefficientString = new Regex(Patterns.CoefficientMonomial).Match(monomialString).Value;

            try
            {
                this.Coefficient = Convert.ToDouble(coefficientString);
            }
            catch
            {
                this.Coefficient = Convert.ToDouble(string.Concat(coefficientString, 1));
            }

            foreach (Match cve in ms)
            {
                string[] array = cve.Value.Split('^');
                string charac = array[0];
                string exponent;
                try
                {
                    exponent = array[1];
                }
                catch
                {
                    exponent = "1";
                }

                try
                {
                    this.Variables.Add(charac, new Variable(charac, Convert.ToDouble(exponent)));
                }
                catch
                {
                    if (array[1] == null)
                    {
                        throw new GausGenericException("Group the constant values");
                    }

                    throw new GausGenericException("A monomial should not contain repeatead variables, just gather all of it with the certain exponent");
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Monomial"/> class.
        /// </summary>
        /// <param name="var">
        /// A literal variable representing a variable with exponent equal to 1.
        /// </param>
        public Monomial(char var)
            : this(1, new Variable(var, 1))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Monomial"/> class.
        /// </summary>
        /// <param name="var">
        /// The variable of the monomial.
        /// </param>
        public Monomial(Variable var)
            : this(1)
        {
            this.Variables.Add(var.LiteralToString(), var);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Monomial"/> class.
        /// </summary>
        /// <param name="coefficient">
        /// The coefficient of the monomial.
        /// </param>
        /// <param name="var">
        /// The literal's variable of the monomial.
        /// </param>
        public Monomial(double coefficient, char var)
            : this(coefficient, new Variable(var, 1))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Monomial"/> class.
        /// </summary>
        /// <param name="coefficient">
        /// The coefficient of the monomial.
        /// </param>
        /// <param>
        /// The  variable of the monomial.
        ///     <name>var</name>
        /// </param>
        public Monomial(double coefficient, Variable variable)
            : this(coefficient)
        {
            this.Variables.Add(variable.LiteralToString(), variable);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Monomial"/> class.
        /// </summary>
        /// <param name="coefficient">
        /// The coefficient of the monomial.
        /// </param>
        /// <param name="variables">
        /// The variables of the monomial.
        /// </param>
        public Monomial(double coefficient, IEnumerable<Variable> variables)
            : this(coefficient)
        {
            foreach (Variable v in variables)
            {
                this.Variables.Add(v.LiteralToString(), v);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Monomial"/> class.
        /// </summary>
        /// <param name="coefficient">
        /// The coefficient of the monomial.
        /// </param>
        /// <param name="variables">
        /// Dictionary value-key: math expression: variable object
        /// </param>
        public Monomial(double coefficient, Dictionary<string, Variable> variables)
            : this(coefficient)
        {
            this.Variables = variables;
        }

        /// <summary>
        /// Applies an operation over a monomial and a operable object.
        /// </summary>
        /// <param name="argument1">
        /// The argument monomial 1.
        /// </param>
        /// <param name="argument2">
        /// The argument operable 2.
        /// </param>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <returns>
        /// The <see cref="Monomial"/>.
        /// </returns>
        private static Monomial Operate(Monomial argument1, IOperable argument2, Operation operation)
        {
            return operation(new[] { argument1, new Monomial(argument2.Coefficient) });
        }

        /// <summary>
        /// Function to support the public operator for the operation on two monomials
        /// </summary>
        /// <param name="argument1">
        /// The argument monomial 1.
        /// </param>
        /// <param name="argument2">
        /// The argument monomial 2.
        /// </param>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <returns>
        /// The <see cref="Monomial"/>.
        /// </returns>
        private static Monomial Operate(Monomial argument1, Monomial argument2, Operation operation)
        {
            return operation(new[] { argument1, argument2 });
        }

        /// <summary>
        /// Function to support the public operator for the operation over some monomials
        /// </summary>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <returns>
        /// The <see cref="Monomial"/>.
        /// </returns>
        private static Monomial Operate(IEnumerable<Monomial> arguments, Operation operation)
        {
            return operation(arguments);
        }

        /// <summary>
        /// Returns true if the variable exists in the monomial.
        /// </summary>
        /// <param name="variable">
        /// The variable.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ContainsVariable(Variable variable)
        {
            return this.Variables.Any(v => v.Value.Literal.Equals(variable.Literal));
        }

        /// <summary>
        /// Returns true if the monomial is multiple of the one passed by argument.
        /// </summary>
        /// <param name="monomial">
        /// The monomial.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsMultiple(Monomial monomial)
        {
            var monomialVariablesLiteral = (from var in monomial.Variables.Values select var.Literal).OrderBy(v => v);
            var variablesLiteral = from var in this.Variables.Values select var.Literal;

            return variablesLiteral.OrderBy(v => v).Intersect(monomialVariablesLiteral).Equals(monomialVariablesLiteral);
        }

        /// <summary>
        /// Returns whether the monomials have the same variables.
        /// </summary>
        /// <param name="m">
        /// The monomial.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasSameVariables(Monomial m)
        {
            if (m.Variables.Count != this.Variables.Count)
            {
                return false;
            }

            foreach (KeyValuePair<string, Variable> kv in this.Variables)
            {
                if (m.Variables.ContainsKey(kv.Key))
                {
                    return !m.Variables[kv.Key].Equals(kv.Value);
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the monomials are equal.
        /// </summary>
        /// <param name="monomial"></param>
        /// <returns></returns>
        public bool Equals(Monomial monomial)
        {
            return this.Coefficient.Equals(monomial.Coefficient) && this.HasSameVariables(monomial);
        }

        /// <summary>
        /// Determines whether the monomial is equal to the object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>bool</returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as Monomial);
        }

        /// <summary>
        /// Returns the addition of two monomials.
        /// </summary>
        /// <param name="monomial1">
        /// The monomial 1.
        /// </param>
        /// <param name="monomial2">
        /// The monomial 2.
        /// </param>
        /// <returns>
        /// Monomial
        /// </returns>
        /// <exception cref="GausGenericException">
        /// </exception>
        public static Monomial operator +(Monomial monomial1, Monomial monomial2)
        {
            if (!monomial1.HasSameVariables(monomial1))
            {
                throw new GausGenericException("They have different base variables!");
            }

            return new Monomial(monomial1.Coefficient + monomial2.Coefficient, monomial1.Variables);
        }

        /// <summary>
        /// Function to support the public operator for the addition of a list of monomials.
        /// </summary>
        /// <param name="operands">
        /// The operands.
        /// </param>
        /// <returns>
        /// The <see cref="Monomial"/>.
        /// </returns>
        private Monomial BaseAdd(IEnumerable<Monomial> operands)
        {
            var enumerable = operands as Monomial[] ?? operands.ToArray();
            var n = new Monomial(0, enumerable.First().Variables);

            return enumerable.Aggregate(n, (current, operand) => current + operand);
        }

        /// <summary>
        /// Returns the opposite monomial.
        /// </summary>
        /// <param name="monomial">
        /// The monomial.
        /// </param>
        /// <returns>
        /// </returns>
        public static Monomial operator -(Monomial monomial)
        {
            return new Monomial(-monomial.Coefficient, monomial.Variables);
        }

        /// <summary>
        /// Substracts two monomials.
        /// </summary>
        /// <param name="monomial1">
        /// The monomial 1.
        /// </param>
        /// <param name="monomial2">
        /// The monomial 2.
        /// </param>
        /// <returns>
        /// Monomial
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        public static Monomial operator -(Monomial monomial1, Monomial monomial2)
        {
            if (!monomial1.HasSameVariables(monomial2))
            {
                throw new Exception("They have different base variables!");
            }

            return new Monomial(monomial1.Coefficient - monomial2.Coefficient, monomial1.Variables);
        }

        /// <summary>
        /// Function to support the public operator for the substraction of a list of monomials.
        /// </summary>
        /// <param name="operands">
        /// The operands.
        /// </param>
        /// <returns>
        /// The <see cref="Monomial"/>.
        /// </returns>
        private Monomial BaseSubstract(IEnumerable<Monomial> operands)
        {
            var enumerable = operands as Monomial[] ?? operands.ToArray();
            Monomial n = new Monomial(0, enumerable.First().Variables);

            return enumerable.Aggregate(n, (current, operand) => current - operand);
        }

        /// <summary>
        /// Multiplies a scalar per a monomial.
        /// </summary>
        /// <param name="monomial">
        /// The monomial.
        /// </param>
        /// <param name="d">
        /// The scalar d.
        /// </param>
        /// <returns>
        /// Monomial
        /// </returns>
        public static Monomial operator *(Monomial monomial, double d)
        {
            return new Monomial(monomial.Coefficient * d, monomial.Variables);
        }

        /// <summary>
        /// Multiplies an monomial per a operable.
        /// </summary>
        /// <param name="monomial">
        /// The monomial.
        /// </param>
        /// <param name="operable">
        /// The operable.
        /// </param>
        /// <returns>
        /// Monomial
        /// </returns>
        public static Monomial operator *(Monomial monomial, IOperable operable)
        {
            return new Monomial(monomial.Coefficient * operable.Coefficient, monomial.Variables);
        }

        /// <summary>
        /// Function to support the public operator for the substraction of a list of monomials.
        /// </summary>
        /// <param name="monomial1">
        /// The monomial 1.
        /// </param>
        /// <param name="monomial2">
        /// The monomial 2.
        /// </param>
        /// <returns>
        /// </returns>
        public static Monomial operator *(Monomial monomial1, Monomial monomial2)
        {
            Monomial m1 = monomial1.Clone();
            m1.Coefficient *= monomial2.Coefficient;

            foreach (KeyValuePair<string, Variable> vari in monomial2.Variables)
            {
                if (m1.Variables.ContainsKey(vari.Key))
                {
                    double value = vari.Value.Exponent + m1.Variables[vari.Key].Exponent;

                    if (value.Equals(0))
                    {
                        m1.Variables[vari.Key].Exponent = value;
                    }
                    else
                    {
                        m1.Variables.Remove(vari.Key);
                    }
                }
            }

            var varsRemaining = monomial2.Variables.Keys.Except(m1.Variables.Keys);
            foreach (string vari in varsRemaining)
            {
                m1.Variables.Add(vari, monomial2.Variables[vari]);
            }

            return m1;
        }

        /// <summary>
        /// Function to support the public operator for the multiplication of a list of monomials.
        /// </summary>
        /// <param name="operands">
        /// The operands.
        /// </param>
        /// <returns>
        /// The <see cref="Monomial"/>.
        /// </returns>
        private Monomial BaseMultiply(IEnumerable<Monomial> operands)
        {
            Monomial m = null;
            foreach (var operand in operands)
            {
                if (m == null)
                {
                    m.Coefficient = operand.Coefficient;
                    m.Variables = operand.Variables;
                }
                else
                {
                    m *= operand;
                }
            }

            return m;
        }

        /// <summary>
        /// Divides an monomial per a scalar.
        /// </summary>
        /// <param name="monomial">
        /// The monomial.
        /// </param>
        /// <param name="scalar">
        /// The scalar.
        /// </param>
        /// <returns>
        /// Monomial
        /// </returns>
        public static Monomial operator /(Monomial monomial, double scalar)
        {
            return new Monomial(monomial.Coefficient / scalar, monomial.Variables);
        }

        /// <summary>
        /// Divides an monomial per a operable.
        /// </summary>
        /// <param name="monomial">
        /// The monomial.
        /// </param>
        /// <param name="operable">
        /// The scalar.
        /// </param>
        /// <returns>
        /// Monomial
        /// </returns>
        public static Monomial operator /(Monomial monomial, IOperable operable)
        {
            return new Monomial(monomial.Coefficient / operable.Coefficient, monomial.Variables);
        }

        /// <summary>
        /// Divides an monomial per a monomial.
        /// </summary>
        /// <param name="monomial1">
        /// The monomial 1.
        /// </param>
        /// <param name="m2">
        /// The monomial 2.
        /// </param>
        /// <returns>
        /// </returns>
        public static Monomial operator /(Monomial monomial1, Monomial m2)
        {
            Monomial m1 = monomial1.Clone();
            m1.Coefficient /= m2.Coefficient;

            foreach (KeyValuePair<string, Variable> kvp in m2.Variables)
            {
                if (m1.Variables.ContainsKey(kvp.Key))
                {
                    double value = m1.Variables[kvp.Key].Exponent - kvp.Value.Exponent;
                    if (value.Equals(0))
                    {
                        m1.Variables.Remove(kvp.Key);
                    }
                    else
                    {
                        m1.Variables[kvp.Key].Exponent = value;
                    }
                }
                else
                {
                    Variable newvar = kvp.Value.Clone();
                    newvar.Exponent = -newvar.Exponent;
                    m1.Variables.Add(kvp.Key, newvar);
                }
            }

            return m1;
        }

        /// <summary>
        /// Function to support the public operator for the division of a list of monomials.
        /// </summary>
        /// <param name="operands">
        /// The operands.
        /// </param>
        /// <returns>
        /// The <see cref="Monomial"/>.
        /// </returns>
        private Monomial BaseDivide(IEnumerable<Monomial> operands)
        {
            Monomial m = null;
            foreach (var operand in operands)
            {
                if (m == null)
                {
                    m.Coefficient = operand.Coefficient;
                    m.Variables = operand.Variables;
                }
                else
                {
                    m /= operand;
                }
            }

            return m;
        }

        /// <summary>
        /// Adds a monomial.
        /// </summary>
        /// <param name="m">
        /// The monomial.
        /// </param>
        public void Add(Monomial m)
        {
            this.SetContent(Operate(this, m, this._Add));
        }

        /// <summary>
        /// Adds some monomials.
        /// </summary>
        /// <param name="mons">
        /// The monomials.
        /// </param>
        public void Add(IEnumerable<Monomial> mons)
        {
            this.SetContent(Operate(mons.Union(new[] { this }), this._Add));
        }

        /// <summary>
        /// Subtracts a monomial.
        /// </summary>
        /// <param name="monomial">
        /// The monomial.
        /// </param>
        public void Substract(Monomial monomial)
        {
            this.SetContent(Operate(this, monomial, this._Substract));
        }

        /// <summary>
        /// Subtracts some monomials.
        /// </summary>
        /// <param name="mons">
        /// The monomials.
        /// </param>
        public void Substract(IEnumerable<Monomial> mons)
        {
            this.SetContent(Operate(mons.Union(new[] { this }), this._Substract));
        }

        /// <summary>
        /// Multiplies a monomial per an scalar.
        /// </summary>
        /// <param name="scalar">
        /// The scalar.
        /// </param>
        public void Multiply(double scalar)
        {
            this.Coefficient *= scalar;
        }

        /// <summary>
        /// Multiplies a monomial per an operable.
        /// </summary>
        /// <param name="operable">
        /// The operable.
        /// </param>
        public void Multiply(IOperable operable)
        {
            this.SetContent(Operate(this, operable, this._Multiply));
        }

        /// <summary>
        /// Multiplies a monomial.
        /// </summary>
        /// <param name="monomial">
        /// The monomial.
        /// </param>
        public void Multiply(Monomial monomial)
        {
            this.SetContent(Operate(this, monomial, this._Multiply));
        }

        /// <summary>
        /// Multiplies some monomials.
        /// </summary>
        /// <param name="mons">
        /// The monomialss.
        /// </param>
        public void Multiply(IEnumerable<Monomial> monomialCollection)
        {
            this.SetContent(Operate(monomialCollection.Union(new[] { this }), this._Multiply));
        }

        /// <summary>
        /// Performs a division using a monomial with coefficient 1 and the variable provided.
        /// </summary>
        /// <param name="variable">
        /// The variable.
        /// </param>
        public void Divide(Variable variable)
        {
            if (this.ContainsVariable(variable))
            {
                string key = variable.LiteralToString();
                double exp = this.Variables[key].Exponent - variable.Exponent;

                if (exp.Equals(0))
                {
                    this.Variables.Remove(key);
                }
                else
                {
                    this.Variables[variable.LiteralToString()].Exponent = exp;
                }
            }
        }

        /// <summary>
        /// Performs a multiplication using a monomial with coefficient 1 and the variable provided.
        /// </summary>
        /// <param name="variable">
        /// The variable.
        /// </param>
        public void Multiply(Variable variable)
        {
            if (this.ContainsVariable(variable))
            {
                string key = variable.LiteralToString();
                double exp = this.Variables[key].Exponent + variable.Exponent;
                if (exp.Equals(0))
                {
                    this.Variables.Remove(key);
                }
                else
                {
                    this.Variables[variable.LiteralToString()].Exponent = exp;
                }
            }
            else
            {
                this.Variables.Add(variable.LiteralToString(), variable);
            }
        }

        /// <summary>
        /// Divides per an scalar.
        /// </summary>
        /// <param name="scalar">
        /// The scalar.
        /// </param>
        /// <exception cref="GausGenericException">
        /// </exception>
        public void Divide(double scalar)
        {
            if (scalar.Equals(0))
            {
                throw new GausGenericException("Cannot divide per zero");
            }

            this.Coefficient /= scalar;
        }

        /// <summary>
        /// Divides per an operable.
        /// </summary>
        /// <param name="operable">
        /// The operable.
        /// </param>
        public void Divide(IOperable operable)
        {
            this.SetContent(Operate(this, operable, this._Divide));
        }

        /// <summary>
        /// Divides per a monomial.
        /// </summary>
        /// <param name="monomial">
        /// The monomial.
        /// </param>
        public void Divide(Monomial monomial)
        {
            this.SetContent(Operate(this, monomial, this._Divide));
        }

        /// <summary>
        /// Divides per some monomials.
        /// </summary>
        /// <param name="monomialCollection">
        /// The monomials.
        /// </param>
        public void Divide(IEnumerable<Monomial> monomialCollection)
        {
            this.SetContent(Operate(monomialCollection.Union(new[] { this }), this._Divide));
        }

        /// <summary>
        /// Pow Operator between a monomial and a given real exponent.
        /// </summary>
        /// <param name="monomial">
        /// The monomial.
        /// </param>
        /// <param name="exponent">
        /// The real exponent.
        /// </param>
        /// <returns>
        /// Monomial
        /// </returns>
        public static Monomial operator ^(Monomial monomial, int exponent)
        {
            Monomial m1 = monomial.Clone();
            if (exponent > 1)
            {
                for (var i = 0; i < exponent; i++)
                {
                    m1 *= monomial;
                }
            }

            return m1;
        }

        /// <summary>
        /// Pow Operation between a monomial and a given integer exponent.
        /// </summary>
        /// <param name="exponent">
        /// The exponent.
        /// </param>
        public void Pow(Int32 exponent)
        {
            Monomial m = this ^ exponent;
            this.Variables = m.Variables;
            this.Coefficient = m.Coefficient;
        }

        /// <summary>
        /// Pow Operation between a monomial and a given operable exponent.
        /// </summary>
        /// <param name="operable">
        /// The operable.
        /// </param>
        public void Pow(IOperable operable)
        {
            this.Pow(Convert.ToInt32(operable.Coefficient));
        }

        /// <summary>
        /// Gets the opposite or the minus monomial.
        /// </summary>
        /// <returns>
        /// The <see cref="Monomial"/>.
        /// </returns>
        public Monomial ReverseOf()
        {
            var arrayController = ArrayController.GetInstance();
            return new Monomial(-this.Coefficient, arrayController.CloneDictionary(this.Variables));
        }

        /// <summary>
        /// Gets the inverse expression of the monomial. The results object keeps being a monomial object but it doesn't fit the meaning of a monomial concept.
        /// inverseOf(monomial) = 1/monomial -> if monomial is a monomial then 1/monomial shouldn't be so.
        /// </summary>
        /// <returns>
        /// The <see cref="Monomial"/>.
        /// </returns>
        public Monomial InverseOf()
        {
            var m = this.Clone();
            m.Coefficient = 1 / m.Coefficient;

            foreach (var kvp in m.Variables)
            {
                kvp.Value.Exponent = -kvp.Value.Exponent;
            }

            return m;
        }

        /// <summary>
        /// Gets the grade of the monomial.
        /// </summary>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public override double GetGrade()
        {
            return this.Variables.Values.Count == 0 ? 0 : (from exp in this.Variables.Values select exp.Exponent).Max();
        }

        /// <summary>
        /// Gets the variables of the monomial into a string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string VariablesToString()
        {
            var sb = new StringBuilder();
            foreach (var kvp in this.Variables)
            {
                sb.Append(string.Concat(kvp.Value.LiteralToString(), kvp.Value.Exponent.ToString(CultureInfo.InvariantCulture)));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets a value indicating whether the monomial is zero.
        /// </summary>
        public override bool IsZero
        {
            get { return this.Coefficient.Equals(0); }
        }

        /// <summary>
        /// Sets the indicating content to the current monomial.
        /// </summary>
        /// <param name="operable">
        /// The new content operable.
        /// </param>
        public override void SetContent(IOperable operable)
        {
            this.SetContent(operable as Monomial);
        }

        /// <summary>
        /// Sets the indicating content to the current object.
        /// </summary>
        /// <param name="monomial">
        /// The monomial.
        /// </param>
        private void SetContent(Monomial monomial)
        {
            this.Coefficient = monomial.Coefficient;
            this.Variables = monomial.Variables;
        }

        /// <summary>
        /// Clones the current object.
        /// </summary>
        /// <returns>
        /// The <see cref="Monomial"/>.
        /// </returns>
        public Monomial Clone()
        {
            var arrayController = ArrayController.GetInstance();
            return new Monomial(this.Coefficient, arrayController.CloneDictionary(this.Variables));
        }

        /// <summary>
        /// Returns a clone of the current monomial's variables.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<Variable> CloneVariables()
        {
            return from Variable v in this.Variables.Values select new Variable(v.Literal, v.Exponent);
        }

        /// <summary>
        /// Returns the instance of the object as a string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var kv in this.Variables)
            {
                sb.Append(kv.Value);
            }

            return string.Concat(!this.Coefficient.Equals(1) ? this.Coefficient.ToString(CultureInfo.InvariantCulture) : string.Empty, sb.ToString());
        }
    }
}
