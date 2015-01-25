using UnityEngine;
using System.Collections;

public class Generator : MonoBehaviour {

	public GameObject door;
	public Transform dest, camDest;
	private Transform camInit;
	public Camera swagCam;
	public Camera playerCam;
	public AudioClip button_aud, doorOpen_aud;
	private Vector3 temp, doorInitPos, camDestPos, doorDestPos;
	private Quaternion tempQ, camDestRot;
	private AudioSource playerAudSrc, swagCamAudSrc;

	void Start(){
		camInit = swagCam.transform;
		camDestPos = camDest.position;
		camDestRot = camDest.rotation;
		swagCam.camera.active = false;
		doorInitPos = door.transform.position;
		doorDestPos = dest.position;
		playerAudSrc = playerCam.gameObject.GetComponent<AudioSource>();
		swagCamAudSrc = swagCam.GetComponent<AudioSource>();
	}

	public void OnPlayerClicked(){
		//audio.Play();
		if(door.transform.position != doorDestPos){
			StartCoroutine ("OpenDoor");
		}
	}

	private IEnumerator OpenDoor(){
		playerAudSrc.clip = button_aud;
		playerAudSrc.Play ();
		yield return new WaitForSeconds(.25f);
		playerCam.camera.active = false;
		swagCam.camera.active = true;
		yield return new WaitForSeconds(1f);
		StartCoroutine("PanCam");
		yield return new WaitForSeconds(.1f);
		swagCamAudSrc.clip = doorOpen_aud;
		swagCamAudSrc.Play ();
		for (float t=0; t<1f; t+=Time.smoothDeltaTime/3f){
			door.transform.position = temp = Vector3.Lerp(doorInitPos, doorDestPos, t);
			yield return null;
		}
		door.transform.position = doorDestPos;
		yield return new WaitForSeconds(1f);
		swagCam.camera.active = false;
		playerCam.camera.active = true;
	}

	private IEnumerator PanCam(){
		for (float t=0; t<1f; t+=Time.smoothDeltaTime/20f){
			swagCam.transform.position = temp = Vector3.Lerp(camInit.position, camDestPos, t);
			swagCam.transform.rotation = tempQ = Quaternion.Lerp (camInit.rotation,camDestRot, t);
			yield return null;
		}
		swagCam.transform.position = camDestPos;
		swagCam.transform.rotation = camDestRot;


	}



}
