using UnityEngine;
using System.Collections;

public class HeadBob : MonoBehaviour {

	private float timer = 0.0f; 
	public float bobbingSpeed = 0.18f; 
	public float bobbingAmount = 0.2f; 
	public float midpoint = 2.0f; 
	private Vector3 vectChange;
	
	void Update () { 
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
}
