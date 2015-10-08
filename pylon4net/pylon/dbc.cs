using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pylon 
{
    public class DBC
    {
        static public void notNull(string val)
        {
            if (string.IsNullOrEmpty(val))
                throw new ArgumentNullException("string is empty");
        }
        static public void notNull<T>( T val )
        {
        }
    }
}
