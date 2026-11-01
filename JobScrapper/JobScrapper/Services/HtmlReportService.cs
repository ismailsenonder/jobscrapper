using System.Text;
using JobScrapper.Models;

namespace JobScrapper.Services;

public class HtmlReportService
{
    public static void AppendJobsToHtmlFile(string location, string keyword, List<JobPost> newJobs)
    {
        if (newJobs == null || !newJobs.Any()) return;

        string datePrefix = DateTime.Now.ToString("yyyyMMdd");

        string safeLocation = string.Join("_", location.Split(Path.GetInvalidFileNameChars()));
        string safeKeyword = string.Join("_", keyword.Split(Path.GetInvalidFileNameChars()));

        string fileName = $"{datePrefix}-{safeLocation}-{safeKeyword}.html";

        bool fileExists = File.Exists(fileName);

        using var writer = new StreamWriter(fileName, append: true, encoding: Encoding.UTF8);

        // Eğer dosya yeni oluşturuluyorsa temel HTML ve CSS stillerini yazalım
        if (!fileExists)
        {
            writer.WriteLine("<!DOCTYPE html>");
            writer.WriteLine("<html lang='tr'>");
            writer.WriteLine("<head>");
            writer.WriteLine("  <meta charset='UTF-8'>");
            writer.WriteLine("  <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
            writer.WriteLine($"  <title>İlan Raporu - {location} ({keyword})</title>");
            writer.WriteLine("  <style>");
            writer.WriteLine("    body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; background-color: #f4f6f8; color: #333; margin: 0; padding: 20px; }");
            writer.WriteLine("    .container { max-width: 900px; margin: 0 auto; }");
            writer.WriteLine("    h2 { color: #0a66c2; border-bottom: 2px solid #0a66c2; padding-bottom: 8px; }");
            writer.WriteLine("    .job-card { background: #fff; border-radius: 8px; padding: 16px 20px; margin-bottom: 12px; box-shadow: 0 1px 3px rgba(0,0,0,0.1); display: flex; justify-content: space-between; align-items: center; }");
            writer.WriteLine("    .job-info { flex-grow: 1; }");
            writer.WriteLine("    .job-title { font-size: 1.1rem; font-weight: 600; color: #181818; margin-bottom: 4px; }");
            writer.WriteLine("    .job-meta { font-size: 0.9rem; color: #5e5e5e; }");
            writer.WriteLine("    .job-date { font-size: 0.8rem; color: #8c8c8c; margin-top: 4px; }");
            writer.WriteLine("    .btn-apply { background-color: #0a66c2; color: #fff; padding: 8px 16px; border-radius: 20px; text-decoration: none; font-weight: 600; font-size: 0.9rem; white-space: nowrap; margin-left: 16px; }");
            writer.WriteLine("    .btn-apply:hover { background-color: #004182; }");
            writer.WriteLine("  </style>");
            writer.WriteLine("</head>");
            writer.WriteLine("<body>");
            writer.WriteLine("  <div class='container'>");
            writer.WriteLine($"    <h2>İlan Raporu: {keyword} ({location})</h2>");
        }

        // İlan kartlarını ekleyelim
        foreach (var job in newJobs)
        {
            writer.WriteLine("    <div class='job-card'>");
            writer.WriteLine("      <div class='job-info'>");
            writer.WriteLine($"        <div class='job-title'>{System.Net.WebUtility.HtmlEncode(job.Title)}</div>");
            writer.WriteLine($"        <div class='job-meta'>🏢 <strong>{System.Net.WebUtility.HtmlEncode(job.Company)}</strong> &nbsp;|&nbsp; 📍 {System.Net.WebUtility.HtmlEncode(job.Location)}</div>");
            writer.WriteLine($"        <div class='job-date'>🕒 Ekleme: {job.FetchedAt:yyyy-MM-dd HH:mm}</div>");
            writer.WriteLine("      </div>");
            writer.WriteLine($"      <a href='{job.Url}' target='_blank' class='btn-apply'>İlana Git ↗</a>");
            writer.WriteLine("    </div>");
        }
    }
}