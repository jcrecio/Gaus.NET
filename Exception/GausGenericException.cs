namespace GausNET.Exception
{
    using System;

    /// <summary>
    /// The GAUS generic exception.
    /// </summary>
    public class GausGenericException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GausGenericException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message of the exception.
        /// </param>
        public GausGenericException(string message)
            : base(message)
        {
        }
    }
}
