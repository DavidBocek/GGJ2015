using UnityEngine;
using System.Collections;

public class DoubleDoorController : MonoBehaviour {

	public GameObject door1;
	public GameObject door2;
	public Transform door1Final, door2Final;
	public bool requiresKey = false;
	public AudioClip openClip, closeClip;
	public GameObject objectToSpawn;
	public AudioClip stingerClip;
	public float stingerDelay;
	private Vector3 door1Init, door2Init;
	private Vector3 finalPos1, finalPos2;
	private bool canLerp;
	private Vector3 temp;
	private int count = 0;
	private bool hasKey = false;
	private bool stingPlayed = false;

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
		if (requiresKey && !hasKey){
			return;
		}
		if(door1.transform.position == finalPos1 && count == 0)
		{
			if(canLerp){
				StartCoroutine("LerpClosed");
			}
		}
		if(door1.transform.position == door1Init && count > 0)
		{
			if(canLerp){
				StartCoroutine("LerpOpen");
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
			if(canLerp && !(requiresKey && !hasKey)){
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
		if (objectToSpawn!=null && !stingPlayed){
			objectToSpawn.SetActive(true);
			StartCoroutine("DelayForSound");
		}
		AudioSource.PlayClipAtPoint(openClip, transform.position, .75f);
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
		AudioSource.PlayClipAtPoint(closeClip, transform.position, .75f);
		for (float t=0; t<1f; t+=Time.smoothDeltaTime/1.25f){
			door1.transform.position = temp = Vector3.Lerp(finalPos1, door1Init, t);
			door2.transform.position = temp = Vector3.Lerp(finalPos2, door2Init, t);
			yield return null;
		}
		canLerp = true;
		door1.transform.position = door1Init;
		door2.transform.position = door2Init;
	}

	private IEnumerator DelayForSound(){
		yield return new WaitForSeconds(stingerDelay);
		AudioSource.PlayClipAtPoint(stingerClip, transform.position, 1f);
		stingPlayed = true;
	}

}
