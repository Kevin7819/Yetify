using api.Data;
using api.Dtos.Activity;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using api.Constants;
using HtmlAgilityPack;
using Microsoft.Playwright;


namespace api.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public ActivityController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet("allActivitiesByUser/{userid}")]
        public async Task<IActionResult> GetAllActivitiesByUser([FromRoute] int userid)
        {
            var activities = await _context.Activities.Where(act => act.userId == userid).ToListAsync();
            if (!activities.Any())
            {
                return NotFound(new { message = ActivityConstants.NoActivitiesRegistered });
            }
            var activitiesDto = activities.Select(actv => actv.toDto());
            return Ok(activitiesDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetActivityById([FromRoute] int id)
        {
            var activity = await _context.Activities.FirstOrDefaultAsync(actv => actv.id == id);
            if (activity == null)
                return NotFound(new
                {
                    error = MessageConstants.EntityNotFound("La actividad"),
                    suggestion = MessageConstants.Generic.TryAgain
                });
            return Ok(activity.toDto());
        }

        [HttpPost]
        public async Task<IActionResult> CreateActivity([FromBody] CreateActivityRequestDto activityRequestDto)
        {
            if (activityRequestDto == null)
                return BadRequest(new { message = MessageConstants.Generic.RequiredFields });

            if (string.IsNullOrWhiteSpace(activityRequestDto.activityName))
                return BadRequest(new { message = ActivityConstants.NameRequired });

            if (string.IsNullOrWhiteSpace(activityRequestDto.activityType))
                return BadRequest(new { message = ActivityConstants.TypeRequired });

            if (activityRequestDto.identifyActivity == 0)
                return BadRequest(new { message = ActivityConstants.IdentifyRequired });

            if (activityRequestDto.creationDate <= DateTime.Now)
                return BadRequest(new { message = ActivityConstants.InvalidDate });

            if (string.IsNullOrWhiteSpace(activityRequestDto.activityStatus))
                return BadRequest(new { message = ActivityConstants.StatusRequired });

            if (string.IsNullOrWhiteSpace(activityRequestDto.urlSources))
                return BadRequest(new { message = ActivityConstants.ApiSourceRequired });

            var activity = activityRequestDto.ToActivityFromCreateDto();
            await _context.Activities.AddAsync(activity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetActivityById), new { id = activity.id },
                new
                {
                    message = MessageConstants.EntityCreated("La actividad"),
                    data = activity.toDto()
                });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateActivity([FromRoute] int id, [FromBody] UpdateActivityRequestDto updateActivityRequest)
        {
            if (updateActivityRequest == null)
                return BadRequest(new { message = MessageConstants.Generic.RequiredFields });

            if (string.IsNullOrWhiteSpace(updateActivityRequest.activityStatus))
                return BadRequest(new { message = ActivityConstants.StatusRequired });

            var activity = await _context.Activities.FirstOrDefaultAsync(act => act.id == id);
            if (activity == null)
            {
                return NotFound(new
                {
                    error = MessageConstants.EntityNotFound("La actividad"),
                    suggestion = MessageConstants.Generic.TryAgain
                });
            }

            activity.ToActivityFromUpdateDto(updateActivityRequest);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = MessageConstants.EntityUpdated("La actividad"),
                data = activity.toDto()
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity([FromRoute] int id)
        {
            var activity = await _context.Activities.FirstOrDefaultAsync(act => act.id == id);
            if (activity == null)
                return NotFound(new
                {
                    error = MessageConstants.EntityNotFound("La actividad"),
                    suggestion = MessageConstants.Generic.TryAgain
                });

            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = MessageConstants.EntityDeleted("La actividad")
            });
        }

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