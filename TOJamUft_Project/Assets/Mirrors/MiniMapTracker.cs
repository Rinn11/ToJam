using UnityEngine;

public class MiniMapTracker : MonoBehaviour
{
    public GameObject Player;
    private void LateUpdate()
    {
        transform.position = new Vector3(Player.transform.position.x, 30, Player.transform.position.z);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
