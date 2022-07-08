namespace ND.UI.Core
{
    public interface IRating
    {
        float current { set; get; }
        int total { set; get; }
        
        bool canModifyTotalByCode { get; }
    }
}