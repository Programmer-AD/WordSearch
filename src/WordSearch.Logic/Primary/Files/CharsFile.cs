using System.Collections;
using WordSearch.Logic.Exceptions.CharsFile;
using WordSearch.Logic.Interfaces.IO;
using WordSearch.Logic.Interfaces.Primary.Files;

namespace WordSearch.Logic.Primary.Files
{
    internal class CharsFile : ICharsFile
    {
        private const int BufferRecordsCount = 1000;

        private readonly IFileIO fileIO;

        private readonly int recordSize;
        private readonly Memory<byte> buffer;
        private readonly CharsRecord[] bufferRecords;

        public CharsFile(IFileIO fileIO, int charsCountLength)
        {
            this.fileIO = fileIO;

            recordSize = CharsRecord.GetByteSize(charsCountLength);
            buffer = new byte[recordSize * BufferRecordsCount];
            bufferRecords = new CharsRecord[BufferRecordsCount];
            for (int i = 0; i < bufferRecords.Length; i++)
            {
                bufferRecords[i].Bytes = buffer.Slice(i * recordSize, recordSize);
            }
        }

        public void Add(Action<CharsRecord> setupRecord)
        {
            var record = bufferRecords[0];
            setupRecord(record);

            fileIO.StreamPosition = fileIO.StreamLength;
            fileIO.Writer.Write(record.Bytes);
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
            var lastRecordPosition = fileIO.StreamLength - recordSize;
            if (recordPosition != lastRecordPosition)
            {
                var recordBuffer = bufferRecords[0].Bytes;
                fileIO.StreamPosition = lastRecordPosition;
                fileIO.Reader.GetBytes(recordBuffer);

                fileIO.StreamPosition = recordPosition;
                fileIO.Writer.Write(recordBuffer);
                fileIO.Writer.Flush();
            }
            fileIO.StreamLength -= recordSize;
        }

        private IEnumerable<(long position, CharsRecord record)> GetPositionedRecordsEnumerable()
        {
            fileIO.StreamPosition = 0;
            var length = fileIO.StreamLength;
            long position;
            while ((position = fileIO.StreamPosition) < length)
            {
                var readed = fileIO.Reader.GetBytes(buffer);
                var readedRecords = readed / recordSize;
                for (int i = 0; i < readedRecords; i++)
                {
                    yield return (position, bufferRecords[i]);
                    position += recordSize;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
