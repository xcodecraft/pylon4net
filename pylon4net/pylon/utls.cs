using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.Net;
using System.IO;

namespace Pylon
{
    public class HttpResponse
    {

        public string status;
        public string body;
    }
    public class HttpCall 
    {
        static public HttpResponse get( string url, int bufmax = 2048, int port = 80, int timeout = 0 )
        {
            ILog log = LogManager.GetLogger("to-abc");

            DBC.notNull(url);
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";

            if (timeout != 0)
            {
                request.Timeout = timeout;
            }
            log.InfoFormat("[GET] URL:{0}", url);
            return remoteCall(bufmax, log, request);

        }

        private static HttpResponse remoteCall( int bufmax, ILog log, HttpWebRequest request )
        {
            HttpWebResponse response;
            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException e)
            {
                response = e.Response as HttpWebResponse;
            }
            Stream receiveStream = response.GetResponseStream();
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader readStream = new StreamReader(receiveStream, encode);
            Char[] read = new Char[bufmax];
            int count = readStream.Read(read, 0, bufmax);
            if (count == bufmax)
                throw new Exception("buf is small ");

            HttpResponse callRespon = new HttpResponse();
            callRespon.body = new String(read, 0, count);
            callRespon.status = response.StatusCode.ToString();
            log.InfoFormat("[GET] status {0} body:{1}", callRespon.status, callRespon.body);
            response.Close();
            readStream.Close();
            return callRespon;
        }

        static public HttpResponse post( string url, string data, int bufsize = 2048, int timeout = 0 )
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            return post(url, bytes, bufsize, timeout);
        }
        static public HttpResponse post( string url, byte[] data, int bufsize = 2048, int timeout = 0 )
        {
            ILog log = LogManager.GetLogger("to-abc");
            DBC.notNull(url);
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            log.InfoFormat("[POST] URL:{0} ", url);
            request.Method = "POST";


            if (timeout != 0)
            {
                request.Timeout = timeout;
            }

            request.ContentLength   = data.Length;
            Stream requestStream    = request.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();


            return remoteCall(bufsize, log, request);
        }
    }
}
