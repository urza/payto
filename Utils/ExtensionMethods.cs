﻿using System.Globalization;
using System.Numerics;

namespace ExtensionMethods;
public static class ExtensionMethods
{
    /// <summary>
    /// Parsing numbers as it should be in the framework.
    /// use like this:
    /// var parse = "12.5".TryParseNumber<double>();
    ///  if (parse.success)
    ///  { 
    ///    double x = parse.result;
    ///  }
    /// </summary>
    public static (bool success, T result) TryParseNumber<T>(this string value) where T : INumber<T>
    {
        T result = default(T);
        var succ = T.TryParse(value, CultureInfo.InvariantCulture, out result);
        return (succ, result);
    }

    /// <summary>
    /// Instead of !string.IsNullOrEmpty(str) just do str.IsNotEmpty()
    /// </summary>
    public static bool IsNotEmpty(this string str) => !string.IsNullOrEmpty(str);

    /// <summary>
    /// Just string.IsNullOrEmpty(this) in chainable format
    /// </summary>
    public static bool IsEmpty(this string str) => string.IsNullOrEmpty(str);

    /// <summary>
    /// Instead of !string.IsNullOrWhiteSpace(str) just do str.IsNotEmptyOrWhiteSpace()
    /// </summary>
    public static bool IsNotNullOrWhiteSpace(this string str) => !string.IsNullOrWhiteSpace(str);

    /// <summary>
    /// Sum for ulong
    /// </summary>
    public static ulong Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, ulong> summer)
    {
        ulong total = 0;

        foreach (var item in source)
            total += summer(item);

        return total;
    }

    /// <summary>
    /// Split string by new lines
    /// https://stackoverflow.com/questions/1508203/best-way-to-split-string-into-lines/41176852#41176852
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<string> GetLines(this string str, bool removeEmptyLines = false)
    {
        using (var sr = new StringReader(str))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (removeEmptyLines && String.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                yield return line;
            }
        }
    }

    /// <summary>
    /// 12345678 => "12_235_678"
    /// </summary>
    /// <typeparam name="T">any INumber</typeparam>
    /// <param name="separator">default is '_'</param>
    public static string AmountWithSeparators<T>(this T amount, char separator = '_') where T : INumber<T>
    {
        return amount.ToString() // "12345678"
                .Reverse().Chunk(3) // [['8','7','6'],['5','4','3'],['2','1']]
                .Select(x => x.Reverse()) // [['6','7','8'],['3','4','5'],['1','2']]
                .Reverse() // //[['1','2'],['3','4','5'],['6','7','8']]
                .Select(x => new string(x.ToArray())) // ["12", "345", "678"]
                .Aggregate((acc, curr) => acc + separator + curr); //12_345_678
    }


}
