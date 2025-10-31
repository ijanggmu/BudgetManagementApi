namespace Models.Common.PolicyPayment;

public class KhaltiPaymentRedirectResponseModel
{
    public string Pidx { get; set; }

    public string TransactionId { get; set; }

    public int Amount { get; set; }

    public int TotalAmount { get; set; }

    public string Status { get; set; }

    public string Mobile { get; set; }

    public string Tidx { get; set; }

    public string PurchaseOrderId { get; set; }

    public string PurchaseOrderName { get; set; }

}

