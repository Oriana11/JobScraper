using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium.Support.UI;

namespace JobScraper
{
    public class Program
    {
        public class Job
        {
            public string? Url { get; set; }
            public string? Name { get; set; }
        }

        public static void Main()
        {
            // Set up Chrome options
            var options = new ChromeOptions();
            options.AddArgument("--disable-gpu");
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--disable-usb-discovery");
            options.AddArgument("--headless");  // Enable headless mode for non-GUI environments

            using (IWebDriver driver = new ChromeDriver(options))
            {
                Console.WriteLine("🔍 Opening it-jobbank.dk...");
                driver.Navigate().GoToUrl("https://www.it-jobbank.dk/jobsoegning?q=embedded&ref=google-sudv&gclid=Cj0KCQiAwtu9BhC8ARIsAI9JHakpqA0k9mh-m9PMW79N26UuqGe_tzA631CsZdIvSncfQrrTiczHbYcaArckEALw_wcB");

                // Scroll to ensure content is loaded
                IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;

                for (int i = 0; i < 5; i++)  // Scroll 5 times
                {
                    jsExecutor.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
                    Thread.Sleep(2000);  // Wait for new jobs to load
                }

                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                var jobElements = wait.Until(drv => drv.FindElements(By.XPath("//h3/a")));

                if (jobElements.Count == 0)
                {
                    Console.WriteLine("❌ No job listings found. Check the XPath selector.");
                }

                List<Job> jobs = new List<Job>();

                foreach (var jobElement in jobElements)
                {
                    var job = new Job
                    {
                        Name = jobElement.Text.Trim(),
                        Url = jobElement.GetAttribute("href")  // Use the relative URL directly without the base URL
                    };
                    jobs.Add(job);
                }

                // Print job results
                foreach (var job in jobs)
                {
                    Console.WriteLine($"✅ Job Title: {job.Name}");
                    Console.WriteLine($"🔗 Job Link: {job.Url}");
                    Console.WriteLine("-----------------------------");
                }

                Console.WriteLine($"✅ Successfully scraped {jobs.Count} jobs!");
            }
        }
    }
}
