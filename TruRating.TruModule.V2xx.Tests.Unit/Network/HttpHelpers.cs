// The MIT License
// 
// Copyright (c) 2017 TruRating Ltd. https://www.trurating.com
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace TruRating.TruModule.V2xx.Tests.Unit.Network
{
    public static class HttpHelpers
    {
        public static WebResponse CreateWebResponse(HttpStatusCode httpStatus, MemoryStream responseObject)
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint) l.LocalEndpoint).Port;
            l.Stop();

            // Create a listener.
            string prefix = "http://localhost:" + port + "/";
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(prefix);
            listener.Start();
            try
            {
                listener.BeginGetContext((ar) =>
                {
                    HttpListenerContext context = listener.EndGetContext(ar);
                    HttpListenerRequest request = context.Request;

                    // Obtain a response object.
                    HttpListenerResponse response = context.Response;

                    response.StatusCode = (int) httpStatus;

                    // Construct a response.
                    if (responseObject != null)
                    {
                        byte[] buffer = responseObject.ToArray();

                        // Get a response stream and write the response to it.
                        Stream output = response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);
                    }

                    response.Close();
                }, null);

                WebClient client = new WebClient();
                try
                {
                    WebRequest request = WebRequest.Create(prefix);
                    request.Timeout = 30000;
                    return request.GetResponse();
                }
                catch (WebException e)
                {
                    return e.Response;
                }
            }
            finally
            {
                listener.Stop();
            }

            return null;
        }
    }
}