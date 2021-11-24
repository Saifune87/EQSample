using System;

namespace WSEmailQueueService.MessageArguments
{
    /// <summary>
    /// Arguments specific with order confirmation emails plus generic arguments. 
    /// </summary>
    public sealed class OrderConfirmationMessageArgs
    {
        public Guid OrderGroupID { get; set; }
        public int MessageID { get; set; }
    }

}
