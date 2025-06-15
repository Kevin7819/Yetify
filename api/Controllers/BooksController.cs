using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using api.Constants;
using HtmlAgilityPack;
using Microsoft.Playwright;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {

        [HttpGet("CategoryBooks")]
        public async Task<IActionResult> GetCategoriesBooks()
        {
            try
            {
                var listCategoriesBooks = new List<Dictionary<string, string>>();
                using var playwright = await Playwright.CreateAsync();
                var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
                var page = await browser.NewPageAsync();
                var baseCategorieUrl = ActivityConstants.baseUrlSources + "/cuentos-infantiles-cortos";

                await page.RouteAsync("**/*", async route =>
                {
                    var resourceType = route.Request.ResourceType;
                    if (resourceType is "image" or "stylesheet" or "font" or "media" or "other")
                        await route.AbortAsync();
                    else
                        await route.ContinueAsync();
                });
                await page.GotoAsync(baseCategorieUrl, new PageGotoOptions { WaitUntil = WaitUntilState.Load, Timeout = 40000 });
                await page.WaitForSelectorAsync("div.col-xl-3", new PageWaitForSelectorOptions { State = WaitForSelectorState.Attached });

                for (int i = 0; i < 5; i++)
                {
                    await page.EvaluateAsync(@"() => window.scrollBy(0, window.innerHeight)");
                    await page.WaitForTimeoutAsync(500); //wait for imgs
                }

                var pageContent = await page.ContentAsync();

                var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(pageContent);
                var nodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'col-xl-3') and contains(@class, 'col-lg-4') and contains(@class, 'col-md-4') and contains(@class, 'col-sm-6') and contains(@class, 'col-6') and contains(@class, 'display-div')]");

                if (nodes == null)
                    return Ok();

                foreach (var nodo in nodes)
                {
                    var linkNode = nodo.SelectSingleNode(".//a[contains(@class, 'no-underline')]");
                    var imgNode = linkNode?.SelectSingleNode(".//img[contains(@class, 'img-fluid') and contains(@class, 'landing-icon-img')]");
                    var titleNode = nodo.SelectSingleNode(".//h3[contains(@class, 'icon-txt')]");

                    var href = linkNode?.GetAttributeValue("href", "") ?? "";
                    var src = imgNode?.GetAttributeValue("src", "") ?? "";
                    var title = titleNode?.InnerText.Trim() ?? "";

                    var item = new Dictionary<string, string>
                    {
                        { "url",href },
                        { "image", src },
                        { "category", title }
                    };
                    listCategoriesBooks.Add(item);
                }
                await browser.CloseAsync();
                return Ok(listCategoriesBooks);
            }
            catch (TimeoutException tex)
            {
                return StatusCode(504, new { error = "La página tardó demasiado en responder.", detail = tex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ocurrió un error al obtener los libros.", detail = ex.Message });
            }

        }
        
        [HttpGet("BooksByCategory")]
        public async Task<IActionResult> GetBooks([FromQuery] string category)
        {
            try
            {
                var listBooks = new List<Dictionary<string, string>>();
                using var playwright = await Playwright.CreateAsync();
                var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
                var page = await browser.NewPageAsync();

                var urlBooksByCategory = ActivityConstants.baseUrlSources + category;
                Console.WriteLine(urlBooksByCategory);

                await page.RouteAsync("**/*", async route =>
                {
                    var resourceType = route.Request.ResourceType;
                    if (resourceType is "image" or "stylesheet" or "font" or "media" or "other")
                        await route.AbortAsync();
                    else
                        await route.ContinueAsync();
                });


                await page.GotoAsync(urlBooksByCategory, new PageGotoOptions { WaitUntil = WaitUntilState.Load, Timeout = 40000 });
                await page.WaitForSelectorAsync("div.col-xl-3", new PageWaitForSelectorOptions { State = WaitForSelectorState.Attached });

                for (int i = 0; i < 5; i++)
                {
                    await page.EvaluateAsync(@"() => window.scrollBy(0, window.innerHeight)");
                    await page.WaitForTimeoutAsync(500);
                }
                var pageContent = await page.ContentAsync();

                var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(pageContent);
                var nodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'col-xl-3') and contains(@class, 'col-lg-3') and contains(@class, 'col-md-4') and contains(@class, 'col-6')]");

                if (nodes == null)
                    return Ok();

                foreach (var nodo in nodes)
                {
                    var linkNode = nodo.SelectSingleNode(".//a[contains(@class, 'thumbnail-item-link')]");
                    var imgNode = linkNode?.SelectSingleNode(".//img[contains(@class, 'thumb-img')]");
                    
                    var href = linkNode?.GetAttributeValue("href", "") ?? "";
                    var src = imgNode?.GetAttributeValue("src", "") ?? "";

                    var item = new Dictionary<string, string>{
                        { "url", ActivityConstants.baseUrlSources + href },
                        { "image", src }
                    };
                    listBooks.Add(item);
                }
                await browser.CloseAsync();
                return Ok(listBooks);
            }
            catch (TimeoutException tex)
            {
                return StatusCode(504, new { error = "La página tardó demasiado en responder.", detail = tex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ocurrió un error al obtener los libros.", detail = ex.Message });
            }
        }

    }
}