using System;
using OpenRasta.TypeSystem;

namespace OpenRasta.Configuration.Fluent
{
    public interface IHandlerParentDefinition : INoIzObject
    {
        IHandlerForResourceWithUriDefinition Handler<T>();
        IHandlerForResourceWithUriDefinition Handler(Type type);
        IHandlerForResourceWithUriDefinition Handler(IType type);
    }
}