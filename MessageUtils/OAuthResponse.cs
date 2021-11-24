using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EQService.MessageUtils
{
    /// <summary>
    ///   A class to hold an OAuth response message.
    /// </summary>
    public sealed class OAuthResponse
    {
        public string AllText { get; set; }
        private Dictionary<String, String> _params;

        /// <summary>
        ///   a Dictionary of response parameters.
        /// </summary>
        public string this[string ix]
        {
            get
            {
                return _params[ix];
            }
        }

        public OAuthResponse(string alltext)
        {
            AllText = alltext;
            _params = new Dictionary<String, String>();
            var kvpairs = alltext.Split('&');
            foreach (var pair in kvpairs)
            {
                var kv = pair.Split('=');                
            }
        }
    }
}
