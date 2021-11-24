using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using EQService.ApiSubmissionModel;
using EQService.MessageArguments;
using EQService.MessageHandlers;
using EQService.MessageTypeModels;

namespace EQService.MessageTypeHandlers
{
    /// <summary>
    /// MessageType handler for OrderConfirmation
    /// </summary>
    public sealed class OrderConfirmationMessageTypeHandler : IMessageTypeHandler
    {
        /// <summary>
        /// handel method that brings the map, getData and arguments together for Order Confirmation.
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
            var result = new 
            { 
                MessageID = message.MessageID, 
                OrderGroupID = message.OrderGroupID 
            };

            return result;
        }

        /// <summary>
        /// private methdod for mapping the data to the model for order confirmation. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private ApiSubmission Map(MessageTypeModelWrapper model, Message message)
        {
            ApiSubmission mapper = new ApiSubmission();
            var rt = model.RT;
            var odts = model.ODTr;

            mapper.CustomerID = model.RT.CustId;
            mapper.Records = new List<ApiSubmissionRecords>();

            var RecepientTransRecord = new ApiSubmissionRecords();
            RecepientTransRecord.SubmissionSequence = 2;
            RecepientTransRecord.FormId = 126; // int.Parse(rt.RT_FormID);
            RecepientTransRecord.TableName = "Recipient_Transactional";

            
            RecepientTransRecord.Records = new List<ApiSubmissionRecord>();
            var rtrecord = new ApiSubmissionRecord();
            rtrecord.Fields = new List<ApiSubmissionRecordField>();

            
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "guid", Value = message.EQTMID });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "division", Value = rt.Division });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "shipfirstname", Value = rt.ShipFirstName });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "shiplastname", Value = rt.ShipLastName });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "shipaddress1", Value = rt.ShipAddress1 });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "shipaddress2", Value = rt.ShipAddress2 });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "shipcity", Value = rt.ShipCity });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "shipstate", Value = rt.ShipState });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "shipzip", Value = rt.ShipZip });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "shippingtotal", Value = rt.ShippingTotal });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "shippingmethod", Value = rt.ShippingMethod });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "name_first", Value = rt.FirstName });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "name_last", Value = rt.LastName });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "email", Value = rt.SoldToEmailAddress });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "subtotal", Value = rt.SubTotal });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "taxtotal", Value = rt.TaxTotal });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "paymentmethod", Value = rt.PaymentMethod });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "totaldiscount", Value = rt.TotalDiscount });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "totalrewards", Value = rt.TotalRewards });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "ordernumber", Value = rt.OrderNumber });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "transnumber", Value = rt.TransNumber });
            rtrecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "transdate", Value = rt.TransDate });

            RecepientTransRecord.Records.Add(rtrecord);
            
            var OrderDetailTransRecord = new ApiSubmissionRecords();
            OrderDetailTransRecord.SubmissionSequence = 1;
            OrderDetailTransRecord.FormId = int.Parse(rt.ODT_FormID);
            OrderDetailTransRecord.TableName = "Order_Detail_Transactional";

            OrderDetailTransRecord.Records = new List<ApiSubmissionRecord>();

            foreach (var odt in odts)
            {
                var odtRecord = new ApiSubmissionRecord();
                odtRecord.Fields = new List<ApiSubmissionRecordField>();
                
                odtRecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "guid", Value = message.EQTMID });
                odtRecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "productid", Value = odt.ProductID });
                odtRecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "brand", Value = odt.Brand });
                odtRecord.Fields.Add(new ApiSubmissionRecordField { FieldName = "style", Value = odt.Style });
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
            }
            mapper.Records.Add(RecepientTransRecord);
            mapper.Records.Add(OrderDetailTransRecord);
            return mapper;
        }

        /// <summary>
        /// private method for getting data for the model in Order confirmation. 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private MessageTypeModelWrapper GetData(object args)
        {
            string sqlConn = ConfigurationManager.ConnectionStrings["EQTestLocal_Con_Local"].ConnectionString;

            var rt = new RecipientTransactionalModelMaster();

            IEnumerable<OrderDetailTransactionalModelMaster> odts = null;

            using (var con = new SqlConnection(sqlConn))
            {
                con.Open();

                using (var multi = con.QueryMultiple("",
                param: args,
                commandType: CommandType.StoredProcedure))
                {
                    rt = multi.Read<RecipientTransactionalModelMaster>().FirstOrDefault();
                    odts = multi.Read<OrderDetailTransactionalModelMaster>();
                }
            }

            if (rt == null || !odts.Any())
            {
                return null;
            }

            return new MessageTypeModelWrapper()
            {
                RT = rt,
                ODTr = odts

            };

        }        
    }

}
