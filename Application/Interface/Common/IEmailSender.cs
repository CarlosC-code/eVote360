using System.Threading;
using System.Threading.Tasks;

namespace eVote360.Core.Application.Interface.Common
{
    public interface IEmailSender
    {
        Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken ct = default);
    }
}
