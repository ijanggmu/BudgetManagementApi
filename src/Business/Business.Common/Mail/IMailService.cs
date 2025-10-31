using Business.Common.Email;
using Business.Common.Mail;
using Data.Entities.EmailLogEntity;
using Hangfire.States;
using Hangfire;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using SharedKernel.Config.Mail;

namespace Common.Mail;
public interface IMailService
{
    void QueueEmail(MailRequest mailRequest, CancellationToken ct);

    Task SendAsync(MailRequest request, CancellationToken ct);
}

public class SmtpMailService(IOptions<MailOptions> settings, ILogger<SmtpMailService> logger, IMailLogService emailLogService) : IMailService
{
    private readonly MailOptions _settings = settings.Value;
    private readonly ILogger<SmtpMailService> _logger = logger;

    public void QueueEmail(MailRequest mailRequest, CancellationToken ct)
    {
        BackgroundJobClient jobClient = new BackgroundJobClient();
        IState state = new EnqueuedState("emails");
        jobClient.Create(() => SendAsync(mailRequest, ct), state);
    }
    public async Task SendAsync(MailRequest request, CancellationToken ct)
    {
        using var email = new MimeMessage();

        // From
        email.From.Add(new MailboxAddress(_settings.DisplayName, request.From ?? _settings.From));

        // To
        foreach (string address in request.To)
            email.To.Add(MailboxAddress.Parse(address));

        // Reply To
        if (!string.IsNullOrEmpty(request.ReplyTo))
            email.ReplyTo.Add(new MailboxAddress(request.ReplyToName, request.ReplyTo));

        // Bcc
        if (request.Bcc != null)
        {
            foreach (string address in request.Bcc.Where(bccValue => !string.IsNullOrWhiteSpace(bccValue)))
                email.Bcc.Add(MailboxAddress.Parse(address.Trim()));
        }

        // Cc
        if (request.Cc != null)
        {
            foreach (string address in request.Cc.Where(ccValue => !string.IsNullOrWhiteSpace(ccValue)))
                email.Cc.Add(MailboxAddress.Parse(address.Trim()));
        }

        // Headers
        if (request.Headers != null)
        {
            foreach (var header in request.Headers)
                email.Headers.Add(header.Key, header.Value);
        }

        // Content
        var builder = new BodyBuilder();
        email.Sender = new MailboxAddress(request.DisplayName ?? _settings.DisplayName, request.From ?? _settings.From);
        email.Subject = request.Subject;
        builder.HtmlBody = request.Body;

        // Create the file attachments for this e-mail message
        if (request.AttachmentData.Count > 0)
        {
            foreach (var attachmentInfo in request.AttachmentData)
            {
                using var stream = new MemoryStream();
                await stream.WriteAsync(attachmentInfo.Value, ct);
                stream.Position = 0;
                await builder.Attachments.AddAsync(attachmentInfo.Key, stream, ct);
            }
        }

        email.Body = builder.ToMessageBody();

        var emailLog = new EmailLog
        {
            From = request.From ?? _settings.From,
            DisplayName = _settings.DisplayName,
            To = string.Join(", ", request.To),
            Cc = string.Join(",", request.Cc),
            Bcc = string.Join(",", request.Bcc),
            Subject = request.Subject,
            Body = request.Body,
            SentDate = DateTime.UtcNow,
            EmailType = request.Type
        };

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(_settings.Host, _settings.Port, cancellationToken: ct);
            await client.AuthenticateAsync(_settings.UserName, _settings.Password, ct);
            await client.SendAsync(email, ct);
            if (request.SensitiveData.Count > 0)
            {
                foreach (var data in request.SensitiveData)
                {
                    request.Body = request.Body.Replace(data, "****");
                }
            }
            emailLog.IsSuccess = true;

        }
        catch (Exception ex)
        {
            emailLog.IsSuccess = false;
            emailLog.ErrorMessage = ex.Message;
            _logger.LogError(ex, "An error occurred while sending email: {Message}", ex.Message);
            //Todo log the failed email
        }
        finally
        {
            await client.DisconnectAsync(true, ct);
            if (request.SensitiveData != null && request.SensitiveData.Count > 0)
            {
                foreach (var data in request.SensitiveData)
                {
                    emailLog.Body = request.Body.Replace(data, "****");
                }
            }
            await emailLogService.LogEmailAsync(emailLog);
        }
    }
}
