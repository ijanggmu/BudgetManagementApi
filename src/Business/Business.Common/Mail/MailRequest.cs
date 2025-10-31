using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Business.Common.Mail;

public class MailRequest(Collection<string> to,
    string subject,
    string emailType,
    string body = null,
    string from = null,
    string displayName = null,
    string replyTo = null,
    string replyToName = null,
    Collection<string> bcc = null,
    Collection<string> cc = null,
    IDictionary<string, byte[]> attachmentData = null,
    IDictionary<string, string> headers = null,
    Collection<string> sensitiveData = null)
{

    public Collection<string> To { get; } = to;

    public string Subject { get; } = subject;

    public string Body { get; set; } = body;

    public string From { get; } = from;

    public string DisplayName { get; } = displayName;

    public string ReplyTo { get; } = replyTo;

    public string ReplyToName { get; } = replyToName;

    public Collection<string> Bcc { get; } = bcc ?? [];

    public Collection<string> Cc { get; } = cc ?? [];

    public IDictionary<string, byte[]> AttachmentData { get; } = attachmentData ?? new Dictionary<string, byte[]>();

    public IDictionary<string, string> Headers { get; } = headers ?? new Dictionary<string, string>();
    public Collection<string> SensitiveData { get; } = sensitiveData ?? [];
    [Required]
    public string Type { get; } = emailType;
}
