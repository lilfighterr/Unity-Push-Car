using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour {

	// Use this for initialization
	void Start () {
        iTween.MoveTo(this.gameObject, iTween.Hash("path", iTweenPath.GetPath("carPath"), "time", 10, "orienttopath", true, "lookahead", 1, "axis", "z"));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
