namespace Athena.Domain.Models.DTO;
public class RootResponse
{
    public bool Success { get; set; }
    public int Status { get; set; }
    public int Count { get; set; }
    public List<ImageResponse> Images { get; set; }
}
