using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pylon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net.Config;
using log4net ;
namespace Pylon.Tests
{

    [TestClass()]
    public class PylonRestIMPLTests
    {
        public void setup()
        {
           
        }
        [TestMethod()]
        public void PylonRestSvcTest()
        {
            log4net.Config.BasicConfigurator.Configure();
            Task t = Task.Run(() => {
                var svc = new PylonRestSvc();
                svc.registAssemble("pylon4net");
                svc.run();
                svc.wait();
            });

            Thread.Sleep(100); 
            HttpResponse resp = HttpCall.get("http://127.0.0.1/demo");
            Assert.AreEqual(resp.body, "{\"notice\":\"OK\"}");
            resp = HttpCall.get("http://127.0.0.1/demo");
            resp = HttpCall.get("http://127.0.0.1/demo");
            resp = HttpCall.get("http://127.0.0.1/notfound");
            Console.WriteLine(resp.body);
            PylonRestSvc.stop();
            t.Wait();
        }

    }
}