using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineRandomizer : MonoBehaviour {

    private BezierSpline splineScript;

    public void Randomize(bool keepStartSame = false)
    {
        splineScript = transform.gameObject.GetComponent<BezierSpline>();
        int splineCount = splineScript.ControlPointCount;
        float delta = 0.75f;
        int pointCounter = 0, nodeCounter = 0;        

        for (int i = 0; i < splineCount; i++) //For every point of the spline
        {
            //int randomNum = Random.Range(0, pointList.Count);
            Vector3 deltaPos = new Vector3(Random.Range(-delta, delta), Random.Range(-delta, delta), 0);
            //splineScript.SetControlPoint(i, transform.GetChild(randomNum).localPosition + deltaPos);
            //pointList.Remove(randomNum);
            if (pointCounter == 0)
            {
                if (keepStartSame && (i == 0 || i == 24))
                {
                    if (i == 24)
                    {
                        int secondPoint = 1;
                        Vector3 thirdPoint = splineScript.GetControlPoint(secondPoint);
                        splineScript.SetControlPoint(secondPoint, thirdPoint);
                    }
                }
                else
                {
                    splineScript.SetControlPoint(i, transform.GetChild(nodeCounter).localPosition + deltaPos);
                } 
                nodeCounter++;
            }
            else if (pointCounter == 1)
            {
                splineScript.SetControlPoint(i, transform.GetChild(nodeCounter).localPosition + deltaPos);
            }
            else //pointCounter == 2
            {
                Vector3 prevPoint = splineScript.GetControlPoint(i - 1);
                splineScript.SetControlPoint(i, prevPoint);
                pointCounter = -1;
                nodeCounter++;
                if (nodeCounter > 15)
                {
                    nodeCounter = 0;
                }
            }
            pointCounter++;
        }

    }
}
