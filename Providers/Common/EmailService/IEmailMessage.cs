using TrackFinder.Providers.Common.EmailService.Messages;

namespace TrackFinder.Providers.Common.EmailService
{
    public interface IEmailMessage
    {
        Task CreateMessage(string email);
    }
}
