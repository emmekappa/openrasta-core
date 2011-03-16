using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using OpenRasta.TypeSystem;
using OpenRasta.Web;

namespace OpenRasta.Codecs
{
    public class LinkingXmlDataContractCodec : XmlDataContractCodec
    {
        
        ICommunicationContext _context;
        const string ATOM_NS = "http://www.w3.org/2005/Atom";

        public LinkingXmlDataContractCodec(ICommunicationContext context)
        {
            _context = context;
        }

        public override void WriteToCore(object entity, IHttpEntity response)
        {
            var serializer = new DataContractSerializer(entity.GetType());
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8);
            serializer.WriteObject(writer, entity);
            writer.Flush();
            ms.Position = 0;
            var newDocument = XDocument.Load(new XmlTextReader(ms));

            foreach (var link in _context.OperationResult.Links)
            {
                newDocument.Root.AddFirst(new XElement(XName.Get("link", ATOM_NS),
                                                       link.Select(x => new XAttribute(x.Key, x.Value))));
            }
            newDocument.Save(Writer);
        }
    }
}
