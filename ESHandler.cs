using EQService.MessageHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace EQService
{
    public class ESHandler : ServiceBase
    {
        /// <summary>
        /// Entry point of the application
        /// </summary>
        static void Main()
        {
            MessageQueueHandler prog = new MessageQueueHandler();

            string output = prog.ProcessMessageQueue();

        }
    }
}
