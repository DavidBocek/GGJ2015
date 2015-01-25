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
	}
	
	// Update is called once per frame
	void Update () 
	{
		//mouse click
		if (Input.GetButtonDown("Fire1")){
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

		mouseX = Input.GetAxis ("Mouse X");

		transform.Rotate(Vector3.up, mouseX*mouseSens);

		mouseY = Input.GetAxis ("Mouse Y");

		gameObject.GetComponentInChildren<Camera>().transform.Rotate (Vector3.right, -.5f*mouseY*mouseSens);

		isSprinting = Input.GetButton("Sprint") && curSprint > 0 && rb.velocity.magnitude > 0;
		
	}

	void FixedUpdate()
	{
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

	public void EnterGUIState(){

	}

	public void ExitGUIState(){

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

	private void OnCollisionEnter(Collision coll){
		if(coll.gameObject.CompareTag("Stairs")){
			onStairs = true;
		}
	}

	private void OnCollisionExit(Collision coll){
		if(coll.gameObject.CompareTag("Stairs")){
			onStairs = false;
		}
	}
}
