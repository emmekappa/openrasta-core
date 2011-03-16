#region License
/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reflection;
using OpenRasta.Codecs;
using OpenRasta.OperationModel;
using OpenRasta.TypeSystem;
using OpenRasta.Web;

namespace OpenRasta.Pipeline
{
    /// <summary>
    /// </summary>
    /// <remarks>Need to inherit from a yet to be created SafeDictionary</remarks>
    public class Environment : Dictionary<string, object>
    {
        const string OPENRASTA_PREFIX = "openrasta.";
        const string PIPELINE_STATE = OPENRASTA_PREFIX + "PipelineStage";
        const string OPERATIONS = OPENRASTA_PREFIX + "Operations";
        const string RESOURCE_KEY = OPENRASTA_PREFIX + "ResourceKey";
        const string RESPONSE_CODEC = OPENRASTA_PREFIX + "ResponseCodec";
        const string SELECTED_HANDLERS = OPENRASTA_PREFIX + "SelectedHandlers";
        const string SELECTED_RESOURCE = OPENRASTA_PREFIX + "SelectedResource";
        
        public IEnumerable<IOperation> Operations
        {
            get { return SafeGet<IEnumerable<IOperation>>(OPERATIONS); }
            set { base[OPERATIONS] = value; }
        }
        /// <summary>
        /// Gets the resource key associated with the requestURI. 
        /// </summary>
        public object ResourceKey
        {
            get { return SafeGet<object>(RESOURCE_KEY); }
            set { base[RESOURCE_KEY] = value; }
        }

        /// <summary>
        /// Gets the Codec associated with the response entity.
        /// </summary>
        public CodecRegistration ResponseCodec
        {
            get { return SafeGet<CodecRegistration>(RESPONSE_CODEC); }
            set { base[RESPONSE_CODEC] = value; }
        }

        public ICollection<IType> SelectedHandlers
        {
            get { return SafeGet<ICollection<IType>>(SELECTED_HANDLERS); }
            set { base[SELECTED_HANDLERS] = value; }
        }

        /// <summary>
        /// Provides access to the matched resource registration for a request URI.
        /// </summary>
        public UriRegistration SelectedResource
        {
            get { return SafeGet<UriRegistration>(SELECTED_RESOURCE); }
            set { base[SELECTED_RESOURCE] = value; }
        }

        public PipelineStage PipelineStage
        {
            get { return SafeGet<PipelineStage>(PIPELINE_STATE); }
            set { base[PIPELINE_STATE] = value; }
        }

        public new object this[string key]
        {
            get { return ContainsKey(key) ? base[key] : null; }
            set { base[key] = value; }
        }
        //T GetOrCreate<T>(string key) where T: class, new()
        //{
        //    object value;
        //    if (!TryGetValue(key, out value))
        //        this[key] = value = new T();
        //    return (T)value;
        //}
        T SafeGet<T>(string key) where T : class
        {
            object o;
            return TryGetValue(key, out o) ? o as T : null;
        }
    }
}

#region Full license
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion