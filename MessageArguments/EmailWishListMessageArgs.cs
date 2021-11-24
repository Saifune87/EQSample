using System;

namespace WSEmailQueueService.MessageArguments
{
    /// <summary>
    /// Arguments specific with email wishlist email plus generic arguments.
    /// </summary>
    public sealed class EmailWishListMessageArgs 
    {
        public string UserID { get; set; }
        public Guid OrderGroupID { get; set; }
        public int MessageID { get; set; }
    }

}
