using System;
using EQService.MessageEnums;
using System.Collections.Generic;

namespace EQService.MessageHandlers
{
    public sealed class Message
    {
        public int MessageID { get; set; }
        public MessageType MessageTypeID { get; set; }
        public Guid OrderGroupID { get; set; }
        public string Subject { get; set; }
        public string TextMessage { get; set; }
        public string Arguments { get; set; }
        public string ToEmailAddress { get; set; }
        public string RecipientName { get; set; }
        public string SenderName { get; set; }
        public string SenderEmailAddress { get; set; }
        public string SwimStoreID { get; set; }
        public DateTime DateCreated { get; set; }
        public int StatusCode { get; set; }
        public DateTime DateLastTried { get; set; }
        public int FailedCount { get; set; }
        public Guid EQTMID { get; set; }
        //Two below are not apart of the Email Message Queue table but are used in this MessageType class
        public string OrderNumber { get; set; }
        public string OrigOrderGroupID { get; set; }
        public string HomeURL { get; set; }
    }

}
