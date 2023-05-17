using System.Windows;

namespace SqlQueryStudio{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window{
        private readonly WindowView _windowView = new ();
        public MainWindow(){
            InitializeComponent();
            DataContext = _windowView;
        }
    }
}