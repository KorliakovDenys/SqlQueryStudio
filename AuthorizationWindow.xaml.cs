using System.Windows;

namespace SqlQueryStudio;

public partial class AuthorizationWindow : Window{
    public AuthorizationWindow(AuthorizationWindowView windowView){
        InitializeComponent();
        DataContext = windowView;
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e){
        DialogResult = true;
        this.Close();
    }
}