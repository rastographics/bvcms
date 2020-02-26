using IntegrationTests.Support;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using SharedTestFixtures;
using System;
using System.Net;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace IntegrationTests
{
    public class WebAppFixture : IDisposable
    {
        private IISExpress cmswebInstance;

        private static IWebDriver SharedWebDriver { get; set; }

        public WebAppFixture()
        {
            Settings.EnsureApplicationHostConfig();
            SetupWebConfig();
            cmswebInstance = IISExpress.Start(Settings.ApplicationHostConfig, "CMSWeb");
            Warmup();
        }

        private void SetupWebConfig()
        {
            var xml = DatabaseFixture.LoadWebConfig();
            var appSettings = xml.GetElementsByTagName("appSettings")[0];
            var list = appSettings.SelectNodes("add[@key='UseRuntimeSettingsCache']");
            if (list.Count == 0)
            {
                var setting = xml.CreateElement("add");
                var attr = xml.CreateAttribute("key");
                attr.Value = "UseRuntimeSettingsCache";
                setting.Attributes.Append(attr);
                attr = xml.CreateAttribute("value");
                attr.Value = "false";
                setting.Attributes.Append(attr);
                appSettings.AppendChild(setting);
                xml.Save(DatabaseFixture.FindWebConfigPath());
            }
        }

        private void CleanupWebConfig()
        {
            var xml = DatabaseFixture.LoadWebConfig();
            var appSettings = xml.GetElementsByTagName("appSettings")[0];
            var list = appSettings.SelectNodes("add[@key='UseRuntimeSettingsCache']");
            if (list.Count > 0)
            {
                appSettings.RemoveChild(list[0]);
                xml.Save(DatabaseFixture.FindWebConfigPath());
            }
        }

        private void Warmup()
        {
            var attempts = 0;
            while (attempts++ < 100)
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        client.DownloadString(Settings.RootUrl);
                        break;
                    }
                }
                catch { }
            }
        }

        public static IWebDriver GetWebDriver(bool shared)
        {
            if (shared && SharedWebDriver != null)
            {
                return SharedWebDriver;
            }
            IWebDriver driver;
            switch (Settings.WebDriverProvider)
            {
                case "Edge":
                    driver = GetEdgeDriver(shared);
                    break;
                case "Firefox":
                    driver = GetFirefoxDriver(shared);
                    break;
                default: //"Chrome"
                    driver = GetChromeDriver(shared);
                    break;
            }

            if (shared)
            {
                SharedWebDriver = driver;
            }
            return driver;
        }

        private static IWebDriver GetFirefoxDriver(bool shared)
        {
            new DriverManager().SetUpDriver(new FirefoxConfig());
            FirefoxDriver edgeDriver;
            FirefoxOptions options = new FirefoxOptions();
            options.AcceptInsecureCertificates = true;
            options.PageLoadStrategy = PageLoadStrategy.Normal;
            edgeDriver = new FirefoxDriver(options);
            return edgeDriver;
        }

        private static IWebDriver GetEdgeDriver(bool shared)
        {
            new DriverManager().SetUpDriver(new EdgeConfig());
            EdgeDriver edgeDriver;
            EdgeOptions options = new EdgeOptions();
            options.AcceptInsecureCertificates = true;
            options.PageLoadStrategy = PageLoadStrategy.Normal;
            edgeDriver = new EdgeDriver(options);
            return edgeDriver;
        }

        private static IWebDriver GetChromeDriver(bool shared)
        {
            new DriverManager().SetUpDriver(new ChromeConfig());
            ChromeDriver chromeDriver;
            ChromeOptions options = new ChromeOptions();
            // ChromeDriver is just AWFUL because every version or two it breaks unless you pass cryptic arguments
            options.PageLoadStrategy = PageLoadStrategy.Normal; // https://www.skptricks.com/2018/08/timed-out-receiving-message-from-renderer-selenium.html
            options.AddArgument("ignore-certificate-errors");
            options.AddArgument("start-maximized"); // https://stackoverflow.com/a/26283818/1689770
            options.AddArgument("enable-automation"); // https://stackoverflow.com/a/43840128/1689770
            options.AddArgument("--no-sandbox"); //https://stackoverflow.com/a/50725918/1689770
            options.AddArgument("--disable-infobars"); //https://stackoverflow.com/a/43840128/1689770
            options.AddArgument("--disable-dev-shm-usage"); //https://stackoverflow.com/a/50725918/1689770
            options.AddArgument("--disable-browser-side-navigation"); //https://stackoverflow.com/a/49123152/1689770
            options.AddArgument("--disable-gpu"); //https://stackoverflow.com/questions/51959986/how-to-solve-selenium-chromedriver-timed-out-receiving-message-from-renderer-exc
            var chromedriverDir = Environment.GetEnvironmentVariable("ChromeDriverDir");
            if (string.IsNullOrEmpty(chromedriverDir))
            {
                chromeDriver = new ChromeDriver(options);
            }
            else
            {
                chromeDriver = new ChromeDriver(chromedriverDir, options, TimeSpan.FromSeconds(120));
            }
            return chromeDriver;
        }

        public void Dispose()
        {
            cmswebInstance?.Stop();
            cmswebInstance = null;
            SharedWebDriver?.Quit();
            SharedWebDriver = null;
            CleanupWebConfig();
        }
    }
}
