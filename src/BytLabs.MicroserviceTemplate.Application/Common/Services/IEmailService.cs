namespace BytLabs.MicroserviceTemplate.Application.Common.Services
{
    public interface IEmailService
    {
        Task SendEmail(string email, string title, string body);
    }
}
