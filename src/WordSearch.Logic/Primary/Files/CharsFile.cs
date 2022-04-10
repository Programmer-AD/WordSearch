using WordSearch.Logic.Interfaces.IO;
using WordSearch.Logic.Interfaces.Primary.Files;

namespace WordSearch.Logic.Primary.Files
{
    internal class CharsFile : ICharsFile
    {
        private readonly IFileIO charsFileIO;

        public CharsFile(IFileIO charsFileIO)
        {
            this.charsFileIO = charsFileIO;
        }

        public Task AddAsync(Action<CharsRecord> setupRecord)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerator<CharsRecord> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<long> GetRecordPositionByWordPosition(long wordPosition)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(long recordPosition)
        {
            throw new NotImplementedException();
        }
    }
}
