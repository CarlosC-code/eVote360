using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eVote360.Core.Application.Interface.Common;
using Tesseract;

namespace eVote360.Infrastructure.Persistence.Shared.Ocr
{
    public sealed class DevOcrService : IOcrService
    {
        public async Task<string?> ExtractDocumentoAsync(Stream cedulaFrontImage, CancellationToken ct = default)
        {
            // 1️⃣ Guardar temporalmente la imagen
            var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.jpg");

            using (var fileStream = new FileStream(tempFile, FileMode.Create))
            {
                await cedulaFrontImage.CopyToAsync(fileStream, ct);
            }

            try
            {
                // 2️⃣ Ruta ABSOLUTA correcta a wwwroot/tessdata
                var tessPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "tessdata"
                );

                using var engine = new TesseractEngine(tessPath, "spa+eng", EngineMode.Default);
                using var img = Pix.LoadFromFile(tempFile);
                using var page = engine.Process(img);

                var text = page.GetText();

                // 3️⃣ Extraer solo números
                var digits = new string(text.Where(char.IsDigit).ToArray());

                return string.IsNullOrWhiteSpace(digits) ? null : digits;
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }
    }
}