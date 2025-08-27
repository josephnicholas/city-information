namespace CityInformation.API.Services;

public sealed class LocalMailService : IMailService {
    readonly string _mailTo;
    readonly string _mailFrom;
    private ILogger<LocalMailService> _logger;

    public LocalMailService(ILogger<LocalMailService> logger, IConfiguration config) {
        _logger = logger;
        _mailFrom = config["MailSettings:FromAddress"] ?? string.Empty;
        _mailTo = config["MailSettings:ToAddress"] ?? string.Empty;
    }
    public void Send(string subject, string message) {
        _logger.LogInformation($"Mail from {_mailFrom} to {_mailTo}, with {nameof(LocalMailService)}");
        _logger.LogInformation($"Subject: {subject}, Message: {message}");
    }
}
