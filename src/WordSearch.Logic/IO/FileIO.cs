using WordSearch.Logic.Interfaces.IO;
using Encoding = System.Text.Encoding;

namespace WordSearch.Logic.IO
{
    internal class FileIO : IFileIO, IDisposable
    {
        private static readonly Encoding encoding = Encoding.UTF8;

        private readonly Stream stream;
        private readonly Lazy<FileReader> fileReaderLazy;
        private readonly Lazy<FileWriter> fileWriterLazy;

        private bool disposedValue;

        public FileIO(Stream stream)
        {
            this.stream = stream;
            fileReaderLazy = new Lazy<FileReader>(() => new(stream, encoding));
            fileWriterLazy = new Lazy<FileWriter>(() => new(stream, encoding));
        }

        public long StreamPosition
        {
            get => stream.Position;
            set => stream.Position = value;
        }

        public long StreamLength
        {
            get => stream.Length;
            set => stream.SetLength(value);
        }

        public IFileReader Reader => fileReaderLazy.Value;
        public IFileWriter Writer => fileWriterLazy.Value;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    stream.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
