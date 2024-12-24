using System;
using System.Collections.Generic;

namespace AVS.CoreLib.Extensions;

public static class ArrayExtensions
{
    public static T[] Combine<T>(this T[] arr1, T[] arr2)
    {
        if (arr2.Length == 0)
            return arr1;

        if (arr1.Length == 0)
            return arr2;

        var result = new T[arr1.Length + arr2.Length];
        Array.Copy(arr1, result, arr1.Length);
        Array.Copy(arr2, 0, result, arr1.Length, arr2.Length);
        return result;
    }

    public static bool AreSame(this decimal[] arr, decimal[] arrToCompare, decimal tolerance)
    {
        if (arr.Length != arrToCompare.Length)
            return false;

        for (var i = 0; i < arr.Length; i++)
        {
            if (!arr[i].IsEqual(arrToCompare[i], tolerance))
                return false;
        }

        return true;
    }

    public static int CountBestMatch(this decimal[] arr1, decimal[] arr2, decimal tolerance = 0.0m)
    {
        var maxCount = 0;

        for (var i = 0; i < arr1.Length; i++)
        {
            if (i + maxCount >= arr1.Length)
                break;

            var ind = Array.FindIndex(arr2, x => arr1[i].IsEqual(x, tolerance));

            if (ind == -1)
                continue;

            if (ind + maxCount >= arr2.Length)
                continue;

            var count = 1;

            for (var j = ind + 1; j < arr2.Length; j++)
            {
                if (i + count == arr1.Length)
                    break;

                if (!arr1[i + count].IsEqual(arr2[j], tolerance))
                    break;

                count++;
            }

            //i += count - 1;
            maxCount = Math.Max(maxCount, count);
        }

        return maxCount;

    }

    public static int GetShortestLength(this Array arr, Array other)
    {
        return arr.Length <= other.Length ? arr.Length : other.Length;
    }

    /// <summary> 
    /// Creates copy of the source array size+1 and inserts an element into a new array at a given index.
    /// </summary>
    public static T[] Insert<T>(this T[] source, T item, int index = 0)
    {
        var list = new List<T>(source.Length + 1);

        if (index == 0)
        {
            list.Add(item);
            list.AddRange(source);
        }
        else
        {
            list.AddRange(source);
            list.Insert(index, item);
        }

        return list.ToArray();
    }
}