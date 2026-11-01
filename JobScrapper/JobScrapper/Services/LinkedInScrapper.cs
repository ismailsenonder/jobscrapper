using JobScrapper.Models;
using Microsoft.Playwright;

namespace JobScrapper.Services;

public class LinkedInScrapper
{
    public async Task<List<JobPost>> ScrapeJobsAsync(string keyword, string location)
    {
        var jobs = new List<JobPost>();

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true // Görsel olarak görmek istersen false yapabilirsin
        });

        var page = await browser.NewPageAsync();

        // LinkedIn Public Job Search URL
        var searchUrl = $"https://www.linkedin.com/jobs/search/?keywords={Uri.EscapeDataString(keyword)}&location={Uri.EscapeDataString(location)}&f_TPR=r86400";
        // Note: f_TPR=r86400 son 24 saatteki ilanları getirir.

        await page.GotoAsync(searchUrl);
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // İlan kartlarını yakalayalım
        var jobCards = await page.QuerySelectorAllAsync("ul.jobs-search__results-list > li");

        foreach (var card in jobCards)
        {
            try
            {
                var titleElement = await card.QuerySelectorAsync("h3.base-search-card__title");
                var companyElement = await card.QuerySelectorAsync("h4.base-search-card__subtitle");
                var locationElement = await card.QuerySelectorAsync("span.job-search-card__location");
                var linkElement = await card.QuerySelectorAsync("a.base-card__full-link");

                var title = titleElement != null ? (await titleElement.InnerTextAsync()).Trim() : "";
                var company = companyElement != null ? (await companyElement.InnerTextAsync()).Trim() : "";
                var jobLocation = locationElement != null ? (await locationElement.InnerTextAsync()).Trim() : "";
                var url = linkElement != null ? await linkElement.GetAttributeAsync("href") ?? "" : "";

                // URL'den benzersiz Job ID çıkarma
                var jobId = ExtractJobId(url);

                if (!string.IsNullOrEmpty(jobId))
                {
                    jobs.Add(new JobPost
                    {
                        Id = jobId,
                        Title = title,
                        Company = company,
                        Location = jobLocation,
                        Url = url.Split('?')[0] // URL parametrelerini temizleyelim
                    });
                }
            }
            catch
            {
                // Tekil kart hatalarını es geçip devam et
            }
        }

        return jobs;
    }

    private string ExtractJobId(string url)
    {
        if (string.IsNullOrEmpty(url)) return string.Empty;

        // Örn URL formatı: .../view/1234567890/...
        var parts = url.Split('-');
        var lastPart = parts.LastOrDefault()?.Split('?').FirstOrDefault();
        return lastPart ?? Guid.NewGuid().ToString();
    }
}