using UnityEngine;

public class PersistManager : MonoBehaviour
{
    // thanks to this link https://www.youtube.com/watch?v=j_eQGp-IbCE for reference.
    public static PersistManager Instance;
    public GameObject[] persistentObjects;

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scene reloads
        MarkPersistence();

        Debug.Log("PersistManager initialized and marked for persistence.");
    }

    void MarkPersistence()
    {
        // This method can be used to mark the object as persistent in a way that is visible in the editor.
        // For example, you could add a component or set a property that indicates this object should not be destroyed.
        Debug.Log("PersistManager is marked for persistence across scenes.");

        foreach (GameObject obj in persistentObjects)
        {
            if (obj != null)
            {
                DontDestroyOnLoad(obj);
                Debug.Log($"Marked {obj.name} for persistence.");
            }
            else
            {
                Debug.LogWarning("Found a null object in persistentObjects array.");
            }
        }
    }    

}
