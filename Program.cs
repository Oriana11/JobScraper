// See https://aka.ms/new-console-template for more information

using System.Globalization;
using CsvHelper;
using HtmlAgilityPack;
using System.IO;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        var web = new HtmlWeb();
        var document =
            web.Load("https://dk.indeed.com/jobs?q=software+developer&l=aarhus&from=searchOnHP&vjk=aab1613a0233192d");
        var jobs = new List<Job>();

        // CRITICAL: Inspect the HTML and update these selectors!
        // Verify this selector targets the ENTIRE job card.
        string jobCardSelector = "//div[contains(@class, 'jobsearch-SerpJobCard')]"; //Likely Correct

        var jobHTMLElements = document.DocumentNode.SelectNodes(jobCardSelector);

        if (jobHTMLElements != null)
        {
            Console.WriteLine($"Found {jobHTMLElements.Count} job elements."); // Debugging

            foreach (var jobHTMLElement in jobHTMLElements)
            {
                try
                {
                    // **Update these XPath expressions to be RELATIVE to the jobCardSelector**
                    // **Also incorporate the data-testid from the h2**

                    var titleNode = jobHTMLElement.SelectSingleNode(".//h2[@data-testid='simpler-jobTitle']"); // using data-testid
                    var companyNode = jobHTMLElement.SelectSingleNode(".//span[contains(@class, 'companyName')]");   // Update this if needed
                    var locationNode = jobHTMLElement.SelectSingleNode(".//div[contains(@class, 'companyLocation')]"); // Update this if needed
                    var descriptionNode = jobHTMLElement.SelectSingleNode(".//div[contains(@class, 'job-snippet')]"); // Update this if needed
                    var imageNode = jobHTMLElement.SelectSingleNode(".//img");  // Update this if needed

                    var url = titleNode?.SelectSingleNode(".//a")?.GetAttributeValue("href", ""); //URL inside the h2
                    var imageUrl = imageNode?.GetAttributeValue("src", "");
                    var title = titleNode?.InnerText?.Trim(); //Get innerText directly from the h2
                    var company = companyNode?.InnerText?.Trim();
                    var location = locationNode?.InnerText?.Trim();
                    var description = descriptionNode?.InnerText?.Trim();

                     if(url != null)
                    {
                        url = HtmlEntity.DeEntitize(url);
                    }

                    var job = new Job() {Title = title, Company = company, Location = location, Description = description };
                    jobs.Add(job);

                    Console.WriteLine($"Title: {title}, Company: {company}, Location: {location}");  // Debugging

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing job element: {ex.Message}");
                }
            }
        }
        else
        {
            Console.WriteLine("No job elements found.  Check the jobCardSelector.");
        }

        // initializing the CSV output file
        using (var writer = new StreamWriter("jobs.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(jobs);
        }
    }


    public class Job
    {
        public string? Title { get; set; }
        public string? Company { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
    }
}