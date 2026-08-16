using UnityEngine;

public class OnStartRotate : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float randomY = UnityEngine.Random.Range(0f, 360f);
        gameObject.transform.rotation = Quaternion.Euler(0f, randomY, 0f);

        float randomScale = UnityEngine.Random.Range(.9f, 1.3f);
        gameObject.transform.localScale *=randomScale;
    }
}
