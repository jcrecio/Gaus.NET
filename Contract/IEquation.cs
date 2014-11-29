namespace GausNET.Contract
{
    using System.Collections.Generic;

    /// <summary>
    /// The Equation interface. Represents a generic equation.
    /// </summary>
    public interface IEquation
    {
        /// <summary>
        /// Gets the solutions for an object which implements the IEquation interface.
        /// </summary>
        /// <returns>
        /// Solutions for the equation.
        /// </returns>
        IEnumerable<double> GetSolutions();

        /// <summary>
        /// Gets the variables in the equation.
        /// </summary>
        /// <returns>
        /// Variables compounding the equation.
        /// </returns>
        IEnumerable<char> GetVariables();

        /// <summary>
        /// Gets the number of variables in the equation.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int GetNumberDisctinctVariables();

        /// <summary>
        /// Gets a value indicating whether the equation has solution/s
        /// </summary>
        bool HasSolution { get; }
    }
}
