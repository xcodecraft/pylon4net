using System;
using System.Linq;
using System.Text;

namespace Pylon
{
    [REST("/demo")]
    public class DemoService : RestService
    {
        public void _get(Context context, Request request, Response response)
        {
            response.success("notice", "OK");
        }
        public void _post(Context context, Request request, Response response)
        { }
    }
    [REST("/404")]
    public class NotFound : RestService
    {
        public void _get(Context context, Request request, Response response)
        {
            response.fail(404, "notfount");
        }
        public void _post(Context context, Request request, Response response)
        {
            response.fail(404, "notfount");
        }
    }
}
