﻿using System.Collections.Generic;
using AVS.CoreLib.Guards;

namespace AVS.CoreLib.Extensions.Collections;

public static class CropExtensions
{
    public static T[] Crop<T>(this T[] source, int startIndex, int endIndex)
    {
        Guard.MustBe.WithinRange(startIndex, 0, endIndex);
        Guard.MustBe.WithinRange(endIndex, startIndex, source.Length);

        var list = new List<T>(endIndex - startIndex);
        for (var i = startIndex; i < endIndex; i++)
        {
            list.Add(source[i]);
        }

        return list.ToArray();
    }

    public static IList<T> Crop<T>(this IList<T> source, int startIndex, int endIndex)
    {
        Guard.MustBe.WithinRange(startIndex, 0, endIndex);
        Guard.MustBe.WithinRange(endIndex, startIndex, source.Count);

        var list = new List<T>(endIndex - startIndex);
        for (var i = startIndex; i < endIndex; i++)
        {
            list.Add(source[i]);
        }

        return list;
    }
}