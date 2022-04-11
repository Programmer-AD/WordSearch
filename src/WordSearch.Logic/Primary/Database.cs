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

        public void Add(string word)
        {
            CheckWord(word);

            var wordPosition = wordsFile.Add(word);
            var charCounts = wordEncoder.GetCharCounts(word);
            charsFile.Add(record =>
            {
                record.WordPosition = wordPosition;
                record.CharCounts = charCounts;
            });
        }

        public void Delete(string word)
        {
            CheckWord(word);

            var wordPosition = wordsFile.GetWordPosition(word);
            var recordPosition = charsFile.GetRecordPositionByWordPosition(wordPosition);

            wordsFile.Delete(wordPosition);
            charsFile.Delete(recordPosition);
        }

        public IEnumerable<string> GetWords(string word, byte maxDifference)
        {
            CheckWord(word);

            var charCounts = wordEncoder.GetCharCounts(word);
            var result = charsFile.Where(charsRecord =>
            {
                var difference = DatabaseHelpers.GetDifference(charCounts, charsRecord.CharCounts.Span);
                return difference <= maxDifference;
            }).Select(x => wordsFile.GetWord(x.WordPosition))
            .ToList();

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
