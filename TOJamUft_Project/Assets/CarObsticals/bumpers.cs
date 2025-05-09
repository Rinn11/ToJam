using Unity.VisualScripting;
using UnityEngine;

public class bumpers : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //slow players speed with Ienumer to max

        other.gameObject.GetComponent<Material>().SetColor("_Color", Color.blue);
    }
}
