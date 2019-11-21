/* Tal Rastopchin
 * August 2, 2019
 * 
 * A script that lets a player investigate the construction of an array of different
 * ruled surfaces. The player uses their controller's to draw out rulings in space,
 * trying to construct each of the available ruled surfaces.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* The RuledSurfaceController lets a player investigate the contruction of an array
 * of different ruled surfaces. In front of the player is a vizualization of a
 * possible ruled surface that they can construct. Player's use the right hand
 * controller B button and A button respectively to cycle forwards and backwards
 * through an array of ruled surfaces to vizualize. Once a player selects a ruled
 * surface to vizualize, they will construct it using the positions of their left
 * and right controllers. To visualize where they will create a ruling, one only
 * need hold down one of the left or right triggers. To start drawing, the player
 * should hold down both triggers and move their controllers through space. The
 * player can use the left or right grip to move and orient their resulting drawing.
 * Lastly, the player can press the left controller Y button to reset their drawing.
 * 
 * The controllerLeft and controllerRight GameObjects correspond ot the Left
 * Controller and Right Controller CameraRig objects. The rulingVizualization
 * GameObject corresponds to the cylindrical object that will be oriented between
 * the player's controllers.
 */
public class RuledSurfaceController : MonoBehaviour
{
    // CameraRig left and right controllers
    public GameObject controllerLeft;
    public GameObject controllerRight;

    // surfaceViewer GameObject
    public SurfaceViewer surfaceViewer;

    // the object we use for the ruling vizualization
    public GameObject rulingVizualization;

    // objects for getting controller input
    private SteamVR_TrackedObject trackedControllerLeft;
    private SteamVR_Controller.Device deviceLeft;
    private SteamVR_TrackedObject trackedControllerRight;
    private SteamVR_Controller.Device deviceRight;

    // list to keep track of instantiated rulings
    private List<GameObject> rulings = new List<GameObject>();

    // distance between each ruling draw
    public float drawRulingDelta = .001f;

    // radius of each ruling cylinder
    public float radius = .05f;

    // we keep track of the old left and right handle locations
    private Vector3 oldLeftPoint;
    private Vector3 oldRightPoint;

    // boolean variables to keep track of controller states
    private bool drawing = false;
    private bool grabbing = false;

    // struct to store our controller input to simplify our code
    private struct ControllerInput
    {
        public bool pressDownLeftTrigger;
        public bool pressLeftTrigger;
        public bool pressDownLeftGrip;
        public bool pressLeftGrip;
        public bool pressUpLeftGrip;

        public bool pressDownLeftJoystick;

        public bool pressDownRightTrigger;
        public bool pressRightTrigger;
        public bool pressUpRightTrigger;
        public bool pressDownRightGrip;
        public bool pressRightGrip;
        public bool pressUpRightGrip;

        public bool pressDownRightJoystick;
    }

    // instance of ControllerInput struct
    private ControllerInput input;

    /* Start is called before the first frame update
     * 
     * Gets the TrackedObject components from the controllerLeft and controllerRight
     * GameObjects accordingly. Initializes oldLeftPoint and oldRightPoint to the
     * locations of the controllerLeft and controllerRight GameObjects.
     */
    void Start()
    {
        // get the TrackedObject components
        trackedControllerLeft = controllerLeft.GetComponent<SteamVR_TrackedObject>();
        trackedControllerRight = controllerRight.GetComponent<SteamVR_TrackedObject>();
    }

    /* Update is called once per frame
     * 
     * Gets the controller devices, handles the controller input, and orients
     * the ruling vizualization object.
     */
    void Update()
    {
        GetControllerDevices();
        GetControllerInput();
        RecordControllerPositions();
        DrawRulingVisualization();
        DrawHint();
        DrawRulings();
        DestroyRulings();
        GripGrab();
    }

    /* GetControllerDevices
     * 
     * Assigns the SteamVR_Controller.Device objects to the
     * corresponding left and right controller devices.   
     */
    private void GetControllerDevices ()
    {
        deviceLeft = SteamVR_Controller.Input((int)trackedControllerLeft.index);
        deviceRight = SteamVR_Controller.Input((int)trackedControllerRight.index);
    }

    /* GetControllerInput
     * 
     * Gets and stores the used button press states from the deviceLeft and deviceRight
     * SteamVR_Controller.Device devices. We do this to simplify getting the state of
     * a button.
     */
    private void GetControllerInput ()
    {
        input.pressDownLeftTrigger = deviceLeft.GetPressDown(SteamVR_Controller.ButtonMask.Trigger);
        input.pressLeftTrigger = deviceLeft.GetPress(SteamVR_Controller.ButtonMask.Trigger);
        input.pressDownLeftGrip = deviceLeft.GetPressDown(SteamVR_Controller.ButtonMask.Grip);
        input.pressLeftGrip = deviceLeft.GetPress(SteamVR_Controller.ButtonMask.Grip);
        input.pressUpLeftGrip = deviceLeft.GetPressUp(SteamVR_Controller.ButtonMask.Grip);

        input.pressDownLeftJoystick = deviceLeft.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad);

        input.pressDownRightTrigger = deviceRight.GetPressDown(SteamVR_Controller.ButtonMask.Trigger);
        input.pressRightTrigger = deviceRight.GetPress(SteamVR_Controller.ButtonMask.Trigger);
        input.pressDownRightGrip = deviceRight.GetPressDown(SteamVR_Controller.ButtonMask.Grip);
        input.pressRightGrip = deviceRight.GetPress(SteamVR_Controller.ButtonMask.Grip);
        input.pressUpRightGrip = deviceRight.GetPressUp(SteamVR_Controller.ButtonMask.Grip);

        input.pressDownRightJoystick = deviceRight.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad);
    }

    /* RecordControllerPositions
     * 
     * When one of the left or right trigger is pressed down, record and store the
     * positions of the corresponding controllerLeft and controllerRight GameObjects
     * in the oldLeftPoint and oldRightPoint fields.
     */
    private void RecordControllerPositions ()
    {
        // left trigger press down -> record left controller location
        if (input.pressDownLeftTrigger)
        {
            deviceLeft.TriggerHapticPulse(500);
            oldLeftPoint = controllerLeft.transform.position;
        }

        // right trigger press down -> record right controller location
        else if (input.pressDownRightTrigger)
        {
            deviceRight.TriggerHapticPulse(500);
            oldRightPoint = controllerRight.transform.position;
        }
    }

    /* DrawRulingVisualization
     * 
     * If currently not grabbing and one of the left or right trigger is pressed
     * down, the display the ruling visualization.
     */
    private void DrawRulingVisualization ()
    {
        if (!grabbing)
        {
            // right trigger or left trigger -> display ruling vizualization
            if (input.pressLeftTrigger || input.pressRightTrigger)
                rulingVizualization.SetActive(true);
            else
                rulingVizualization.SetActive(false);

            // we orient our ruling visualization
            Utils.OrientCylinder(rulingVizualization, controllerLeft.transform.position, controllerRight.transform.position, radius);
        }
    }

    /* DrawHint
     * 
     * If the left joystick button is pressed, draw a hint for the active surface. Gets
     * the active surface from the surfaceViewer object. Then, determines whether or not
     * the first child of the active surface is a hint prefab. To determine if this is
     * the case, we check whether or not getting a RuledSurfaceHint component returns
     * a null pointer. If it does not, then the child is a valid hint prefab.
     * 
     */
    private void DrawHint ()
    {
        if (input.pressDownLeftJoystick)
        {
            GameObject currentSurface = surfaceViewer.GetCurrentSurface();

            // if the current surface has at least one child, then check if valid hint prefab
            if (currentSurface.transform.childCount != 0)
            {
                if (currentSurface.transform.GetChild(0).GetComponent<RuledSurfaceHint>() != null)
                {
                    // if valid hint prefab, instantiate it and make it active

                    GameObject hint = Instantiate(currentSurface.transform.GetChild(0), currentSurface.transform).gameObject;
                    hint.SetActive(true);
                }
            }
        }
    }

    /* DrawRulings
     * 
     * If currently not grabbing and both the left and right triggers are pressed, set
     * the drawing boolean variable to true and begin drawing. If not grabbing and both
     * of the left and right triggers are not pressed, set the drawing boolean variable
     * to false. While drawing, draw a new ruling only if the controllers have moved
     * sufficiently from their previous positions according to the drawRulingDelta
     * threshold.
     */
    private void DrawRulings ()
    {
        if (!grabbing)
        {
            // left and right trigger press -> drawing
            if (input.pressLeftTrigger && input.pressRightTrigger)
            {
                drawing = true;

                Vector3 newLeftPoint = controllerLeft.transform.position;
                Vector3 newRightPoint = controllerRight.transform.position;

                // if the handles have moved sufficiently
                if ((newLeftPoint - oldLeftPoint).magnitude > drawRulingDelta || (newRightPoint - oldRightPoint).magnitude > drawRulingDelta)
                {
                    CreateRuling(newLeftPoint, newRightPoint, radius);

                    // update old left and right handle locations
                    oldLeftPoint = newLeftPoint;
                    oldRightPoint = newRightPoint;
                }
            }
            else
                drawing = false;
        }
    }

    /* DestroyRulings
     * 
     * If not drawing, not grabbing, and the right controller joystick is pressed
     * down, destroy all of the created rulings.
     */
    private void DestroyRulings ()
    {
        if (!drawing && !grabbing)
        {
            // right grip press -> destroy created rulings
            if (input.pressDownRightJoystick)
            {
                deviceRight.TriggerHapticPulse(500);

                // destroy each of the ruling GameObjects
                foreach (GameObject ruling in rulings)
                {
                    GameObject.Destroy(ruling);
                }

                // rulings is now a list full of references to destroyed GameObjects
                // we reinitialze it to pass this useless list to the garbage collector, as
                // we do not need to clear the contents of the list immediately.
                rulings = new List<GameObject>();
            }
        }
    }

    /* GripGrab
     * 
     * If not drawing, when the right grip is pressed down temporarily set this object transform
     * parent to that of the right controller. if the right grip is pressed up, reset this object
     * transform parent to null. If the left grip is pressed down, temporarily set the transform parent
     * of the current surface being displayed by the surfaceViewer to that of the left controller.
     * If the right grip is pressed up, reset the object transform of the current displayed surface
     * to null.
     */
    private void GripGrab ()
    {
        if (!drawing)
        {
            // right grip press down -> set right controller parent
            if (input.pressDownRightGrip && !input.pressLeftTrigger && !input.pressRightTrigger)
            {
                deviceRight.TriggerHapticPulse(500);
                transform.parent = controllerRight.transform;
                grabbing = true;
            }

            // left grip press down -> set left controller parent
            if (input.pressDownLeftGrip && !input.pressLeftTrigger && !input.pressRightTrigger)
            {
                deviceLeft.TriggerHapticPulse(500);
                surfaceViewer.GetCurrentSurface().transform.parent = controllerLeft.transform;
                grabbing = true;
            }

            // right grip press up -> reset current object's transform parent
            if (input.pressUpRightGrip)
            {
                deviceRight.TriggerHapticPulse(500);
                transform.parent = null;
                grabbing = false;
            }

            // left grip press up -> reset current object's transform parent
            if (input.pressUpLeftGrip)
            {
                deviceLeft.TriggerHapticPulse(500);
                surfaceViewer.GetCurrentSurface().transform.parent = null;
                grabbing = false;
            }
        }
    }

    /* CreateRuling
     * 
     * Creates a cylinder with specified radius connecting point1 to point2. To
     * do this, createRuling instantiates a new Cylinder primitive and then 'fits'
     * the cylinder in between the given points with the specified radius. Sets
     * the newly instantiated Cylinder's transform parent to this object's transform.
     * We do that so we can 'move around' the created rulings by just manipulating
     * this object's transform. We additionally add the newly created Cylinder to
     * our rulings list so that we can delete all of them to reset the drawing.   
     */
    private void CreateRuling (Vector3 point1, Vector3 point2, float radius)
    {
        GameObject newCylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        newCylinder.transform.SetParent(transform);
        rulings.Add(newCylinder);

        Utils.OrientCylinder(newCylinder, point1, point2, radius);   
    }
}

/* Sources
 * https://answers.unity.com/questions/35541/problem-finding-relative-rotation-from-one-quatern.html
 */
