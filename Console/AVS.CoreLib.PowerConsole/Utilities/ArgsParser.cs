﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AVS.CoreLib.PowerConsole.Utilities
{
    /// <remarks>
    /// If you want to deal with command line args use https://github.com/commandlineparser/commandline package
    /// which seems very nice solution to deal with command-line args")
    /// </remarks>
    [Obsolete("I don't remember where this was used, to parse command line args there are nuget packages.. ArgsParser will be removed")]
    public class ArgsParser
    {
        private const string ARGS_REGEX = @"-((?<param>(\w)+) (?<arg>(\w|\S)+))";

        /// <summary>
        /// Parse args, required format: -arg1:value1 -arg2:value2 
        /// </summary>
        public static Dictionary<string, string> Parse(string[] args)
        {
            var dict = new Dictionary<string, string>();
            if (args.Length == 0)
                return dict;

            foreach (var arg in args)
            {
                if (!arg.StartsWith("-"))
                    continue;
                var ind = arg.IndexOf(':');

                if (ind <= 0)
                    continue;

                var param = arg.Substring(1, ind - 1);
                var value = arg.Substring(ind + 1);
                dict.Add(param, value);
            }

            return dict;
        }

        public static Dictionary<string, string> Parse(string args)
        {
            var dict = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(args))
                return dict;

            var results = Regex.Matches(args, ARGS_REGEX, RegexOptions.Compiled);
            foreach (Match match in results)
            {
                string key = null;
                var gr = match.Groups["param"];
                if (gr.Success)
                {
                    key = gr.Value;
                }
                gr = match.Groups["arg"];
                if (gr.Success && !string.IsNullOrEmpty(key))
                {
                    dict.Add(key, gr.Value);
                }
            }

            return dict;
        }

        public static bool TryGetValue(string[] args, string parameter, out string value)
        {
            value = null;
            if (args.Length == 0)
                return false;

            foreach (var arg in args)
            {
                if (!arg.StartsWith("-" + parameter))
                    continue;

                var ind = arg.IndexOf(':');

                if (ind <= 0)
                    continue;

                value = arg.Substring(ind + 1);
                return true;
            }

            return false;
        }

        public static T Parse<T>(string args) where T : IDictionary<string, string>, new()
        {
            var dict = new T();
            if (string.IsNullOrEmpty(args))
                return dict;

            var results = Regex.Matches(args, ARGS_REGEX, RegexOptions.Compiled);
            foreach (Match match in results)
            {
                string key = null;
                var gr = match.Groups["param"];
                if (gr.Success)
                {
                    key = gr.Value;
                }
                gr = match.Groups["arg"];
                if (gr.Success && !string.IsNullOrEmpty(key))
                {
                    dict.Add(key, gr.Value);
                }
            }

            return dict;
        }
    }
}