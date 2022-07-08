namespace ND.UI.Core
{
    public interface IUIExpansion : IBindable
    {
        UnityEngine.Object[] BindedObjects { get; set; }

        UnityEngine.Object GetBindObject(int index);

        IController GetController(int index);

        IController GetController(string name);

        ITransition GetTransition(int index);

        ITransition GetTransition(string name);
    }
}