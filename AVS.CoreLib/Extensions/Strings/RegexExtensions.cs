﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AVS.CoreLib.Extensions
{
    [Obsolete("Use AVS.CoreLib.Extensions package")]
    public static class RegexExtensions
    {
        public static string[] GetMatches(this string input, string regExpression = "@(?<value>\\w+)")
        {
            var regex = new Regex(regExpression);
            var matches = new List<string>();
            foreach (Match match in regex.Matches(input))
            {
                matches.Add(match.Groups["value"].Success ? match.Groups["value"].Value : match.Value);
            }

            return matches.ToArray();
        }

        public static string[] Replace(this Regex regex, ref string input, string replacement = "")
        {
            var matches = new List<string>();
            var sb = new StringBuilder(input);
            foreach (Match match in regex.Matches(input))
            {
                matches.Add(match.Groups["value"].Success ? match.Groups["value"].Value : match.Value);
                sb.Replace(match.Value, replacement);
            }

            input = sb.ToString().TrimEnd(' ');
            return matches.ToArray();
        }
    }
}