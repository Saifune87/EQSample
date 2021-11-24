using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EQService.MessageHandlers
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMessageQueue 
    {
        Message Pop();

        void Handled(Message message);

        MessageBrandCredentials BrandPop();

        void BrandHandled(MessageBrandCredentials messageBrand);
    }
}
