using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace Pylon
{
    public class REST : Attribute
    {
        public string uri;
        public REST(string uri)
        {
            this.uri = uri;
        }
    }
    public interface RestService
    {
        void _get( Context context, Request request, Response response);
        void _post(Context context, Request request, Response response);
        //void clone();
    }

    public class Request
    {
        public Request()
        {
            this.headers    = new Dictionary<string, string>();
            this.body       = new ArraySegment<byte>();
        }
        public string method;
        public string uri;
        public string query;

        public IDictionary<string, string> headers;
        public ArraySegment<byte> body;
    }

    class RestError
    {
        public string code;
        public string message;
        public string type;
        public string sub_code;
        public string prompt_info;
        public string prompt_type;
    }
    public class Response
    {

        public Response()
        {
            this.headers = new Dictionary<string, string>();
            this.codes = new Dictionary<int, string>();

            initCodes();



            headers["Server"] = "PYLON-REST/1.0(windows)";
            headers["Content-type"] = "application/json";


        }

        private void initCodes()
        {
            this.codes[100] = "Continue";
            this.codes[101] = "Switching Protocols";
            this.codes[200] = "OK";
            this.codes[201] = "Created";
            this.codes[202] = "Accepted";
            this.codes[203] = "Non-authoritative Information";
            this.codes[204] = "No Content";
            this.codes[205] = "Reset Content";
            this.codes[206] = "Partial Content";
            this.codes[300] = "Multiple Choices";
            this.codes[301] = "Moved Permanently";
            this.codes[302] = "Found";
            this.codes[303] = "See Other";
            this.codes[304] = "Not Modified";
            this.codes[305] = "Use Proxy";
            this.codes[306] = "Unused";
            this.codes[307] = "Temporary Redirect";


            this.codes[400] = "Bad Request";
            this.codes[401] = "Unauthorized";
            this.codes[402] = "Payment Required";
            this.codes[403] = "Forbidden";
            this.codes[404] = "Not Found";
            this.codes[405] = "Method Not Allowed";
            this.codes[406] = "Not Acceptable";
            this.codes[407] = "Proxy Authentication Required";
            this.codes[408] = "Request Timeout";
            this.codes[409] = "Conflict";
            this.codes[410] = "Gone";
            this.codes[411] = "Length Required";
            this.codes[412] = "Precondition Failed";
            this.codes[413] = "Request Entity Too Large";

            this.codes[500] = "Internal Server Error";
            this.codes[501] = "Not Implemented";
            this.codes[502] = "Bad Gateway";
            this.codes[503] = "Service Unavailable";
            this.codes[504] = "Gateway Timeout";
            this.codes[505] = "HTTP Version Not Supported";
        }

        public int status;
        public string body;
        public IDictionary<string, string> headers;
        public IDictionary<int, string> codes;

        public void send(Stream stream)
        {
            var data = string.Format("HTTP/1.1 {0:D3} {1}\r\n", this.status, this.codes[this.status]);
            sendString(stream, data);
            
            this.headers["Content-Length"] = Encoding.UTF8.GetBytes(this.body).Length.ToString();
            string headsStr = "";
            foreach (var i in this.headers)
            {
                headsStr += string.Format("{0}: {1}\r\n", i.Key.ToString(), i.Value.ToString());
            }
            headsStr += "\r\n";
            sendString(stream, headsStr);
            sendString(stream, this.body);
            stream.Close();
        }
        void sendString(Stream stream, string content)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            stream.Write(bytes, 0, bytes.Length);
        }
        public void success<T>(string name , T data,int status=200)
        {
            this.status = status;
            var dict    = new Dictionary<string, Object>();
            dict[name]  = data;
            this.body   = JsonConvert.SerializeObject(dict);

        }
        public void fail(int status, string message)
        {
            this.status = status;
            RestError e = new RestError();
            e.code      = status.ToString();
            e.message   = message;
            var data = new Dictionary<string, Object>();
            data["error"] = e; 
            this.body   = JsonConvert.SerializeObject(data);
        }
    }


    public class Context
    { }
    public class PylonRest
    {
        public void run()
        {

        }

    }
}
