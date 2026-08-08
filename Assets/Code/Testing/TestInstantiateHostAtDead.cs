using System.Collections;
using UnityEngine;

public class TestInstantiateHostAtDead : MonoBehaviour
{
    [SerializeField]HostView hm;

    void Start()
    {
        StartCoroutine(waitToSpawn());
    }
    IEnumerator waitToSpawn()
    {
        yield return new WaitForSeconds(3);
        if (!HostManager.Instance.AddDeadViewToArray(hm))
        {
            Debug.Log("failed");
        }
    }
}
