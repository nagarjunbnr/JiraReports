using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optify
{
    /// <summary>
    /// Tokenizes a command line string
    /// </summary>
    /// <remarks>
    /// For this tokenizer, we use the term "flag" to indicate a command line flag/switch ( a single character or a 
    /// group of characters preceeded with the - "dash" character. We use the term "argument" to indicate a value that 
    /// populates a non-bool datatype (a number, string, etc)
    /// </remarks>
    public static class CommandLineTokenizer
    {
        /// <summary>
        /// Parses a command line string with a few simple rules, and returns the parsed string as tokens
        /// </summary>
        /// <param name="commandLine"></param>
        /// <returns></returns>
        public static IEnumerable<Token> Parse(string commandLine)
        {
            // To begin with, the string is not escaped (via a backslash) or enclosed (via a quote)
            bool escaped = false;
            bool enclosed = false;

            // Create a string builder. This is used to build up a fragment that can be parsed
            StringBuilder sb = new StringBuilder();

            // Iterate through the characters in the command line one at a time
            for (int index = 0; index < commandLine.Length; index++)
            {
                // Grab the character at the current index
                char indexChar = commandLine[index];

                // Make a decision based on the current character, if it is not currently escaped
                if (!escaped)
                {
                    switch (indexChar)
                    {
                        // If the current character is a backslash, we will escape the next character
                        case '\\':
                            escaped = true;
                            continue;

                        // If the current character is a quote and the current character is not enclosed, we will set 
                        // our current state as enclosed (by quotes)
                        case '"' when !enclosed:
                            enclosed = true;
                            continue;

                        // If the current character is a quote and the string state is enclosed, we will end the 
                        // enclosed state and yield an argument with the currently constructed string
                        case '"' when enclosed:
                            yield return new Token() { Value = sb.ToString(), TokenType = TokenType.Argument };
                            enclosed = false;
                            sb.Clear();
                            continue;

                        // If the current character is a space and the string state is not enclosed, yield either an 
                        // argument or flags, depending on the opening character (if it was a -, then flags, otherwise 
                        // argument)
                        case ' ' when !enclosed:
                            string value = sb.ToString();
                            sb.Clear();

                            foreach (Token token in ParseFragment(value, false))
                            {
                                yield return token;
                            }
                            continue;
                    }
                }

                // The current character was not a special character, or was otherwise escaped. Add it to the 
                // constructed string
                sb.Append(indexChar);

                // Cancel character escaping
                escaped = false;
            }

            // Parse the tokens in the leftover string
            foreach (Token token in ParseFragment(sb.ToString(), enclosed))
            {
                yield return token;
            }
        }

        /// <summary>
        /// Parses a fragment of a string to determine if it is an argument or a list of flags
        /// </summary>
        /// <param name="fragment">The fragment of a string to be parsed</param>
        /// <param name="enclosed">Indicates whether or not the provided <paramref name="fragment"/> was enclosed</param>
        /// <returns>A token or tokens that the provided <paramref name="fragment"/> represented</returns>
        private static IEnumerable<Token> ParseFragment(string fragment, bool enclosed)
        {
            // If the fragment being parsed is empty/null, don't bother
            if (String.IsNullOrEmpty(fragment)) yield break;

            // non enclosed strings that start with - will count as "flags"
            if (!enclosed && fragment.StartsWith("-"))
            {
                for (int i = 1; i < fragment.Length; i++)
                {
                    char c = fragment[i];

                    if (c == '-') continue;
                    yield return new Token() { Value = c.ToString(), TokenType = TokenType.Flag };
                }
            }
            // Otherwise it counts as an "argument"
            else
            {
                yield return new Token() { Value = fragment, TokenType = TokenType.Argument };
            }
        }
    }

    /// <summary>
    /// Represents a token in a command line string
    /// </summary>
    public class Token
    {
        /// <summary>
        /// The type of token
        /// </summary>
        public TokenType TokenType { get; set; }

        /// <summary>
        /// The value of the token
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// The type of token (Argument, Flag)
    /// </summary>
    public enum TokenType
    {
        Argument,
        Flag
    }
}
