using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using OpenRasta.DI;
using OpenRasta.Web;

namespace OpenRasta
{
    public class Link : IEnumerable<KeyValuePair<string,string>>
    {
        Uri Href { get; set; }
        readonly Dictionary<string, List<string>> _parameters = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        public Link(Uri href)
        {
            Href = href;
        }

        public void Add(string parameterName, string parameterValue)
        {
            List<string> parameterValues;
            if (!_parameters.TryGetValue(parameterName, out parameterValues))
                _parameters[parameterName] = parameterValues = new List<string>();
            parameterValues.Add(parameterValue);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            foreach (var x in from paramKv in _parameters
                              from paramValue in paramKv.Value
                              select new KeyValuePair<string, string>(paramKv.Key, paramValue))
                yield return x;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        public virtual string HeaderValue(IUriResolver resolver)
        {
            return GenerateHeader(resolver, Href.ToString());
        }

        protected string GenerateHeader(IUriResolver resolver, string href)
        {
            return string.Format("<{0}>{1}",
                                 href,
                                 _parameters.Count() == 0
                                         ? string.Empty
                                         : "; " + string.Join("; ",
                                                              (from kv in _parameters
                                                               from value in kv.Value
                                                               select kv.Key + "=" + value).ToArray()
                                                          )
                    );
        }
    }
    public class Link<T> : Link{
        public Link() : base(null)
        {
        }
        public override string HeaderValue(IUriResolver resolver)
        {
            return GenerateHeader(resolver, DependencyManager.GetService<IUriResolver>().CreateUriFor<T>().ToString());
        }
    }
    public class AlternateLink : Link
    {
        public AlternateLink(Uri href, MediaType mediaType) : base(href)
        {
            Add("rel", "alternate");
            Add("type", mediaType.ToString());
        }
    }
    public class CanonicalLink : Link
    {
        public CanonicalLink(Uri href)
            : base(href)
        {
            Add("rel", "canonical");
        }
    }
}
