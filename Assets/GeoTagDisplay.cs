using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeoTagDisplay : MonoBehaviour
{
    public Text title;
    public Text location;

    public void Initialize(GeoTag geotag)
    {
        this.title.text = geotag.name;
        this.location.text = geotag.latitude.ToString() + ", " + geotag.longitude.ToString() + ", " + geotag.altitude.ToString();
    }
}
