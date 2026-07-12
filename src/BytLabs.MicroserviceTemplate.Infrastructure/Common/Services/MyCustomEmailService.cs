using BytLabs.MicroserviceTemplate.Application.Common.Services;

namespace BytLabs.MicroserviceTemplate.Infrastructure.Common.Services
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
