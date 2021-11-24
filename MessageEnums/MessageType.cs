using System;

namespace EQService.MessageEnums
{
    /// <summary>
    /// publicically used MessageType Enums
    /// </summary>
    public enum MessageType
    {
        OrderConfirmation = 1,
        ShippingNotification = 2,
        ReturnConfirmation = 3,
        EmailFriend = 4,
        EmailWishList = 5,
        AbandonedCart = 6,
        SizeNotificationThankYou = 7,
        SizeNotificationAvailable = 8,
        SizeNotificationOptions = 9,
        HD_OrderConfirmation = 14,
        HD_ShippingNotification = 15,
        HD_OrderCutOff = 16,
        STH_ReadyPickUp = 17,
        STH_PickUpReminder = 18,
        STH_OrderExpired = 19,
        EReceiptsFF = 20,
        EReceiptsNat = 21,
        EReceiptsNatCAEng = 22,
        EReceiptsNatCAFre = 23,
        EReceiptsDrSch = 24,
        EReceiptsSamE = 25
    }
}
