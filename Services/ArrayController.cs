namespace GausNET.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;
    using Implementation;
    using GausNET.Mapper;

    public class ArrayController
    {
        private static ArrayController _arrayController;

        private DictionaryMapper dictionaryMapper = new DictionaryMapper();

        private ArrayController(DictionaryMapper dictionaryMapper)
        {
        }

        public static ArrayController GetInstance()
        {
            return _arrayController ?? (_arrayController = new ArrayController(new DictionaryMapper()));
        }

        public Dictionary<TK, TV> CloneDictionary<TK, TV>(Dictionary<TK, TV> dict)
        {
            Dictionary<TK, TV> newDict = null;

            if (dict != null)
            {
                if (((typeof(TK).IsValueType || typeof(TK) == typeof(string)) &&
                     (typeof(TV).IsValueType) || typeof(TV) == typeof(string)))
                {
                    newDict = new Dictionary<TK, TV>();
                    foreach (var kvp in dict)
                    {
                        newDict[kvp.Key] = kvp.Value;
                    }
                }
                else
                {
                    var bf = new BinaryFormatter();
                    var ms = new MemoryStream();
                    bf.Serialize(ms, dict);
                    ms.Position = 0;
                    newDict = (Dictionary<TK, TV>)bf.Deserialize(ms);
                }
            }

            return newDict;
        }

        public Dictionary<TK, TV> SortDictionaryBy<TK, TV>(Func<TK, TV> sortFunction, Dictionary<TK, TV> dic)
        {
            return dic.OrderBy(k => sortFunction).ToDictionary(k => k.Key, k => k.Value);
        }

        public PolynomialRoots GetMcm(IEnumerable<Polynomial> polynomials)
        {
            return GetMcm(from p in polynomials select p.GetRootsPolynomial());
        }

        public PolynomialRoots GetMcm(IEnumerable<PolynomialRoots> polynomialRoots)
        {
            var polynomialSet = new Dictionary<double, int>();

            var allTheRoots = polynomialRoots.Select(p => p.RootAndExponents);

            foreach (var poly in allTheRoots)
            {
                foreach (var root in poly)
                {
                    if (polynomialSet.ContainsKey(root.Key))
                    {
                        if (root.Value > polynomialSet[root.Key])
                        {
                            polynomialSet[root.Key] = root.Value;
                        }
                    }
                    else
                    {
                        polynomialSet.Add(root.Key, root.Value);
                    }

                }
            }

            return this.dictionaryMapper.DictionaryRootsToPolynomialRoots(polynomialSet);
        }
    }
}
