using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ElementDetector.selenium
{
    /*
     * 
     * History
     * Date        Ver Author        Change Description
     * ----------- --- ------------- ----------------------------------------
     * 13 Mar 2015 001 Kar           Create, copy and revise from java code
     */
    
    public class SeleniumUtil {
	
	    protected static readonly String TEST_BROWSER = Browsers.FIREFOX;
	
	    /**
	     * Max waiting time for page elements to be loaded is set to be 10 seconds 
	     * and the time interval is 1 second.
	     */
	    private static readonly int DEFAULT_TIMEOUT = 30000; 
	    private static readonly int DEFAULT_WAIT = 800; 
	
	    // Whenever findElement(by), findElements(by) find no element, it will
	    // cost at least this period of time to return. 
	    private static readonly int IMPLICITLY_WAIT_MILLISECONDS = 300;
	
	    private static RemoteWebDriver driver = null;
	    private static Boolean inGridMode = false;

	    /**
	     * The following system properties can be passed to UT at the startup:
	     * -DseleniumHost -DseleniumPort
	     */
	    private static String seleniumHost = "";
	    private static String seleniumPort = "";

	    /**
	     * The following system properties can be passed to UT at the startup:
	     * -Dbrowser
	     */
	    private static String browser = "Firefox";
	
	    private static String imgPathStr = "d:/temp/img";
	
	    private enum IsMethod {
		    IS_ENABLED, IS_DISPLAYED, IS_SELECTED
	    }
	
	    public static IOptions manage() {
		    return driver.Manage();
	    }
	
	    public static void implicitlyWait() {
            manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(IMPLICITLY_WAIT_MILLISECONDS / 1000));
	    }
	
	    public static RemoteWebDriver startBrowser() {
		    return driver = loadDriver(browser,null);
	    }
	
	    public static RemoteWebDriver startBrowser(String firefoxProfileName) {
		    return driver = loadDriver(browser, firefoxProfileName);
	    }
	
	    private static RemoteWebDriver loadDriver(String browser, String firefoxProfileName) {
		    if("ie".Equals(browser)) {
			    return loadIeDriver();
		    } else if("firefox".Equals(browser)){
			    return loadFirefoxDriver(firefoxProfileName);
		    } else if ("chrome".Equals(browser)) {
			    return loadChromeDriver();
		    }

		    return new FirefoxDriver();
	    }

	    private static RemoteWebDriver loadChromeDriver() {
		    return new ChromeDriver();
	    }

	    private static RemoteWebDriver loadFirefoxDriver(String firefoxProfileName) {
		    String profileName = null;
		    if(firefoxProfileName != null){
			    profileName = firefoxProfileName;
		    } else {
			    return new FirefoxDriver();
		    }
		    FirefoxProfile profile = new FirefoxProfile(profileName);
		    return new FirefoxDriver(profile);
	    }

	    private static RemoteWebDriver loadIeDriver() {
		    return new InternetExplorerDriver();
	    }

	    public static int getDefaultTimeout(){
		    return DEFAULT_TIMEOUT;
	    }
	
	    public static int getSeleniumPort() {
		    if (seleniumPort == null) {
			    seleniumPort = "4444"; // default selenium port
		    }

		    return Int32.Parse(seleniumPort);
	    }

	    public static String getBrowser() {
		    if (browser == null)
			    browser = "*iexplore"; // default browser

		    return browser;
	    }

	    public static void get(String url) {
            driver.Navigate().GoToUrl(url);
	    }
	
	    public static void refresh() {
		    driver.Navigate().Refresh();
	    }
	
	    private class Wait {
		    private long timeOutInMilliseconds = DEFAULT_TIMEOUT;
		
		    public Wait(int timeOutInMilliseconds){
			    this.timeOutInMilliseconds = timeOutInMilliseconds;
		    }

		    private Boolean _for(Func<Boolean> booleanFunction){
			    long startTime = System.DateTime.Now.Millisecond;
			    long timeOut = startTime + timeOutInMilliseconds;
			    while (System.DateTime.Now.Millisecond <= timeOut) {
				    try {
					    Boolean rtn = booleanFunction();
					    if (rtn) {
						    return true;
					    }
					    System.Threading.Thread.Sleep(DEFAULT_WAIT);
				    } catch (Exception e){
					    // let it go
				    }
			    }
			    return false;
		    }
		
		    /**
		     * Take care that this wait strategy will ignore all exceptions while waiting. 
		     * false value will be returned if condition is not archived.  
		     * */
		    public static Boolean until(Func<Boolean> booleanFunction, int timeout){
			    return new Wait(timeout)._for(booleanFunction);
		    }
	    }
	
	    /** wait for fixed period time by milliseconds*/
	    public static void waitForFixedMilliseconds(long milliseconds){
		   Thread.Sleep((int)milliseconds);
	    }
	
	    public static void waitForJsPerceivableSeconds(){
		    waitForFixedMilliseconds(500);
	    }
	
	    public static void waitForHumanPerceivableSeconds(){
		    waitForFixedMilliseconds(800);
	    }
	
	    /**
	     * History
	     * Date        Ver Author        Change Description
	     * ----------- --- ------------- ----------------------------------------
	     * 28 May 2013 002 Karl          Add method to wait
	     * */
	    public static void waitForCondition(
			    Func<IWebDriver, Boolean> booleanCondition, int timeOutInMilliseconds,
			    String message) {
		    try {
			    WebDriverWait wait = new WebDriverWait(
                    driver, TimeSpan.FromSeconds(timeOutInMilliseconds / 1000));
                wait.IgnoreExceptionTypes(new Type[]{typeof(StaleElementReferenceException)});
                wait.Until(booleanCondition);
		    } catch(WebDriverTimeoutException e){
			    
                if(e.InnerException != null){
				    fail(message + "Exception occurred during the waiting, detail is: " + e.InnerException);
			    } else {
				    fail(message);
			    }
		    }
	    }
	
	    /** 
	     * History
	     * Date        Ver Author        Change Description
	     * ----------- --- ------------- ----------------------------------------
	     * 08 Jun 2013 003 Karl          Add method to wait for list to load completely
	     */
	    public static void waitForElementsToLoadCompletely(By list, int waitRound, 
		    int waitMillisecondsPerRound) {
		    const int defaultTimeout = 60000;
		    waitForElementsToLoadCompletely(list, waitRound, waitMillisecondsPerRound, defaultTimeout);
	    }
	
	    public static void waitForElementsToLoadCompletely(By list, int waitRound, 
		    int waitMillisecondsPerRound, int timeOut) {
		
		    long beforeCallTime = System.DateTime.Now.Millisecond;
		
		    // if result list shows, wait for all records to load
		    long prevCount = 0;
		    int stableRound = 0;
		    Boolean someRecordsLoaded = false;
		    while(true){
			    long rowCount = driver.FindElements(list).Count;
			    long endFind = System.DateTime.Now.Millisecond;
			
			    if(endFind - beforeCallTime > timeOut){
				    break;
			    }
			
			    if(prevCount == 0 && rowCount > 0){
				    someRecordsLoaded = true;
			    }
			
			    // if record count becomes stable, quit wait, else continue 
			    if(rowCount == prevCount && someRecordsLoaded){
				    stableRound ++;
				    if(stableRound == waitRound) break;
			    } else {
				    stableRound = 0;
			    }
			    prevCount = rowCount;
			    waitForFixedMilliseconds(waitMillisecondsPerRound);
		    }
	    }
	
	    public static void waitForListToLoadSomething(
		    String listName, By listBy, int timeOutInMilliseconds){
		    SeleniumUtil.waitForCondition(
			    (driver) => 
                {
                    if(SeleniumUtil.findElements(listBy).Count > 0)
                    {
						return true;
					}
					return false;
                }, 
			    timeOutInMilliseconds, "Can't load " + listName + " items within " + 
				    (timeOutInMilliseconds/1000) + " seconds. " +
				    "Please make sure network performs ok. "
		    );
	    }
	
	    public static void waitForAnyWindowToClose(HashSet<String> beforeWindows, String operationName){
		    waitForAnyWindowToClose(beforeWindows, 15000, operationName);
	    }
	
	    public static void waitForAnyWindowToClose(HashSet<String> beforeWindows, int timeout, 
			    String operationName){
		    SeleniumUtil.waitForCondition(
			    (driver) => 
                {
                    Boolean rtn = false;
					List<String> currentWins = SeleniumUtil.getWindowHandles();
					foreach(String loopOldWin in beforeWindows){
						if(!currentWins.Contains(loopOldWin)){
							rtn = true;
							break;
						}
					}
					// wait for extra second to let page to be stable
					SeleniumUtil.waitForFixedMilliseconds(1000);
					return rtn;
                }, 
			    timeout, operationName + " operation performed, but no window closed within " + 
				    (timeout/1000) + " seconds. Please make sure network performs ok. "
		    );
	    }
	
	    public static void waitForWindowToClose(String windowName, String window, int timeout){
		    SeleniumUtil.waitForCondition(
			    (driver) => 
                {
                    return !SeleniumUtil.getWindowHandles().Contains(window);
                }, 
			    timeout, "The " + windowName + " didn't close within " + (timeout/1000) + " seconds. Please make sure network performs ok. "
		    );
	    }
	
	    public static void waitForElementTrimText(By elementBy, String expectedText, 
		    int timeout, String message){
		    SeleniumUtil.waitForCondition(
			    (driver) => 
                {
                    if(SeleniumUtil.findElements(elementBy).Count > 0 && 
						SeleniumUtil.findElement(elementBy).Displayed && 
						expectedText.Equals(SeleniumUtil.findElement(elementBy).Text.Trim())){
						return true;
					}
					return false;
                }, 
			    timeout, message
		    );
	    }
	
	    public static void waitForElementToDisappear(By elementBy,  
		    int timeout, String message){
		    SeleniumUtil.waitForCondition(
			    (driver) => 
                {
                    if(SeleniumUtil.findElements(elementBy).Count > 0 && 
						SeleniumUtil.findElement(elementBy).Displayed){
						return false;
					}
					return true;
                }, 
			    timeout, message
		    );
	    }
	
	    public static void waitForElementToBeRemovedFromHtml(By elementBy,  
		    int timeout, String message){
		    SeleniumUtil.waitForCondition(
			    (driver) => 
                {
                    return SeleniumUtil.findElements(elementBy).Count == 0;
                }, 
			    timeout, message
		    );
	    }
	
	    public static void waitForElementToContainText(By elementBy,  
		    int timeout, String message, String containedText){
		    SeleniumUtil.waitForCondition(
			    (driver) => 
                {
                    if(SeleniumUtil.findElements(elementBy).Count > 0 && 
						SeleniumUtil.findElement(elementBy).Displayed && 
						SeleniumUtil.findElement(elementBy).Text.Contains(containedText)){
						return true;
					}
					return false;
                }, 
			    timeout, message
		    );
	    }
	
	    public static void waitForElementToContainTexts(By elementBy,  
		    int timeout, String message, String[] containedTexts){
		    SeleniumUtil.waitForCondition(
			    (driver) => 
                {
                    if(SeleniumUtil.findElements(elementBy).Count > 0 && 
						SeleniumUtil.findElement(elementBy).Displayed){
						String text = SeleniumUtil.findElement(elementBy).Text;
						foreach(String oneValue in containedTexts){
							if(text.Contains(oneValue)){
								return true;
							}
						}
						return true;
					}
					return false;
                }, 
			    timeout, message
		    );
	    }
	
	    public static void waitForElementToDiscardText(By elementBy,  
		    int timeout, String message, String containedText){
		    SeleniumUtil.waitForCondition(
			    (driver) => 
                {
                    if(SeleniumUtil.findElements(elementBy).Count > 0 && 
						SeleniumUtil.findElement(elementBy).Displayed && 
						SeleniumUtil.findElement(elementBy).Text.Contains(containedText)){
						return false;
					}
					return true;
                }, 
			    timeout, message
		    );
	    }
	
	    public static void waitForElementToLoadStyle(By elementBy,  
		    int timeout, String message, String containedStyle){
		    SeleniumUtil.waitForElementToLoadAttribute(elementBy, timeout, message, "style", containedStyle);
	    }
	
	    public static void waitForElementToLoadAttribute(By elementBy, int timeout, 
			    String message, String attributeName, String containedAttributeValue){
		    SeleniumUtil.waitForCondition(
			    (driver) => 
                {
                    if(SeleniumUtil.findElements(elementBy).Count > 0 && 
						SeleniumUtil.findElement(elementBy).Displayed && 
						SeleniumUtil.findElement(elementBy).GetAttribute(attributeName).Contains(containedAttributeValue)){
						return true;
					}
					return false;
                }, 
			    timeout, message
		    );
	    }
	
	    public static void waitForInputToPopulateOneValue(By inputBy,  
		    int timeout, String message){
		    SeleniumUtil.waitForCondition(
			    (driver) => 
                {
                    if(SeleniumUtil.findElements(inputBy).Count > 0 && 
						SeleniumUtil.findElement(inputBy).Displayed && 
						!"".Equals(getAttribute(inputBy, "value"))){
						return true;
					}
					return false;
                }, 
			    timeout, message
		    );
	    }
	
	    public static String getAttribute(By by, String attributeName) {
		    return SeleniumUtil.findElement(by).GetAttribute(attributeName);
	    }
	
	    public static String getTrimAttribute(By by, String attributeName) {
		    String rtn = getAttribute(by, attributeName);
		    return rtn != null ? rtn.Trim() : null;
	    }
	
	    public static void waitForInputToPopulateValue(By inputBy,  
		    int timeout, String message, String expectedText){
		    SeleniumUtil.waitForCondition(
			    (driver) => 
                {
                    if(SeleniumUtil.findElements(inputBy).Count > 0 && 
						SeleniumUtil.findElement(inputBy).Displayed && 
						expectedText.Equals(SeleniumUtil.findElement(inputBy).GetAttribute("value").Trim())){
						return true;
					}
					return false;
                }, 
			    timeout, message
		    );
	    }
	
	    /**
	     * To make the function be stable, not check if it is displayed on page or not. 
	     * Verify only the text. 
	     * */
	    public static void waitForElementToChangeText(By elementBy, int timeout, 
		    String message, String toBeUpdatedText){
		    SeleniumUtil.waitForCondition(
			    (driver) => 
                {
                    if(SeleniumUtil.findElements(elementBy).Count > 0 && 
						!toBeUpdatedText.Equals(SeleniumUtil.findElement(elementBy).Text.Trim())){
						return true;
					}
					return false;
                }, 
			    timeout, message
		    );
	    }
	
	    public static void waitForSelectToSelectByText(By elementBy, int timeout, 
		    String message, String toBeLoadedText){
		
		    SeleniumUtil.waitForCondition(
			    (driver) => 
                {
                    Boolean rtn = false;
					if(SeleniumUtil.findElements(elementBy).Count > 0){
						SelectElement select = new SelectElement(SeleniumUtil.findElement(elementBy));
						if(select.Options.Count > 0){
							if(toBeLoadedText.Equals(select.SelectedOption.Text)){
								rtn = true;
							}
						}
					}
					SeleniumUtil.waitForFixedMilliseconds(500);
					return rtn;
                }, timeout, 
			    message
		    );
	    }
	
	    public static void waitForSelectToDeSelectByText(By elementBy, int timeout, 
		    String message, String toBeDeselectedText){
		
		    SeleniumUtil.waitForCondition(
			    (driver) => 
                {
                    Boolean rtn = false;
					if(SeleniumUtil.findElements(elementBy).Count > 0){
						SelectElement select = new SelectElement(SeleniumUtil.findElement(elementBy));
						if(select.Options.Count > 0){
							if(!toBeDeselectedText.Equals(select.SelectedOption.Text)){
								rtn = true;
							}
						}
					}
					SeleniumUtil.waitForFixedMilliseconds(500);
					return rtn;
                }, timeout, 
			    message
		    );
	    }
	
	    public static void waitForSelectToSelectOneText(By elementBy, int timeout, 
		    String message){
		    SeleniumUtil.waitForCondition(
			    (driver) => 
                {
                    Boolean rtn = false;
					if(SeleniumUtil.findElements(elementBy).Count > 0){
						SelectElement select = new SelectElement(SeleniumUtil.findElement(elementBy));
						if(select.Options.Count > 0){
							if(!"".Equals(select.SelectedOption.Text.Trim())){
								rtn = true;
							}
						}
					}
					SeleniumUtil.waitForFixedMilliseconds(500);
                    return rtn;
                }, timeout, 
			    message
		    );
	    }
	
	    public static void waitForSelectToLoadOptionText(By elementBy, int timeout, 
		    String message, String optionText){
		    SeleniumUtil.waitForCondition(
			    (driver) => 
                {
                    Boolean rtn = false;
					try {
						if(SeleniumUtil.findElements(elementBy).Count > 0){
							IWebElement select = SeleniumUtil.findElement(elementBy);
                            By optionBy = By.XPath(String.Format("./option[contains(text(), '{0}')]",optionText));
							if(SeleniumUtil.findElements(select, optionBy).Count > 0){
								rtn = true;
							}
						}
					} catch(StaleElementReferenceException e) { } // element refreshing, problematic point, just ignore it
					SeleniumUtil.waitForFixedMilliseconds(500);
					return rtn;
                }, timeout, 
			    message
		    );
	    }
	
	    /**
	     * Wait for an select to load and then de-select the reset value
	     * (such as "Select Billing Rate Status"). Use this method to wait a select element
	     * to load meaningful data in an edit page. 
	     * */
	    public static void waitForSelectToLoadAndDeSelectByText(By elementBy, int timeout, 
		    String message, String toBeDeselectedText){
		    SeleniumUtil.waitForCondition(
			    (driver) => 
                {
                    Boolean rtn = false;
					try {
						if(SeleniumUtil.findElements(elementBy).Count == 0){
							rtn = false;
						} else if(new SelectElement(SeleniumUtil.findElement(elementBy)).Options.Count ==0) {
							rtn = false;
						} else {
							SelectElement select = new SelectElement(SeleniumUtil.findElement(elementBy));
							String actualText = select.SelectedOption.Text;
						    if("".Equals(actualText.Trim())) {
						    	rtn = false;
						    } else if(!toBeDeselectedText.Equals(actualText.Trim())){
								rtn = true;
							}
						}
					} catch(StaleElementReferenceException e) { } // element refreshing, problematic point, just ignore it
					SeleniumUtil.waitForFixedMilliseconds(500);
					return rtn;
                }, timeout, 
			    message
		    );
	    }
	
	    private static Boolean isExecuted(Enum isMethod, IWebElement element) {
		    return isExecuted(isMethod, element, DEFAULT_TIMEOUT);
	    }
	
	    /**
	     * The logic of waiting elements to load.
	     */
	    private static Boolean isExecuted(Enum isMethod, IWebElement element, int timeout) {
		    long timer = 0;
		    while (true) {
			    try {
				    if ((IsMethod.IS_ENABLED.Equals(isMethod) && element.Enabled)
						    || (IsMethod.IS_DISPLAYED.Equals(isMethod) && element.Displayed)
						    || (IsMethod.IS_SELECTED.Equals(isMethod) && element.Selected)) {
					    return true;
				    } else {
					    Thread.Sleep(DEFAULT_WAIT);
				    }
			    } catch (Exception e) {
			    }
			    timer += DEFAULT_WAIT;
			    if (timer > timeout) {
				    break;
			    }
		    }
		    return false;
	    }
	
	    public static Boolean isDisplayed(IWebElement element) {
		    return isExecuted(IsMethod.IS_DISPLAYED, element);
	    }
	
	    public static Boolean isDisplayed(IWebElement element, int timeout) {
		    return isExecuted(IsMethod.IS_DISPLAYED, element, timeout);
	    }
	
	    public static Boolean isEnabledImmediately(By by) {
		    return findElement(by).Enabled;
	    }
	
	    public static Boolean isEnabled(IWebElement element) {
		    return isExecuted(IsMethod.IS_ENABLED, element);
	    }

	    public static Boolean isSelected(IWebElement element) {
		    return isExecuted(IsMethod.IS_SELECTED, element);
	    }
	
	    public static Boolean isSelectedImmediately(By elementBy) {
		    return findElement(elementBy).Selected;
	    }
	
	    public static Boolean isSelectedImmediately(IWebElement element) {
		    return element.Selected;
	    }
	
	    public static Boolean isExistImmediately(By elementBy){
		    return driver.FindElements(elementBy).Count > 0;
	    }
	
	    public static Boolean isExistImmediately(By parentBy, By childBy){
		    if(isExistImmediately(parentBy)){
			    return findElements(parentBy, childBy).Count > 0;
		    }
		    return false;
	    }
	
	    public static Boolean isExistImmediately(IWebElement parent, By childBy){
		    return findElements(parent, childBy).Count > 0;
	    }
	
	    public static Boolean isDisplayedImmediately(By elementBy){
		    return isExistImmediately(elementBy) && driver.FindElement(elementBy).Displayed;
	    }
	
	    public static Boolean isDisplayedImmediately(By parentBy, By by){
		    return driver.FindElements(parentBy).Count > 0 && 
			    driver.FindElement(parentBy).FindElements(by).Count > 0 &&
			    driver.FindElement(parentBy).FindElement(by).Displayed;
	    }
	
	    public static Boolean isExist(By elementBy, int timeout){
		    return Wait.until(
			    () => 
                {
                    return driver.FindElements(elementBy).Count > 0;
                }, timeout);
	    }
	
	    public static Boolean isDisplayed(By by){
		    return isDisplayed(by, DEFAULT_TIMEOUT);
	    }
	
	    public static Boolean isDisplayed(By by, int timeout){
		    return Wait.until(
			    () => 
                {
                    if(driver.FindElements(by).Count > 0 && 
						driver.FindElement(by).Displayed) {
						return true;
					}
					return false;
                }, 
			    timeout);
	    }
	
	    public static Boolean isDisappear(By by, int timeout){
		    return Wait.until(
			    () => 
                {
                    if(driver.FindElements(by).Count == 0 || 
						!driver.FindElement(by).Displayed){
						return true;
					}
					return false;
                }, 
			    timeout);
	    }
	
	    public static Boolean isDisplayed(By parentBy, By by, int timeout){
		    return Wait.until(
			    () => 
                {
                    return isDisplayedImmediately(parentBy, by);
                }, 
			    timeout);
	    }
	
	    public static Boolean isEnabled(By by){
		    return Wait.until(
			    () => 
                {
                    if(driver.FindElements(by).Count > 0 && 
						driver.FindElement(by).Enabled){
						return true;
					}
					return false;
                }, 
			    DEFAULT_TIMEOUT);
	    }
	
	    public static Boolean isSelected(By by){
		    return Wait.until(
			    () => 
                {
                    if(driver.FindElements(by).Count > 0 && 
						driver.FindElement(by).Selected){
						return true;
					}
					return false;
                }, 
			    DEFAULT_TIMEOUT);
	    }
	
	    public static Boolean isEnabled(By by, int timeout){
		    return Wait.until(
		    () => 
            {
                if(driver.FindElements(by).Count > 0 && 
					driver.FindElement(by).Enabled){
					return true;
				}
				return false;
            }, timeout);
	    }
	
	    public static Boolean isSelectContains(By selectBy, String expectedText, int timeout){
		    return Wait.until(
		    () => 
            {
                 if(driver.FindElements(selectBy).Count > 0){
					By optionBy = By.XPath("./option[text()='" + expectedText + "']");
					return findElements(selectBy, optionBy).Count > 0;
				}
				return false;
            }, timeout);
	    }
	
	    /** 
	     * switch to prompted window
	     * @return null if switch do nothing, or the old window name
	     * 
	     * */
	    public static String switchToPromptedWindow(HashSet<String> before, HashSet<String> after){
		    String oldWin = SeleniumUtil.getWindowHandle();
		    List<String> whs = new List<String>(after);
            whs.RemoveAll(str => before.Contains(str));
		    if(whs.Count > 0){
                driver.SwitchTo().Window(whs.ElementAt(0));
			    return oldWin;
		    }
		    return null;
	    }
	
	    /** 
	     * switch to prompted window after wait for some milliseconds
	     * */
	    public static String switchToPromptedWindow(HashSet<String> before, long waitForMilliseconds, 
			    String expectedWindowName){
		    String foundNewWindow = getPromptedWindow(before, waitForMilliseconds);
		    if(foundNewWindow != null){
			    String oldWin = SeleniumUtil.getWindowHandle();
			    SeleniumUtil.switchToWindow(foundNewWindow);
			    return oldWin;
		    }
		    throw new InvalidOperationException("Time out waiting for " + expectedWindowName + " window to prompted "
			    + "out within " + (waitForMilliseconds/1000) + " seconds. ");
	    }
	
	    public static String switchToPromptedWindow(HashSet<String> before, long waitForMilliseconds){
		    return switchToPromptedWindow(before, waitForMilliseconds, "new");
	    }
	
	    /** 
	     * switch to prompted window if any after wait for some milliseconds, return null for no new window
	     * prompted out, else return the old window handle
	     * */
	    public static String switchToPromptedWindowIfAny(HashSet<String> before, long waitForMilliseconds){
		    String oldWin = SeleniumUtil.getWindowHandle();
		    String foundNewWindow = getPromptedWindow(before, waitForMilliseconds);
		    if(foundNewWindow != null){
			    SeleniumUtil.switchToWindow(foundNewWindow);
			    return oldWin;
		    }
		    return null;
	    }
	
	    public static Boolean closePromptedWindowIfAny(HashSet<String> before, long waitForMilliseconds) {
		    String oldWin = SeleniumUtil.switchToPromptedWindowIfAny(before, 7000);
		    if(oldWin != null){
			    SeleniumUtil.close();
			    SeleniumUtil.switchToWindow(oldWin);
			    return true;
		    }
		    return false;
	    }
	
	    /**
	     * @return null if no prompted window found within waitForMilliseconds
	     * */
	    public static String getPromptedWindow(HashSet<String> before, long waitForMilliseconds){
		    const long oneRoundWait = 500;
		    long deadLine = System.DateTime.Now.Millisecond + waitForMilliseconds;
		
		    String foundNewWindow = null;
		    while(true){
			    List<String> after = new List<String>(SeleniumUtil.getWindowHandles());
			    after.RemoveAll(str => before.Contains(str));
			    if(after.Count > 0){
				    foundNewWindow = after.ElementAt(0);
				    return foundNewWindow;
			    }
			
			    SeleniumUtil.waitForFixedMilliseconds(oneRoundWait);
			    if(System.DateTime.Now.Millisecond > deadLine){
				    return null;
			    } 
		    }
	    }
	
	    /** switch to prompted alert window after wait for some milliseconds*/
	    public static IAlert switchToPromptedAlert(long waitForMilliseconds)
	    {
		    const long ONE_ROUND_WAIT = 100;
		    NoAlertPresentException lastException = null;
		
		    long endTime = System.DateTime.Now.Millisecond + waitForMilliseconds;
		
		    for(long i=0; i<waitForMilliseconds; i += ONE_ROUND_WAIT){

			    try{
				    IAlert alert = driver.SwitchTo().Alert();
				    return alert;
			    } catch(NoAlertPresentException e){
				    lastException = e;
			    }
			    waitForFixedMilliseconds(ONE_ROUND_WAIT);
			
			    if(System.DateTime.Now.Millisecond > endTime){
				    break;
			    }
		    }
		    throw lastException;
	    }
	
	    private static ITargetLocator switchTo(){
		    return driver.SwitchTo();
	    }
	
	    public static void switchToWindow(String windowHandle){
		    switchTo().Window(windowHandle);
	    }
	
	    public static IAlert switchToAlert(){
		    return switchTo().Alert();
	    }
	
	    public static Point getPosition() {
		    return driver.Manage().Window.Position;
	    }
	
	    public static void maximize() {
		    driver.Manage().Window.Maximize();
	    }
	
	    /** get window handler */
	    public static String getWindowHandle(){
		    return driver.CurrentWindowHandle;
	    }
	
	    /** get window handlers */
	    public static List<String> getWindowHandles(){
		    return driver.WindowHandles.ToList();
            
        }
	
	    public static String getCurrentUrl() {
		    return driver.Url;
	    }
	
	    public static String getPageSource() {
		    return driver.PageSource;
	    }
	
	    /** select default content*/
	    public static void selectDefaultContent(){
		    driver.SwitchTo().DefaultContent();
	    }
	
	    /** select frame by index*/
	    public static void selectFrame(int frameIdx){
		    driver.SwitchTo().Frame(frameIdx);
	    }
	
	    /** select frame by frame name or id*/
	    public static void selectFrame(String locator){
		    /* 
		     * chrome browser(19.0.1084.52) frame(String) not work, if frame 
		     * not found, try frame(WebElement) again. 
		     */
		    try{
			    driver.SwitchTo().Frame(locator);
		    } catch (NoSuchFrameException e){
			    if(findElements(By.Name(locator)).Count > 0){
				    driver.SwitchTo().Frame(findElement(By.Name(locator)));
			    } else if(findElements(By.Id(locator)).Count > 0){
				    driver.SwitchTo().Frame(findElement(By.Id(locator)));
			    } else {
				    throw e;
			    }
		    }
	    }
	
	    public static void selectFrame(IWebElement frameEle){
		    driver.SwitchTo().Frame(frameEle);
	    }
	
	    public static void selectFrame(By frameEleBy){
		    driver.SwitchTo().Frame(driver.FindElement(frameEleBy));
	    }
	
	    public static IWebElement findElement(By by) {
		    return driver.FindElement(by);
	    }
	
	    public static IWebElement findElement(IWebElement element, By by) {
		    return element.FindElement(by);
	    }
	
	    public static IWebElement findElement(By parentBy, By by) {
		    IWebElement parent = driver.FindElement(parentBy);
		    return parent.FindElement(by);
	    }
	
	    /**
	     * <font color='red'>NOTICE: </font> don't call this function on huge page. It will cause 
	     * 'A script on this page may be busy, or it may have stopped responding. You can 
	     * stop the script now, or you can continue to see if the script will complete.' problem. 
	     * */
	    public static String getBodyText(){
		    By bodyby = By.XPath("/html/body");
		    return getTrimText(bodyby);
	    }
	
	    public static String getText(By elementBy){
		    IWebElement element = driver.FindElement(elementBy);
		    return getText(element);
	    }
	
	    public static String getText(IWebElement element){
		    if(!isInputOrButtonElement(element)) {
			    return element.Text;
		    } else {
			    return getInputText(element);
		    }
	    }
	
	    public static String getTrimText(By elementBy){
		    return getText(elementBy).Trim();
	    }
	
	    public static String getTrimText(IWebElement element){
		    return getText(element).Trim();
	    }

	    private static Boolean isInputOrButtonElement(IWebElement element) {
		    String tagName = element.TagName;
		    Boolean inputOrTextAreaElement = 
			    tagName.ToUpper().Equals("INPUT") || tagName.ToUpper().Equals("TEXTAREA");
		    if(!inputOrTextAreaElement) {
			    return false;
		    }
		
		    String subInputType = element.GetAttribute("type");
		
		    Boolean isInput = "TEXT".Equals(subInputType.ToUpper());
		    Boolean isButton = "BUTTON".Equals(subInputType.ToUpper()) 
			    || "SUBMIT".Equals(subInputType.ToUpper())
			    || "CLEAR".Equals(subInputType);
		    Boolean isTextArea = tagName.Equals("TEXTAREA");
		
		    if(isInput || isButton || isTextArea) {
			    return true;
		    }
		
		    return false;
	    }
	
	    public static String getText(By parentBy, By elementBy){
		    return getText(findElement(parentBy, elementBy));
	    }
	
	    public static String getTrimText(By parentBy, By elementBy){
		    return getTrimText(findElement(parentBy, elementBy));
	    }
	
	    public static String getText(IWebElement parent, By elementBy){
		    return getText(findElement(parent, elementBy));
	    }
	
	    public static String getTrimText(IWebElement parent, By elementBy){
		    return getTrimText(findElement(parent, elementBy));
	    }
	
	    private static String getInputText(By inputBy){
		    return getInputText(driver.FindElement(inputBy));
	    }
	
	    private static String getInputText(IWebElement inputElement){
		    return inputElement.GetAttribute("value");
	    }
	
	    private static String getInputTrimText(By inputBy){
		    return getInputText(inputBy).Trim();
	    }
	
	    public static String getAttribute(IWebElement parent, By by, String attribute){
		    return findElement(parent, by).GetAttribute(attribute);
	    }
	
	    public static String getTrimAttribute(IWebElement parent, By by, String attribute){
		    return getAttribute(parent, by, attribute).Trim();
	    }
	
	    public static List<String> getSelectOptions(By selectBy){
		    List<String> rtn = new List<String>();
		    List<IWebElement> options = findElements(selectBy, By.TagName("option"));
		    foreach(IWebElement loopEle in options){
			    rtn.Add(loopEle.Text);
		    }
		    return rtn;
	    }
	
	    /** get web element innerHTML, for debug only, not allowed to show in test case*/
	    public static String getInnerHtml(IWebElement element){
            return driver.ExecuteScript("return arguments[0].innerHTML;", element).ToString();

	    }
	
	    public static SelectElement findSelect(By by) {
		    return new SelectElement(driver.FindElement(by));
	    }
	
	    /**
	     * If no option is selected, return null
	     * */
	    public static String getFirstSelectedText(By selectBy){
		    SelectElement select = findSelect(selectBy);
		    if(select.AllSelectedOptions.Count == 0) {
			    return null;
		    }
		    return select.SelectedOption.Text;
	    }
	
	    public static String getAnalyzedFirstSelectedText(By selectBy) {
		    String firstSelectedText = getFirstSelectedText(selectBy);
		
		    if(firstSelectedText == null) {
			    return "";
		    } else {
			    return firstSelectedText.Trim();
		    }
	    }
	
	    public static int getRecordNumber(By recordListBy){
		    return driver.FindElements(recordListBy).Count;
	    }
	
	    public static List<IWebElement> findElements(By by) {
		    return driver.FindElements(by).ToList();
	    }
	
	    public static List<IWebElement> findElements(By parentBy, By childBy) {
		    return driver.FindElement(parentBy).FindElements(childBy).ToList();
	    }
	
	    public static List<IWebElement> findElements(IWebElement element, By by) {
		    return element.FindElements(by).ToList();
	    }
	
	    public static void sendKeys(IWebElement input, String keys) {
		    input.SendKeys(keys);
	    }
	
	    public static void sendKeys(By inputBy, String keys) {
		    driver.FindElement(inputBy).SendKeys(keys);
	    }
	
	    // make sure that all scripts are in sequence
	    public static void executeScript(String[] scripts) {
		    foreach (String script in scripts)
            {
                driver.ExecuteScript(script);
            }
	    }
	
	    public static void clear(IWebElement element) {
		    element.Clear();
	    }
	
	    public static void clear(By inputBy) {
		    driver.FindElement(inputBy).Clear();
	    }
	
	    public static void clearByJs(IWebElement input){
		    executeJavaScript("arguments[0].value='';", input);
	    }
	
	    public static void clearByJs(By inputBy){
		    clearByJs(driver.FindElement(inputBy));
	    }
	
	    public static void mark(By by) {
		    mark(driver.FindElement(by));
	    }
	
	    public static void mark(IWebElement ele) {
		    setRedBorder(ele);
	    }
	
	    public static void setRedBorder(By by) {
		    setRedBorder(driver.FindElement(by));
	    }
	
	    public static void setRedBorder(IWebElement ele) {
		    setBorder(ele, "#FF0000");
	    }
	
	    public static void setGreenBorder(By by) {
		    setGreenBorder(driver.FindElement(by));
	    }
	
	    public static void setGreenBorder(IWebElement ele) {
		    setBorder(ele, "#00FF00");
	    }
	
	    public static void setBlueBorder(By by) {
		    setBlueBorder(driver.FindElement(by));
	    }
	
	    public static void setBlueBorder(IWebElement ele) {
		    setBorder(ele, "#0000FF");
	    }
	
	    public static void setBorder(By by, String color) {
		    setBorder(driver.FindElement(by), color);
	    }
	
	    public static void setBorder(IWebElement ele, String color) {
		    clearBorder(ele);
		    String borderStyle = String.Format("border:2px solid {0}", color);
		
		    String originalStyle = ele.GetAttribute("style");
		    if(originalStyle != null) {
			    borderStyle = originalStyle + ";" + borderStyle;
		    }
		
		    borderStyle = String.Format("arguments{0}.style='%s';", borderStyle);
		    executeJavaScript(borderStyle, ele);
	    }
	
	    public static void unmark(By by) {
		    unmark(driver.FindElement(by));
	    }
	
	    public static void unmark(IWebElement ele) {
		    clearBorder(ele);
	    }
	
	    public static void clearBorder(By by) {
		    clearBorder(driver.FindElement(by));
	    }
	
	    public static void clearBorder(IWebElement ele) {
		    String style = ele.GetAttribute("style");
		    if(style != null) {
                Regex rgx = new Regex("border\\s*:\\s*.*?(;|$)");
			    String replacedStyle = rgx.Replace(style, "").Trim();
			    executeJavaScript(String.Format("arguments[0].style='%s';", replacedStyle), ele);
		    }
	    }
	
	    /*send keys slowly
	     * When the speed of selenium send keys is too quickly, the related drop down list can't 
	     * display, so let it slowly.
	     * */
	    public static void clearSendKeysSlowly(By inputBy, String str){
		    IWebElement input = driver.FindElement(inputBy);
		    driver.FindElement(inputBy).Clear();
		    char[] strChar = str.ToCharArray();
		    foreach(char s in strChar) {
			    input.SendKeys(s+"");
			    waitForFixedMilliseconds(800);
		    }
	    }
	
	    public static void clearSendKeys(IWebElement input, String str){
		    input.Clear();
		    input.SendKeys(str);
	    }
	
	    public static void clearSendKeys(By inputBy, String str){
		    IWebElement input = driver.FindElement(inputBy);
		    input.Clear();
		    input.SendKeys(str);
	    }
	
	    public static void clearSendKeysByJs(By inputBy, String str){
		    IWebElement input = driver.FindElement(inputBy);
		    executeJavaScript("arguments[0].value='" + str + "';", input);
	    }
	
	    public static void click(IWebElement element) {
		    element.Click();
	    }
	
	    public static void click(By elementBy) {
		    driver.FindElement(elementBy).Click();
	    }
	
	    public static void click(By parentBy, By elementBy) {
		    driver.FindElement(parentBy).FindElement(elementBy).Click();
	    }
	
	    public static void click(IWebElement parent, By elementBy) {
		    parent.FindElement(elementBy).Click();
	    }
	
	    public static void clickByJs(IWebElement element){
		    String clickCommand = "arguments[0].click();";
		
		    if(!element.Displayed){
			    try {
				    SeleniumUtil._scrollIntoView(element);
			    } catch (Exception e){ 
				    System.Console.WriteLine("!Warning: Failed to scroll " + element + " into view before click. " +
					    "Test script just ignore it. ");
			    }
		    }
		    SeleniumUtil.executeJavaScript(clickCommand, element);
	    }
	
	    public static void clickByJs(By elementBy){
		    clickByJs(driver.FindElement(elementBy));
	    }
	
	    public static void clickByJs(By parentBy, By elementBy){
		    clickByJs(findElement(parentBy, elementBy));
	    }
	
	    /** use javascript to operate the target element e*/
	    public static void executeJavaScript(String js, IWebElement element){
		    driver.ExecuteScript(js, element);
	    }
	
	    public static void executeJavaScript(String js, By elementBy){
		    executeJavaScript(js, driver.FindElement(elementBy));
	    }
	
	    public static void deselectAll(By selectBy){
		    new SelectElement(driver.FindElement(selectBy)).DeselectAll();
	    }
	
	    public static void SelectByText(By selectBy, String text){
		    new SelectElement(driver.FindElement(selectBy)).SelectByText(text);
	    }
	
	    public static void SelectByText(IWebElement select, String text){
		    SelectByText(new SelectElement(select), text);
	    }
	
	    public static void SelectByText(SelectElement select, String text){
		    select.SelectByText(text);
	    }
	
	    public static void selectWithin8Seconds(By selectBy, String selectItem){
		    selectWithin("", "", selectBy, selectItem, 8000);
	    }
	
	    public static void selectWithin8Seconds(String pageName, String selectName, 
		    By selectBy, String selectItem){
		    selectWithin(pageName, selectItem, selectBy, selectItem, 8000);
	    }
	
	    public static void selectWithin(String pageName, By selectBy, String selectItem, int timeout){
		    selectWithin(pageName, "", selectBy, selectItem, timeout);
	    }
	
	    public static void selectWithin(String pageName, String selectName, By selectBy, 
		    String selectItem, int timeout){
		
		    String desc = (pageName != null && !"".Equals(pageName)) ? ("On " + pageName + ", ") : "";
		    String selectByDesc = (selectName != null && !"".Equals(selectName)) ? 
			    selectName : selectBy.ToString(); 
		
		    desc += "[" + selectByDesc + "] list failed to load item with text [" 
			    + selectItem + "] within " + (timeout/1000) + " seconds. ";
		
		    waitForCondition(
			    (driver) => 
                {
                    if(findElements(selectBy).Count > 0){
						SelectElement select = new SelectElement(findElement(selectBy));
						SelectByText(select, selectItem);
						return true;
					}
					waitForFixedMilliseconds(500);
					return false;
                }, 
			    timeout, desc
		    );
		    waitForFixedMilliseconds(300);
	    }
	
	    public static Boolean isNonEmptyValueSelected(By selectBy){
		    SelectElement select = new SelectElement(SeleniumUtil.findElement(selectBy));
		    if(select.AllSelectedOptions.Count > 0){
			    if(!"".Equals(select.SelectedOption.Text.Trim())){
				    return true;
			    }
		    } 
		    return false;
	    }
	
	    public static void check(By checkboxBy){
		    IWebElement checkbox = driver.FindElement(checkboxBy);
		    check(checkbox);
	    }
	
	    public static void check(IWebElement checkbox){
		    if(!checkbox.Selected){
			    checkbox.Click();
		    }
	    }
	
	    public static void uncheck(By checkboxBy){
		    IWebElement checkbox = driver.FindElement(checkboxBy);
		    uncheck(checkbox);
	    }
	
	    public static void uncheck(IWebElement checkbox){
		    if(checkbox.Selected){
			    checkbox.Click();
		    }
	    }
	
	    /**
	     * Note: only tested in FireFox 19. 
	     * */
	    public static void scrollIntoView(IWebElement element) {
		    try {
			    _scrollIntoView(element);
		    } catch(Exception e){
			    System.Console.WriteLine("!Waring: selenium failed to scroll element into view. Detail error message is : \n" +
				    e.Message);
		    }
	    }
	
	    public static void scrollIntoView(By elementBy) {
		    scrollIntoView(driver.FindElement(elementBy));
	    }
	
	    private static void _scrollIntoView(IWebElement element){
		    SeleniumUtil.executeJavaScript("arguments[0].scrollIntoView(true);", element);
	    }
	
	    public static void scrollIntoViewAndMoveToElement(By elementBy){
		    scrollIntoView(elementBy);
		    SeleniumUtil.waitForFixedMilliseconds(1000);
		    new Actions(driver).MoveToElement(SeleniumUtil.findElement(elementBy)).Perform();
	    }
	
	    public static void mouseOver(By elementBy){
		    new Actions(driver).MoveToElement(SeleniumUtil.findElement(elementBy)).Perform();
	    }
	
	    /** drag the source element to target element*/
	    public static void dragAndDrop(IWebElement source, IWebElement target){
		    Actions action = new Actions(driver);
		    action.DragAndDrop(source, target);
		    SeleniumUtil.waitForFixedMilliseconds(500);
		    action.Perform();
		    SeleniumUtil.waitForFixedMilliseconds(500);
	    }
	
	    public static void quit() {
		    driver.Quit();
	    }
	
	    public static void close() {
		    driver.Close();
	    }
	
	    /**Like JUnit's Assert.assertEquals, compare two objects */
	    public static void assertEquals(Object expected, Object actual) {
            Assert.AreEqual(expected, actual);
	    }

	    /**Like JUnit's Assert.assertEquals, compare two strings */
	    public static void assertEquals(String expected, String actual) {
            Assert.AreEqual(expected, actual);
	    }

	    /**Like JUnit's Assert.assertEquals, compare two string arrays */
	    public static void assertEquals(String[] expected, String[] actual) {
            Assert.AreEqual(expected, actual);
	    }
	
	    protected static void assertInputEqualsSelect(By inputBy, By selectBy){
		    String inputText = SeleniumUtil.getInputTrimText(inputBy);
		    assertTextEqualsSelect(inputText, selectBy);
	    }
	
	    public static void assertTextEqualsSelect(String inputText, By selectBy){
		    String selectedText = SeleniumUtil.getAnalyzedFirstSelectedText(selectBy);
		    SeleniumUtil.assertEquals(inputText, selectedText);
	    }

	    /**Like JUnit's Assert.assertTrue, if true then pass*/
	    public static void assertTrue(Boolean condition) {
            Assert.IsTrue(condition);
	    }
	
	    /**Like JUnit's Assert.assertTrue with message, if true then pass*/
	    public static void assertTrue(String message, Boolean condition) {
            if (!condition) 
            {
                Assert.Fail(message);
            }
	    }

	    /**Like JUnit's Assert.assertFalse, if false then pass*/
	    public static void assertFalse(Boolean condition) {
            Assert.IsFalse(condition);
	    }

	    /**Asserts that two Boolean values are not the same*/
	    public static void assertNotEquals(Boolean expected, Boolean actual) {
            Assert.AreNotEqual(expected, actual);
	    }

	    /**Asserts that two objects are not the same*/
	    public static void assertNotEquals(Object expected, Object actual) {
            Assert.AreNotEqual(expected, actual);
	    }

	    /**when the system running is not expect, break running and report a error message*/
	    public static void fail(String errorMessage){
		    Assert.Fail(errorMessage);
	    }
	
	    public static void assertDisplayed(By elementBy){
		    SeleniumUtil.assertTrue(SeleniumUtil.isDisplayed(elementBy));
	    }
	
	    public static void assertDisplayed(String message, By elementBy){
		    SeleniumUtil.assertTrue(message, SeleniumUtil.isDisplayed(elementBy));
	    }
	
	    public static void assertDisplayed(By elementBy, int timeout){
		    SeleniumUtil.assertTrue(SeleniumUtil.isDisplayed(elementBy, timeout));
	    }
	
	    public static void assertDisplayed(String message, By elementBy, int timeout){
		    SeleniumUtil.assertTrue(message, SeleniumUtil.isDisplayed(elementBy, timeout));
	    }
	
	    public static void assertEnabled(By elementBy){
		    SeleniumUtil.assertTrue(SeleniumUtil.isEnabled(elementBy));
	    }
	
	    public static void assertEnabled(IWebElement element){
		    SeleniumUtil.assertTrue(element.Enabled);
	    }
	
	    public static void assertEnabled(By elementBy, int timeout){
		    SeleniumUtil.assertTrue(SeleniumUtil.isEnabled(elementBy, timeout));
	    }
	
	    public static void assertSelectContainsOption(String message, By selectBy, String optionText){
		    SeleniumUtil.waitForCondition(
			    (driver) => 
                {
                    if(SeleniumUtil.isDisplayedImmediately(selectBy)){
						return SeleniumUtil.getSelectOptions(selectBy).Contains(optionText);
					}
					return false;
                }, 15000, message
		    );
	    }
	
	    public static void getScreenShot(SystemException e) {
		     getScreenShot();
		     throwIt(e);
	    }

        private static void throwIt(SystemException e) {
		    throw new WebDriverException(e.Message);
	    }
	
	    public static void getScreenShot() {
            getScreenShot(prepareCaseFile());
        }

        public static void getScreenShot(String fullFilePath)
        {
            try
            {
                Screenshot screenshot = getScreenShotFile();
                screenshot.SaveAsFile(fullFilePath, ImageFormat.Png);
            }
            catch (Exception e)
            { // swallow all exception and give a warning
                System.Console.WriteLine("Exception happen when getting screen shot, detail is : [" + e.Message + "]. " +
                    "The screen shot operatioin was ignored. ");
            }
        }
	
	    private static Screenshot getScreenShotFile() {
		    return driver.GetScreenshot();
	    }
	
	    private static String prepareCaseFile() {
		    String caseFolder = prepareCaseFolder();
		    return caseFolder + getDateTimeStampString() + ".png";
	    }
	
	    private static String prepareCaseFolder() {
            String caseFolderStr = imgPathStr + "/" + getDateTimeStampString() + "/";
		    if (!Directory.Exists(caseFolderStr)) {
			    Directory.CreateDirectory(caseFolderStr);
		    }
		    return caseFolderStr;
	    }

	    public static String getTodayString(){
            return System.DateTime.Now.ToString("yyyyMMdd");
	    }
	
	    public static String getStandardDateStr(DateTime date){
		    return getDatetimeStr(date, "M/d/yyyy");
	    }
	
	    public static String getDatetimeStr(DateTime date, String datetimeFmt){
            return date.ToString(datetimeFmt);
	    }
	
	    /**
	     * @param datetimeStr for example: 3/28/2014
	     * */
	    public static DateTime parseStandardDatetime(String datetimeStr){
            return DateTime.ParseExact(datetimeStr, "M/d/yyyy HH:mm a", CultureInfo.InvariantCulture);
		}
	
	    public static DateTime parseDatetime(String datetimeStr, String datetimeFmt){
		    return DateTime.ParseExact(datetimeStr, datetimeFmt, CultureInfo.InvariantCulture);
	    }
	
	    public static String getNowString(){
		    return System.DateTime.Now.ToString("yyyyMMddHHmmss");
	    }
	
	    public static String getDateTimeStampString(){
		    return System.DateTime.Now.ToString("yyyyMMddHHmmssSSS");
	    }
	
	    public static String getRandomIntOfToday(int max){
            return getDatetimeStr(DateTime.Now, "yyyyMMdd-") + getRandomInt(max);
	    } 
	
	    /**
	     * Get an random number from 1 to max(max included)
	     * */
	    public static int getRandomInt(int max){
		    if(max < 1){
			    throw new Exception("Parameter max less than 1. ");
		    }
		    return getRandomInt(1, max);
	    }
	
	    /**
	     * Get an random number from min to max(max included)
	     * */
	    public static int getRandomInt(int min, int max){
            return min + new Random().Next() % max;
	    }
	
	    /**
	     * Given a record number, and how many sample you need, return a set of record index.
	     * */
	    public static List<int> getSampleIndexes(int recordCount, int sampleNumber){
		    if(recordCount <= 0){
			    throw new ArgumentException("Invalid recordNumber number, please give a value bigger than 0. ");
		    }
		
		    if(sampleNumber <= 0){
			    throw new ArgumentException("Invalid sample number, please give a value bigger than 0. ");
		    }
		
		    double step = recordCount * 1.0 / sampleNumber;
		    List<int> sampleRtn = new List<int>();
		    for(double i=0; i<recordCount; i += step){
			    int sampleIndex = (int)(Math.Round(i));
			    if(sampleIndex < recordCount){
				    sampleRtn.Add(sampleIndex);
			    }
		    }
		    return sampleRtn;
	    }
	
	    public static List<String> getSampleTexts(By listBy, int sampleNumber){
		    List<String> rtn = new List<String>();
		    List<IWebElement> list = getSampleList(findElements(listBy), sampleNumber);
		    foreach(IWebElement loopEle in list){
			    rtn.Add(loopEle.Text.Trim());
		    }
		    return rtn;
	    }
	    
        private static List<T> getSampleList<T>(List<T> records, int sampleNumber){
		    List<T> rtn = new List<T>();
		    List<int> sampleIndexes = getSampleIndexes(records.Count, sampleNumber);
		    foreach(int i in sampleIndexes){
			    rtn.Add(records.ElementAt(i));
		    }
		    return rtn;
	    }

	    /**
	     * @param recordByFmt like .//*[@id='projectdatacenters']//table[@class='flexprojectdatacenter']/tbody/tr/td[4]/div[text()='%s']/../..
	     * */
	    public static List<IWebElement> getRecords(String recordByFmt, Object[] keywords){
		    By recordBy = By.XPath(String.Format(recordByFmt, keywords));
		    return findElements(recordBy);
	    }
	
	    /**
	     * @param recordByFmt like .//*[@id='projectdatacenters']//table[@class='flexprojectdatacenter']/tbody/tr/td[4]/div[text()='%s']/../..
	     * */
	    public static IWebElement getRecord(String recordByFmt, Object[] keywords){
		    List<IWebElement> records = getRecords(recordByFmt, keywords);
		    if(records.Count > 0){
			    return records.ElementAt(0);
		    } 
		    return null;
	    }
	
	    /**
	     * @param recordByFmt like .//*[@id='projectdatacenters']//table[@class='flexprojectdatacenter']/tbody/tr/td[4]/div[text()='%s']/../..
	     * */
	    public static IWebElement findRecord(String recordName, String recordByFmt, String keywordName, Object[] keywords){
		    IWebElement record = getRecord(recordByFmt, keywords);
		    if(record != null){
			    return record;
		    }
		
		    throw new InvalidOperationException("Can't find any " + recordName + " record with " 
			    + keywordName + (keywords.Length > 0 ? " [" + keywords + "]. " : ". "));
	    }
	
	    /**
	     * @param recordByFmt must be like: './/*[@id='projectTeamResources']//tr[%s]'
	     * */
	    public static IWebElement findRecordByIndex(String recordByFmt, int recordIndex){
		    String recordByStr = String.Format(recordByFmt, recordIndex + 1);
		    return SeleniumUtil.findElement(By.XPath(recordByStr));
	    }

	    public static Boolean acceptAlertIfAny(String actionShortDesc, String alertPartialWords, int timeout) {
		     return acceptAlertIfAny(actionShortDesc, new String[]{alertPartialWords}, timeout, false);
	    }
	
	    public static void acceptAlert(String actionShortDesc, String alertPartialWords, int timeout) {
		     acceptAlertIfAny(actionShortDesc, new String[]{alertPartialWords}, timeout, true);
	    }
	
	    public static Boolean acceptAlertIfAny(String actionShortDesc,
			    String[] alertPartialWords, int timeout)
	    {
		    return acceptAlertIfAny(actionShortDesc, alertPartialWords, timeout, false);
	    }
	
	    public static void acceptAlert(String actionShortDesc, String[] alertPartialWords, 
            int timeout)
	    {
		    acceptAlertIfAny(actionShortDesc, alertPartialWords, timeout, true);
	    }
	
	    private static Boolean acceptAlertIfAny(String actionShortDesc,
			    String[] alertPartialWords, int timeout, Boolean throwIfNoAlertAtAll)
	    {
		    IAlert normalSaveAlert = null;
		    try {
			    normalSaveAlert = SeleniumUtil.switchToPromptedAlert(timeout);
			    String alertText = normalSaveAlert.Text;
			    System.Console.WriteLine(alertText);
			    SeleniumUtil.waitForJsPerceivableSeconds();
			    Boolean matched = false;
			    foreach(String loopStr in alertPartialWords){
				    if(alertText.Contains(loopStr)){
					    matched = true;
					    break;
				    }
			    }
			    if(matched){
				    normalSaveAlert.Accept();
				    return true;
			    } else {
				    normalSaveAlert.Dismiss();
				    throw new InvalidOperationException("Unexpected alert window prompted out while " + actionShortDesc + ", " +
					    "saying: [" + alertText + "]. Please ask test script writer to handle this. ");
			    }
		    } catch (NoAlertPresentException e){
			    if(throwIfNoAlertAtAll){
				    throw new NoAlertPresentException("While " + actionShortDesc + ", expected alert with words " + 
					    alertPartialWords + " was not found within " + (timeout/1000) + " seconds. ");
			    }
		    }
		    return false;
	    }
	
	    public static Boolean isInGridMode(){
		    return inGridMode;
	    }
	
	    /**
	     * @param lineIndex start from 0
	     * */
	    /*
        public static String getLine(String text, int lineIndex)
	    {
		    String rtn = null;
		    BufferedReader r = new BufferedReader(new StringReader(text));
		    int i = 0;
		    try {
			    String line = null;
			    while((line = r.readLine()) != null){
				    if(lineIndex == i){
					    rtn = line;
				    }
				    i++;
			    }
		    } catch (IOException e) {
			    throw new Exception("Error occurred when getting the " + (lineIndex + 1) + 
				    "th line of text [" + text + "]. ");
		    } finally {
			    try {
				    r.close();
			    } catch (Exception e){}
		    }
		    if(rtn != null){
			    return rtn;
		    }
		
		    throw new InvalidOperationException("Failed to get the " + (lineIndex+1) + "th line of text [" + text + "]. "
			    + "Please ask test script writer to fix this issue. ");
	    }
	    */

        /*
	    public static void assertImgValid(String message, IWebElement imgElement) {
		    String src = imgElement.GetAttribute("src");
		    int maxMilliseconds = 30000;
		    long startTime = System.DateTime.Now.Millisecond;
		    try{
			    URLConnection conn = new URL(src).openConnection();
			    conn.setConnectTimeout(maxMilliseconds);
			    conn.setReadTimeout(maxMilliseconds);
			    conn.getInputStream();
		    } catch (MalformedURLException e){
			    throw new InvalidOperationException(message + " Image url is :[" + src + "]. Image Url format is not correct.");
		    } catch(UnknownHostException e){
			    throw new InvalidOperationException(message + " Image url is :[" + src + "]. The host name is invalid.");
		    } catch (FileNotFoundException e){
			    throw new InvalidOperationException(message + " Image url is :[" + src + "]. Can't find image at this Url.");
		    } catch (SocketTimeoutException e){
			    throw new InvalidOperationException(message + " Failed to find image within " + 
				    ((System.DateTime.Now.Millisecond - startTime)/1000) + " seconds");
		    } catch (IOException e){
			    System.Console.WriteLine("Verify image: [" + src + "] end at [" + new SimpleDateFormat("HH:mm:ss").format(new Date()) + "] failed.");
			    throw new InvalidOperationException(message + " IO Exception happened when finding the image.");
		    }
	    }
	
	    public static String getFileContent(InputStream in) {
		    BufferedReader br = new BufferedReader(new InputStreamReader(in));
		    // no need close the input stream
		    return getFileContent(br);
	    }
	
	    public static String getFileContent(File file){
		    BufferedReader br = null;
		    try {
			    br = new BufferedReader(new FileReader(file));
			    return getFileContent(br);
		    } catch (FileNotFoundException e) {
			    throw new RuntimeException(e);
		    } finally {
			    try {
				    br.close();
			    } catch (Exception e) { }
		    }
	    }
	
	    private static String getFileContent(BufferedReader br){
		    StringBuffer bf = new StringBuffer("");
		
		    try {
			    char[] ch = new char[4096];
			    int readCount = -1;
			    while((readCount = br.read(ch)) > 0){
				    bf.append(ch, 0, readCount);
			    }
		    } catch (FileNotFoundException e) {
			    throw new RuntimeException(e);
		    } catch (IOException e) {
			    throw new RuntimeException(e);
		    }
		
		    return bf.toString();
	    }
	
	    public static void writeContent(File outputFile, String content){
		    BufferedWriter writer = null;
		    try {
			    writer = new BufferedWriter(new FileWriter(outputFile));
			    writer.write(content);
		    } catch (IOException e) {
			    throw new RuntimeException(e);
		    } finally {
			    try {
				    writer.close();
			    } catch (Exception e) { }
		    }
	    }
	    */
	    /** 
	     * get method name which calls this getStackMethodName(int) method
	     */
	   
	    /*
	     * Since this NON-parm version getStackMethodName call another version of
	     * getStackMethodName(int),so go one step further to get the real method
	     * which calls this method
	     
	    public static String getMethodName() {
		    return getStackMethodName(1);
	    }*/
	
    }

}
