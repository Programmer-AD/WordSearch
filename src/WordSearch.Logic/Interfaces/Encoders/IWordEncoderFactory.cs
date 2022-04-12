namespace WordSearch.Logic.Interfaces.Encoders
{
    public interface IWordEncoderFactory
    {
        IWordEncoder MakeWordEncoder(string chars);
    }
}
