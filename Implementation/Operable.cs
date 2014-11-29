namespace GausNET.Implementation
{
    using System;
    using System.Collections.Generic;
    using Contract;

    /// <summary>
    /// The generic operable math object implementation. It provides the minimal base functionality supposed to be operable.
    /// </summary>
    [Serializable]
    public abstract class Operable : IOperable, IDisposable
    {
        /// <summary>
        /// Being disposed.
        /// </summary>
        protected bool Disposed;

        /// <summary>
        /// Gets or sets the coefficient.
        /// </summary>
        public double Coefficient { get; set; }

        /// <summary>
        /// Gets a value indicating whether the operable is zero.
        /// </summary>
        public abstract bool IsZero { get; }

        /// <summary>
        /// Operation to be applied by the object with the provided parameters.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        private delegate Operable Operation(IEnumerable<Operable> args);

        /// <summary>
        /// Sets the indicating content to the current object.
        /// </summary>
        /// <param name="operable">
        /// The new operable.
        /// </param>
        public abstract void SetContent(IOperable operable);

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }

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
        public IOperable Operate(IOperable argument, Func<IEnumerable<IOperable>, IOperable> operation)
        {
            return operation(new[] { argument });
        }

        /// <summary>
        /// Applies the operation indicated among the current element and the ones passed by argument
        /// </summary>
        /// <param name="arguments">
        /// The argument.
        /// </param>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <returns>
        /// The <see cref="IOperable"/>.
        /// </returns>
        public IOperable Operate(IEnumerable<IOperable> arguments, Func<IEnumerable<IOperable>, IOperable> operation)
        {
            return operation(arguments);
        }

        /// <summary>
        /// Gets the grade of the element.
        /// </summary>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public virtual double GetGrade()
        {
            return 0;
        }
    }
}
