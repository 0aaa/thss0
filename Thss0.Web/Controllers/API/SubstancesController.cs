using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Thss0.Web.Config;
using Thss0.Web.Data;
using Thss0.Web.Models.ViewModels;

namespace Thss0.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubstancesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _client;
        public SubstancesController(ApplicationDbContext context)
        {
            _context = context;
            _client = new HttpClient();
        }

        [HttpGet("{order:bool?}/{printBy:int?}/{page:int?}")]
        public async Task<ActionResult<IEnumerable<SubstanceViewModel>>> GetSubstances(bool order = true, int printBy = 20, int page = 1)
        {
            var content = new List<SubstanceViewModel>();
            JObject resJson;
            resJson = await HandleApi("", true, order, printBy, page);
            for (int i = 0; i < printBy; i++)
            {
                content.Add(new SubstanceViewModel
                {
                    Id = resJson["results"]?[i]?["product_id"]?.ToString()!
                    , Name = resJson["results"]?[i]?["brand_name"]?.ToString()!
                    , GenericName = resJson["results"]?[i]?["generic_name"]?.ToString()!
                    , ListingExpirationDate = DateTime.ParseExact(resJson["results"]?[0]?["listing_expiration_date"]?.ToString()!, "yyyyMMdd", null).ToShortDateString()
                    , MarketingCategory = resJson["results"]?[i]?["marketing_category"]?.ToString()!
                    , DosageForm = resJson["results"]?[i]?["dosage_form"]?.ToString()!
                    , ProductType = resJson["results"]?[i]?["product_type"]?.ToString()!
                    , MarketingStartDate = DateTime.ParseExact(resJson["results"]?[0]?["marketing_start_date"]?.ToString()!, "yyyyMMdd", null).ToShortDateString()
                });
            }
            return Json(new
            {
                content
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SubstanceViewModel>> GetSubstance(string id, bool isId = true)
        {
            var resJson = await HandleApi(id, isId);
            var drug = new SubstanceViewModel
            {
                Id = resJson["results"]?[0]?["product_id"]?.ToString()!
                , Name = resJson["results"]?[0]?["brand_name"]?.ToString()!
                , GenericName = resJson["results"]?[0]?["generic_name"]?.ToString()!
                , ListingExpirationDate = DateTime.ParseExact(resJson["results"]?[0]?["listing_expiration_date"]?.ToString()!, "yyyyMMdd", null).ToShortDateString()
                , MarketingCategory = resJson["results"]?[0]?["marketing_category"]?.ToString()!
                , DosageForm = resJson["results"]?[0]?["dosage_form"]?.ToString()!
                , ProductType = resJson["results"]?[0]?["product_type"]?.ToString()!
                , MarketingStartDate = DateTime.ParseExact(resJson["results"]?[0]?["marketing_start_date"]?.ToString()!, "yyyyMMdd", null).ToShortDateString()
            };
            if (drug == null)
            {
                return NoContent();
            }
            return drug;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSubstance(string id)
        {
            var procedure = await _context.Procedures.FirstOrDefaultAsync(p => p.Substance.Any(s => s.Id == id));
            if (procedure == null)
            {
                return NotFound();
            }
            var substance = procedure.Substance.FirstOrDefault();
            if (substance == null)
            {
                return NotFound();
            }
            procedure.Substance.Remove(substance);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return NoContent();
        }

        private async Task<JObject> HandleApi(string identifier = "", bool isId = true, bool order = true, int printBy = 3, int page = 1)
        {
            var requestStr = $"https://api.fda.gov/drug/ndc.json?api_key={AuthCredentials.SUBSTANCES_API_KEY}";
            if (identifier != "")
            {
                requestStr += $"&search={(isId ? "product_id" : "brand_name")}:{identifier}";
            }
            else
            {
                requestStr += $"&skip={(page - 1) * printBy}&limit={printBy}";
            }
            try
            {
                var res = await _client.GetStringAsync(requestStr);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return JObject.Parse(await _client.GetStringAsync(requestStr) ?? "No content");
        }
    }
}