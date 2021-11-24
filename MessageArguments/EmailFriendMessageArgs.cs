using System;

namespace WSEmailQueueService.MessageArguments
{
    /// <summary>
    /// Arguments specific with email friend emails plus generic arguments. 
    /// </summary>
    public sealed class EmailFriendMessageArgs 
    {
        public string WebStore { get; set; }
        public string ProductId { get; set; }
        public int MessageID { get; set; }
    }
}
