using AngleSharp;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace SamishopV2_Template_1.Controllers
{
    public class ErrorsController : Controller
    {
        private readonly IConfiguration configurationSetting;
        public ErrorsController(IConfiguration configuration)
        {
            configurationSetting = configuration;
        }
        [Route("/{**catchAll}")]
        public async Task<IActionResult> Error404(string catchAll)
        {
            try
            {
                var UrlGoogleStorage = configurationSetting["UrlCdnStorage"];

                string hostFolderClient = Request.Headers.Host;
                hostFolderClient = hostFolderClient.ToLower();
                hostFolderClient = hostFolderClient.Replace("/", "");
                hostFolderClient = hostFolderClient.Replace("www.", "");

                var UrlCdnClient = UrlGoogleStorage + "/" + hostFolderClient;

                var config = Configuration.Default.WithDefaultLoader();
                var context = BrowsingContext.New(config);

                var documentFather = await context.OpenAsync(UrlCdnClient + "/" + "template" + ".html");
                var htmlFather = documentFather.Source.Text;

                var documentError = await context.OpenAsync(UrlCdnClient + "/" + "error" + ".html");
                var htmlError = documentError.Source.Text;
                htmlFather = htmlFather.Replace("[[HTML_CONTENT]]", htmlError);

                return new ContentResult
                {
                    ContentType = "text/html",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = htmlFather
                };
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
