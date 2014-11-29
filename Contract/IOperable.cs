namespace GausNET.Contract
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The Operable interface. It defines the way the operable elements such as monomials, polynomials and so on behave.
    /// </summary>
    public interface IOperable
    {
        /// <summary>
        /// Gets or sets the coefficient.
        /// </summary>
        double Coefficient { get; set; }

        /// <summary>
        /// Gets a value indicating whether the element is zero.
        /// </summary>
        bool IsZero { get; }

        /// <summary>
        /// Sets the indicating content to the current object.
        /// </summary>
        /// <param name="operable">
        /// The new operable.
        /// </param>
        void SetContent(IOperable operable);

        /// <summary>
        /// Gets the grade of the element.
        /// </summary>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        double GetGrade();

        /// <summary>
        /// Applies the operation indicated between the current element and the one passed by argument
        /// </summary>
        /// <param name="argument">
        /// The argument.
        /// </param>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <returns>
        /// The <see cref="IOperable"/>.
        /// </returns>
        IOperable Operate(IOperable argument, Func<IEnumerable<IOperable>, IOperable> operation);

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
        /// The <see cref="IOperable"/>.
        /// </returns>
        IOperable Operate(IEnumerable<IOperable> arguments, Func<IEnumerable<IOperable>, IOperable> operation);
    }
}
