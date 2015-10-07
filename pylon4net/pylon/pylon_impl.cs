using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.Net;
using System.Net.Sockets;
using HttpMachine;
using System.Reflection;
using log4net;
namespace Pylon
{

    public class  ParserHandler : IHttpParserHandler
    {
        private Request request;
        private string headerName;
        private string headerValue;
        public void setData(Request request)
        {
            this.request    = request;
            this.headerName = "";
            this.headerValue = "";

        }
        public void OnBody(HttpParser parser, ArraySegment<byte> data)
        {
            this.request.body = data;
        }
        public void OnFragment(HttpParser parser, string fragment)
        { }
        public void OnHeaderName(HttpParser parser, string name)
        {
            this.headerName = name; 

        }
        public void OnHeadersEnd(HttpParser parser)
        { }
        public void OnHeaderValue(HttpParser parser, string value)
        {
            this.headerValue = value;
            this.request.headers[this.headerName] = this.headerValue;
        }
        public void OnMessageBegin(HttpParser parser)
        { }
        public void OnMessageEnd(HttpParser parser)
        { }
        public void OnMethod(HttpParser parser, string method)
        {
            this.request.method = method;

        }
        public void OnQueryString(HttpParser parser, string queryString)
        {
            this.request.query = queryString; 
        }
        public void OnRequestUri(HttpParser parser, string requestUri)
        {
            this.request.uri = requestUri;
        }
    }

    class HttpSvc
    {
        private TcpListener listener;
        Router router;
        ILog    logger;
        public HttpSvc()
        {
            logger = LogManager.GetLogger("_pylon");
        }
        public void start(Router router,int port = 80 )
        {
            this.router = router;
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            logger.InfoFormat("Pylon HttpSvc started! port[{0:d}]", port);
            listener.BeginAcceptTcpClient(new AsyncCallback(AcceptCallback), listener);
        }
        public  void stop()
        {
            if (listener != null)
            {
                listener.Stop();
                logger.Info("Pylon HttpSvc Stoped!");
            }
        }
        private void AcceptCallback(IAsyncResult ar)
        {
            var listener = ar.AsyncState as TcpListener;

            try
            {
                var client = listener.EndAcceptTcpClient(ar);       
                if (client != null)
                {
                    NewTcpClient(client);
                    //new Thread(new ParameterizedThreadStart(NewTcpClient)).Start(client);
                }
            }
            finally
            {
                try { listener.BeginAcceptTcpClient(new AsyncCallback(AcceptCallback), listener); } catch { }
            }
        }

        private void NewTcpClient(object obj)
        {

            var client      = obj as TcpClient;
            var stream      = client.GetStream();
            var handler     = new ParserHandler();
            var requ        = new Request();
            var context     = new Context();
            var response    = new Response();
          
            var parser      = new HttpParser(handler);
            handler.setData(requ);
            int bytesRead = 0 ;
            var buffer = new byte[1024 /* or whatever you like */];

            try
            {

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (bytesRead == parser.Execute(new ArraySegment<byte>(buffer, 0, bytesRead)))
                        break;
                }
                // ensure you get the last callbacks.
                parser.Execute(default(ArraySegment<byte>));

                logger.InfoFormat("[{0}] URI : {1}", requ.method,requ.uri);
                var svc = this.router.route(requ.uri);
                switch (requ.method)
                {
                   case "GET" :
                     svc._get(context, requ, response);
                     break;

                    case "POST":
                        svc._post(context, requ, response);
                        break;
                }
                response.send(stream);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("exception: {0} ", ex.Message);
            }
            finally
            {
                stream.Close();
                client.Close();

            }
        }
    }
    class Router
    {
        IDictionary<string, RestService> uriSvcs;
        public Router()
        {
            this.uriSvcs = new Dictionary<string, RestService>();
        }
        public void assemble()
        {
            Type t = typeof(DemoService);
      
            registSvc(t);

        }

        public void registAssemble(Assembly ass)
        {

            //Assembly ass = Assembly.Load(name);
            Type[] types  = ass.GetTypes();

            foreach (Type t in types)
            {
                if (t.IsClass && t.GetInterface("RestService") != null)
                {
                    this.registSvc(t);
                }
            }

        }

        private void registSvc(Type t)
        {
            Type[] nullType= new Type[0];
            object[] args = new object[0];


            RestService svc = t.GetConstructor(nullType).Invoke(args) as RestService;  
            object[] objs = t.GetCustomAttributes(typeof(REST), true);
            if (objs.Length == 1)
            {
                REST cur = objs[0] as REST;
                this.uriSvcs[cur.uri] = svc;

            }
        }
        public RestService route(string uri)
        {
            if (this.uriSvcs.ContainsKey(uri))
            {
                RestService svc = this.uriSvcs[uri];
                //TODO clone ;
                return svc;
            }
            return new NotFound();
        }
    }
    public class PylonRestIMPL
    {

        HttpSvc httpSvc;
        Router router;

        public PylonRestIMPL()
        {

            this.httpSvc = new HttpSvc();
            this.router = new Router();
            this.router.assemble();
        }
        public void registAssemble(Assembly ass )
        {
            this.router.registAssemble(ass);
        }
        public void registAssemble(string assname)
        {
            Assembly ass = Assembly.Load(assname);
            this.router.registAssemble(ass);
        }

        public void run()
        {

            this.httpSvc.start(this.router);

        }
        public void stop()
        {
            this.httpSvc.stop();
        }
    }

     

       
}
