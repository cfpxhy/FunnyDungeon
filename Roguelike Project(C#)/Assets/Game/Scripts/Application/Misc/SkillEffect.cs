using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffect : MonoBehaviour {


	void Start () {
		
	}
	
	void Update () {
		if (gameObject.name == "InvincibleEffect(Clone)")
        {
            Destroy(gameObject, 5.0f);
        }
	}
}
