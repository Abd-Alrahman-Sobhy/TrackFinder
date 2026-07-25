using System.ComponentModel.DataAnnotations;

namespace TrackFinder.ViewModels.Settings
{
    public class SettingsVM
    {
        [Required]
        public string Key { get; set; } = string.Empty;
        [Required]
        public string Value { get; set; } = string.Empty;
    }

    public class SettingListVM
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public class SettingDetailsVM
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    // ViewModel-suffixed aliases for Razor Views
    public class SettingsViewModel : SettingsVM { }
    public class SettingListViewModel : SettingListVM { }
    public class SettingDetailsViewModel : SettingDetailsVM { }
}
