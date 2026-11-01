namespace JobScrapper.Services;

public class TxtReportService
{
    public static void AppendJobsToFile(string location, string keyword, List<Models.JobPost> newJobs)
    {
        if (newJobs == null || !newJobs.Any()) return;

        // Dosya adı formatı: YYYYMMDD-Location-Keyword.txt
        string datePrefix = DateTime.Now.ToString("yyyyMMdd");

        // Dosya adındaki geçersiz karakterleri temizleyelim
        string safeLocation = string.Join("_", location.Split(Path.GetInvalidFileNameChars()));
        string safeKeyword = string.Join("_", keyword.Split(Path.GetInvalidFileNameChars()));

        string fileName = $"{datePrefix}-{safeLocation}-{safeKeyword}.txt";

        using var writer = new StreamWriter(fileName, append: true);

        foreach (var job in newJobs)
        {
            writer.WriteLine($"[Tarih]: {job.FetchedAt:yyyy-MM-dd HH:mm}");
            writer.WriteLine($"[Başlık]: {job.Title}");
            writer.WriteLine($"[Şirket]: {job.Company}");
            writer.WriteLine($"[Konum]: {job.Location}");
            writer.WriteLine($"[Link]: {job.Url}");
            writer.WriteLine(new string('-', 60));
        }
    }
}