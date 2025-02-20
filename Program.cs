using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace JobScraper
{
    public class Job
    {
        public string? Title { get; set; }
        public string? Company { get; set; }
        public string? Url { get; set; }
        public string? Site { get; set; }
    }

    public class Program
    {
        public static async Task Main(string[] args)
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless");

            // Run scraping task for The Hub
            List<Job> jobs;
            using (IWebDriver driver = new ChromeDriver(options))
            {
                jobs = ScrapeHubSelenium(driver, "https://thehub.io/jobs?q=software+developer&l=aarhus", "🚀 The Hub");
            }

            // Write to CSV
            WriteJobsToCsv(jobs, "jobs.csv");

            Console.WriteLine("\n✨ Scraping Complete! Results saved to jobs.csv");
        }

        static List<Job> ScrapeHubSelenium(IWebDriver driver, string url, string siteName)
        {
            Console.WriteLine($"\n🔍 {siteName} - Scraping Started...");
            driver.Navigate().GoToUrl(url);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
            wait.Until(d => d.FindElements(By.XPath("//div[contains(@class, 'card-job-find-list')]")).Count > 0);

            List<Job> jobs = new List<Job>();

            foreach (IWebElement jobNode in driver.FindElements(By.XPath("//div[contains(@class, 'card-job-find-list')]")))
            {
                Job job = new Job();
                try
                {
                    // Check if title element exists before extracting
                    IWebElement titleElement = null;
                    try
                    {
                        titleElement = jobNode.FindElement(By.XPath(".//span[contains(@class, 'card-job-find-list__position')]"));
                    }
                    catch (NoSuchElementException) { /* Element doesn't exist, move on */ }
                    job.Title = titleElement?.Text.Trim();

                    // Check if company element exists before extracting
                    IWebElement companyElement = null;
                    try
                    {
                        companyElement = jobNode.FindElement(By.XPath(".//div[contains(@class, 'bullet-inline-list')]/span[1]"));
                    }
                    catch (NoSuchElementException) { /* Element doesn't exist, move on */ }
                    job.Company = companyElement?.Text.Trim();

                    // Check if URL element exists before extracting
                    IWebElement urlElement = null;
                    try
                    {
                        urlElement = jobNode.FindElement(By.XPath(".//a[contains(@class, 'card-job-find-list__link')]"));
                    }
                    catch (NoSuchElementException) { /* Element doesn't exist, move on */ }
                    job.Url = urlElement?.GetAttribute("href");
                    job.Site = siteName;

                }
                catch (Exception e)
                {
                    Console.WriteLine($"⚠️ {siteName} - Unexpected error extracting job details: {e.Message}");
                    continue;
                }

                if (!string.IsNullOrEmpty(job.Title) && !string.IsNullOrEmpty(job.Url))
                {
                    jobs.Add(job);
                }
            }

            PrintJobs(jobs, siteName);
            return jobs;
        }

        static void PrintJobs(List<Job> jobs, string siteName)
        {
            Console.WriteLine($"\n📝 {siteName} - Found {jobs.Count} Jobs:");
            foreach (Job job in jobs)
            {
                Console.WriteLine($"📌 Title: {job.Title}");
                Console.WriteLine($"🏢 Company: {job.Company}");
                Console.WriteLine($"🔗 URL: {job.Url}");
                Console.WriteLine("-------------------");
            }
        }

        static void WriteJobsToCsv(List<Job> jobs, string filePath)
        {
            try
            {
                using (var writer = new StreamWriter(filePath))
                using (var csv = new CsvWriter(writer, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)))
                {
                    csv.WriteRecords(jobs);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error writing to CSV: {ex.Message}");
            }
        }
    }
}