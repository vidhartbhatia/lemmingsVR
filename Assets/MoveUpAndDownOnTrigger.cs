using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUpAndDownOnTrigger : MonoBehaviour {
    GameObject target = null;

    public bool triggerDown, triggerStay, triggerUp;

    // Update is called once per frame
    void Update()
    {
        //Checks if the trigger is down and the target is currently within range
        //Replace with your own controller code
        if (triggerStay && target != null)
        {
            //Changes only the y position of the transform to match the target
            //Possibly replace this with a Mathf.Lerp for a better effect
            this.transform.position= new Vector3(this.transform.position.x,target.transform.position.y, this.transform.position.z);
        }
    }

    //On Collision Enter assigns the target once it is in range
    //Add any other platform initialization code here
    void OnCollisionEnter(Collision col)
    {
        target = col.gameObject;
    }

    //On Collision Exit removes the target when out of range
    //Add any other platform deactivation code here
    void OnCollisionExit(Collision col)
    {
        if (target == col.gameObject)
            target = null;
    }
}