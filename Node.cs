using Prism.Commands;
using System.Collections.ObjectModel;

namespace SqlQueryStudio{
    public class Node{
        public Node Parent{ get; set; }
        public string Name{ get; set; }
        public ObservableCollection<Node> Nodes{ get; set; }
        public DelegateCommand<Node> Command{ get; set; }
    }
}