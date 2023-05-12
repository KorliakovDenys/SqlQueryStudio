using System.Windows;

namespace SqlQueryStudio{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window{
        private readonly ViewModel _viewModel = new ();
        public MainWindow(){
            InitializeComponent();
            DataContext = _viewModel;
        }
    }
}