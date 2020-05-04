using System.Collections.Generic;

namespace Engine.Models
{
    public class Globals
    {
        public IDictionary<string, object> globals;
        public Globals(IDictionary<string, object> parameters)
        {
            globals = parameters;
        }
    }
}
