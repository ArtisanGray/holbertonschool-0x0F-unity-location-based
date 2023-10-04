using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class arGPS : MonoBehaviour
{
    public GameObject prefab;
    public GameObject coordLat;
    public GameObject coordLong;
    public GameObject coordAlt;
    public GameObject coordUCS;
    public GameObject coordDist;
    public Button destinationSet;
    public Button distanceSet;

    private Vector3 currentUCS;
    private Vector2 currentPos;
    private Vector2 destPos;
    private float EarthRadiusMeters = 6371000;

    private GameObject startNode;
    private GameObject endNode;
    // Start is called before the first frame update
    void Start()
    {
        Input.location.Start();
    }
    private void Awake()
    {
        startNode = Instantiate(prefab, transform.position, transform.rotation);
    }
    // Update is called once per frame
    void Update()
    {
        coordLat.GetComponentsInChildren<Text>()[1].text = Input.location.lastData.latitude.ToString();
        coordLong.GetComponentsInChildren<Text>()[1].text = Input.location.lastData.longitude.ToString();
        coordAlt.GetComponentsInChildren<Text>()[1].text = Input.location.lastData.altitude.ToString();
        currentPos = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
    }
    public void CalculateDistance()
    {
        if (destPos != Vector2.zero) //if the destination hasnt been set, it doesnt calculate.
        {
            float distance = CalculateHaversineDistance(currentPos, destPos);
            Debug.Log(distance);
            coordDist.GetComponentsInChildren<Text>()[1].text = distance.ToString();
        }

    }
    public void setDestination()
    {
        destPos = new Vector2(currentPos.x, currentPos.y);
        currentUCS = GPSEncoder.GPSToUCS(destPos);
        coordUCS.GetComponentsInChildren<Text>()[1].text = currentUCS.ToString();
        Debug.Log(destPos);
        if (endNode)
        {
            Destroy(endNode);
        }
        endNode = Instantiate(prefab, new Vector3(currentUCS.x, currentUCS.y, currentUCS.z), Quaternion.identity);
        endNode.GetComponentInChildren<Text>().text = "Destination";
    }
    public float CalculateHaversineDistance(Vector2 fromCoordinates, Vector2 toCoordinates)
    {
        float dLat = DegreesToRadians(toCoordinates.x - fromCoordinates.x);
        float dLon = DegreesToRadians(toCoordinates.y - fromCoordinates.y);

        float lat1 = DegreesToRadians(fromCoordinates.x);
        float lat2 = DegreesToRadians(toCoordinates.x);

        float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
                   Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2) * Mathf.Cos(lat1) * Mathf.Cos(lat2);
        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));

        return EarthRadiusMeters * c;
    }

    // Convert degrees to radians
    private static float DegreesToRadians(float degrees)
    {
        return degrees * (Mathf.PI / 180);
    }
}
