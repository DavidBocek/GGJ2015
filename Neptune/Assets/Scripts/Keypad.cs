using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Keypad : MonoBehaviour {

	public DoubleDoorController doorController;
	public GameObject keypadGUIObj;
	public int keyValue;
	public AudioClip incorrectClip;
	public AudioClip correctClip;
	public AudioSource keyAudioSource;

	private bool GUIActive = false;

	void OnPlayerClicked(){
		if (!GUIActive){
			keypadGUIObj.GetComponent<KeypadGUI>().Attach(gameObject);
			foreach (Transform transform in keypadGUIObj.GetComponentsInChildren<Transform>()){
				if (!transform.gameObject == keypadGUIObj){
					//
				}
			}
			GUIActive = true;
			GameObject.FindWithTag("Player").GetComponent<Movement>().EnterGUIState();
		}
	}

	public void InputKeyValue(int value){
		if (value == keyValue){
			doorController.UnlockDoor();
			keyAudioSource.clip = correctClip;
			keyAudioSource.Play();
			DeactivateGUI();
		} else {
			keyAudioSource.clip = incorrectClip;
			keyAudioSource.Play();
			DeactivateGUI();
		}
	}

	public void DeactivateGUI(){
		foreach (Transform transform in keypadGUIObj.GetComponentsInChildren<Transform>()){
			if (!transform.gameObject == keypadGUIObj) transform.gameObject.SetActive(false);
		}
		GUIActive = false;
		GameObject.FindWithTag("Player").GetComponent<Movement>().ExitGUIState();
	}

}
