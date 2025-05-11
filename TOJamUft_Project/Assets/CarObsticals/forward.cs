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

    void Start()
    {
        //Fetch the Rigidbody from the GameObject with this script attached
        m_Rigidbody = GetComponent<Rigidbody>();

        tileSpawner = FindAnyObjectByType<TileSpawner>().gameObject;
        _BackTile = tileSpawner.transform.GetChild(0).gameObject;
        _FrountTile = tileSpawner.transform.GetChild(tileSpawner.transform.childCount - 1).gameObject;

        getControlPoint();
    }

    public void getTargetChild(int _targetedChild)
    {
        targetChildPosition = _targetedChild;
    }

    public void getControlPoint()
    {
        int backIndex = 0 + controlPoint;
        int frontIndex = tileSpawner.transform.childCount - 1 - controlPoint;

        // Validate indices before accessing
        if (backIndex >= tileSpawner.transform.childCount || frontIndex < 0)
        {
            Destroy(this.gameObject);
            return;
        }

        // Get the tiles safely
        _BackTile = tileSpawner.transform.GetChild(backIndex).gameObject;
        _FrountTile = tileSpawner.transform.GetChild(frontIndex).gameObject;


        // Get their "Routes" children
        Transform backRoutes = _BackTile.transform.Find("Routes");
        Transform frontRoutes = _FrountTile.transform.Find("Routes");

        // Assign control points: [0, 1] from backRoutes, [2, 3] from frontRoutes
        Transform[] controlPoints = new Transform[4];
        if (targetChildPosition <  2) { controlPoints[targetChildPosition] = backRoutes.GetChild(targetChildPosition); }
        else { controlPoints[targetChildPosition] = frontRoutes.GetChild(targetChildPosition); }

        
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
}
