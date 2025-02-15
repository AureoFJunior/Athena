namespace Athena.Domain.Models.DTO;
public class ImageResponse
{
    public string Id { get; set; }
    public string Category { get; set; }
    public ImageData Image { get; set; }
    public string[] Tags { get; set; }
    public string Rating { get; set; }
}
