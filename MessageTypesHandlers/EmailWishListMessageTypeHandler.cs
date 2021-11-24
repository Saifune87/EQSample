using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Xml.Linq;
using Dapper;
using EQService.ApiSubmissionModel;
using EQService.MessageArguments;
using EQService.MessageHandlers;
using EQService.MessageTypeModels;

namespace EQService.MessageTypeHandlers
{
    /// <summary>
    /// Handler class for Email wishlist emails.
    /// </summary>
    public sealed class EmailWishListMessageTypeHandler : IMessageTypeHandler
    {
        /// <summary>
        /// handel method that brings the map, getData and arguments together for email wish list.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public ApiSubmission Handle(Message message)
        {
            var Args = ParseMessageArguments(message);

            var dataResult = GetData(Args);

            var model = new ApiSubmission();

            if (dataResult == null)
            {
                model = null;
            }

            else if (dataResult != null)
            {
                model = Map(dataResult, message);
            }

            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private object ParseMessageArguments(Message message)
        {
            XDocument doc = XDocument.Parse(message.Arguments);

            var root = doc.Root;

            var result = new 
            { 
                UserID = root.Element("UserID").Value, 
                MessageID = message.MessageID,
                OrderGroupID = message.OrderGroupID 
            };

            return result;
        }

        /// <summary>
        /// Mapper method to write out the Xml from the data gathered by the GetData method.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private ApiSubmission Map(MessageTypeModelWrapper model, Message message)
        {
            ApiSubmission mapper = new ApiSubmission();
            var rt = model.RT;
            var odtr = model.ODTr;

            mapper.CustomerID = rt.CustId;
            mapper.Records = new List<ApiSubmissionRecords>();

            var RecepientTransRecord = new ApiSubmissionRecords();
            RecepientTransRecord.SubmissionSequence = 2;
            RecepientTransRecord.FormId = int.Parse(rt.RT_FormID);
            RecepientTransRecord.TableName = "Recepient_Transactional";

   
            RecepientTransRecord.Records = new List<ApiSubmissionRecord>();
            var rtrecord = new ApiSubmissionRecord();
            rtrecord.Fields = new List<ApiSubmissionRecordField>();


            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "guid", Value = message.EQTMID });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "email", Value = rt.ToEmail });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "division", Value = rt.Division });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "name_first", Value = rt.FirstName });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "referral_note", Value = rt.ReferralNote });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "referrer_name", Value = rt.ReferrerName });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "sender_email", Value = rt.SenderEmail });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "subject", Value = rt.Subject });

            RecepientTransRecord.Records.Add(rtrecord);

            var OrderDetailTransRecord = new ApiSubmissionRecords();
            OrderDetailTransRecord.SubmissionSequence = 1;
            OrderDetailTransRecord.FormId = int.Parse(rt.ODT_FormID);
            OrderDetailTransRecord.TableName = "Order_Detail_Transactional";

            OrderDetailTransRecord.Records = new List<ApiSubmissionRecord>();

            foreach (var odt in odtr)
            {
                var odtRecord = new ApiSubmissionRecord();
                odtRecord.Fields = new List<ApiSubmissionRecordField>();


                odtRecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "guid", Value = message.EQTMID });
                odtRecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "brand", Value = odt.Brand });
                odtRecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "color", Value = odt.Color });
                odtRecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "productid", Value = odt.ProductID });
                odtRecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "producturl", Value = odt.ProductUrl });
                odtRecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "productvariantid", Value = odt.ProductvariantID });
                odtRecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "quantity", Value = odt.Quantity });
                odtRecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "size", Value = odt.Size });
                odtRecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "thumbnailurl", Value = odt.ThumbnailUrl });
                odtRecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "MaterialType", Value = odt.MaterialType });

                OrderDetailTransRecord.Records.Add(odtRecord);
            }
            mapper.Records.Add(RecepientTransRecord);
            mapper.Records.Add(OrderDetailTransRecord);


            return mapper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private MessageTypeModelWrapper GetData(object args)
        {
            string sqlConn = ConfigurationManager.ConnectionStrings["EQTestLocal_Con_Local"].ConnectionString;

            var rt = new RecipientTransactionalModelMaster();

            IEnumerable<OrderDetailTransactionalModelMaster> odtr = null;

            using (var con = new SqlConnection(sqlConn))
            {
                con.Open();

                using (var multi = con.QueryMultiple("",
                param: args,
                commandType: CommandType.StoredProcedure))
                {
                    rt = multi.Read<RecipientTransactionalModelMaster>().FirstOrDefault();

                    odtr = multi.Read<OrderDetailTransactionalModelMaster>();
                }
            }

            if (rt == null || !odtr.Any())
            {
                return null;
            }

            return new MessageTypeModelWrapper()
            {
                RT = rt,
                ODTr = odtr
            };
        }
        
    }
}
