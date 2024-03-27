namespace Olx.Services.Abstract;

public interface IPhotoManager
{
    /// <summary>
    /// Saves the photo to storage.
    /// </summary>
    /// <param name="photo">Photo from form</param>
    /// <returns>Url that can be used to access a photo</returns>
    public Task<string> SavePhotoAsync(IFormFile photo);
    
    /// <summary>
    /// Gets the photo from storage.
    /// </summary>
    /// <param name="photoUrl">Url of the photo</param>
    /// <returns>Stream that contains photo data</returns>
    public Task<Stream> GetPhotoAsync(string photoUrl);
    
    /// <summary>
    /// Removes the photo from storage.
    /// </summary>
    /// <param name="photoUrl">Url of the photo</param>
    /// <returns></returns>
    public Task DeletePhotoAsync(string photoUrl);
}