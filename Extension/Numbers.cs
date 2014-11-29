namespace GausNET.Extension
{
    using System;

    /// <summary>
    /// Extension for numbers
    /// </summary>
    public static class Numbers
    {
        /// <summary>
        /// Returns whether a string is numeric
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumeric(this string value)
        {
            try
            {
                Convert.ToDouble(value);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
