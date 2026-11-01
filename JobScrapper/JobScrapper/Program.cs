using JobScrapper.Models;
using JobScrapper.Services;

// Tarayıcı bağımlılığını otomatik indir / doğrula
Microsoft.Playwright.Program.Main(new[] { "install" });

var dbService = new DatabaseService();
var scraper = new LinkedInScrapper();

// Target list
var targetLocations = new[] { 
  //  "Austria", 
    "Hungary", 
  //  "Estonia", 
   // "Netherlands", 
   // "Switzerland", 
   // "Croatia", 
 //   "Portugal", "Spain", "Ireland", "Belgium", "Poland", "Slovakia", "Slovenia", "Czechia", 
   // "Norway", "Denmark", "Finland", "Sweden", "Lithuania", "Luxembourg", "Italy", 
};
var targetKeywords = new[] { ".NET", "C#", "dotnet", "Software Engineer", "Software Developer" };

Console.WriteLine("===========================================");
Console.WriteLine("  LinkedIn Job Scraper Başlatılıyor...     ");
Console.WriteLine("===========================================\n");

int totalNewJobs = 0;

foreach (var location in targetLocations)
{
    foreach (var keyword in targetKeywords)
    {
        Console.WriteLine($"[QUERY] Keyword: '{keyword}' | Location: '{location}' aranıyor...");

        var jobs = await scraper.ScrapeJobsAsync(keyword, location);
        var newlyAddedJobs = new List<JobPost>();

        foreach (var job in jobs)
        {
            if (!dbService.IsJobExists(job.Id))
            {
                dbService.SaveJob(job);
                newlyAddedJobs.Add(job);
                totalNewJobs++;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  [YENİ İLAN] {job.Title} @ {job.Company}");
                Console.WriteLine($"              Konum: {job.Location}");
                Console.WriteLine($"              Link: {job.Url}");
                Console.ResetColor();
                Console.WriteLine(new string('-', 50));
            }
        }

        // Eğer yeni ilan eklendiyse ilgili .txt dosyasına da yazdıralım
        if (newlyAddedJobs.Any())
        {
            HtmlReportService.AppendJobsToHtmlFile(location, keyword, newlyAddedJobs);
            Console.WriteLine($"  --> {newlyAddedJobs.Count} yeni ilan HTML dosyasına kaydedildi.");
        }
        else
        {
            Console.WriteLine("  --> Yeni ilan bulunamadı.");
        }

        Console.WriteLine();
    }
}

Console.WriteLine("===========================================");
Console.WriteLine($"Tarama tamamlandı! Toplam {totalNewJobs} yeni ilan eklendi.");
Console.WriteLine("===========================================");