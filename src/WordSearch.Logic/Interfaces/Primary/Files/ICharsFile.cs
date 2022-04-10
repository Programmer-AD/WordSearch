using WordSearch.Logic.Primary;

namespace WordSearch.Logic.Interfaces.Primary.Files
{
    public interface ICharsFile : IAsyncEnumerable<CharsRecord>
    {
        public Task AddAsync(Action<CharsRecord> setupRecord);
        public Task DeleteAsync(long recordPosition);
        public Task<long> GetRecordPositionByWordPosition(long wordPosition);
    }
}
