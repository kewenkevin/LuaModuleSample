namespace ND.UI.Core
{
    public interface IController
    {
        int SelectedIndex { get; set; }

        string Name { get; }

        int PageNum { get; }
    }
}