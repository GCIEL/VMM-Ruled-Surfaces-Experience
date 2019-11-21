/* Tal Rastopchin
 * August 1, 2019
 * 
 * A script that displays a hint for how to construct a ruled surface 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* A RuledSurfaceHint is an abstract class that facilitates the display
 * of a hint for the construction of a ruled surface. When a RuledSurfaceHint
 * is instantiated, an ruling animated according to the parameterized
 * EvaluateLeftPoint and EvaluateRightPoint will be displayed. After reaching
 * the parameterEnd, this script will destroy the visualization GameObjects as
 * well as GameObject it is attatched to.
 */
public abstract class RuledSurfaceHint : MonoBehaviour
{
    // prefabs for the visualization ruling
    public GameObject pointObject;
    public GameObject rulingObject;

    // GameObjects to store the instantiated prefabs
    private GameObject leftHand;
    private GameObject rightHand;
    private GameObject ruling;

    // parameters for the visualization ruling
    public float pointRadius = .1f;
    public float rulingRadius = .05f;

    // parameters for the parametric EvaluateLeftPoint and EvaluateRightPoint functions
    public float speed = 1f;
    public float parameterStart = 0;
    public float parameterEnd = 1;

    // the current t value in our parameterization
    private float parameterTimestep = 0;

    private Vector3 inverseScale;

    /* Start
     * 
     * Instantiates the three ruling visualization objects with this GameObject as their
     * transform parent as well as calls the UpdateRuling method to initially position
     * and orient the ruling visualization.
     * 
     * Called before the first frame update
     */
    void Start()
    {
        leftHand = Instantiate(pointObject);
        leftHand.transform.localScale = new Vector3(pointRadius, pointRadius, pointRadius);
        leftHand.transform.parent = transform;

        rightHand = Instantiate(pointObject);
        rightHand.transform.localScale = new Vector3(pointRadius, pointRadius, pointRadius);
        rightHand.transform.parent = transform;

        ruling = Instantiate(rulingObject);
        ruling.transform.localScale = new Vector3(1, 1, 1);
        ruling.transform.parent = transform;

        parameterTimestep = parameterStart;

        UpdateRuling();
    }

    /* Update
     * 
     * If the current parameterTimestep is less than or equal to the parameterEnd, updates
     * the timestep and calls the UpdateRuling method to update the ruling visualization. If
     * the current parameterTimestepp is greater than the parameterEnd, destroys the visualization
     * GameObjects and the GameObject this script is attatched to.
     * 
     * Called once per frame
     */
    void Update()
    {
        if (parameterTimestep <= parameterEnd)
        {
            parameterTimestep += speed * Time.deltaTime * (parameterEnd - parameterStart);
            UpdateRuling();
        }
        else
        {
            Destroy();
        }
    }

    /* Destroy
     * 
     * Destroys all of the instantiated visualization GameObjects as well as the GameObject
     * this script is attatched to, effectively ending the hint visualization.
     */
    private void Destroy()
    {
        Destroy(leftHand);
        Destroy(rightHand);
        Destroy(ruling);
        Destroy(this);
    }

    /* UpdateRuling
     * 
     * Passes the parameterTimestep field to the EvaluateLeftPoint and EvaluateRightPoint
     * methods to compute the current points of the ruling visualization in local coordinates.
     * Positions and orients the leftHand, rightHand, and ruling visualization GameObjects
     * in local coordinates accordingly.
     */
    private void UpdateRuling()
    {
        Vector3 leftPoint = EvaluateLeftPoint(parameterTimestep);
        Vector3 rightPoint = EvaluateRightPoint(parameterTimestep);

        leftHand.transform.localPosition = leftPoint;
        rightHand.transform.localPosition = rightPoint;

        Vector3 midpoint = Vector3.Lerp(leftHand.transform.position, rightHand.transform.position, .5f);
        ruling.transform.position = midpoint;
        ruling.transform.up = (leftHand.transform.position - rightHand.transform.position).normalized;
        ruling.transform.localScale = new Vector3(rulingRadius, (leftPoint - rightPoint).magnitude / 2, rulingRadius);
    }

    /* EvaluateLeftPoint and EvaluateRightPoint
     * 
     * These two functions are parametric curves describing the left and right end points
     * of the ruling visualizations. The parameterization is such that the value of t runs
     * from parameterStart to parameterEnd;
     */
    protected abstract Vector3 EvaluateLeftPoint(float t);
    protected abstract Vector3 EvaluateRightPoint(float t);
}
