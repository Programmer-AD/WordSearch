using WordSearch.Logic.Interfaces.Encoders;
using WordSearch.Logic.Interfaces.Primary;
using WordSearch.Logic.Interfaces.Primary.Files;

namespace WordSearch.Logic.Primary
{
    internal class Database : IDatabase
    {
        private readonly ICharsFile charsFile;
        private readonly IWordsFile wordsFile;
        private readonly IWordEncoder wordEncoder;

        public string Name { get; }
        public string Chars => wordEncoder.Chars;

        public Database(string name, ICharsFile charsFile, IWordsFile wordsFile, IWordEncoder wordEncoder)
        {
            Name = name;
            this.charsFile = charsFile;
            this.wordsFile = wordsFile;
            this.wordEncoder = wordEncoder;
        }

        public async Task AddAsync(string word)
        {
            CheckWord(word);

            var wordPosition = await wordsFile.AddAsync(word);
            var charCounts = wordEncoder.GetCharCounts(word);
            await charsFile.AddAsync(record =>
            {
                record.WordPosition = wordPosition;
                record.CharCounts = charCounts;
            });
        }

        public async Task DeleteAsync(string word)
        {
            CheckWord(word);

            var wordPosition = await wordsFile.GetWordPositionAsync(word);
            var recordPosition = await charsFile.GetRecordPositionByWordPosition(wordPosition);

            await Task.WhenAll(
                wordsFile.DeleteAsync(wordPosition),
                charsFile.DeleteAsync(recordPosition));
        }

        public async Task<IEnumerable<string>> GetWordsAsync(string word, byte maxDifference)
        {
            CheckWord(word);

            var charCounts = wordEncoder.GetCharCounts(word);
            var result = new List<string>();

            await foreach (var charsRecord in charsFile)
            {
                var difference = DatabaseHelpers.GetDifference(charCounts, charsRecord.CharCounts.Span);
                if (difference <= maxDifference)
                {
                    var nearWord = await wordsFile.GetWordAsync(charsRecord.WordPosition);
                    result.Add(nearWord);
                }
            }

            return result;
        }

        private static void CheckWord(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                throw new ArgumentException("Word cant be null or empty!", nameof(word));
            }
        }
    }
}
