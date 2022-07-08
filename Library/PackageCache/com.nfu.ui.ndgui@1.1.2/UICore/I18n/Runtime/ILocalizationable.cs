namespace ND.UI.I18n
{
    public interface ILocalizationable
    {
        bool IsLocalized{ get;}
        int LocalizationGearType { get; }
        string LocalizationKey { get; set; }
    }
}