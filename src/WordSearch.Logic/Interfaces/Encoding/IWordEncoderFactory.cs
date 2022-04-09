namespace WordSearch.Logic.Interfaces.Encoding
{
    public interface IWordEncoderFactory
    {
        IWordEncoder CreateWordEncoder(string chars);
    }
}
