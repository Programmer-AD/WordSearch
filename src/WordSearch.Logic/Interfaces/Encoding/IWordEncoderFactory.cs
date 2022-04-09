namespace WordSearch.Logic.Interfaces.Encoding
{
    public interface IWordEncoderFactory
    {
        IWordEncoder MakeWordEncoder(string chars);
    }
}
