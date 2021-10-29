using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports
{
    public static class Extensions
    {
        public static long ToLongUnixDateTime(this DateTime date)
        {
            return (long)date.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        public static string ToBase64String(this string str)
        {
            return Convert.ToBase64String(Encoding.Default.GetBytes(str));
        }

        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> source, out TKey Key, out TValue Value)
        {
            Key = source.Key;
            Value = source.Value;
        }

        public static void RemoveRange<TType>(this List<TType> list, IEnumerable<TType> values)
        {
            foreach(TType value in values)
            {
                list.Remove(value);
            }
        }

        public static IEnumerable<TValue> Distinct<TValue>(this IEnumerable<TValue> values, 
            Func<TValue, TValue, bool> equalityComparer)
        {
            List<TValue> distinctValues = new List<TValue>();

            foreach(TValue value in values)
            {
                bool distinct = true;
                foreach(TValue distinctValue in distinctValues)
                {
                    if(equalityComparer(value, distinctValue))
                    {
                        distinct = false;
                        break;
                    }
                }

                if(distinct)
                {
                    distinctValues.Add(value);
                }
            }

            return distinctValues;
        }
    }
}
