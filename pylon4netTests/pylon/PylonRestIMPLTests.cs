using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pylon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace Pylon.Tests
{

    [TestClass()]
    public class PylonRestIMPLTests
    {
        static bool isCompleted;
        public static ManualResetEvent testTaskEvent = new ManualResetEvent(false);
        public void setup()
        {
           
        }
        [TestMethod()]
        public void PylonRestIMPLTest()
        {
            testTaskEvent.Reset();
            Task t = Task.Run(() => {
                var svc = new PylonRestIMPL();
                svc.registAssemble("pylon4net");
                svc.run();
                testTaskEvent.WaitOne();
            });

            Thread.Sleep(2000); 
            string resp = HttpCall.get("http://127.0.0.1/demo");
            testTaskEvent.Set();
            t.Wait();
            //Assert.Fail();
        }

        [TestMethod()]
        public void PylonRestIMPLTest1()
        {
            return;
            var svc = new PylonRestIMPL();
            svc.registAssemble("pylon4net");
            svc.run();
            Thread.Sleep(100000); 
            Assert.Fail();
        }
    }
}