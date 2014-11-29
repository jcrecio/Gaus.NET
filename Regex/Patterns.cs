namespace GausNET.Regex
{
    /// <summary>
    /// Patterns of the library
    /// </summary>
    public static class Patterns
    {
        /// <summary>
        /// Defines the expression a string must match to create a monomial
        /// </summary>
        public const string Monomial = @"[a-z](\^[1-9]+)*";

        /// <summary>
        /// Defines the expression a string must match to create a polynomial
        /// </summary>
        public const string Polynomial = @"((\+|\-)([a-z]|[0-9]|(\^[0-9]+))*)";

        /// <summary>
        /// Defines the expression a string must match to create a polynomial defined by its roots
        /// </summary>
        public const string PolynomialRoots = @"(\([a-z][+|-][0-9][1-9]*\)(\^\([1-9][0-9]*\)|\^[1-9][0-9]*)*)";

        /// <summary>
        /// Defines the expression a string must match to create an algebraical fraction
        /// </summary>
        public const string AlgebraicalFraction = @"((\+|\-)([a-z]|[0-9]|(\^[0-9]+))*)\/((\+|\-)([a-z]|[0-9]|(\^[0-9]+))*)";

        /// <summary>
        /// Defines the expression a string must match to extract the coefficient of a monomial
        /// </summary>
        public const string CoefficientMonomial = @"[0-9]*";
    }
}
