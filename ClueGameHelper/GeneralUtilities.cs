using System;
using System.Collections.Generic;

namespace ClueGameHelper
{
    class GeneralUtilities
    {
        public static void AddKeyValueIntoDictFromCollection<K, V>(
            Dictionary<K, V> dictionaryToAddTo, V value, ICollection<K> collection)
        {
            foreach(K element in collection)
            {
                dictionaryToAddTo.Add(element, value);
            }
        }

        public static bool IsElementValid<T>(T element, List<T> container)
        {
            bool isSuspectValid = false;

            if (container.Contains(element))
            {
                isSuspectValid = true;
            }

            if ((!isSuspectValid) && (!element.ToString().Equals("end")))
            {
                Console.WriteLine($"{element} is not valid");
            }

            return isSuspectValid;
        }
    }
}
