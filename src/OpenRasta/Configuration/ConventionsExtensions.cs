using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using OpenRasta.Configuration.Fluent;
using OpenRasta.Configuration.MetaModel;
using OpenRasta.TypeSystem;

namespace OpenRasta.Configuration
{
    public static class ConventionsExtensions
    {
        const string RESOURCES_SUFFIX = ".Resources";
        const string RESOURCES_MIDDLE = ".Resources.";

        public static void ConventionsFrom(this IUses uses, object source)
        {
            var assembly = source.GetType().Assembly;
            // try to find a base namespace that has a child Handlers and a child Resources namespace
            var allTypes = assembly.GetExportedTypes();
            var allNamespaces = allTypes.Select(x => x.Namespace).ToList();
            var rootNamespace =
                    (
                            from @namespace in allNamespaces
                            let resourcesLocation = @namespace.IndexOf(RESOURCES_MIDDLE)
                            where resourcesLocation != -1
                            let root = @namespace.Substring(0, resourcesLocation)
                            // root is Blah whe Blah.Resources exists
                            where allNamespaces.Any(x => x.StartsWith(root + ".Handlers."))
                            select root
                    ).Union(
                            from @namespace in allNamespaces
                            where @namespace.EndsWith(RESOURCES_SUFFIX)
                            let root = @namespace.Substring(0, @namespace.Length - RESOURCES_SUFFIX.Length)

                            where allNamespaces.Any(x => x.StartsWith(root + ".Handlers"))
                            select root
                            ).FirstOrDefault();

            if (rootNamespace == null) return;

            var resourcesToRegister = from resourceType in allTypes
                                      where resourceType.Namespace.StartsWith(rootNamespace + RESOURCES_SUFFIX)
                                      select new
                                      {
                                          Path = resourceType.Namespace.Length == rootNamespace.Length
                                                         ? Enumerable.Empty<string>()
                                                         : resourceType.Namespace.Substring(rootNamespace.Length + RESOURCES_SUFFIX.Length).Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries),
                                          Type = resourceType,
                                          Handler =
                                          (
                                                  from handlerType in allTypes
                                                  where handlerType.Namespace.StartsWith(rootNamespace + ".Handlers") &&
                                                        handlerType.Name.StartsWith(resourceType.Name + "Handler")
                                                  select handlerType
                                          ).FirstOrDefault()
                                      };
            var registrar = (IFluentTarget)uses;
            foreach (var resource in resourcesToRegister)
            {
                var resourceModel = new ResourceModel
                {
                        ResourceKey = registrar.TypeSystem.FromClr(resource.Type),
                        Uris =
                                { 
                                        new UriModel
                                        { 
                                                Uri =
                                                        (resource.Path.Count() > 0 ? string.Join("/",resource.Path.Select(x=>x.ToLowerInvariant()).ToArray()) : string.Empty) +
                                                        "/" +
                                                        ConvertResourceNameToUri(resource.Type.Name)
                                        }
                                }
                };
                if (resource.Handler != null)
                    resourceModel.Handlers.Add(registrar.TypeSystem.FromClr(resource.Handler));
                registrar.Repository.ResourceRegistrations.Add(resourceModel);
            }
        }

        static string ConvertResourceNameToUri(string typeName)
        {
            StringBuilder finalName = new StringBuilder(char.ToLower(typeName[0]).ToString());

            bool lastWasUpper = false;
            foreach(var currentChar in typeName.Skip(1))
            {
                finalName.Append(char.IsUpper(currentChar) ? "-" + char.ToLower(currentChar) : currentChar.ToString());
            }
            return Uri.EscapeUriString(finalName.ToString());
        }
    }
}
