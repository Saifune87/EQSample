using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using Dapper;
using EQService.ApiSubmissionModel;
using EQService.MessageEnums;
using EQService.MessageTypeHandlers;
using EQService.MessageUtils;

namespace EQService.MessageHandlers
{
    public sealed class MessageQueueHandler
    {
        public MessageQueueHandler()
        {
            var brandQueue = new MessageQueue(ConfigurationManager.ConnectionStrings[""].ConnectionString);

            MessageBrandCredentials brandCreds = null;

            brandQueue.GetBrandInfo();

            brandCreds = brandQueue.BrandPop();

            _random = new Random();
            _params = new Dictionary<String, String>();
            _params["callback"] = "oob";
            _params["consumer_key"] = "";
            _params["consumer_secret"] = "";
            _params["timestamp"] = GenerateTimeStamp();
            _params["nonce"] = GenerateNonce();
            _params["signature_method"] = "HMAC-SHA1";
            _params["signature"] = "";
            _params["token"] = "";
            _params["token_secret"] = "";
            _params["version"] = "1.0";

            Init();
            BrandInit();
        }

        public MessageQueueHandler(string consumerKey,
                       string consumerSecret,
                       string token,
                       string tokenSecret)
            : this()
        {
            _params["consumer_key"] = consumerKey;
            _params["consumer_secret"] = consumerSecret;
            _params["token"] = token;
            _params["token_secret"] = tokenSecret;
        }


        /// <summary>
        /// public method to process messages and instantiate message handlers
        /// </summary>                       
        public string ProcessMessageQueue()
        {
            string retOutput = "";

            var queue = new MessageQueue(ConfigurationManager.ConnectionStrings[""].ConnectionString);

            Message message = null;

            // Foreach message...
            while ((message = queue.Pop()) != null)
            {
                // Find the handler associated with the message type.
                var handlerType = _messageTypeHandlers[message.MessageTypeID];               

                // Create the handler.
                var handler = (IMessageTypeHandler)Activator.CreateInstance(handlerType);

                // Handle the message (aka, get additional information).
                var model = handler.Handle(message);

                string output = null;

                using (var ms = new MemoryStream())
                {
                    var ser = new XmlSerializer(typeof(ApiSubmission));

                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("", "");

                    ser.Serialize(ms, model,ns);

                    ms.Seek(0, SeekOrigin.Begin);

                    using (var sr = new StreamReader(ms))
                    {
                        output = sr.ReadToEnd();
                    }
                }                

                retOutput = output.ToString();

                if (retOutput == null || model == null)
                {
                    Rejected(message, MessageStatus.Rejected);
                }
                else if (retOutput != null || model != null)
                {
                    //Acquire Request token & Post Data - OAuth.
                    AccessTokenGet(retOutput, model.CustomerID);

                    //Updates Database with dataSent
                    Sent(message, MessageStatus.Sent);
                }
            }

            return retOutput;

        }

        /// <summary>
        /// 
        /// </summary>
        private void BrandInit()
        {
            //Will be adding test samples later
        }

        /// <summary>
        /// public method defining the handler of a message type. 
        /// </summary>
        private void Init()
        {
            _messageTypeHandlers[MessageType.AbandonedCart] = typeof(AbandonedCartMessageTypeHandler);
            _messageTypeHandlers[MessageType.EmailFriend] = typeof(EmailFriendMessageTypeHandler);
            _messageTypeHandlers[MessageType.EmailWishList] = typeof(EmailWishListMessageTypeHandler);
            _messageTypeHandlers[MessageType.EReceiptsFF] = typeof(EReceiptsMessageTypeHandler);
            _messageTypeHandlers[MessageType.EReceiptsNat] = typeof(EReceiptsMessageTypeHandler);
            _messageTypeHandlers[MessageType.EReceiptsNatCAEng] = typeof(EReceiptsMessageTypeHandler);
            _messageTypeHandlers[MessageType.EReceiptsNatCAFre] = typeof(EReceiptsMessageTypeHandler);
            _messageTypeHandlers[MessageType.EReceiptsSamE] = typeof(EReceiptsMessageTypeHandler);
            _messageTypeHandlers[MessageType.EReceiptsDrSch] = typeof(EReceiptsMessageTypeHandler);
            _messageTypeHandlers[MessageType.HD_OrderConfirmation] = typeof(HDOrderConfirmationMessageTypeHandler);
            _messageTypeHandlers[MessageType.HD_OrderCutOff] = typeof(HDOrderCutOffsMessageTypeHandler);
            _messageTypeHandlers[MessageType.HD_ShippingNotification] = typeof(HDShippingNotificationMessageTypeHandler);
            _messageTypeHandlers[MessageType.OrderConfirmation] = typeof(OrderConfirmationMessageTypeHandler);
            _messageTypeHandlers[MessageType.ReturnConfirmation] = typeof(ReturnConfirmationMessageTypeHandler);
            _messageTypeHandlers[MessageType.ShippingNotification] = typeof(ShippingNotificationMessageTypeHandler);
            _messageTypeHandlers[MessageType.STH_OrderExpired] = typeof(ShipToHomeMessageTypeHandler);
            _messageTypeHandlers[MessageType.STH_PickUpReminder] = typeof(ShipToHomeMessageTypeHandler);
            _messageTypeHandlers[MessageType.STH_ReadyPickUp] = typeof(ShipToHomeMessageTypeHandler);
            _messageTypeHandlers[MessageType.SizeNotificationAvailable] = typeof(SizeNotificationMessageTypeHandler);
            _messageTypeHandlers[MessageType.SizeNotificationOptions] = typeof(SizeNotificationMessageTypeHandler);
            _messageTypeHandlers[MessageType.SizeNotificationThankYou] = typeof(SizeNotificationMessageTypeHandler);
        }

        /// <summary>
        /// public method to be called when a message is sent successfully. 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="status"></param>
        private void Sent(Message message, MessageStatus status)
        {
            UpdateMessageQueue(message, MessageStatus.Sent, "");
        }

        /// <summary>
        /// public method to be called when a message is rejected for an error.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="status"></param>
        private void Rejected(Message message, MessageStatus status)
        {
            UpdateMessageQueue(message, MessageStatus.Rejected, "DataModel Null");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="log"></param>
        private void LogError(Message message, MessageStatus status, string errorMessage)
        {
            UpdateMessageQueue(message, MessageStatus.Rejected, "DataModel Null");
        }

        /// <summary>
        /// Method for updating the email message queue arguments.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private void UpdateMessageQueue(Message message, MessageStatus Status, string ErrorMessage)
        {
            //Fix Sql portion later. 

        }

        /// <summary>
        /// Exchange the request token for an access token.
        /// </summary>
        /// <param name="authToken">OAuth token acquired by TestMP.</param>
        private void AccessTokenGet(string postData, string model)
        {
            string authToken = string.Empty;

            this.token = authToken;

            //Takes request token that was acquired and Post it in Message.
            string response = OAuthWebRequest(Method.POST, token, postData, model);

            if (response.Length > 0)
            {
                //Store the Token for the request. 
                NameValueCollection qs = HttpUtility.ParseQueryString(response);
                if (qs["oauth_token"] != null)
                {
                    this.token = qs["oauth_token"];
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method">POST</param>
        /// <param name="postData">Output of the Serialized Xml.</param>
        /// <returns>Xml Output</returns>
        private string OAuthWebRequest(Method method, string token, string postData, string model)
        {
            var bQueue = new MessageQueue(ConfigurationManager.ConnectionStrings[""].ConnectionString);

            MessageBrandCredentials bCreds = null;

            bQueue.GetBrandInfo();

            string retVal = "";

            bCreds = bQueue.BrandPop();
            string url = "http://testURI.com/POST";          

            string queryheader = "";

            //To place the messageBrand Uri into this current context.
            Uri uri = new Uri(url);
            string urls = uri.ToString();

            string timeStamp = this.GenerateTimeStamp();

            //Generate Signature - Second method.
            string oSig = GenerateAuthzHeader(urls, method.ToString());

            queryheader = HttpUtility.UrlDecode(oSig);

            if (method == Method.POST && queryheader.Length > 0)
            {
               retVal = AcquireRequestToken(urls, "POST", postData).ToString();                  
            }

            return retVal;

        }   

        #region Generate Signature method 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="postData"></param>
        /// <returns></returns>
        private OAuthResponse AcquireRequestToken(string uri, string method, string postData)
        {
            NewRequest();
            uri = "http://testURI.com/POST";
            method = "POST";
            var authzHeader = GetAuthorizationHeader(uri, method);

            // prepare the token request
            var request = (HttpWebRequest)System.Net.WebRequest.Create(uri);
            request.Headers.Add("Authorization", authzHeader);
            request.Method = method;             
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = byteArray.Length; 
            request.ContentType = "raw";

            string responseData = "";

            using (var sw = new StreamWriter(request.GetRequestStream()))
            {
                sw.Write(postData,0,postData.Length);
                sw.Close();
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    responseData = reader.ReadToEnd();

                    var r = new OAuthResponse(reader.ReadToEnd());

                    return r;
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ix"></param>
        /// <returns></returns>
        private string this[string ix]
        {
            get
            {
                if (_params.ContainsKey(ix))
                    return _params[ix];
                throw new ArgumentException(ix);
            }
            set
            {
                if (!_params.ContainsKey(ix))
                    throw new ArgumentException(ix);
                _params[ix] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string UrlEncode(string value)
        {
            var result = new System.Text.StringBuilder();
            foreach (char symbol in value)
            {
                if (unreservedChars.IndexOf(symbol) != -1)
                    result.Append(symbol);
                else
                    result.Append('%' + String.Format("{0:X2}", (int)symbol));
            }
            return result.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private string EncodeRequestParameters(ICollection<KeyValuePair<String, String>> p)
        {
            var sb = new System.Text.StringBuilder();
            foreach (KeyValuePair<String, String> item in p.OrderBy(x => x.Key))
            {
                if (!String.IsNullOrEmpty(item.Value) && !item.Key.EndsWith("secret"))
                    sb.AppendFormat("oauth_{0}=\"{1}\", ",
                                    item.Key,
                                    UrlEncode(item.Value));
            }

            return sb.ToString().TrimEnd(' ').TrimEnd(',');
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="method"></param>
        /// <param name="realm"></param>
        /// <returns></returns>
        private string GenerateCredsHeader(string uri, string method, string realm)
        {
            NewRequest();
            var authzHeader = GetAuthorizationHeader(uri, method, realm);
            return authzHeader;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private string GenerateAuthzHeader(string uri, string method)
        {
            NewRequest();
            var authzHeader = GetAuthorizationHeader(uri, method, null);
            return authzHeader;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private string GetAuthorizationHeader(string uri, string method)
        {
            return GetAuthorizationHeader(uri, method, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="method"></param>
        /// <param name="realm"></param>
        /// <returns></returns>
        private string GetAuthorizationHeader(string uri, string method, string realm)
        {
            //to set the realm to equal the uri
            realm = uri;

            if (string.IsNullOrEmpty(this._params["consumer_key"]))
                throw new ArgumentNullException("consumer_key");

            if (string.IsNullOrEmpty(this._params["consumer_secret"]))
                throw new ArgumentNullException("consumer_secret");

            if (string.IsNullOrEmpty(this._params["signature_method"]))
                throw new ArgumentNullException("signature_method");

            Sign(uri, method);

            var erp = EncodeRequestParameters(this._params);
            Trace.TraceInformation("erp = {0}", erp);
            return (String.IsNullOrEmpty(realm))
                ? "OAuth " + erp
                : String.Format("OAuth realm=\"{0}\", ", realm) + erp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="method"></param>
        private void Sign(string uri, string method)
        {
            var signatureBase = GetSignatureBase(uri, method);
            var hash = GetHash();

            byte[] dataBuffer = System.Text.Encoding.ASCII.GetBytes(signatureBase);
            byte[] hashBytes = hash.ComputeHash(dataBuffer);

            this["signature"] = Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        ///        
        /// </summary>
        /// <param name="value">The value to Url encode</param>
        /// <returns>Returns a Url encoded string</returns>
        private string UriEncode(string value)
        {
            StringBuilder result = new StringBuilder();

            foreach (char symbol in value)
            {
                //using the unreserved characters listed on line 742
                if (unreservedChars.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else
                {
                    result.Append('%' + String.Format("{0:X2}", (int)symbol));
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Formats the list of request parameters into "signature base" string as
        /// defined by RFC 5849.  This will then be MAC'd with a suitable hash.
        /// </summary>
        private string GetSignatureBase(string url, string method)
        {
            // normalize the URI
            var uri = new Uri(url);
            var normUrl = string.Format("{0}://{1}", uri.Scheme, uri.Host);
            if (!((uri.Scheme == "http" && uri.Port == 80) ||
                  (uri.Scheme == "https" && uri.Port == 443)))
                normUrl += ":" + uri.Port;

            normUrl += uri.AbsolutePath;

            // the sigbase starts with the method and the encoded URI
            var sb = new System.Text.StringBuilder();
            sb.Append(method)
                .Append('&')
                .Append(UriEncode(normUrl))
                .Append('&');

            var p = ExtractQueryParameters(uri.Query);
            // add all non-empty params to the "current" params
            foreach (var p1 in this._params)
            {
                if (!String.IsNullOrEmpty(this._params[p1.Key]) &&
                    !p1.Key.EndsWith("_secret") &&
                    !p1.Key.EndsWith("signature"))
                    p.Add("oauth_" + p1.Key, p1.Value);
            }

            // concat+format all those params
            var sb1 = new System.Text.StringBuilder();
            foreach (KeyValuePair<String, String> item in p.OrderBy(x => x.Key))
            {
                // even "empty" params need to be encoded this way.
                sb1.AppendFormat("{0}={1}&", item.Key, item.Value);
            }

            // append the UrlEncoded version of that string to the sigbase
            sb.Append(UrlEncode(sb1.ToString().TrimEnd('&')));
            var result = sb.ToString();
            Trace.TraceInformation("Sigbase: '{0}'", result);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private HashAlgorithm GetHash()
        {
            if (this["signature_method"] != "HMAC-SHA1")
                throw new NotImplementedException();

            string keystring = string.Format("{0}&{1}",
                                             UrlEncode(this["consumer_secret"]),
                                             UrlEncode(this["token_secret"]));
            Trace.TraceInformation("keystring: '{0}'", keystring);
            var hmacsha1 = new HMACSHA1
            {
                Key = System.Text.Encoding.ASCII.GetBytes(keystring)
            };
            return hmacsha1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private Dictionary<String, String> ExtractQueryParameters(string queryString)
        {
            if (queryString.StartsWith("?"))
                queryString = queryString.Remove(0, 1);

            var result = new Dictionary<String, String>();

            if (string.IsNullOrEmpty(queryString))
                return result;

            foreach (string s in queryString.Split('&'))
            {
                if (!string.IsNullOrEmpty(s) && !s.StartsWith("oauth_"))
                {
                    if (s.IndexOf('=') > -1)
                    {
                        string[] temp = s.Split('=');
                        result.Add(temp[0], temp[1]);
                    }
                    else
                        result.Add(s, string.Empty);
                }
            }

            return result;
        }

        #endregion

        /// <summary>
        /// Combination of timestamp and new request. 
        /// </summary>
        private void NewRequest()
        {
            _params["nonce"] = GenerateNonce();
            _params["timestamp"] = GenerateTimeStamp();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GenerateTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - _epoch;
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /// <summary>
        /// Random item to be sent along with the request. 
        /// </summary>
        /// <returns></returns>
        private string GenerateNonce()
        {
            var sb = new System.Text.StringBuilder();
            Random rnd = new Random();
            for (int i = 0; i < 8; i++)
            {
                int g = rnd.Next(16);
                switch (g)
                {
                    case 0:
                        // lowercase alpha
                        sb.Append((char)(rnd.Next(26) + 97), 1);
                        break;
                    default:
                        // numeric digits
                        sb.Append((char)(rnd.Next(10) + 48), 1);
                        break;
                }
            }
            return sb.ToString();
        }

        //Private properties

        /// <summary>
        /// Dictornary property for _messageTypeHandlers
        /// </summary>
        private readonly Dictionary<MessageType, Type> _messageTypeHandlers = new Dictionary<MessageType, Type>();

        /// <summary>
        /// 
        /// </summary>
        private readonly Dictionary<MessageBrand, Type> _brandCredentials = new Dictionary<MessageBrand, Type>();

        #region OAuth Properties: First and Second method.




        
        private string _token = "";
        private string token { get { return _token; } set { _token = value; } }
        private string _tokenSecret = "";
        private string tokenSecret { get { return _tokenSecret; } set { _tokenSecret = value; } }
        
        private enum Method { GET, POST, PUT, DELETE };
        private enum SignatureTypes { HMACSHA1, HMACSHA256 };

        
        private string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        private Dictionary<String, String> _params;
        private Random _random;

        #endregion
    }
}
