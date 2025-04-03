using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;

namespace PlaywrightTests;

[TestClass]
public class DemoTest : PageTest
{
    private IPlaywright _playwright;
    private IBrowser _browser;
    private IBrowserContext _browserContext;
    private IPage _page;

    [TestInitialize]
    public async Task Setup()
    {
        _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
            SlowMo = 1000 // Lägger in en fördröjning så vi kan se vad som händer
        });
        _browserContext = await _browser.NewContextAsync();
        _page = await _browserContext.NewPageAsync();
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        await _browserContext.CloseAsync();
        await _browser.CloseAsync();
        _playwright.Dispose();
    }

    [TestMethod]
    public async Task GivenIAmOnHomePage()
    {
        await _page.GotoAsync("http://localhost:5000/");
    }

    [TestMethod]
    public async Task GivenIAmLoggedInAsHuggo()
    {
        GivenIAmOnHomePage();
        // Vänta tills sidan har laddats klart
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        // Klicka på knappen "Logga in"
        await _page.GetByRole(AriaRole.Button, new() { Name = "Login" }).ClickAsync();

        // Vänta tills sidan har laddats klart
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        // Fyll i användarnamn och lösenord
        await _page.GetByLabel("Email").FillAsync("Hej@gmail.com");
        await _page.GetByLabel("Password").FillAsync("abc123");

        // Klicka på knappen "Logga in"
        await _page.GetByRole(AriaRole.Button, new() { Name = "Submit" }).ClickAsync();

        // Vänta tills sidan har laddats klart
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        // Kontrollera att vi är inloggade
        var loggedInText = await _page.GetByText("Welcome, Huggo").InnerTextAsync();
        Assert.IsTrue(loggedInText.Contains("Welcome, Huggo"));
    }

    [TestMethod]
    public async Task GetShopLink()
    {
        GivenIAmOnHomePage();
        // Vänta tills sidan har laddats klart
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        // Klicka på länken till "Shop"
        await _page.GetByRole(AriaRole.Link, new() { Name = "Shop" }).ClickAsync();

        // Kontrollera att knappen "Electronics" finns på sidan
        await Expect(_page.GetByRole(AriaRole.Button, new() { Name = "Electronics" })).ToBeVisibleAsync();
    }
} //Hej