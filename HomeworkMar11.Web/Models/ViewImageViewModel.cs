using HomeworkMar11.Data;

namespace HomeworkMar11.Web.Models
{
    public class ViewImageViewModel
    {
        public Image Image { get; set; }
        public List<int> ImageIDs { get; set; }
        public bool IncorrectPassword { get; set; }
    }
}
