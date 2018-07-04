namespace RomMaster.Common
{
    using System.Text.RegularExpressions;

    public class Exclude
    {
        private static Regex regex;

        public string Pattern { get; set; }

        public bool Match(string file)
        {
            if (regex == null)
            {
                var pattern = Pattern
                    .Replace(".", "\\.")
                    .Replace("?", ".")
                    //.Replace("**", ".*?") //TODO reorder
                    .Replace("*", ".*");

                regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
            }

            return regex.IsMatch(file);
        }
    }
}