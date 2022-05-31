using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class GPSLocation : MonoBehaviour
{
    // Variables to show on the screen
    public Text GPSStatus;
    public Text latitudeValue;
    public Text longitudeValue;
    public Text alittudeValue;
    public Text horixontalAccuracyValue;
    public Text TimeStampValue;

    // Debug Variables
    public Text locationSaved;
    public Text unityLocation;
    public Text objectName;
    public Text distance;
    // public Text placedGameObject;
    // public Text placedLocation;
    // Gameobject prefabs
    public GameObject savedPrefab;
    public CanvasGroup createGeoTagCanvas;
    public GameObject InputObject;
    public InputField geoTagName;

    // Local Position variables
    private Vector3 location;
    [SerializeField] private GameObject ARCamera;
    private List<GeoTag> GeoTags = new List<GeoTag>();

    // Distance variables
    private float r, lat1, lat2, deltaLat, deltaLong, long1, long2;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GPSLoc());
    }

    IEnumerator GPSLoc()
    {
        // check if user has location service enabled
        if(!Input.location.isEnabledByUser)
            yield break;

        // start service before querying location
        Input.location.Start();

        // wait until service initialize
        int maxWait =20;
        while(Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // service didn't init in 20 sec
        if (maxWait < 1)
        {
            GPSStatus.text = "time out";
            yield break;
        }

        // connection failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            GPSStatus.text = "unable to determine device location";
            yield break;
        }
        else
        {
            // access granted
            GPSStatus.text = "Running";
            InvokeRepeating("UpdateGPSData", 0.5f, 1f);
        }
    }//end of GPSLoc

    public void SavedLocation()
    {
        InputObject.SetActive(true);
        // Create geotag
        GeoTag geotag = new GeoTag();
        geotag.latitude = Input.location.lastData.latitude;
        geotag.longitude = Input.location.lastData.longitude;
        geotag.altitude = Input.location.lastData.altitude;
        geotag.name = geoTagName.text;

        // Set Variables for GetDistance()
        lat2 = geotag.latitude;
        long2 = geotag.longitude;

        // Add to GeoTags List
        GeoTags.Add(geotag);

        // Display saved location
        locationSaved.text = "Latitude: " + geotag.latitude.ToString() + ", Longitude: " + geotag.longitude.ToString() + ", Altitude: " + geotag.altitude.ToString();
        location = GPSEncoder.GPSToUCS(geotag.latitude, geotag.longitude);
        unityLocation.text = "Unity Local Position:" + location.ToString();

        // GPSEncoder.SetLocalOrigin(geotag.latitude, geotag.longitude);

        SpawnPrefab(Camera.main.transform.position, geotag);
        objectName.text = "Name: " + geotag.name;

        // Create prefab


        // obj.transform.rotation = Quaternion.LookRotation(transform.position - ARCamera.transform.position);
        // obj.GetComponent<GeoTagDisplay>().Initialize(geotag);

    }
    public void EndInput()
    {
        InputObject.SetActive(false);
    }
    private void SpawnPrefab(Vector3 location, GeoTag geotag)
    {
        GameObject instantiatedObj = Instantiate(savedPrefab, location + (Camera.main.transform.forward * 1), Quaternion.identity);
        instantiatedObj.transform.rotation = Quaternion.LookRotation(transform.position - (Camera.main.transform.position));
        instantiatedObj.GetComponent<GeoTagDisplay>().Initialize(geotag);
        // placedGameObject.text = instantiatedObj.name;
        // placedLocation.text = instantiatedObj.transform.position.ToString();
    }
    private void UpdateGPSData()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            // access granted to gps values and it has been init
            GPSStatus.text = "Running";
            latitudeValue.text = Input.location.lastData.latitude.ToString();
            longitudeValue.text = Input.location.lastData.longitude.ToString();
            alittudeValue.text = Input.location.lastData.altitude.ToString();
            horixontalAccuracyValue.text = Input.location.lastData.horizontalAccuracy.ToString();
            TimeStampValue.text = Input.location.lastData.timestamp.ToString();

            // Set variables for GetDistance()
            lat1 = Input.location.lastData.latitude;
            long1 = Input.location.lastData.longitude;

        }
        else
        {
            // service stopped
            GPSStatus.text = "Stop";

        }
    }//end of UpdateGPSData

    
    public void GetDistance()
    {
        // Radius of earth in kilometers. Use 3956 for miles
        r = 6371;
        // Convert to radians
        lat1 = lat2 * (float)Math.PI/180;
        lat2 = lat2 * (float)Math.PI/180;

        // Haversine Formula
        deltaLat = (lat2 - lat1) * (float)Math.PI/180;
        deltaLong = (long2 - long1) * (float)Math.PI/180;
        float a = (float)Math.Sin(deltaLat/2) * (float)Math.Sin(deltaLat/2) + (float)Math.Cos(lat1) * (float)Math.Cos(lat2) * (float)Math.Sin(deltaLong/2) * (float)Math.Sin(deltaLong/2);
        float c = 2 * (float)Math.Atan2(Math.Sqrt(a), (float)Math.Sqrt(1-a));
        float d = r * c;
        distance.text = "Distance: " + d.ToString() + "KM";
    }
}
