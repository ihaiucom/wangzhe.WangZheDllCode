
namespace Photon.LoadBalancing.Common
{
    using System.Collections;
    using System.Collections.Generic;

    public static class DictionaryExtensions
    {
        public static bool IsSubsetOf(this Hashtable hashtable, Dictionary<object, object> target)
        {
            // empty dictionary is always a subset
            if (hashtable.Count == 0)
            {
                return true;
            }

            // if hashtable provides more entries as the target
            // it cannot be a subset of the target
            if (hashtable.Count > target.Count)
            {
                return false;
            }

            // check if each entry in the hahstable is also
            // present in the target dictionary and values of 
            // the both entries are equal
            foreach (DictionaryEntry entry in hashtable)
            {
                object targetValue;
                if (!target.TryGetValue(entry.Key, out targetValue))
                {
                    return false;
                }

                if (entry.Value != null)
                {
                    if (entry.Value.Equals(targetValue) == false)
                    {
                        return false;
                    }
                }
                else
                {
                    if (targetValue != null)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool Equals(Hashtable h1, Hashtable h2)
        {
            if (h1.Count != h2.Count)
            {
                return false;
            }

            foreach (DictionaryEntry entry in h1)
            {
                if (!h2.ContainsKey(entry.Key))
                {
                    return false;
                }

                var value = h2[entry.Key];
                if (entry.Value != null && value != null)
                {
                    if (entry.Value.Equals(value) == false)
                    {
                        return false;
                    }
                }
                else
                {
                    if (!(entry.Value == null && value == null))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static int GetHashCode(Hashtable h)
        {
            if (h.Count == 0)
            {
                return 0;
            }

            int result = 0;
            foreach (DictionaryEntry entry in h)
            {
                var keyHash = entry.Key.GetHashCode();
                result ^= (result << 5) + (result >> 2) + keyHash;

                var valueHash = entry.Value == null ? 0 : entry.Value.GetHashCode();
                result ^= (result << 5) + (result >> 2) + valueHash;
            }

            return result;
        }

        /// <summary>
        /// Determines whether a Dictionary object is a subset of another Dictionary.
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsSubsetOf<TKey, TValue>(this Dictionary<TKey, TValue> dict, Dictionary<TKey, TValue> target) where TValue : class 
        {
            // emty dictionary is always a subset
            if (dict.Count == 0)
            {
                return true;
            }

            // if dictionary provides more entries as the target
            // it cannot be a subset of the target
            if (dict.Count > target.Count)
            {
                return false;
            }

            // check if each entry in the dictionary is also
            // present in the target dictionary and values of 
            // the both entries are equal
            foreach (var entry in dict)
            {
                TValue targetValue;
                if (!target.TryGetValue(entry.Key, out targetValue))
                {
                    return false;
                }

                if (entry.Value != null)
                {
                    if (entry.Value.Equals(targetValue) == false)
                    {
                        return false;
                    }
                }
                else
                {
                    if (targetValue != null)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

    }
}
