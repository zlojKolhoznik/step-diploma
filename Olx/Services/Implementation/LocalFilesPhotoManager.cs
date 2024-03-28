using Olx.Services.Abstract;

namespace Olx.Services.Implementation;

public class LocalFilesPhotoManager : IPhotoManager
{
    private readonly ILogger<LocalFilesPhotoManager> _logger;

    public LocalFilesPhotoManager(ILogger<LocalFilesPhotoManager> logger)
    {
        _logger = logger;
    }

    public async Task<string> SavePhotoAsync(IFormFile photo)
    {
        string photoName = string.Join('-', Path.GetFileNameWithoutExtension(photo.FileName).ToLower().Split(' ')) +
                           Path.GetExtension(photo.FileName);
        string relativePath = Path.Combine("assets", "photos", photoName);
        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);
        if (File.Exists(fullPath))
        {
            fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "photos",
                Guid.NewGuid() + "_" + photoName);
        }
        await using var fileStream = new FileStream(fullPath, FileMode.Create);
        await photo.CopyToAsync(fileStream);
        return relativePath;
    }

    public async Task<Stream> GetPhotoAsync(string photoUrl)
    {
        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", photoUrl[1..].Replace("/", "\\"));
        try
        {
            await using var fileStream = new FileStream(fullPath, FileMode.Open);
            var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to open file {FullPath}", fullPath);
            throw new Exception(e.Message, e);
        }
    }

    public Task DeletePhotoAsync(string photoUrl)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", photoUrl);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
        
        return Task.CompletedTask;
    }
}