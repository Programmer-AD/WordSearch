namespace WordSearch.Logic.Interfaces.Encoding
{
    public interface IWordEncoder
    {
        string Chars { get; }

        void GetBytes(string word, out Span<byte> bytes);
    }
}
