using System;

namespace WSEmailQueueService.MessageArguments
{
    /// <summary>
    /// Arguments specific with return confirmation emails plus generic arguments.
    /// </summary>
    public sealed class ReturnConfirmationMessageArgs
    {
        public Guid OrderGroupID { get; set; }
        public string OrigOrderGroupID { get; set; }
        public int MessageID { get; set; }
        public string SwimStoreID { get; set; }
    }

}
