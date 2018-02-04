namespace MultiMiner.Engine.Extensions
{
    public static class StringExtensions
    {
        public static string ToSpaceDelimitedWords(this string text)
        {
            var result = "";

            var currentPos = 0;
            var lastPos = text.Length - 1;

            for (currentPos = 0; currentPos <= lastPos; currentPos++)
            {
                var curChar = text[currentPos];
                var prevChar = curChar;

                if (currentPos > 0)
                {
                    prevChar = text[currentPos - 1];
                }

                if (char.IsUpper(curChar) && char.IsLower(prevChar))
                {
                    result += " ";
                }

                result += curChar;
            }
            
            return result;
        }
    }
}
