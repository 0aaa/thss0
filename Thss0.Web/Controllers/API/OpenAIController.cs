using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenAI;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Thss0.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpenAIController
    {
        [HttpGet("{input}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, professional")]
        public async Task<string?> GetGeneration(string input)
        {
            var key = "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";
            var hc = new HttpClient();
            hc.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);
            try
            {
                var res = await hc.PostAsync("https://api.openai.com/v1/chat/completions"
                    , new StringContent(JsonSerializer.Serialize(new OpenAIRequest
                        {
                            Model = "gpt-3.5-turbo"
                            , Messages = [new() { Role = "user", Content = input }]
                        })
                    , Encoding.UTF8
                    , "application/json"));
                var json = await res.Content.ReadAsStringAsync();
                if (!res.IsSuccessStatusCode)
                {
                    throw new Exception(JsonSerializer.Deserialize<OpenAIErrorResponse>(json).Error.Message);
                }
                return JsonSerializer.Deserialize<Root>(json).Choices[0].Message.Content;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}