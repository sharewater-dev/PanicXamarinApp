﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
#if NETFX
using System.Web;
#endif

namespace Yodiwo.Tools
{
    public static class Http
    {
        public const string UserAgent = "Mozilla/5.0 (compatible; MSIE 6.0; Windows CE)";

        public delegate void ProgressReportDelegate(long download, long total, double percentage);

        public struct RequestResult
        {
            public bool IsValid;
            public bool IsSuccessStatusCode;
            public List<Cookie> Cookies;
            public HttpStatusCode StatusCode;
            public Uri ResponseUri;
            public byte[] ResponseBodyBinary;
            public string[] ContentTypeEntries;
            public string Charset;

            string _ResponseBodyText;
            public string ResponseBodyText
            {
                get
                {
                    if (ResponseBodyBinary == null)
                        return null;
                    else
                        lock (ResponseBodyBinary)
                        {
                            //convert to string
                            if (_ResponseBodyText == null)
                            {
#if NETFX
                                //decode to text
                                if (Charset == "utf-8")
                                    _ResponseBodyText = UTF8Encoding.UTF8.GetString(ResponseBodyBinary);
                                else if (Charset == "ascii")
                                    _ResponseBodyText = ASCIIEncoding.ASCII.GetString(ResponseBodyBinary);
                                else
                                    _ResponseBodyText = UTF8Encoding.UTF8.GetString(ResponseBodyBinary);
#elif UNIVERSAL
                                //decode to text
                                if (Charset == "utf-8")
                                    _ResponseBodyText = UTF8Encoding.UTF8.GetString(ResponseBodyBinary, 0, ResponseBodyBinary.Length);
                                else if (Charset == "ascii")
                                {
                                    DebugEx.Assert("Not Supported");
                                    _ResponseBodyText = UTF8Encoding.UTF8.GetString(ResponseBodyBinary, 0, ResponseBodyBinary.Length);
                                    //_ResponseBodyText = ASCIIEncoding.ASCII.GetString(ResponseBodyBinary, 0, ResponseBodyBinary.Length);
                                }
                                else
                                    _ResponseBodyText = UTF8Encoding.UTF8.GetString(ResponseBodyBinary, 0, ResponseBodyBinary.Length);
#endif
                            }
                            //return cached string
                            return _ResponseBodyText;
                        }
                }
            }
        }

        public static RequestResult RequestGET(string url)
        {
            return Request(HttpMethods.Get, url, null, HttpRequestDataFormat.FormData, null, null, null, null, null);
        }

        public static RequestResult RequestGET(string url, ProgressReportDelegate progressReport)
        {
            return Request(HttpMethods.Get, url, null, HttpRequestDataFormat.FormData, null, null, null, null, progressReport);
        }

        public static RequestResult RequestGET(string url, ICredentials Credentials)
        {
            return Request(HttpMethods.Get, url, null, HttpRequestDataFormat.FormData, null, null, Credentials, null, null);
        }

        public static RequestResult RequestGET(string url, ICredentials Credentials, ProgressReportDelegate progressReport)
        {
            return Request(HttpMethods.Get, url, null, HttpRequestDataFormat.FormData, null, null, Credentials, null, progressReport);
        }

        public static RequestResult RequestGET(string url, IEnumerable<Cookie> Cookies)
        {
            return Request(HttpMethods.Get, url, null, HttpRequestDataFormat.FormData, null, Cookies, null, null, null);
        }

        public static RequestResult RequestGET(string url, IEnumerable<Cookie> Cookies, ProgressReportDelegate progressReport)
        {
            return Request(HttpMethods.Get, url, null, HttpRequestDataFormat.FormData, null, Cookies, null, null, progressReport);
        }

        public static RequestResult RequestGET(string url, IEnumerable<Cookie> Cookies, ICredentials Credentials)
        {
            return Request(HttpMethods.Get, url, null, HttpRequestDataFormat.FormData, null, Cookies, Credentials, null, null);
        }

        public static RequestResult RequestGET(string url, IEnumerable<Cookie> Cookies, ICredentials Credentials, ProgressReportDelegate progressReport)
        {
            return Request(HttpMethods.Get, url, null, HttpRequestDataFormat.FormData, null, Cookies, Credentials, null, progressReport);
        }

        public static RequestResult RequestGET(string url, Dictionary<string, string> data, IEnumerable<Cookie> Cookies)
        {
            return Request(HttpMethods.Get, url + "?" + BuildParameters(data), null, HttpRequestDataFormat.Text, null, Cookies, null, null, null);
        }

        public static RequestResult RequestGET(string url, Dictionary<string, string> data, IEnumerable<Cookie> Cookies, ProgressReportDelegate progressReport)
        {
            return Request(HttpMethods.Get, url + "?" + BuildParameters(data), null, HttpRequestDataFormat.Text, null, Cookies, null, null, progressReport);
        }

        public static RequestResult RequestGET(string url, Dictionary<string, string> data, IEnumerable<Cookie> Cookies, ICredentials Credentials)
        {
            return Request(HttpMethods.Get, url + "?" + BuildParameters(data), null, HttpRequestDataFormat.Text, null, Cookies, Credentials, null, null);
        }

        public static RequestResult RequestGET(string url, Dictionary<string, string> data, IEnumerable<Cookie> Cookies, ICredentials Credentials, ProgressReportDelegate progressReport)
        {
            return Request(HttpMethods.Get, url + "?" + BuildParameters(data), null, HttpRequestDataFormat.Text, null, Cookies, Credentials, null, progressReport);
        }

        public static RequestResult Request(HttpMethods method, string url, string data, HttpRequestDataFormat dataFormat)
        {
            return Request(method, url, data, dataFormat, null, null, null, null, null);
        }

        public static RequestResult Request(HttpMethods method, string url, string data, HttpRequestDataFormat dataFormat, ICredentials Credentials)
        {
            return Request(method, url, data, dataFormat, null, null, Credentials, null, null);
        }

        public static RequestResult Request(HttpMethods method, string url, string data, HttpRequestDataFormat dataFormat, Dictionary<string, string> headers, IEnumerable<Cookie> Cookies)
        {
            return Request(method, url, data, dataFormat, headers, Cookies, null, null, null);
        }

        public static RequestResult Request(HttpMethods method, string url, string data, HttpRequestDataFormat dataFormat, Dictionary<string, string> headers, IEnumerable<Cookie> Cookies, ICredentials Credentials)
        {
            return Request(method, url, data, dataFormat, headers, Cookies, Credentials, null, null);
        }

        public static Task<RequestResult> RequestAsync(HttpMethods method, string url, string data, HttpRequestDataFormat dataFormat, Dictionary<string, string> headers, IEnumerable<Cookie> Cookies, Action<RequestResult> callback)
        {
            return Task.Run(() => Request(method, url, data, dataFormat, headers, Cookies, null, callback, null));
        }

        public static RequestResult RequestPost(string url, string data, HttpRequestDataFormat dataFormat)
        {
            return Request(HttpMethods.Post, url, data, dataFormat, null, null, null, null, null);
        }

        public static RequestResult RequestPost(string url, string data, HttpRequestDataFormat dataFormat, IEnumerable<Cookie> Cookies)
        {
            return Request(HttpMethods.Post, url, data, dataFormat, null, Cookies, null, null, null);
        }

        public static RequestResult RequestPost(string url, string data, HttpRequestDataFormat dataFormat, IEnumerable<Cookie> Cookies, ICredentials Credentials)
        {
            return Request(HttpMethods.Post, url, data, dataFormat, null, Cookies, Credentials, null, null);
        }

        public static RequestResult RequestPost(string url, Dictionary<string, string> data, IEnumerable<Cookie> Cookies)
        {
            return Request(HttpMethods.Post, url, BuildParameters(data), HttpRequestDataFormat.FormData, null, Cookies, null, null, null);
        }

        public static RequestResult RequestPost(string url, Dictionary<string, string> data, IEnumerable<Cookie> Cookies, ICredentials Credentials)
        {
            return Request(HttpMethods.Post, url, BuildParameters(data), HttpRequestDataFormat.FormData, null, Cookies, Credentials, null, null);
        }

        public static RequestResult Request(HttpMethods method, string url, string data, HttpRequestDataFormat dataFormat, Dictionary<string, string> headers, IEnumerable<Cookie> Cookies, ICredentials Credentials, Action<RequestResult> callback, ProgressReportDelegate progressReportCallback)
        {
            return _Request(method, url, data != null ? Encoding.UTF8.GetBytes(data) : null, dataFormat, headers, Cookies, Credentials, callback, progressReportCallback);
        }

        public static RequestResult RequestPost(string url, byte[] data, IEnumerable<Cookie> Cookies)
        {
            return _Request(HttpMethods.Post, url, data, HttpRequestDataFormat.Binary, null, Cookies, null, null, null);
        }

        public static RequestResult RequestPost(string url, byte[] data, IEnumerable<Cookie> Cookies, ICredentials Credentials)
        {
            return _Request(HttpMethods.Post, url, data, HttpRequestDataFormat.Binary, null, Cookies, Credentials, null, null);
        }




        private static RequestResult _Request(HttpMethods method, string url, byte[] data, HttpRequestDataFormat dataFormat, Dictionary<string, string> headers, IEnumerable<Cookie> Cookies, ICredentials Credentials, Action<RequestResult> callback, ProgressReportDelegate progressReportCallback)
        {
            //enable protocols
#if NETFX
#if __MonoCS__
            try{ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;}catch{}
#warning WARNING the system is not secure when using only TLS1 !!!! you must use visual studio.
#elif UNIVERSAL
            try{ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;} catch{}
#endif
#endif

            //add request cookies
            var cookieContainer = new CookieContainer();
            if (Cookies != null)
                foreach (var entry in Cookies)
#if NETFX
                    cookieContainer.Add(entry);
#elif UNIVERSAL
                    cookieContainer.Add(new Uri(url), entry);
#endif

            //creat request
            HttpWebRequest webRequest;
            try { webRequest = (HttpWebRequest)WebRequest.Create(url); }
            catch (Exception ex)
            {
                DebugEx.TraceError(ex, "Http request failed");
                if (callback != null)
                    callback(default(RequestResult));
                return default(RequestResult);
            }

            //setup request
            webRequest.CookieContainer = cookieContainer;
            if (Credentials == null)
                webRequest.UseDefaultCredentials = true;
            else
                webRequest.Credentials = Credentials;
#if NETFX
            webRequest.PreAuthenticate = true;
            webRequest.ServicePoint.Expect100Continue = false;

            //collect certificates
            /*
            if (!EnvironmentEx.IsRunningOnMono) 
                webRequest.ClientCertificates = Tools.Certificates.CollectCertificates();
            */

            webRequest.AllowAutoRedirect = true;
            webRequest.MaximumAutomaticRedirections = 10;
            webRequest.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip | DecompressionMethods.None;
            webRequest.UserAgent = UserAgent;
#elif UNIVERSAL
            webRequest.Headers[HttpRequestHeader.UserAgent] = UserAgent;
#endif
            webRequest.Accept = "*/*";

            try
            {
#if NETFX
                //build headers
                if (headers != null)
                    foreach (var header in headers)
                        webRequest.Headers.Add(header.Key, header.Value);
#endif
                //setup method
                switch (method)
                {
                    case HttpMethods.Get:
                        webRequest.Method = "GET";
                        break;
                    case HttpMethods.Post:
                        webRequest.Method = "POST";
                        break;
                    case HttpMethods.Put:
                        webRequest.Method = "PUT";
                        break;
                    default:
                        DebugEx.Assert("Invalid Method");
                        throw new NotImplementedException("Invalid Method");
                }

                //build request
                if (method == HttpMethods.Get)
                {
#if NETFX
                    webRequest.ContentLength = 0;
                    webRequest.ContentType = webRequest.MediaType = "application/x-www-form-urlencoded";
#endif
                }
                else
                {
                    string contentType = null;
                    switch (dataFormat)
                    {
                        case HttpRequestDataFormat.Json:
                            contentType = "application/json";
                            break;
                        case HttpRequestDataFormat.Xml:
                            contentType = "application/xhtml+xml";
                            break;
                        case HttpRequestDataFormat.FormData:
                            contentType = "application/x-www-form-urlencoded";
                            break;
                        case HttpRequestDataFormat.Text:
                            contentType = "mime/text";
                            break;
                        case HttpRequestDataFormat.Soap:
                            if (headers.ContainsKey("SoapContent-Type"))
                            {
                                contentType = headers["SoapContent-Type"];
                                webRequest.Headers.Remove("SoapContent-Type");
                            }
                            break;
                        case HttpRequestDataFormat.Binary:
                            contentType = "application/octet-stream";
                            break;
                    }
                    webRequest.ContentType = contentType;
#if NETFX
                    webRequest.MediaType = contentType;
#endif

                    //write body data
                    var body_data = data ?? new byte[0];
#if NETFX
                    webRequest.ContentLength = body_data.Length;
                    using (var stream = webRequest.GetRequestStream())
                        stream.Write(body_data, 0, body_data.Length);
#elif UNIVERSAL
                    using (var stream = webRequest.GetRequestStreamAsync().GetResults())
                        stream.Write(body_data, 0, body_data.Length);
#endif
                }

                //get response
                HttpWebResponse response = null;
                Stream respstream = null;
                try
                {
#if NETFX
                    var _response = webRequest.GetResponse();
#elif UNIVERSAL
                    var _response = webRequest.GetResponseAsync().GetResults();
#endif
                    response = (HttpWebResponse)_response;
                    respstream = response.GetResponseStream();
                }
                catch (WebException ex)
                {
                    response = ex.Response as HttpWebResponse;
                    if (response != null && response.StatusCode == HttpStatusCode.RedirectMethod)
                        DebugEx.TraceLog("Redirected from url " + url + " to url " + response.ResponseUri);
                    else if (response != null && response.StatusCode == HttpStatusCode.NotFound)
                        DebugEx.TraceLog("404 - Not Found. Url=" + url);
                    else
                        DebugEx.TraceError(ex, "Unhandled exception during webRequest.GetResponse() for url " + url);
                    //try get response stream
                    try
                    {
                        if (response != null)
                            respstream = response.GetResponseStream();
                    }
                    catch (WebException ex2)
                    {
                        DebugEx.TraceError(ex2, "Unhandled exception during redirect webRequest.GetResponseStream() for url " + url);
                    }
                }


                //Read results and execute callback                
                if (response != null)
                {
                    //find encoding
                    var encoding = response.Headers["content-encoding"];
                    var contentType = response.Headers["content-type"];
                    var contentTypeEntries = contentType?.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    if (contentTypeEntries != null)
                        for (int n = 0; n > contentTypeEntries.Length; n++)
                            contentTypeEntries[n] = contentTypeEntries[n].Trim();
                    //find charset
                    string charset = "";
                    if (contentTypeEntries != null && contentType != null && contentType.Contains("charset="))
                        try
                        {
                            var entry = contentTypeEntries.FirstOrDefault(e => e.ToLowerInvariant().StartsWith("charset="));
                            if (!string.IsNullOrWhiteSpace(entry))
                                charset = entry.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries)[1].ToLowerInvariant();
                        }
                        catch { charset = "utf-8"; }

                    //consume stream
                    byte[] bytes = null;
                    if (respstream != null)
                        try
                        {
                            using (var memStream = new MemoryStream())
                            {
                                if (progressReportCallback == null)
                                {
                                    //unzip or just copy to memstream
                                    if (encoding != null && encoding.ToLowerInvariant() == "gzip")
                                        using (var defStream = new GZipStream(respstream, CompressionMode.Decompress))
                                            defStream.CopyTo(memStream);
                                    else
                                        respstream.CopyTo(memStream);
                                }
                                else
                                {
                                    //unzip or just copy to memstream
                                    if (encoding != null && encoding.ToLowerInvariant() == "gzip")
                                        using (var defStream = new GZipStream(respstream, CompressionMode.Decompress))
                                            StreamCopy(defStream, memStream, progressReportCallback, response.ContentLength);
                                    else
                                        StreamCopy(respstream, memStream, progressReportCallback, response.ContentLength);
                                }

                                //get bytes
                                bytes = memStream.ToArray();
                            }
                        }
                        catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception while reading response stream"); }

                    //build result
                    var result = new RequestResult()
                    {
                        IsValid = true,
                        ResponseUri = response.ResponseUri,
                        ResponseBodyBinary = bytes,
                        ContentTypeEntries = contentTypeEntries,
                        Charset = charset,
                        Cookies = cookieContainer.GetCookies(new Uri(url)).Cast<Cookie>().ToList(),
                        StatusCode = response.StatusCode,
                        IsSuccessStatusCode = ((int)response.StatusCode >= 200 && (int)response.StatusCode <= 299),
                    };
                    if (callback != null)
                        callback(result);
                    //close and dispose response
#if NETFX
                    response.Close();
#endif
                    response.Dispose();
                    //give result back
                    return result;
                }
                else
                    return default(RequestResult);
            }
            catch (Exception ex)
            {
                DebugEx.TraceError(ex, "Http request failed");
                return default(RequestResult);
            }
        }


        static void StreamCopy(Stream from, Stream to, ProgressReportDelegate cb, long ContentLength)
        {
            byte[] buffer = new byte[4 * 1024];
            long totalRead = 0;
            double invContentLength = ContentLength == 0 ? 0 : 1d / ContentLength;
            try
            {
                while (true)
                {
                    if (!from.CanRead)
                        return;
                    //copy
                    var read = from.Read(buffer, 0, buffer.Length);
                    if (read == 0 || read == -1)
                        return;
                    to.Write(buffer, 0, read);
                    //accumulate and compute percentage
                    totalRead += read;
                    double perc = (totalRead * invContentLength).Saturate();
                    try { cb?.Invoke(totalRead, ContentLength, perc); } catch { cb = null; } //if it causes exception then be gone with it
                }
            }
            finally { try { cb?.Invoke(totalRead, ContentLength, 1); } catch { } } //closure
        }

        public static string BuildParameters(Dictionary<string, string> parameters)
        {
            var sb = new StringBuilder();
            foreach (var entry in parameters)
            {
                sb.Append(Uri.EscapeDataString(entry.Key));
                sb.Append("=");
                sb.Append(Uri.EscapeDataString(entry.Value));
                sb.Append("&");
            }
            return sb.ToString();
        }
    }
}
