using UnityEngine;
using System.Collections;

public class DoubleDoorController : MonoBehaviour {

	public GameObject door1;
	public GameObject door2;
	public Transform door1Final, door2Final;
	public bool requiresKey = false;
	private Vector3 door1Init, door2Init;
	private Vector3 finalPos1, finalPos2;
	private bool canLerp;
	private Vector3 temp;
	private int count = 0;
	private bool hasKey = false;

	// Use this for initialization
	void Start () 
	{
		door1Init = door1.transform.position;
		door2Init = door2.transform.position;
		finalPos1 = door1Final.position;
		finalPos2 = door2Final.position;
		canLerp = true;
	}
	
	// Update is called once per frame
	void Update () 
	{

		if(Mathf.Abs(door1.transform.position.y - finalPos1.y) < .05 && count == 0)
		{
			if(canLerp){
				if (requiresKey && !hasKey){
					//nothing yet?
				} else {
					StartCoroutine("LerpClosed");
				}
			}
		}
		if(Mathf.Abs(door1.transform.position.y - door1Init.y) < .05 && count > 0)
		{
			if(canLerp){
				if (requiresKey && !hasKey){
					//nothing yet?
				} else {
					StartCoroutine("LerpOpen");
				}
			}
		}
	}

	public void UnlockDoor(){
		hasKey = true;
	}

	private void OnTriggerEnter(Collider coll)
	{
		if(coll.gameObject.CompareTag("Player")){
			count++;
			if(canLerp){
				StartCoroutine("LerpOpen");
			}
		}
	}

	private void OnTriggerExit(Collider coll)
	{
		if(coll.gameObject.CompareTag("Player")){
			count--;
		}
	}

	private IEnumerator LerpOpen()
	{ 
		canLerp = false;
		for (float t=0; t<1f; t+=Time.smoothDeltaTime/1.25f){
			door1.transform.position = temp = Vector3.Lerp(door1Init, finalPos1, t);
			door2.transform.position = temp = Vector3.Lerp(door2Init, finalPos2, t);
			yield return null;
		}
		canLerp = true;
		door1.transform.position = finalPos1;
		door2.transform.position = finalPos2;
	}

	private IEnumerator LerpClosed()
	{ 
		canLerp = false;
		for (float t=0; t<1f; t+=Time.smoothDeltaTime/1.25f){
			door1.transform.position = temp = Vector3.Lerp(finalPos1, door1Init, t);
			door2.transform.position = temp = Vector3.Lerp(finalPos2, door2Init, t);
			yield return null;
		}
		canLerp = true;
		door1.transform.position = door1Init;
		door2.transform.position = door2Init;
	}

}
