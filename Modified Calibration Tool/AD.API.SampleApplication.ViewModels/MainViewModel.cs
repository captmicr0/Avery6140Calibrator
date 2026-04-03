namespace AD.API.SampleApplication.ViewModels;

internal class MainViewModel : BaseViewModel
{
	public bool IsSystemEnabled => base.DeviceConnected;

	public override void ValidateSelf()
	{
		base.ValidateSelf();
		OnPropertyChanged(string.Empty);
	}
}
