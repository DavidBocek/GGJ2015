using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour 
{
	private Rigidbody rb;
	Vector3 vel, mouseLoc;
	public Object burret;
	public Transform muzzle;
	public float jumpForce = 1.0f;
	private float mouseX, mouseY, mouseSens;
	public float lastXVel;
	public float lastZVel;
	private CapsuleCollider playerColl;
	private bool isJumping = false;
	public bool isFalling;
	public bool isSprinting;
	private bool onStairs;
	private bool groundRayBool;
	public float maxSprint = 3.0f;
	private float curSprint;
	public float walkSpeed = 5.0f;
	public float sprintSpeed;
	public float moveSpeed;
	public bool canSprint = true;
	public enum state {WALKING, LADDER, AUTO, GUILOCK};
	public state curState;
	private Camera playerCam;
	private bool canLerp;
	private Vector3 temp;
	private Quaternion tempQ;


	// Use this for initialization
	void Start () 
	{
		vel = new Vector3 ();
		rb = GetComponent<Rigidbody> ();
		mouseSens = 2.0f;
		playerColl = GetComponent<CapsuleCollider>();
		curSprint = maxSprint;
		sprintSpeed = 1.4f * walkSpeed;
		moveSpeed = walkSpeed;
		curState = state.WALKING;
		playerCam = gameObject.GetComponentInChildren<Camera>();
		temp = new Vector3();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//cursor
		Screen.showCursor = (curState == state.GUILOCK);
		Screen.lockCursor = !(curState == state.GUILOCK);

		//mouse click
		if (Input.GetButtonDown("Fire1") && curState == state.WALKING){
			RaycastHit hitInfo = new RaycastHit();
			if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(.5f,.5f,0f)), out hitInfo, 2.5f)){
				if (hitInfo.collider.gameObject.CompareTag("Interactable")){
					hitInfo.collider.gameObject.SendMessage("OnPlayerClicked",SendMessageOptions.DontRequireReceiver);
				}
			}
		}

		//mouseLoc = Camera.main.ScreenToWorldPoint (new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20));
		//Quaternion rot = Quaternion.LookRotation (new Vector3 (mouseLoc.x - transform.position.x, 0, mouseLoc.z - transform.position.z));
		
		//Debug.Log (rot.eulerAngles);
		
		//transform.rotation = rot;
		if(curState == state.WALKING){
			mouseX = Input.GetAxis ("Mouse X");

			transform.Rotate(Vector3.up, mouseX*mouseSens);

			mouseY = Input.GetAxis ("Mouse Y");

			//playerCam.transform.Rotate (Vector3.right, -.5f*mouseY*mouseSens);

			Vector3 goalCamRot = playerCam.transform.localEulerAngles;
			float amountToMoveY = -.5f*mouseY*mouseSens;
			goalCamRot.x += amountToMoveY;
			if (goalCamRot.x < 265f && goalCamRot.x > 180f){goalCamRot.x = 265.0f;}
			else if (goalCamRot.x > 80f && goalCamRot.x< 180f){goalCamRot.x = 80.0f;}
			playerCam.transform.localEulerAngles = goalCamRot;

			isSprinting = Input.GetButton("Sprint") && curSprint > 0 && rb.velocity.magnitude > 0;
		} else if (curState == state.LADDER){
			Vector3 goalCamRot = playerCam.transform.localEulerAngles;
			float amountToMoveY = -.5f*mouseY*mouseSens;
			goalCamRot.x += amountToMoveY;
			if (goalCamRot.x < 315f && goalCamRot.x > 180f){goalCamRot.x = 315.0f;}
			else if (goalCamRot.x > 45f && goalCamRot.x< 180f){goalCamRot.x = 45.0f;}
			playerCam.transform.localEulerAngles = goalCamRot;
		}
	}

	void FixedUpdate()
	{
		if(curState == state.WALKING){
			if(isSprinting && canSprint)
			{
				curSprint = Mathf.Max (curSprint - Time.fixedDeltaTime, 0);
				if(curSprint > 0f)
				{
					moveSpeed = sprintSpeed;
				}
				else
				{
					moveSpeed = walkSpeed;
					StartCoroutine("DelaySprint");
				}

			}
			else
			{
				moveSpeed = walkSpeed;
				curSprint = Mathf.Min(curSprint +  Time.fixedDeltaTime * .5f, maxSprint);
			}

			if(!isFalling){
				vel.x = Input.GetAxisRaw ("Horizontal");
				vel.z = Input.GetAxisRaw ("Vertical");
				vel.Normalize ();
				vel *= moveSpeed;
				
				if(Input.GetButton("Jump")){
					vel.y = 5f;
					lastXVel = vel.x;
					lastZVel = vel.z;
				} else {
					vel.y = 0f;
				}
				rb.velocity = transform.TransformDirection (vel);
			} else {
				vel = transform.TransformDirection(new Vector3(lastXVel*moveSpeed,0f,lastZVel*moveSpeed));
				vel.y = rb.velocity.y-19.6f*Time.fixedDeltaTime;
				rb.velocity = vel;
			}

			isFalling = true;
			RaycastHit hitInfo = new RaycastHit();
			if (onStairs){
				isFalling = false;
			} else if (Physics.Raycast (transform.position, Vector3.down, out hitInfo, transform.localScale.y*playerColl.height/2.0f + .25f)){
				if (!hitInfo.collider.isTrigger){
					isFalling = false;
				}
			}
		}
		else if (curState == state.LADDER)
		{
			vel.x = vel.z = 0;
			vel.y = Input.GetAxisRaw("Vertical");
			vel *= walkSpeed/2.0f;
			rb.velocity = vel;
		}
		else if (curState == state.GUILOCK){
			vel = Vector3.zero;
			rb.velocity = vel;
		}

	}

	public void EnterGUIState(){
		curState = state.GUILOCK;
	}

	public void ExitGUIState(){
		curState = state.WALKING;
	}

	private IEnumerator Jump(){
		isJumping = true;
		for(float t=0f; t<.1f; t+=Time.deltaTime){
			rb.AddForce(new Vector3(0,jumpForce,0));
			yield return null;
		}
		isJumping = false;
	}

	private IEnumerator DelaySprint(){
		canSprint = false;
		yield return new WaitForSeconds(2.0f);
		canSprint = true;
	}

	private void OnCollisionExit(Collision coll){
		if(coll.gameObject.CompareTag("Stairs")){
			onStairs = false;
		}
	}

	private void OnCollisionEnter(Collision coll){
		if(coll.gameObject.CompareTag("Stairs")){
			onStairs = true;
		}
	}

	private void OnTriggerEnter(Collider coll){
		if(coll.gameObject.CompareTag("Ladder"))
		{
			if(curState == state.WALKING)
			{
				StartCoroutine(MountLadder(coll, transform));
			}
			else if(curState == state.LADDER)
			{
				StartCoroutine(DismountLadder(coll, transform));
			}
		}
	}

	private IEnumerator DismountLadder(Collider coll, Transform playerInit){
		Transform dest = coll.gameObject.GetComponentsInChildren<Transform>()[1];
		for (float t=0; t<1f; t+=Time.smoothDeltaTime){
			transform.position = temp = Vector3.Lerp(playerInit.position, dest.position, t);
			transform.rotation = tempQ = Quaternion.Lerp (playerInit.rotation, dest.rotation, t);
			yield return null;
		}
		transform.position = dest.position;
		transform.rotation = dest.rotation;
		curState = state.WALKING;
	}

	private IEnumerator MountLadder(Collider coll, Transform playerInit){
		curState = state.LADDER;
		Transform dest = coll.gameObject.transform;
		for (float t=0; t<1f; t+=Time.smoothDeltaTime){
			transform.position = temp = Vector3.Lerp(playerInit.position, dest.position, t);
			transform.rotation = tempQ = Quaternion.Lerp (playerInit.rotation,dest.rotation, t);
			yield return null;
		}
		Debug.Log("Done");
		transform.position = dest.position;
		transform.rotation = dest.rotation;
	}
}

public GameObject[] shitToTurnOn;


playerjkasdhkash(){
	foreach (GameObject obj in shitToTurnOn){
		obj.SetActive(true);
	}
}
