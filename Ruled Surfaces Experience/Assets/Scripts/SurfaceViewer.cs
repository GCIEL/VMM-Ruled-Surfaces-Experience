/* Tal Rastopchin
 * July 12, 2019
 * 
 * Script that displays one child of a parent object at at time in a circular
 * fashion
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* SurfaceViewer displays one child of the current GameObject at a time.
 * Given specified input, it cycles either forward or backward through all of
 * the children objects.s
 * 
 * We experimentally determined that the bitmask of ButtonA is 128ul.
 */

public class SurfaceViewer : MonoBehaviour
{
    // // buttonmask for ButtonB and ButtonA
    private const ulong ButtonB = SteamVR_Controller.ButtonMask.ApplicationMenu;
    private const ulong ButtonA = 128ul;

    // CameraRig right controller
    public GameObject controller;
    private SteamVR_TrackedObject trackedController;
    private SteamVR_Controller.Device device;

    // variables to keep track of children GameObject surfaces
    private GameObject[] children;
    private int numChildren;
    private int currentChild = 0;

    /* Start is called before the first frame update
     *
     * Gets the TrackedObject components from the controllerRight GameObject
     * accordingly. Stores a GameObject reference to each of this GameObject's
     * children. Makes every child of this GameObject inactive, and makes the
     * currentChild active.
     */
    void Start()
    {
        trackedController = controller.GetComponent<SteamVR_TrackedObject>();

        numChildren = transform.childCount;
        children = new GameObject[numChildren];

        for (int i = 0; i < numChildren; i++)
        {
            children[i] = transform.GetChild(i).gameObject;
            children[i].SetActive(false);
        }

        children[currentChild].SetActive(true);
    }

    /* Update is called once per frame
     * 
     * Cycles through the array of possible child surfaces to display accordingly.
     * Gets the right controller device. Cycles forward if the B button is pressed;
     * cycles backward if the A button is pressed.
     * 
     * It turns out the '%' operator is not the modulo operater but the remainder
     * operator. Hence, the '%' of a negative number is not equal to its modulo,
     * so we treat it as a separate case when cycling backwards.
     */
    void Update()
    {
        device = SteamVR_Controller.Input((int)trackedController.index);

        if (device.GetPressDown(ButtonB))
        {
            device.TriggerHapticPulse(500);
            children[currentChild].SetActive(false);
            currentChild = (currentChild + 1) % numChildren;
            children[currentChild].SetActive(true);
        }

        else if (device.GetPressDown(ButtonA))
        {
             device.TriggerHapticPulse(500);

            children[currentChild].SetActive(false);
            currentChild = (currentChild - 1);

            // if currentChild is negative, we adjust to compute the actual modulo
            if (currentChild < 0)
                currentChild = (currentChild + numChildren) % numChildren;
            
            children[currentChild].SetActive(true);
            Debug.Log(currentChild);
        }
    }

    /* GetCurrentSurface
     * 
     * Returns a reference to the child surface that is currently acive and being
     * displayed.
     */
    public GameObject GetCurrentSurface ()
    {
        return children[currentChild];
    }
}
