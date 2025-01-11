namespace BytLabs.MicroserviceTemplate.Application.Services
{
    public interface IEmailService
    {
        Task SendEmail(string email, string title, string body);
    }
}
