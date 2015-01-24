using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour 
{
	public float moveSpeed = 1.0f;
	private Rigidbody rb;
	Vector3 vel, mouseLoc;
	public Object burret;
	public Transform muzzle;
	public float jumpForce = 1.0f;
	private float mouseX, mouseY, mouseSens;
	private bool isJumping = false;
	public bool isFalling;
	public float lastXVel;
	public float lastZVel;
	private CapsuleCollider playerColl;
	private bool onStairs;
	private bool groundRayBool;

	// Use this for initialization
	void Start () 
	{
		vel = new Vector3 ();
		rb = GetComponent<Rigidbody> ();
		mouseSens = 2.0f;
		playerColl = GetComponent<CapsuleCollider>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//mouseLoc = Camera.main.ScreenToWorldPoint (new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20));
		//Quaternion rot = Quaternion.LookRotation (new Vector3 (mouseLoc.x - transform.position.x, 0, mouseLoc.z - transform.position.z));
		
		//Debug.Log (rot.eulerAngles);
		
		//transform.rotation = rot;

		mouseX = Input.GetAxis ("Mouse X");

		transform.Rotate(Vector3.up, mouseX*mouseSens);

		mouseY = Input.GetAxis ("Mouse Y");

		gameObject.GetComponentInChildren<Camera>().transform.Rotate (Vector3.right, -.5f*mouseY*mouseSens);

		if(Input.GetButtonDown("Fire1"))
		{
			GameObject.Instantiate(burret, muzzle.position, transform.rotation); 
		}

		groundRayBool = Physics.Raycast (transform.position, Vector3.down, playerColl.height/2.0f + .25f);

	}

	void FixedUpdate()
	{
		/*if(!isFalling){
			vel.x = Input.GetAxisRaw ("Horizontal");
			vel.z = Input.GetAxisRaw ("Vertical");
			vel.Normalize ();
			vel *= moveSpeed;
		} else {
			vel.x = transform.InverseTransformDirection(rb.velocity).x;
			vel.z = transform.InverseTransformDirection(rb.velocity).z;
		}

		vel.y = Input.GetButton("Jump") && !isFalling ? 5f : rb.velocity.y-19.6f*Time.deltaTime;

		if(!isFalling && !Input.GetButton("Jump")){
			vel.y=0;
		}
		Debug.Log ("vel: " + vel + " transformdir: " + transform.TransformDirection (vel));
		rb.velocity = transform.TransformDirection (vel);

		isFalling = Physics.Raycast (transform.position, Vector3.down, .5f + .05f) ? false : true; */
	

		if(!isFalling){
			vel.x = Input.GetAxisRaw ("Horizontal");
			vel.z = Input.GetAxisRaw ("Vertical");
			vel.Normalize ();
			vel *= moveSpeed;

			if(Input.GetButton("Jump")){
				vel.y = 5f;
				lastXVel = rb.velocity.x;
				lastZVel = rb.velocity.z;
			} else {
				vel.y = 0f;
			}
			rb.velocity = transform.TransformDirection (vel);
		} else {
			vel.x = lastXVel;
			vel.z = lastZVel;
			vel.y = rb.velocity.y-19.6f*Time.fixedDeltaTime;
			rb.velocity = vel;
		}

		Debug.Log (isFalling + " " + onStairs);

		
		isFalling = !groundRayBool && !onStairs;

	}

	private IEnumerator Jump(){
		isJumping = true;
		for(float t=0f; t<.1f; t+=Time.deltaTime){
			rb.AddForce(new Vector3(0,jumpForce,0));
			yield return null;
		}
		isJumping = false;
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
