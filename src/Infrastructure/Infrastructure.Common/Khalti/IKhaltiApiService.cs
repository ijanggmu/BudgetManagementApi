using Refit;
using SharedKernel.Operation;

namespace Infrastructure.Common.Khalti;
public interface IKhaltiApiService
{
    [Post("/epayment/initiate/")]
    Task<ApiResponse<object>> InitialPaymentAsync(KhaltiPaymentRequestModel requestModel);

    [Post("/epayment/lookup/")]
    Task<KhaltiLookupResponse> LookupPaymentAsync(string pidx);
}
public class KhalitiResponseModel
{
    public string pidx { get; set; }
    public string payment_url { get; set; }
    public string expires_at { get; set; }
    public string expires_in { get; set; }
}
public class Payment
{
    public int Id { get; set; }
    public string Pidx { get; set; } = default!;
    public int Amount { get; set; }
    public string CustomerName { get; set; } = default!;
    public string CustomerEmail { get; set; } = default!;
    public string CustomerPhone { get; set; } = default!;
    public string? CustomerAddress { get; set; }
    public string ProductName { get; set; } = default!;
    public string? ProductId { get; set; }
    public string? ProductDescription { get; set; }
    public string Status { get; set; } = "pending";
    public string? TransactionId { get; set; }
    public string Environment { get; set; } = "test";
    public string? PaymentUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class WebhookLog
{
    public int Id { get; set; }
    public string EventType { get; set; } = default!;
    public string Pidx { get; set; } = default!;
    public string Status { get; set; } = default!;
    public int? ResponseCode { get; set; }
    public int? ResponseTime { get; set; }
    public string Payload { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
public class KhaltiPaymentRequestModel
{
    public string return_url { get; set; } = default!;
    public string website_url { get; set; } = default!;
    public decimal amount { get; set; }
    public string purchase_order_id { get; set; } = default!;
    public string purchase_order_name { get; set; } = default!;
}
public class CustomerInfo
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
}

public class ProductDetail
{
    public string Identity { get; set; } = default!;
    public string Name { get; set; } = default!;
    public int TotalPrice { get; set; }
    public int Quantity { get; set; }
    public int UnitPrice { get; set; }
}

public class KhaltiInitiateResponse
{
    public string Pidx { get; set; } = default!;
    public string PaymentUrl { get; set; } = default!;
    public string Status { get; set; } = default!;
}

public class KhaltiLookupResponse
{
    public string Pidx { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string? TransactionId { get; set; }
}

