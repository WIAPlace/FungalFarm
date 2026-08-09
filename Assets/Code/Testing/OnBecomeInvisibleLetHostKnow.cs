using UnityEngine;

public class OnBecomeInvisibleLetHostKnow : MonoBehaviour
{
    [field: SerializeReference] public HostView host;

    void Start()
    {
        if (host == null)
        {
            host = GetComponentInParent<HostView>();
        }
    }
    void OnBecameInvisible()
    {
        if(host.destroyOnInvis) host.OnChildBecameInvisible();
    }

}
