using System;
using System.IO;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace AdvancedSeleniumHomeworkPartTwo
{
    [TestFixture]
    public class Homework
    {
        IWebDriver driver;
        IJavaScriptExecutor jsExec;
        string downloadDirectoryPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Downloads", DateTime.Now.ToString("yy-MM-dd HH-mm-ss"));

        [SetUp]
        public void SetUp()
        {     
            //Config browser window
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments
                (
                    "--start-fullscreen",
                    "--start-maximized",
                    "--disable-infobars"
                );

            //Change the default download directory
            Directory.CreateDirectory(downloadDirectoryPath);
            chromeOptions.AddUserProfilePreference("download.default_directory", downloadDirectoryPath);
           
            //Creating the driver
            TestContext.WriteLine("Creating driver with option");
            driver = new ChromeDriver(chromeOptions);
            jsExec = (IJavaScriptExecutor)driver;
        }

        [TearDown]
        public void TearDown()
        {
            TestContext.WriteLine("Kill driver");
            driver.Quit();
        }

        [Test]
        public void DownloadPicture()
        {
           
            //Locators
            By image = By.XPath("//img[@alt='cactus succulent plant on white vase']");
            By downloadButton = By.XPath("//a[@href = 'https://unsplash.com/photos/pQwll5IG-I0/download?force=true']");

            //Go to https://unsplash.com/search/photos/test page
            TestContext.WriteLine("Go to https://unsplash.com/search/photos/test page");
            driver.Navigate().GoToUrl("https://unsplash.com/search/photos/test");

            //Scroll to the end of page
            while (true)
            {
                var previousYPosition = jsExec.ExecuteScript("return window.pageYOffset;").ToString();
                jsExec.ExecuteScript("window.scrollBy(0, 250)");
                Thread.Sleep(100);

                var currentYPosition = jsExec.ExecuteScript("return window.pageYOffset;").ToString();
                if (previousYPosition == currentYPosition)
                {
                    break;
                }      
            }

            //Click on Download button of last image
            TestContext.WriteLine("Download picture");
            jsExec.ExecuteScript($"arguments[0].click()", driver.FindElement(downloadButton));
            Thread.Sleep(2000);

            //Checking that file is downloaded.
            Assert.That(Directory.GetFiles(downloadDirectoryPath),  Is.Not.Empty);
        }

    }
}
