using System;

namespace EQService.MessageArguments
{
    /// <summary>
    /// Arguments specific with eReceipts emails plus generic arguments.
    /// </summary>
    public sealed class EReceiptsMessageArgs 
    {
        public int MessageID { get; set; }
        public string WebStore { get; set; }
    }
}
