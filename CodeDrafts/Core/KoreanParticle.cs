namespace MagicEscape.Core
{
    /// <summary>Display-only Korean particles; ignores trailing whitespace and parenthesized labels.</summary>
    public static class KoreanParticle
    {
        public static bool HasFinalConsonant(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            string word = name.TrimEnd();
            if (word.EndsWith(")"))
            {
                int start = word.LastIndexOf('(');
                if (start > 0) word = word.Substring(0, start).TrimEnd();
            }
            if (word.Length == 0) return false;
            char c = word[word.Length - 1];
            return c >= '\uAC00' && c <= '\uD7A3' && (c - '\uAC00') % 28 != 0;
        }
        public static string Object(string name) => name + (HasFinalConsonant(name) ? "?" : "?");
        public static string Subject(string name) => name + (HasFinalConsonant(name) ? "?" : "?");
        public static string With(string name) => name + (HasFinalConsonant(name) ? "?" : "?");
    }
}
