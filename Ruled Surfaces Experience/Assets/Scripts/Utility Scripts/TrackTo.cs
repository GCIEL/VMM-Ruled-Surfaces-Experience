/* Tal Rastopchin
 * August 2, 2019
 * 
 * A script that makes the selected object face away from the specified track
 * object, with an up vector relative to that of the flat ground. We make the
 * object face away because we use this on an empty object with a canvas child
 * so that the canvas child faces toward the camera.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackTo : MonoBehaviour
{
    public GameObject track;

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(2 * transform.position - track.transform.position, Vector3.up);
    }
}
