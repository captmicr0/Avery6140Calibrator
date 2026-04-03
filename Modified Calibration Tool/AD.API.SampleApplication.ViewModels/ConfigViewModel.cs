namespace AD.API.SampleApplication.ViewModels;

internal class ConfigViewModel : BaseViewModel
{
	public override void ValidateSelf()
	{
		base.ValidateSelf();
		OnPropertyChanged(string.Empty);
	}
}
