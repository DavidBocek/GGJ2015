using UnityEngine;
using System.Collections;

public class HeadBob : MonoBehaviour {

	private float timer = 0.0f; 
	public float bobbingSpeed = 0.18f; 
	public float bobbingAmount = 0.2f; 
	private Vector3 vectChange;
	private Movement movementScr;
	private bool canLerp;
	private bool actuallySprinting;

	void Start(){
		movementScr = GetComponentInParent<Movement>();
		canLerp = true;
	}
	
	void Update () { 
		actuallySprinting = movementScr.isSprinting && movementScr.canSprint;
		bobbingSpeed = actuallySprinting ? .3f : .18f;

		if (actuallySprinting && canLerp && gameObject.camera.fov!=70f){
			StartCoroutine("lerpOut",gameObject.camera.fov);
		} else if (!actuallySprinting && canLerp && gameObject.camera.fov!=60f){
			StartCoroutine("lerpIn",gameObject.camera.fov);
		}

		float waveslice = 0.0f; 
		float horizontal = Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D) ? 0f : Input.GetAxis("Horizontal"); 
		float vertical = Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S) ? 0f : Input.GetAxis("Vertical"); 

		if (Mathf.Abs(horizontal) == 0f && Mathf.Abs(vertical) == 0f) { 
			timer = 0.0f; 
		} 
		else { 
			waveslice = Mathf.Sin(timer) - 1; 
			timer = timer + bobbingSpeed; 
			if (timer > Mathf.PI * 2f) { 
				timer = timer - (Mathf.PI * 2f); 
			} 
		} 
		if (waveslice != 0f) { 
			float translateChange = waveslice * bobbingAmount; 
			float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical); 
			totalAxes = Mathf.Clamp (totalAxes, 0.0f, 1.0f); 
			translateChange = totalAxes * translateChange + .5f; 
			vectChange = !GetComponentInParent<Movement>().isFalling ? new Vector3(0f,translateChange,0f) : new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
			transform.localPosition = vectChange; 
		} 
		else { 
			//transform.localPosition.y = midpoint; 
		} 
	}

	private IEnumerator lerpOut(float curFOV){
		canLerp = false;
		for (float t=0; t<1f; t+=Time.smoothDeltaTime/.15f){
			gameObject.camera.fov = Mathf.Lerp(curFOV, 70f, t);
			if (!actuallySprinting){
				break;
			}
			yield return null;
		}
		canLerp = true;
	}

	private IEnumerator lerpIn(float curFOV){
		canLerp = false;
		for (float t=0; t<1f; t+=Time.smoothDeltaTime/.15f){
			gameObject.camera.fov = Mathf.Lerp(curFOV, 60f, t);
			if (actuallySprinting){
				break;
			}
			yield return null;
		}
		canLerp = true;
	}

}
