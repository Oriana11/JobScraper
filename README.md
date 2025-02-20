#  Job Scraper 🎯

An automated job listing extractor built with Selenium WebDriver, focusing on efficient data collection and clean data storage.

###  Technologies Used 🛠️

- OpenQA.Selenium for browser automation
- Chrome WebDriver for headless browsing
- CSV Helper for data persistence
- C#/.NET Core runtime
- Async/Await for performance optimization

###  System Architecture 📊

The diagram above illustrates the scraper's workflow:

1. The process begins with initializing a headless Chrome browser instance
2. Navigation occurs to the specified job board URL
3. The system waits for job listings to load completely
4. Each job entry undergoes extraction and validation:
  - Title, company, and URL are extracted using XPath selectors
  - Data is validated to ensure completeness
  - Invalid entries are skipped while valid ones are stored


5. Finally, all collected data is exported to a CSV file

###  Key Features 💡

- Headless browsing for silent operation
- Robust error handling with element existence checks
- Async operations for performance
- Real-time console feedback with emoji indicators
- CSV export functionality
- Configurable site targeting

###  Usage Instructions 📚

Install required NuGet packages:```bash
dotnet add package Selenium.WebDriver.Chrome
dotnet add package CsvHelper
```

Configure Chrome WebDriver:- Ensure ChromeDriver executable is in PATH
- Match driver version with installed Chrome browser

Run the scraper:```bash
dotnet run
```

###  Implementation Details 📝

The core functionality revolves around the `ScrapeHubSelenium` method, which handles:

- Browser navigation and page loading
- Job listing element detection
- Data extraction using XPath selectors
- Error handling for missing elements
- Collection of valid job entries

###  Output Format 🖥️

Results are displayed in console with emoji indicators:

```
🔍 Site Name - Scraping Started...
📝 Site Name - Found X Jobs:
📌 Title: Job Title
🏢 Company: Company Name
🔗 URL: https://example.com/job-url
-------------------
```

All scraped data is automatically saved to `jobs.csv` in the project directory.

###  Error Handling ⚠️

The system implements comprehensive error checking:

- Element existence verification before extraction
- Graceful handling of missing data fields
- CSV writing error management
- Invalid URL handling
