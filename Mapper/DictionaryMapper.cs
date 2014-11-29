namespace GausNET.Mapper
{
    using GausNET.Implementation;
    using System.Collections.Generic;

    /// <summary>
    /// Maps between different objects
    /// </summary>
    public class DictionaryMapper
    {
        /// <summary>
        /// Turnst a dictionary roots to a polynomial roots
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public PolynomialRoots DictionaryRootsToPolynomialRoots(Dictionary<double, int> dic)
        {
            var polynomialRoots = new PolynomialRoots();
            foreach (var kvp in dic)
            {
                polynomialRoots.InsertRoot(kvp);
            }

            return polynomialRoots;
        }
    }
}