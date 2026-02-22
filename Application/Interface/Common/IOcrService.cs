using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace eVote360.Core.Application.Interface.Common
{
    public interface IOcrService
    {
        
        Task<string?> ExtractDocumentoAsync(Stream cedulaFrontImage, CancellationToken ct = default);
    }
}

