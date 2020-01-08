namespace FileParcer.Interfaces
{
    public interface IFileNameData
    {
        string Path { get; }
        string FileName { get; }
        string FileExtention { get; }
    }
}
