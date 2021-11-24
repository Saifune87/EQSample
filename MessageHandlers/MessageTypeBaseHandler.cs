using System;
using EQService.ApiSubmissionModel;

namespace EQService.MessageHandlers
{
    /// <summary>
    /// Abstract class, to handle the Type Parameter of TMessageArgs. 
    /// </summary>
    /// <typeparam name="TMessageArgs"></typeparam>
    public abstract class MessageTypeHandlerBase<TMessageArgs> 
    {
        protected abstract TMessageArgs ParseMessageArguments(Message message);
    }

    /// <summary>
    /// plublic interface utilizing both the type element of Message and also interfacing with MessageArgsBase to use all Args. 
    /// </summary>
    public interface IMessageTypeHandler
    {
        ApiSubmission Handle(Message message);
    }
}
