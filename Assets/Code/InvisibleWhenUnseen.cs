using Unity.VisualScripting;
using UnityEngine;

public class InvisibleWhenUnseen : MonoBehaviour
{
    void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }
}
