using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Xml.Linq;
using Dapper;
using EQService.ApiSubmissionModel;
using EQService.MessageArguments;
using EQService.MessageHandlers;
using EQService.MessageTypeModels;
using System.Configuration;

namespace EQService.MessageTypeHandlers
{
    /// <summary>
    /// MessageType handler for ReturnConfirmation
    /// </summary>
    public sealed class ReturnConfirmationMessageTypeHandler : IMessageTypeHandler
    {
        /// <summary>
        /// handel method that brings the map, getData and arguments together for Return Confirmation.
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
        /// Parsed message Arguments for return confirmation. 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private object ParseMessageArguments(Message message)
        {
            XDocument xdoc = XDocument.Parse(message.Arguments);

            var root = xdoc.Root;

            var result = new
            {
                OrigOrderGroupID = root.Element("OrigOrderGroupID").Value,
                MessageID = message.MessageID,
                OrderGroupID = message.OrderGroupID,
                SwimStoreID = message.SwimStoreID
            };

            return result;
        }

        /// <summary>
        /// Data mapper method for return confirmation.
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
            RecepientTransRecord.SubmissionSequence = 1;
            RecepientTransRecord.FormId = int.Parse(rt.RT_FormID);
            RecepientTransRecord.TableName = "Recipient_Transactional";

            
            RecepientTransRecord.Records = new List<ApiSubmissionRecord>();
            var rtrecord = new ApiSubmissionRecord();
            rtrecord.Fields = new List<ApiSubmissionRecordField>();
            
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "EQTMID", Value = message.EQTMID });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "division", Value = rt.Division });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "name_first", Value = rt.FirstName });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "name_last", Value = rt.LastName });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "email", Value = rt.SoldToEmailAddress });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "subtotal", Value = rt.SubTotal });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "taxtotal", Value = rt.TaxTotal });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "credit_amount", Value = rt.CreditAmount });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "order_number", Value = rt.OrderNumber });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "transnumber", Value = rt.TransNumber });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "transdate", Value = rt.TransDate });                          
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "shippingtotal", Value = rt.ShippingTotal });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "totaldiscount", Value = rt.TotalDiscount });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "paymentmethod", Value = rt.PaymentMethod });

            RecepientTransRecord.Records.Add(rtrecord);

            var OrderDetailTransRecord = new ApiSubmissionRecords();
            OrderDetailTransRecord.SubmissionSequence = 2;
            OrderDetailTransRecord.FormId = int.Parse(rt.ODT_FormID);
            OrderDetailTransRecord.TableName = "Order_Detail_Transactional";

            OrderDetailTransRecord.Records = new List<ApiSubmissionRecord>();

            foreach (var odt in odtr)
            {
                var odtRecord = new ApiSubmissionRecord();
                odtRecord.Fields = new List<ApiSubmissionRecordField>();
                
                odtRecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "EQTMID", Value = message.EQTMID });
                odtRecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "productid", Value = odt.ProductID });
                odtRecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "brand", Value = odt.Brand });
                odtRecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "style", Value = odt.Size });
                odtRecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "color", Value = odt.Color });
                odtRecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "size", Value = odt.Size });
                odtRecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "MaterialType", Value = odt.MaterialType });
                odtRecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "quantity", Value = odt.Quantity });
                odtRecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "price", Value = odt.Price });
                odtRecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "thumbnailurl", Value = odt.ThumbnailUrl });
                odtRecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "producturl", Value = odt.ProductUrl });
                odtRecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "discount", Value = odt.Discount });
                odtRecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "totalcost", Value = odt.TotalCost });

                OrderDetailTransRecord.Records.Add(odtRecord);
                mapper.Records.Add(RecepientTransRecord);
                mapper.Records.Add(OrderDetailTransRecord);
            }
            return mapper;

        }

        /// <summary>
        /// Get data method for return confirmation. 
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
