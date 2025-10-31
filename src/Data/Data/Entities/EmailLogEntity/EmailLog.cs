using Data.Entities.BaseEntity;

namespace Data.Entities.EmailLogEntity;
public class EmailLog : ApplicationBaseEntity
{
    public string From { get; set; }
    public string DisplayName { get; set; }
    public string To { get; set; }
    public string Cc { get; set; }
    public string Bcc { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public DateTime SentDate { get; set; }
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; }
    public string EmailType { get; set; }
}

