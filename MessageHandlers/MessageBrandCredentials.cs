using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EQService.MessageHandlers
{
    public sealed class MessageBrandCredentials
    {
        public string CustID { get; set; }
        public string BrandName { get; set; }
        public string CustUri { get; set; }
        public string CustKey { get; set; }
        public string CustSecret { get; set; }
    }
}
