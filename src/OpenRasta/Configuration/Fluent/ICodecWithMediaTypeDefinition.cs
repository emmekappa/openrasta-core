namespace OpenRasta.Configuration.Fluent
{
    public interface ICodecWithMediaTypeDefinition : ICodecDefinition
    {
        ICodecWithMediaTypeDefinition Extension(string extension);
    }
}