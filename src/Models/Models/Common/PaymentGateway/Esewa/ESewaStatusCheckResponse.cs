using System.Text.Json.Serialization; // For System.Text.Json attributes
                                      // Status Check API Response
public class ESewaStatusCheckResponse
{
    [JsonPropertyName("product_code")]
    public string ProductCode { get; set; }

    [JsonPropertyName("transaction_uuid")]
    public string TransactionUuid { get; set; }

    [JsonPropertyName("total_amount")]
    public decimal TotalAmount { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } // PENDING, COMPLETE, FULL_REFUND, PARTIAL_REFUND, AMBIGUOUS, NOT_FOUND, CANCELED

    [JsonPropertyName("ref_id")]
    public string RefId { get; set; } // eSewa's transaction reference ID
}
