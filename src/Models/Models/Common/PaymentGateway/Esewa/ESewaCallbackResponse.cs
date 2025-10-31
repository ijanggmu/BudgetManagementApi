using System.Text.Json.Serialization; // For System.Text.Json attributes
                                      // Success/Failure Callback Response (received from eSewa, Base64 decoded)
public class ESewaCallbackResponse
{
    [JsonPropertyName("transaction_code")]
    public string TransactionCode { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("total_amount")]
    public string TotalAmount { get; set; }

    [JsonPropertyName("transaction_uuid")]
    public string TransactionUuid { get; set; }

    [JsonPropertyName("product_code")]
    public string ProductCode { get; set; }

    [JsonPropertyName("signed_field_names")]
    public string SignedFieldNames { get; set; }

    [JsonPropertyName("signature")]
    public string Signature { get; set; }
}

