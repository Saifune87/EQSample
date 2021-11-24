using System;

namespace EQService.MessageEnums
{
    /// <summary>
    /// Public Enum for the status of a message in the email message queue table
    /// </summary>
    public enum MessageStatus
    {
        New = 0,
        Sent = 1,
        Rejected = 2
    }
}
