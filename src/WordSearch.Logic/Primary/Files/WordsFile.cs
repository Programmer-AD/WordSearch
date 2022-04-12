using System.Collections;
using WordSearch.Logic.Exceptions.WordsFile;
using WordSearch.Logic.Interfaces.IO;
using WordSearch.Logic.Interfaces.Primary.Files;

namespace WordSearch.Logic.Primary.Files
{
    internal class WordsFile : IWordsFile
    {
        private readonly IFileIO fileIO;

        private readonly Lazy<string> charsLazy;

        public WordsFile(IFileIO fileIO)
        {
            this.fileIO = fileIO;

            charsLazy = new Lazy<string>(ReadChars);
        }

        public string Chars => charsLazy.Value;
        private long wordsStartStreamPosition;

        public long Add(string word)
        {
            CheckWordAlreadyExists(word);

            var wordPosition = fileIO.StreamPosition = fileIO.StreamLength;
            fileIO.Writer.Write(word);
            fileIO.Writer.Flush();
            return wordPosition;
        }

        public void Delete(long position)
        {
            var word = GetWord(position);
            var substitute = new string('\0', word.Length);

            fileIO.StreamPosition = position;
            fileIO.Writer.Write(substitute);
            fileIO.Writer.Flush();
        }

        public IEnumerator<string> GetEnumerator()
        {
            var result = GetPositionedWordsEnumerable()
                .Select(x => x.word)
                .GetEnumerator();

            return result;
        }

        public string GetWord(long position)
        {
            fileIO.StreamPosition = position;
            var result = fileIO.Reader.GetString();
            return result;
        }

        public long GetWordPosition(string word)
        {
            try
            {
                var result = GetPositionedWordsEnumerable()
                    .First(x => x.word == word)
                    .position;

                return result;
            }
            catch (InvalidOperationException)
            {
                throw new WordNotFoundException(word);
            }
        }

        private string ReadChars()
        {
            fileIO.StreamPosition = 0;
            var chars = fileIO.Reader.GetString();
            wordsStartStreamPosition = fileIO.StreamPosition;
            return chars;
        }

        private void CheckWordAlreadyExists(string word)
        {
            try
            {
                GetWordPosition(word);
                throw new WordAlreadyExistsException(word);
            }
            catch (WordNotFoundException) { }
        }

        private IEnumerable<(long position, string word)> GetPositionedWordsEnumerable()
        {
            fileIO.StreamPosition = wordsStartStreamPosition;
            var length = fileIO.StreamLength;
            long position;
            while ((position = fileIO.StreamPosition) < length)
            {
                var word = fileIO.Reader.GetString();
                yield return (position, word);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
