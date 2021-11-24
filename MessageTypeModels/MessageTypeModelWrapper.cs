using System;
using System.Collections.Generic;

namespace EQService.MessageTypeModels
{
    public sealed class MessageTypeModelWrapper
    {
        public RecipientTransactionalModelMaster RT { get; set; }
        
        /// <summary>
        /// used for those with single lineitems
        /// </summary>
        public OrderDetailTransactionalModelMaster ODTs { get; set; }

        /// <summary>
        /// used for those with repeating lineitems
        /// </summary>
        public IEnumerable<OrderDetailTransactionalModelMaster> ODTr { get; set; }
    }
}
