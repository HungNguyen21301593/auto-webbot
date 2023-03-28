using Core.Model;

namespace WebApi.ViewModels
{
    public class LogTreeNode
    {
        public string Name { get; set; }
        public LogTreeStatus Status { get; set; }
        public DateTime Created { get; set; }
        public List<LogTreeNode> Children { get; set; }
    }
}
