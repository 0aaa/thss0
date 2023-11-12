using System.Text.Json.Serialization;

namespace OpenAI
{
    public struct Message
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }
        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
    public struct Choice
    {
        [JsonPropertyName("message")]
        public Message Message { get; set; }
        public int Index { get; set; }
        public object LogProbs { get; set; }
        public string FinishReason { get; set; }
    }
    public struct Root
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public int Created { get; set; }
        public string Model { get; set; }
        [JsonPropertyName("choices")]
        public List<Choice> Choices { get; set; }
        public Usage Usage { get; set; }
    }
    public struct Usage
    {
        public int PromptTokens { get; set; }
        public int CompletionTokens { get; set; }
        public int TotalTokens { get; set; }
    }
    public struct OpenAIRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; }
        [JsonPropertyName("messages")]
        public Message[] Messages { get; set; }
    }
    public struct OpenAIErrorResponse
    {
        [JsonPropertyName("error")]
        public OpenAIError Error { get; set; }
    }
    public struct OpenAIError
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("param")]
        public string Param { get; set; }
        [JsonPropertyName("code")]
        public string Code { get; set; }
    }
}