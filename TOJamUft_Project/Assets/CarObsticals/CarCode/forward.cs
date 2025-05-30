using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class forward : MonoBehaviour
{
    Rigidbody m_Rigidbody;
    public float m_Speed = 100;

    public GameObject tileSpawner;
    public GameObject _FrountTile;
    public GameObject _BackTile;

    public int controlPoint = 0;
    private Transform targetPosition;
    public int targetChildPosition;

    public GameObject carModelParent;

    Transform[] controlPoints = new Transform[4];

    void Start()
    {
        //Fetch the Rigidbody from the GameObject with this script attached
        m_Rigidbody = GetComponent<Rigidbody>();

        tileSpawner = FindAnyObjectByType<TileSpawner>().gameObject;

        carModelParent.transform.GetChild(Random.Range(1, carModelParent.transform.childCount)).gameObject.SetActive(true);

        getControlPoint();

        CarDestruction();

        // Assign control points: [0, 1] from backRoutes, [2, 3] from frontRoutes
        //controlPoints = new Transform[4];
    }

    public void getTargetChild(int _targetedChild, int currentControlPoint)
    {
        targetChildPosition = _targetedChild;
        controlPoint = currentControlPoint;
    }

    public void getControlPoint()
    {
        int backIndex = controlPoint;
        int frontIndex = (tileSpawner.transform.childCount - 1) - controlPoint;

        // Validate indices before accessing
        if (backIndex >= tileSpawner.transform.childCount || frontIndex < 0)
        {
            Destroy(this.gameObject);
            return;
        }
        
        if (targetChildPosition <  2) {
            _BackTile = tileSpawner.transform.GetChild(backIndex).gameObject;
            Transform backRoutes = _BackTile.transform.Find("Routes");
            controlPoints[targetChildPosition] = backRoutes.GetChild(targetChildPosition); 
        }
        else {
            _FrountTile = tileSpawner.transform.GetChild(frontIndex).gameObject;
            Transform frontRoutes = _FrountTile.transform.Find("Routes");
            controlPoints[targetChildPosition] = frontRoutes.GetChild(targetChildPosition); 
        }

        
        targetPosition = controlPoints[targetChildPosition];

        controlPoint++;
    }

    void FixedUpdate()
    {
       if (targetPosition == null) { getControlPoint(); }
        m_Rigidbody.MovePosition(Vector3.MoveTowards(this.gameObject.transform.position, targetPosition.position, Time.fixedDeltaTime * m_Speed));
        transform.LookAt(targetPosition);

        float distance = Vector3.Distance(transform.position, targetPosition.position);
        if (distance < 5)
        { 
            getControlPoint();
        }
    }

    IEnumerator CarDestruction()
    {
        yield return new WaitForSeconds(30);
        Destroy(this.gameObject);

    }
}
