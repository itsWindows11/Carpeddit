namespace Carpeddit.App.ViewModels
{
    public class SettingsViewModel : SettingsManager
    {
        public SettingsViewModel() { }

        #region Appearance
        public bool ShowAccountBtnInTitleBar
        {
            get => Get("Appearance", nameof(ShowAccountBtnInTitleBar), true);
            set => Set("Appearance", nameof(ShowAccountBtnInTitleBar), value);
        }
        #endregion
    }
}