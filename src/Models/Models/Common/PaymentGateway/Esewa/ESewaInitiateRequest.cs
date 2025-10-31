
// Payment Initiation Request (used for generating the HTML form)
// Note: These fields directly map to the form field names
public class ESewaInitiateRequest
{
    public decimal amount { get; set; }
    public decimal tax_amount { get; set; }
    public decimal total_amount { get; set; }
    public string transaction_uuid { get; set; } // Your unique transaction ID
    public string product_code { get; set; } // Merchant ID
    public decimal product_service_charge { get; set; }
    public decimal product_delivery_charge { get; set; }
    public string success_url { get; set; }
    public string failure_url { get; set; }
    public string signed_field_names { get; set; } // "total_amount,transaction_uuid,product_code"
    public string signature { get; set; } // HMAC-SHA256 signature
}
