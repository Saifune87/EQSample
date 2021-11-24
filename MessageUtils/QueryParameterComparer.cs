using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EQService.MessageUtils
{
    public sealed class QueryParameterComparer : IComparer<QueryParameter>
    {        
        public int Compare(QueryParameter x, QueryParameter y)
        {
            if (x.Name == y.Name)
            {
                return string.Compare(x.Value, y.Value);
            }
            else
            {
                return string.Compare(x.Name, y.Name);
            }
        }

        
    }

}
