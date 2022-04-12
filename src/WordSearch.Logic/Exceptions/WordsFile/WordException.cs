using System.Runtime.Serialization;

namespace WordSearch.Logic.Exceptions.WordsFile
{
    public class WordException : Exception
    {
        public WordException()
        {
        }

        public WordException(string message) : base(message)
        {
        }

        public WordException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WordException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
