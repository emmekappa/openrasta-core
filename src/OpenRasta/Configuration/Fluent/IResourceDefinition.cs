namespace OpenRasta.Configuration.Fluent
{
    public interface IResourceDefinition : INoIzObject
    {
        ICodecParentDefinition Anywhere { get; }
        IUriDefinition Uri(string uri);
    }
}