using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineRandomizer : MonoBehaviour {

    private BezierSpline splineScript;

    public void Randomize(bool keepStartSame = false)
    {
        splineScript = transform.gameObject.GetComponent<BezierSpline>();
        int splineCount = splineScript.ControlPointCount;
        float delta = 1;
        List<int> pointList = new List<int>(new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9});
        

        for (int i = 0; i < splineCount; i++) //For every point of the spline
        {
            //int randomNum = Random.Range(0, pointList.Count);
            Vector3 deltaPos = new Vector3(Random.Range(-delta, delta), Random.Range(-delta, delta), 0);
            //splineScript.SetControlPoint(i, transform.GetChild(randomNum).localPosition + deltaPos);
            //pointList.Remove(randomNum);
            
            splineScript.SetControlPoint(i, transform.GetChild(i).localPosition + deltaPos);
            
        }

    }
}
