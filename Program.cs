using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobScraper
{
    public class Job
    {
        public string? Title { get; set; }
        public string? Company { get; set; }
        public string? Url { get; set; }
    }

    public class Program
    {
        public static async Task Main(string[] args)
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless");

            // Run each scraping task concurrently, each with its own driver instance.
            var hubTask = Task.Run(() =>
            {
                using (IWebDriver driver = new ChromeDriver(options))
                {
                    ScrapeHubSelenium(driver, "https://thehub.io/jobs?q=software+developer&l=aarhus", "The Hub");
                }
            });

            var jobIndexTask = Task.Run(() =>
            {
                using (IWebDriver driver = new ChromeDriver(options))
                {
                    ScrapeJobindexSelenium(driver, "https://www.jobindex.dk/jobsoegning/region-midtjylland?q=backend", "JobIndex.dk");
                }
            });

            var itJobbankTask = Task.Run(() =>
            {
                using (IWebDriver driver = new ChromeDriver(options))
                {
                    ScrapeItJobbankSelenium(driver, "https://www.it-jobbank.dk/jobsoegning/testquality-assurance", "IT Jobbank");
                }
            });

            await Task.WhenAll(hubTask, jobIndexTask, itJobbankTask);
        }

        static void ScrapeHubSelenium(IWebDriver driver, string url, string siteName)
        {
            driver.Navigate().GoToUrl(url);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            // Wait until at least one job card is loaded.
            wait.Until(d => d.FindElements(By.XPath("//div[contains(@class, 'card-job-find-list')]")).Count > 0);

            List<Job> jobs = new List<Job>();

            // Loop through each job card element and extract data.
            var jobNodes = driver.FindElements(By.XPath("//div[contains(@class, 'card-job-find-list')]"));
            foreach (IWebElement jobNode in jobNodes)
            {
                Job job = new Job();

                try
                {
                    job.Title = jobNode.FindElement(By.XPath(".//span[contains(@class, 'card-job-find-list__position')]")).Text.Trim();
                }
                catch (NoSuchElementException ex)
                {
                    Console.WriteLine($"Error extracting title on {siteName}: {ex.Message}");
                    continue; // Skip this job if title is not found.
                }

                try
                {
                    job.Company = jobNode.FindElement(By.XPath(".//div[contains(@class, 'bullet-inline-list')]/span[1]")).Text.Trim();
                }
                catch (NoSuchElementException)
                {
                    job.Company = "Unknown";
                }

                try
                {
                    job.Url = jobNode.FindElement(By.XPath(".//a[contains(@class, 'card-job-find-list__link')]")).GetAttribute("href");
                }
                catch (NoSuchElementException)
                {
                    Console.WriteLine($"Error extracting URL on {siteName}.");
                    continue; // Skip if URL is not found.
                }

                if (!string.IsNullOrEmpty(job.Title) && !string.IsNullOrEmpty(job.Url))
                {
                    jobs.Add(job);
                }
            }

            PrintJobs(jobs, siteName);
        }

        static void ScrapeItJobbankSelenium(IWebDriver driver, string url, string siteName)
        {
            driver.Navigate().GoToUrl(url);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.FindElements(By.XPath("//div[@class='PaidJob'] | //div[@class='jix_robotjob']")).Count > 0);

            List<Job> jobs = new List<Job>();

            foreach (IWebElement jobNode in driver.FindElements(By.XPath("//div[@class='PaidJob'] | //div[@class='jix_robotjob']")))
            {
                Job job = new Job();
                try
                {
                    job.Title = jobNode.FindElement(By.XPath(".//h4/a")).Text.Trim();
                    job.Url = jobNode.FindElement(By.XPath(".//h4/a")).GetAttribute("href");
                    job.Company = jobNode.FindElement(By.XPath(".//div[@class='jix_robotjob--top']/a")).Text.Trim();
                }
                catch (NoSuchElementException)
                {
                    job.Company = "Unknown";
                }

                if (!string.IsNullOrEmpty(job.Title) && !string.IsNullOrEmpty(job.Url))
                {
                    jobs.Add(job);
                }
            }

            PrintJobs(jobs, siteName);
        }

        static void ScrapeJobindexSelenium(IWebDriver driver, string url, string siteName)
        {
            driver.Navigate().GoToUrl(url);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.FindElements(By.XPath("//div[contains(@id, 'jobad-wrapper')]")).Count > 0);

            List<Job> jobs = new List<Job>();

            foreach (IWebElement jobNode in driver.FindElements(By.XPath("//div[contains(@id, 'jobad-wrapper')]")))
            {
                Job job = new Job();
                try
                {
                    job.Title = jobNode.FindElement(By.XPath(".//h4/a")).Text.Trim();
                    job.Url = jobNode.FindElement(By.XPath(".//h4/a")).GetAttribute("href");

                    try
                    {
                        job.Company = jobNode.FindElement(By.XPath(".//div[@class='jix-toolbar-top__company']/a")).Text.Trim();
                    }
                    catch (NoSuchElementException)
                    {
                        job.Company = "Unknown";
                    }
                }
                catch (NoSuchElementException)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(job.Title) && !string.IsNullOrEmpty(job.Url))
                {
                    jobs.Add(job);
                }
            }

            PrintJobs(jobs, siteName);
        }

        static void PrintJobs(List<Job> jobs, string siteName)
        {
            Console.WriteLine($"--- {siteName} Jobs ---");
            foreach (Job job in jobs)
            {
                Console.WriteLine($"Title: {job.Title}");
                Console.WriteLine($"Company: {job.Company}");
                Console.WriteLine($"URL: {job.Url}");
                Console.WriteLine("-------------------");
            }
        }
    }
}
