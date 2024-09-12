using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Thss0.Web.Config;
using Thss0.Web.Data;
using Thss0.Web.Models;
using Thss0.Web.Models.ViewModels;

namespace Thss0.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubstanceController(ApplicationDbContext c) : Controller
    {
        private readonly HttpClient _client = new();

        [HttpGet("{printBy:int?}/{page:int?}/{order:bool?}/{toFind?}")]
        public async Task<ActionResult<Response>> Get(int printBy = 20, int page = 1, bool order = true, string toFind = "")
        {
            var content = new List<SubstanceViewModel>();
            JObject res;
            res = await HandleApi("", true, order, printBy, page);
            for (int i = 0; i < printBy; i++)
            {
                content.Add(new SubstanceViewModel
                {
                    Id = res["results"]?[i]?["product_id"]?.ToString()!
                    , Name = res["results"]?[i]?["brand_name"]?.ToString()!
                    , GenericName = res["results"]?[i]?["generic_name"]?.ToString()!
                    , ListingExpirationDate = DateTime.ParseExact(res["results"]?[0]?["listing_expiration_date"]?.ToString()!, "yyyyMMdd", null).ToShortDateString()
                    , MarketingCategory = res["results"]?[i]?["marketing_category"]?.ToString()!
                    , DosageForm = res["results"]?[i]?["dosage_form"]?.ToString()!
                    , ProductType = res["results"]?[i]?["product_type"]?.ToString()!
                    , MarketingStartDate = DateTime.ParseExact(res["results"]?[0]?["marketing_start_date"]?.ToString()!, "yyyyMMdd", null).ToShortDateString()
                });
            }
            return Json(new Response
            {
                Content = content
                , TotalAmount = content.Count
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SubstanceViewModel>> Get(string id, bool isId = true)
        {
            var res = await HandleApi(id, isId);
            if (res["content"]?.ToString() == "No content")
            {
                return NoContent();
            }
            var drug = new SubstanceViewModel
            {
                Id = res["results"]?[0]?["product_id"]?.ToString()!
                , Name = res["results"]?[0]?["brand_name"]?.ToString()!
                , GenericName = res["results"]?[0]?["generic_name"]?.ToString()!
                , ListingExpirationDate = DateTime.ParseExact(res["results"]?[0]?["listing_expiration_date"]?.ToString()!, "yyyyMMdd", null).ToShortDateString()
                , MarketingCategory = res["results"]?[0]?["marketing_category"]?.ToString()!
                , DosageForm = res["results"]?[0]?["dosage_form"]?.ToString()!
                , ProductType = res["results"]?[0]?["product_type"]?.ToString()!
                , MarketingStartDate = DateTime.ParseExact(res["results"]?[0]?["marketing_start_date"]?.ToString()!, "yyyyMMdd", null).ToShortDateString()
            };
            return drug;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var p = await c.Procedures.FirstOrDefaultAsync(p => p.Substance.Any(s => s.Id == id));
            if (p == null)
            {
                return NotFound();
            }
            var s = p.Substance.FirstOrDefault();
            if (s == null)
            {
                return NotFound();
            }
            p.Substance.Remove(s);
            try
            {
                await c.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return NoContent();
        }

        private async Task<JObject> HandleApi(string identifier = "", bool isId = true, bool order = true, int printBy = 3, int page = 1)
        {
            var req = $"https://api.fda.gov/drug/ndc.json?api_key={AuthCredentials.SUBSTANCES_API_KEY}";
            string res;
            if (identifier != "")
            {
                req += $"&search={(isId ? "product_id" : "brand_name")}:{identifier}";
            }
            else
            {
                req += $"&skip={(page - 1) * printBy}&limit={printBy}";
            }
            try
            {
                res = await _client.GetStringAsync(req);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return JObject.FromObject(new { content = "No content" });
            }
            return JObject.Parse(res);
        }
    }
}