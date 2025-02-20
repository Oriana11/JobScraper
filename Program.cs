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
            //options.AddArgument("--headless");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--disable-usb-discovery");

            using (IWebDriver driver = new ChromeDriver(options))
            {
                Console.WriteLine("🔍 Opening Indeed...");
                driver.Navigate().GoToUrl("https://www.it-jobbank.dk/jobsoegning?q=embedded&ref=google-sudv&gclid=Cj0KCQiAwtu9BhC8ARIsAI9JHakpqA0k9mh-m9PMW79N26UuqGe_tzA631CsZdIvSncfQrrTiczHbYcaArckEALw_wcB");

                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                var jobElements = wait.Until(drv => drv.FindElements(By.XPath("//*[@id=\"jobad-wrapper-h1540626\"]/div/div/div/h3/a")));



                // Find job elements



                

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
                        Url = "https://dk.indeed.com" + jobElement.GetAttribute("href")
                    };
                    jobs.Add(job);
                }

                // Print job results
                foreach (var job in jobs)
                {
                    Console.WriteLine($"✅ Job Title: {job.Name}");
                    Console.WriteLine($"🔗 Job Link: {job.Url}");
                    Console.WriteLine("--------------------------------------------------");
                }

                Console.WriteLine($"✅ Successfully scraped {jobs.Count} jobs!");
            }
        }
    }
}
