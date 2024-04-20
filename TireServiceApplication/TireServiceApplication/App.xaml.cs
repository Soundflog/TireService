using TireServiceApplication.Source.Pages.Others;

namespace TireServiceApplication;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		var navPage = new AuthenticationPage();
        MainPage = navPage;
        UserAppTheme = AppTheme.Light;
	}
	
	// Основная страница
	public static void SetMainPage()
	{
		var navPage = new MainPage();
		if (Current != null) Current.MainPage = navPage;
	}
	
	// Страница авторизации
	public static void SetAuthenticationPage()
	{
		var navPage = new AuthenticationPage();
		if (Current != null) Current.MainPage = navPage;
	}
}
