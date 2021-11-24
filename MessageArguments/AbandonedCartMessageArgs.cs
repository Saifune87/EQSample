using System;

namespace WSEmailQueueService.MessageArguments
{
    /// <summary>
    /// Arguments specific with abandoned cart emails plus generic arguments.
    /// </summary>
    public sealed class AbandonedCartMessageArgs
    {
        public Guid OrderGroupID { get; set; }
        public string UserID { get; set; } 
    }
}
