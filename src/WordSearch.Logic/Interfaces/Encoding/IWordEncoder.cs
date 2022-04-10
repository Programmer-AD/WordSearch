namespace WordSearch.Logic.Interfaces.Encoding
{
    public interface IWordEncoder
    {
        string Chars { get; }

        byte[] GetCharCounts(string word);
    }
}
