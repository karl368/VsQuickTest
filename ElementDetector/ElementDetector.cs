using ElementDetector.selenium;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElementDetector
{
    public partial class ElementDetector : Form
    {
        private String defaultScreenshotSavingPath = "D:/tmp";
        private String currentFileName = null;

        private RemoteWebDriver driver;

        public ElementDetector()
        {
            InitializeComponent();
            openBrowser_Click(this, null);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void openBrowser_Click(object sender, EventArgs e)
        {
            if (driver == null)
            {
                driver = SeleniumUtil.startBrowser("Firefox");
                SeleniumUtil.maximize();
                SeleniumUtil.get("http://www.baidu.com");
            }
            else
            {
                MessageBox.Show("Browser already opened. Please don't open multiple browsers. ");
            }
            refreshComponentsState();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (driver != null)
            {
                SeleniumUtil.quit();
                driver = null;
            }
            else 
            {
                MessageBox.Show("Browser not yet opened. Please Open Browser first. ");
            }
            refreshComponentsState();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (driver != null)
            {
                driver.Quit();
            }

            base.OnFormClosing(e);
        }

        private void getScreenshot_Click(object sender, EventArgs e)
        {
            String currentTimeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            currentFileName = "vs-" + currentTimeStamp + ".png";
            SeleniumUtil.getScreenShot(defaultScreenshotSavingPath + "/" + currentFileName);
            showScreenshot();
        }

        private void showScreenshot() 
        {
            screenshotPictureBox.ImageLocation = defaultScreenshotSavingPath + "/" + currentFileName;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            screenshotGroupBox.Width = Width - (735 - 696);
            screenshotGroupBox.Height = Height - (496 - 409);
            
        }

        private void refreshComponentsState() 
        {
            if (driver != null)
            {
                openFirefox.Enabled = false;
                getScreenshot.Enabled = true;
                closeFirefox.Enabled = true;
            }
            else
            {
                openFirefox.Enabled = true;
                getScreenshot.Enabled = false;
                closeFirefox.Enabled = false;
            }
        }
       
    }
}
