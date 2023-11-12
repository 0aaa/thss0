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
        [HttpGet("{userInput}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, professional")]
        public async Task<string?> GetGeneration(string userInput)
        {
            var apiKey = "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";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            try
            {
                var response = await client.PostAsync("https://api.openai.com/v1/chat/completions"
                    , new StringContent(JsonSerializer.Serialize(new OpenAIRequest
                        {
                            Model = "gpt-3.5-turbo"
                            , Messages = new Message[] { new() { Role = "user", Content = userInput } }
                        })
                    , Encoding.UTF8
                    , "application/json"));
                var resjson = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(JsonSerializer.Deserialize<OpenAIErrorResponse>(resjson).Error.Message);
                }
                return JsonSerializer.Deserialize<Root>(resjson).Choices[0].Message.Content;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
