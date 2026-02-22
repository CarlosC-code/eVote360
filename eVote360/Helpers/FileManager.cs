namespace eVote360.Helpers
{
    public static class FileManager
    {
        public static async Task<string?> UploadAsync(
                    IFormFile? file,
                    int id,
                    string folderName,
                    bool isEditMode = false,
                    string? imagePath = "",
                    CancellationToken ct = default)
        {
            if (isEditMode && file == null)
                return imagePath;

            if (file == null || file.Length == 0)
                return string.Empty;

            string basePath = $"Images/{folderName}/{id}";
            string physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", basePath);

            if (!Directory.Exists(physicalPath))
                Directory.CreateDirectory(physicalPath);

            string extension = Path.GetExtension(file.FileName);
            string fileName = $"{Guid.NewGuid():N}{extension}";
            string fullPath = Path.Combine(physicalPath, fileName);

            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream, ct);
            }

            if (isEditMode && !string.IsNullOrWhiteSpace(imagePath))
            {
                // borrar archivo viejo
                string oldFileName = imagePath.Split('/', StringSplitOptions.RemoveEmptyEntries).Last();
                string oldFullPath = Path.Combine(physicalPath, oldFileName);

                if (File.Exists(oldFullPath))
                    File.Delete(oldFullPath);
            }

            return $"{basePath}/{fileName}".Replace("\\", "/");
        }

    }

}

