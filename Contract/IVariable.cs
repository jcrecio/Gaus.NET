namespace GausNET.Contract
{
    /// <summary>
    /// The Variable interface.
    /// </summary>
    public interface IVariable
    {
        /// <summary>
        /// Gets or sets the literal representing the variable, such as 'x', 'y', etc
        /// </summary>
        char Literal { get; set; }

        /// <summary>
        /// Gets or sets the exponent applied over the variable.
        /// </summary>
        double Exponent { get; set; }

        /// <summary>
        /// Compares two variables.
        /// </summary>
        /// <param name="variable">
        /// The v.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool Equals(IVariable variable);

        /// <summary>
        /// Turns literal of the variable into a string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string LiteralToString();
    }
}
