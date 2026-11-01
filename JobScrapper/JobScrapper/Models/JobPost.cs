namespace JobScrapper.Models;

public class JobPost
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public DateTime FetchedAt { get; set; } = DateTime.Now;
}