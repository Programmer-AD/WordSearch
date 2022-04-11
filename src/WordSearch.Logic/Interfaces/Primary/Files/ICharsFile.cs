using WordSearch.Logic.Primary;

namespace WordSearch.Logic.Interfaces.Primary.Files
{
    public interface ICharsFile : IEnumerable<CharsRecord>
    {
        public void Add(Action<CharsRecord> setupRecord);
        public void Delete(long recordPosition);
        public long GetRecordPositionByWordPosition(long wordPosition);
    }
}
