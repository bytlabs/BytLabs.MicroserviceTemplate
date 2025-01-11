using BytLabs.MicroserviceTemplate.Application.Services;

namespace BytLabs.MicroserviceTemplate.Infrastructure.Services
{
    public class MyCustomEmailService : IEmailService
    {
        public Task SendEmail(string email, string title, string body)
        {

            //Implement here your email provider logic
            return Task.CompletedTask;
        }
    }
}
