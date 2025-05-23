using UnityEngine;


// Update to use prefabs
public class SetCarModel : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.transform.GetChild(Random.Range(1, this.transform.childCount)).gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
