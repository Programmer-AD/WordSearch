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

        public async Task<long> AddAsync(string word)
        {
            await CheckWordAlreadyExistsAsync(word);

            var wordPosition = fileIO.StreamPosition = fileIO.StreamLength;
            await fileIO.Writer.WriteAsync(word);
            await fileIO.Writer.FlushAsync();
            return wordPosition;
        }

        public async Task DeleteAsync(long position)
        {
            var word = await GetWordAsync(position);
            var substitute = new string('\0', word.Length);

            fileIO.StreamPosition = position;
            await fileIO.Writer.WriteAsync(substitute);
            await fileIO.Writer.FlushAsync();
        }

        public async IAsyncEnumerator<string> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            var enumerator = GetPositionedWordsAsyncEnumerable();
            await foreach (var (_, word) in enumerator)
            {
                yield return word;
            }
        }

        public async Task<string> GetWordAsync(long position)
        {
            fileIO.StreamPosition = position;
            var result = await fileIO.Reader.GetStringAsync();
            return result;
        }

        public async Task<long> GetWordPositionAsync(string word)
        {
            var enumerator = GetPositionedWordsAsyncEnumerable();
            await foreach (var (position, currentWord) in enumerator)
            {
                if (currentWord == word)
                {
                    return position;
                }
            }

            throw new WordNotFoundException(word);
        }

        private string ReadChars()
        {
            fileIO.StreamPosition = 0;
            var readingTask = fileIO.Reader.GetStringAsync();
            var chars = readingTask.Result;
            wordsStartStreamPosition = fileIO.StreamPosition;
            return chars;
        }

        private async Task CheckWordAlreadyExistsAsync(string word)
        {
            try
            {
                await GetWordPositionAsync(word);
                throw new WordAlreadyExistsException(word);
            }
            catch (WordNotFoundException) { }
        }

        private async IAsyncEnumerable<(long position, string word)> GetPositionedWordsAsyncEnumerable()
        {
            fileIO.StreamPosition = wordsStartStreamPosition;
            var length = fileIO.StreamLength;
            long position;
            while ((position = fileIO.StreamPosition) < length)
            {
                var word = await fileIO.Reader.GetStringAsync();
                yield return (position, word);
            }
        }
    }
}
