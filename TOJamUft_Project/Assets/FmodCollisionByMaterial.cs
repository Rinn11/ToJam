using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FmodMaterialCollision : MonoBehaviour
{
    [EventRef]
    public string collisionEvent = "event:/Colisions/Metal";

    void OnCollisionEnter(Collision collision)
    {
        string tag = collision.gameObject.tag;
        int materialType = GetMaterialTypeFromTag(tag);

        // Create instance manually to set parameter before playing
        EventInstance instance = RuntimeManager.CreateInstance(collisionEvent);
        instance.setParameterByName("Colisions", materialType);

        RuntimeManager.AttachInstanceToGameObject(instance, transform, GetComponent<Rigidbody>());
        instance.start();
        instance.release();

        Debug.Log($"Collision with: {collision.gameObject.name}, Tag: {tag}, MaterialType: {materialType}");
    }

    int GetMaterialTypeFromTag(string tag)
    {
        switch (tag)
        {
            case "Metal": return 0;
            case "Wood": return 1;
            case "Stone": return 2;
            default: return -1; 
        }
    }
}
