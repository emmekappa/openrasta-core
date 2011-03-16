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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;

namespace OpenRasta.Web
{
    /// <summary>
    ///   Provides a list of http headers. In dire need of refactoring to use specific header types similar to http digest.
    /// </summary>
    public class HttpHeaderDictionary : IEnumerable<KeyValuePair<string,string>>
    {
        readonly IDictionary<string, List<string>> _base = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        const string HDR_CONTENT_DISPOSITION = "Content-Disposition";
        const string HDR_CONTENT_LENGTH = "Content-Length";
        const string HDR_CONTENT_TYPE = "Content-Type";
        const string HDR_CONTENT_LOCATION = "Content-Location";

        ContentDispositionHeader _contentDisposition;
        long? _contentLength;
        MediaType _contentType;
        Uri _contentLocation;

        public HttpHeaderDictionary()
        {
        }
        ICollection<string> GetValues(string headerName)
        {
            List<string> values;
            if (!_base.TryGetValue(headerName, out values))
                _base[headerName] = values = new List<string>();
            return values;
        }

        public HttpHeaderDictionary(NameValueCollection sourceDictionary)
        {
            foreach (string key in sourceDictionary.Keys)
                foreach (var value in sourceDictionary.GetValues(key))
                    Add(key, value);
        }
        public IEnumerator<KeyValuePair<string,string>> GetEnumerator()
        {
            return (from kv in _base
                   from value in kv.Value
                   select new KeyValuePair<string,string>(kv.Key, value))
                   .GetEnumerator();
        }
        public ContentDispositionHeader ContentDisposition
        {
            get { return _contentDisposition; }
            set { SetValue(ref _contentDisposition, HDR_CONTENT_DISPOSITION, value); }
        }

        public long? ContentLength
        {
            get { return _contentLength; }
            set { SetValue(ref _contentLength, HDR_CONTENT_LENGTH, value); }
        }

        public Uri ContentLocation
        {
            get { return _contentLocation; }
            set { SetValue(ref _contentLocation, HDR_CONTENT_LOCATION, value); }
        }

        public MediaType ContentType
        {
            get { return _contentType; }
            set { SetValue(ref _contentType, HDR_CONTENT_TYPE, value); }
        }

        public int Count
        {
            get { return _base.Count; }
        }

        public string this[string key]
        {
            get
            {
                List<string> result;
                if (_base.TryGetValue(key, out result))
                    return result.FirstOrDefault();
                return null;
            }
            set
            {
                Set(key, value);
            }
        }

        public void Add(string key, string value)
        {
            GetValues(key).Add(value);
            UpdateValue(key, new[]{value});
        }
        public void Set(string key, string value)
        {
            var values = GetValues(key);
            values.Clear();
            values.Add(value);
            UpdateValue(key, new[] { value });
        }

        void SetValue<T>(ref T typedKey, string key, T value)
        {
            typedKey = value;
            _base[key] = value == null ? new List<string>(0) : new List<string> { value.ToString() };
        }

        void UpdateValue(string headerName, IEnumerable<string> value)
        {
            if (headerName.Equals(HDR_CONTENT_TYPE, StringComparison.OrdinalIgnoreCase))
                _contentType = new MediaType(value.First());
            else if (headerName.Equals(HDR_CONTENT_LENGTH, StringComparison.OrdinalIgnoreCase))
            {
                long contentLength;
                if (long.TryParse(value.First(), NumberStyles.Float, CultureInfo.InvariantCulture, out contentLength))
                    _contentLength = contentLength;
            }
            else if (headerName.Equals(HDR_CONTENT_DISPOSITION, StringComparison.OrdinalIgnoreCase))
            {
                _contentDisposition = new ContentDispositionHeader(value.First());
            }
            else if (headerName.Equals(HDR_CONTENT_LOCATION, StringComparison.OrdinalIgnoreCase))
            {
                _contentLocation = new Uri(value.First(), UriKind.RelativeOrAbsolute);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

#region Full license
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion