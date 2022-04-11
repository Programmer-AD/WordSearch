using WordSearch.Logic.Exceptions.CharsFile;
using WordSearch.Logic.Interfaces.IO;
using WordSearch.Logic.Interfaces.Primary.Files;

namespace WordSearch.Logic.Primary.Files
{
    internal class CharsFile : ICharsFile
    {
        private readonly IFileIO fileIO;

        private readonly byte[] buffer;
        private CharsRecord record;

        public CharsFile(IFileIO fileIO, int charsCountLength)
        {
            this.fileIO = fileIO;

            var recordLength = CharsRecord.GetByteSize(charsCountLength);
            buffer = new byte[recordLength];
            record = new CharsRecord(buffer);
        }

        public async Task AddAsync(Action<CharsRecord> setupRecord)
        {
            setupRecord(record);

            fileIO.StreamPosition = fileIO.StreamLength;
            await fileIO.Writer.WriteAsync(buffer);
            await fileIO.Writer.FlushAsync();
        }

        public async IAsyncEnumerator<CharsRecord> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            var enumerable = GetPositionedRecordsAsyncEnumerable();
            await foreach (var (_, record) in enumerable)
            {
                yield return record;
            }
        }

        public async Task<long> GetRecordPositionByWordPosition(long wordPosition)
        {
            var enumerable = GetPositionedRecordsAsyncEnumerable();
            await foreach (var (position, record) in enumerable)
            {
                if (record.WordPosition == wordPosition)
                {
                    return position;
                }
            }

            throw new CharsRecordNotFoundException($"Not found record with WordPosition ({wordPosition})");
        }

        public async Task DeleteAsync(long recordPosition)
        {
            var lastRecordPosition = fileIO.StreamLength - buffer.Length;
            if (recordPosition != lastRecordPosition)
            {
                fileIO.StreamPosition = lastRecordPosition;
                await fileIO.Reader.GetBytesAsync(buffer);

                fileIO.StreamPosition = recordPosition;
                await fileIO.Writer.WriteAsync(buffer);
                await fileIO.Writer.FlushAsync();
            }
            fileIO.StreamLength -= buffer.Length;
        }

        private async IAsyncEnumerable<(long position, CharsRecord record)> GetPositionedRecordsAsyncEnumerable()
        {
            fileIO.StreamPosition = 0;
            var length = fileIO.StreamLength;
            long position; 
            while ((position = fileIO.StreamPosition) < length)
            {
                await fileIO.Reader.GetBytesAsync(buffer);
                yield return (position, record);
            }
        }
    }
}
