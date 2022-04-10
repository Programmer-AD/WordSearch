namespace WordSearch.Logic.Interfaces.Encoders
{
    public interface IWordEncoder
    {
        string Chars { get; }

        byte[] GetCharCounts(string word);
    }
}
