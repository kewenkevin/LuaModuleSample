
namespace ND.UI.NDUI
{
    public delegate string GetCellReuseIdentifierDelegate(int id);
    public delegate float GetSizeForCellDelegate(NDScrollRect tableView, NDTableViewCell cell);
    public delegate void CellOnUseDelegate(NDTableViewCell cell);
    public delegate void CellOnUnUseDelegate(NDTableViewCell cell);
}