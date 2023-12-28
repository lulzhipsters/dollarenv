using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DollarEnv.Module
{
    public static class FileParser
    {
        private static Regex KeyValuePair = new Regex(
            @"(?:^|^)\s*(?:export\s+)?(?<key>[\w.-]+)(?:\s*=\s*?|:\s+?)(?<value>\s*'(?:\\'|[^'])*'|\s*""(?:\\""|[^""])*""|\s*`(?:\\`|[^`])*`|[^#\r\n]+)?\s*(?:#.*)?(?:$|$)",
            RegexOptions.Multiline);

        private static Regex QuotedText = new Regex(
            @"^(['""`])([\s\S]*)\1$", 
            RegexOptions.Multiline);

        private static Regex Newline = new Regex(
            @"\r?\n",
            RegexOptions.Multiline);

        public static Dictionary<string, string> ParseVariables(Stream stream)
        {
            var result = new Dictionary<string, string>();

            // slight risk here of the file being too big
            var content = new StreamReader(stream).ReadToEnd();

            // normalise newlines to environment
            content = Newline.Replace(content, Environment.NewLine);

            foreach (Match match in KeyValuePair.Matches(content))
            {
                var key = match.Groups["key"].Value;
                var value = match.Groups["value"].Value.Trim();

                if (value[0] == '"')
                {
                    // unescape any newlines within quotes
                    value = value
                        .Replace("\\n", "\n")
                        .Replace("\\r", "\r");
                }

                // strip the quotes
                value = QuotedText.Replace(value, "$2");

                result[key] = value;
            }

            return result;
        }
    }
}