using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EQService.MessageUtils
{   
    public sealed class QueryParameter
    {
        private string name = null;
        private string value = null;

        public QueryParameter(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        public string Name
        {
            get { return name; }
        }

        public string Value
        {
            get { return value; }
        }
    }

}
