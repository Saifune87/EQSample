using System;

namespace EQService.MessageTypeModels
{
    public sealed class RecipientTransactionalModelMaster
    {
        public Guid EQTMID { get; set; }
        public string CustId { get; set; }
        public string RT_FormID { get; set; }
        public string ODT_FormID { get; set; }
        public string Division { get; set; }
        public string EReceipt { get; set; }
        public string ShipFirstName { get; set; }
        public string ShipLastName { get; set; }
        public string ShipAddress1 { get; set; }
        public string ShipAddress2 { get; set; }
        public string ToEmail { get; set; }        
        public string ReferrerName { get; set; }
        public string SenderEmail { get; set; }
        public string ReferralNote { get; set; }
        public string Subject { get; set; }
        public string EmailType { get; set; }
        public string ShipCity { get; set; }
        public string ShipState { get; set; }
        public string ShipZip { get; set; }
        public string ShippingTotal { get; set; }
        public string ShippingMethod { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SoldToEmailAddress { get; set; }
        public int SubTotal { get; set; }
        public int TaxTotal { get; set; }
        public string HomeURL { get; set; }
        public string Total { get; set; }
        public int CreditAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string TotalDiscount { get; set; }
        public string TrackingNumber { get; set; }
        public string TrackingURL { get; set; }
        public string TotalRewards { get; set; }
        public string OrderNumber { get; set; }
        public string TransNumber { get; set; }
        public string TransDate { get; set; }
        public DateTime generic1 { get; set; }
    }
}
