using Microsoft.Extensions.Localization;
using MudBlazor;

namespace RadiologyCenter.Desktop.Services;

public sealed class AppMudLocalizer : MudLocalizer
{
    private readonly AppLocalizer _localizer;

    public AppMudLocalizer(AppLocalizer localizer) => _localizer = localizer;

    public override LocalizedString this[string key] => new(key, GetValue(key));

    public override LocalizedString this[string key, params object[] arguments]
    {
        get
        {
            var value = GetValue(key);
            if (arguments.Length > 0)
            {
                try
                {
                    value = string.Format(value, arguments);
                }
                catch (FormatException)
                {
                }
            }

            return new LocalizedString(key, value);
        }
    }

    private string GetValue(string key)
        => key switch
        {
            "MudDataGridPager_RowsPerPage" => _localizer.Get("mud.tablePager.rowsPerPage"),
            "MudDataGridPager_AllItems" => _localizer.Get("mud.tablePager.all"),
            "MudDataGridPager_InfoFormat" => _localizer.Get("mud.tablePager.infoFormat"),
            "MudTablePager_FirstPage" => _localizer.Get("mud.tablePager.first"),
            "MudTablePager_PreviousPage" => _localizer.Get("mud.tablePager.previous"),
            "MudTablePager_NextPage" => _localizer.Get("mud.tablePager.next"),
            "MudTablePager_LastPage" => _localizer.Get("mud.tablePager.last"),
            _ => key,
        };
}