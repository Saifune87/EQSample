using System;

namespace WSEmailQueueService.MessageArguments
{
    /// <summary>
    /// Arguments specific with shipping notification emails plus generic arguments. 
    /// </summary>
    public sealed class ShipmentNotificationMessageArgs 
    {
        public string OrderNumber { get; set; }
    }

}
