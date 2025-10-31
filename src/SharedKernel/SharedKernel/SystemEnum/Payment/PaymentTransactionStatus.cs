namespace SharedKernel.SystemEnum.Payment;
public enum PurchaseStatus { Draft,Pending, Paid, Failed, Cancelled,Acknowledged }
public enum PaymentGateway { Esewa, Khalti }
public enum PaymentTransactionStatus { Initiated, Verified, Failed }
