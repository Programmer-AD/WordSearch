using WordSearch.Logic.Interfaces.IO;

namespace WordSearch.Logic.IO
{
    internal class FileIO : IFileIO, IDisposable
    {
        private readonly Stream stream;
        private readonly Lazy<FileReader> fileReaderLazy;
        private readonly Lazy<FileWriter> fileWriterLazy;

        private bool disposedValue;

        public FileIO(Stream stream)
        {
            this.stream = stream;
            fileReaderLazy = new Lazy<FileReader>();
            fileWriterLazy = new Lazy<FileWriter>();
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
