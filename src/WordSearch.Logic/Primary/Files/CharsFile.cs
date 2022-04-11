using System.Collections;
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

        public void Add(Action<CharsRecord> setupRecord)
        {
            setupRecord(record);

            fileIO.StreamPosition = fileIO.StreamLength;
            fileIO.Writer.Write(buffer);
            fileIO.Writer.Flush();
        }

        public IEnumerator<CharsRecord> GetEnumerator()
        {
            var result = GetPositionedRecordsEnumerable()
                .Select(x => x.record)
                .GetEnumerator();

            return result;
        }

        public long GetRecordPositionByWordPosition(long wordPosition)
        {
            try
            {
                var result = GetPositionedRecordsEnumerable()
                    .First(x => x.record.WordPosition == wordPosition)
                    .position;

                return result;
            }
            catch (InvalidOperationException)
            {
                throw new CharsRecordNotFoundException($"Not found record with WordPosition ({wordPosition})");
            }
        }

        public void Delete(long recordPosition)
        {
            var lastRecordPosition = fileIO.StreamLength - buffer.Length;
            if (recordPosition != lastRecordPosition)
            {
                fileIO.StreamPosition = lastRecordPosition;
                fileIO.Reader.GetBytes(buffer);

                fileIO.StreamPosition = recordPosition;
                fileIO.Writer.Write(buffer);
                fileIO.Writer.Flush();
            }
            fileIO.StreamLength -= buffer.Length;
        }

        private IEnumerable<(long position, CharsRecord record)> GetPositionedRecordsEnumerable()
        {
            fileIO.StreamPosition = 0;
            var length = fileIO.StreamLength;
            long position;
            while ((position = fileIO.StreamPosition) < length)
            {
                fileIO.Reader.GetBytes(buffer);
                yield return (position, record);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
