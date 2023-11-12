using Thss0.Web.Models.ViewModels;

namespace Thss0.Web.Models
{
    public class Response
    {
        public IEnumerable<ViewModel?> Content { get; set; } = Enumerable.Empty<ViewModel>();
        public int TotalAmount { get; set; }
    }
}
