using OpenRasta.Configuration.MetaModel;
using OpenRasta.Web;

namespace OpenRasta.Configuration.Fluent.Implementation
{
    public class CodecMediaTypeDefinition : ICodecWithMediaTypeDefinition
    {
        readonly MediaTypeModel _model;
        readonly CodecDefinition _parent;

        public CodecMediaTypeDefinition(CodecDefinition parent, MediaTypeModel model)
        {
            _parent = parent;
            _model = model;
        }

        public ICodecParentDefinition And
        {
            get { return _parent.And; }
        }

        public ICodecWithMediaTypeDefinition MediaType(MediaType mediaType)
        {
            return _parent.MediaType(mediaType);
        }

        public ICodecWithMediaTypeDefinition Extension(string extension)
        {
            _model.Extensions.Add(extension);
            return this;
        }
    }
}