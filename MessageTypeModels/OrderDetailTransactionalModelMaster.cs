using System;

namespace EQService.MessageTypeModels
{
    public sealed class OrderDetailTransactionalModelMaster
    {
        public string ProductID { get; set; }
        public string Brand { get; set; }
        public string Style { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public string MaterialType { get; set; }
        public string Quantity { get; set; }
        public string Price { get; set; }
        public string ThumbnailUrl { get; set; }
        public string ProductUrl { get; set; }
        public string Discount { get; set; }
        public string TotalCost { get; set; }
        public string ProductvariantID { get; set; }
        public string PartnerID { get; set; }
    }
}
