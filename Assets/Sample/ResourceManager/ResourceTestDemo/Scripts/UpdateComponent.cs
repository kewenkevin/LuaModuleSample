using UnityEngine;

public class UpdateComponent : MonoBehaviour
{
    public virtual void OnUpdateComplete()
    {
        
        gameObject.SetActive(false);
    }
}
