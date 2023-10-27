using System;
using AVS.CoreLib.Extensions.Enums;

namespace AVS.CoreLib.Extensions
{
    public static class StringFormatExtensions
    {
        public static string FormatString(this string str, StringCase @case)
        {
            return @case switch
            {
                StringCase.UPPER_CASE => str.ToUpper(),
                StringCase.lower_case => str.ToLower(),
                StringCase.PascalCase => ToPascalCase(str),
                StringCase.CamelCase => ToCamelCase(str),
                StringCase.snake_case => ToSnakeCase(str),
                StringCase.kebab_case => ToKebabCase(str),
                StringCase.TrainCase => ToTrainCase(str),
                StringCase.MixedCase => ToMixedCase(str),
                StringCase.TitleCase => ToTitleCase(str),
                StringCase.ScreamingSnakeCase => ToScreamingSnakeCase(str),
                _ => str,
            };
        }

        private static string ToPascalCase(string str)
        {
            // Split the string into words based on spaces or underscores
            string[] words = str.Split(new[] { ' ', '_' }, StringSplitOptions.RemoveEmptyEntries);

            // Capitalize the first letter of each word
            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];
                if (word.Length > 0)
                {
                    words[i] = char.ToUpper(word[0]) + (word.Length > 1 ? word.Substring(1).ToLower() : string.Empty);
                }
            }

            // Join the words together without spaces
            return string.Join(string.Empty, words);
        }

        private static string ToCamelCase(string str)
        {
            // Split the string into words based on spaces or underscores
            string[] words = str.Split(new[] { ' ', '_' }, StringSplitOptions.RemoveEmptyEntries);

            // Ensure the first word is in lowercase
            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];
                if (word.Length > 0)
                {
                    words[i] = i == 0
                        ? char.ToLower(word[0]) + (word.Length > 1 ? word.Substring(1).ToLower() : string.Empty)
                        : char.ToUpper(word[0]) + (word.Length > 1 ? word.Substring(1).ToLower() : string.Empty);
                }
            }

            // Join the words together without spaces
            return string.Join(string.Empty, words);
        }

        private static string ToSnakeCase(string str)
        {
            // Split the string into words based on spaces or underscores
            string[] words = str.Split(new[] { ' ', '_' }, StringSplitOptions.RemoveEmptyEntries);

            // Convert each word to lowercase and join them with underscores
            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];
                if (word.Length > 0)
                {
                    words[i] = word.ToLower();
                }
            }

            // Join the words with underscores
            return string.Join("_", words);
        }

        private static string ToKebabCase(string str)
        {
            // Implement kebab-case conversion logic here
            throw new NotImplementedException();
        }

        private static string ToTrainCase(string str)
        {
            // Implement TrainCase conversion logic here
            throw new NotImplementedException();
        }

        private static string ToMixedCase(string str)
        {
            // Implement MixedCase conversion logic here
            throw new NotImplementedException();
        }

        private static string ToTitleCase(string str)
        {
            // Implement TitleCase conversion logic here
            throw new NotImplementedException();
        }

        private static string ToScreamingSnakeCase(string str)
        {
            // Implement ScreamingSnakeCase conversion logic here
            throw new NotImplementedException();
        }
    }

}
