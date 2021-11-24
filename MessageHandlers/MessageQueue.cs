using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace EQService.MessageHandlers
{
    public sealed class MessageQueue : IMessageQueue
    {   
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public MessageQueue(string connectionString)
        {
            _connectionString = connectionString;

            //loading the data for the model. 
            Load();
        }

        /// <summary>
        /// Pulls a message from the data table and returns it in the message format.
        /// </summary>
        /// <returns></returns>
        public Message Pop()
        {
            if (!_messages.Any())
            {
                return null;
            }

            var message = _messages.Dequeue();

            message.EQTMID = Guid.NewGuid();

            return message;
        }

        /// <summary>
        /// Private constructor for Messages in the message queue method.
        /// </summary>
        private Queue<Message> _messages = new Queue<Message>();
        
        /// <summary>
        /// private method to retreive the data from the email message queue. 
        /// </summary>
        /// <returns></returns>
        private void Load()
        {
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();

                _messages = new Queue<Message>(con.Query<Message>("",
                param: null,
                commandType: CommandType.StoredProcedure));

            }
        }

        /// <summary>
        /// Implimentation of interface member IMessageQueue
        /// </summary>
        /// <param name="message"></param>
        public void Handled(Message message)
        {

        }    

        /// <summary>
        /// private connection string to connect the stored procs to the EQTestLocal_Con Db.
        /// </summary>
        private string _connectionString;
        
        ///// Below is used for getting back all Brand credentials to be sent with each message. 
        
        public void GetBrandInfo()
        {            
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();

                _messageBrand = new Queue<MessageBrandCredentials>(con.Query<MessageBrandCredentials>("",
                param: null,
                commandType: CommandType.StoredProcedure));

            }
        }

        public MessageBrandCredentials BrandPop()
        {
            if (!_messageBrand.Any())
            {
                return null;
            }

            var messageBrand = _messageBrand.Dequeue();

            return messageBrand;
        }
        
        public void BrandHandled(MessageBrandCredentials messageBrand)
        {

        }                    

        /// <summary>
        /// Private constructor for Messages in the message queue method.
        /// </summary>
        private Queue<MessageBrandCredentials> _messageBrand = new Queue<MessageBrandCredentials>();

    }    

}
