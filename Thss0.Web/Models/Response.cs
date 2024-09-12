using Thss0.Web.Models.ViewModels;

namespace Thss0.Web.Models
{
    public class Response
    {
        public IEnumerable<ViewModel?> Content { get; set; } = [];
        public int TotalAmount { get; set; }
    }
}