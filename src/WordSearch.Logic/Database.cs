using WordSearch.Logic.Exceptions.Database;
using WordSearch.Logic.Interfaces;
using WordSearch.Logic.Interfaces.Encoders;
using WordSearch.Logic.Interfaces.IO;
using WordSearch.Logic.Models;

namespace WordSearch.Logic
{
    internal class Database : IDatabase
    {
        private readonly IFileIO charsFile;
        private readonly IFileIO wordsFile;
        private readonly IWordEncoder wordEncoder;
        private readonly byte[] recordBuffer;

        public string Name { get; }
        public string Chars => wordEncoder.Chars;

        public Database(string name, IFileIO charsFile, IFileIO wordsFile, IWordEncoder wordEncoder)
        {
            Name = name;
            this.charsFile = charsFile;
            this.wordsFile = wordsFile;
            this.wordEncoder = wordEncoder;
            recordBuffer = new byte[CharsRecord.GetByteSize(Chars.Length)];
        }

        public async Task AddAsync(string word)
        {
            CheckWord(word);

            var wordPosition = wordsFile.StreamPosition = wordsFile.StreamLength;
            await wordsFile.Writer.WriteAsync(word);

            var charsRecord = new CharsRecord
            {
                CharCounts = wordEncoder.GetCharCounts(word),
                WordPosition = wordPosition,
            };
            charsFile.StreamPosition = charsFile.StreamLength;
            charsRecord.GetBytes(recordBuffer);
            await charsFile.Writer.WriteAsync(recordBuffer);

            await FlushFilesAsync();
        }

        public async Task DeleteAsync(string word)
        {
            CheckWord(word);

            var wordPosition = await FindWordPositionAsync(word);
            var recordPosition = await FindRecordPositionWithReferenceAsync(wordPosition);

            await Task.WhenAll(
                DeleteRecordAsync(recordPosition),
                DeleteWordAsync(word, wordPosition));

            await FlushFilesAsync();
        }

        public async Task<IEnumerable<string>> GetWordsAsync(string word, byte maxDifference)
        {
            CheckWord(word);

            var charCounts = wordEncoder.GetCharCounts(word);
            var result = new List<string>();

            while (charsFile.StreamPosition < charsFile.StreamLength)
            {
                var record = await ReadRecordAsync();

                var diff = DatabaseHelpers.GetDifference(charCounts, record.CharCounts);

                if (diff <= maxDifference)
                {
                    var nearWord = await GetWordAsync(record.WordPosition);
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

        private async Task FlushFilesAsync()
        {
            await wordsFile.Writer.FlushAsync();
            await charsFile.Writer.FlushAsync();
        }

        private async Task<long> FindWordPositionAsync(string word)
        {
            wordsFile.StreamPosition = 0;
            await wordsFile.Reader.GetStringAsync();

            while (wordsFile.StreamPosition < wordsFile.StreamLength)
            {
                var wordPosition = wordsFile.StreamPosition;
                var currentWord = await wordsFile.Reader.GetStringAsync();
                if (currentWord == word)
                {
                    return wordPosition;
                }
            }

            throw new WordNotFoundException(word);
        }

        private async Task<long> FindRecordPositionWithReferenceAsync(long referencedPosition)
        {
            while (charsFile.StreamPosition < charsFile.StreamLength)
            {
                var recordPosition = charsFile.StreamPosition;
                var record = await ReadRecordAsync();
                if (record.WordPosition == referencedPosition)
                {
                    return recordPosition;
                }
            }

            return -1;
        }

        private async Task<CharsRecord> ReadRecordAsync()
        {
            await charsFile.Reader.GetBytesAsync(recordBuffer);
            var result = new CharsRecord(recordBuffer, Chars.Length);
            return result;
        }

        private async Task DeleteRecordAsync(long recordPosition)
        {
            if (recordPosition >= 0)
            {
                charsFile.StreamPosition = charsFile.StreamLength - recordBuffer.Length;
                await charsFile.Reader.GetBytesAsync(recordBuffer);
                charsFile.StreamPosition = recordPosition;
                await charsFile.Writer.WriteAsync(recordBuffer);
            }
        }

        private async Task DeleteWordAsync(string word, long wordPosition)
        {
            var fillerString = new string('\0', word.Length);
            wordsFile.StreamPosition = wordPosition;
            await wordsFile.Writer.WriteAsync(fillerString);
        }

        private async Task<string> GetWordAsync(long wordPosition)
        {
            wordsFile.StreamPosition = wordPosition;
            var result = await wordsFile.Reader.GetStringAsync();
            return result;
        }
    }
}
