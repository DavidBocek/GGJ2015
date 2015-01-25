using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Keypad : MonoBehaviour {

	public DoubleDoorController doorController;
	public GameObject keypadGUIObj;
	public GameObject[] keypadGUIObjs;
	public int keyValue;
	public AudioClip incorrectClip;
	public AudioClip correctClip;
	public AudioSource keyAudioSource;

	private bool GUIActive = false;

	void OnPlayerClicked(){
		if (!GUIActive){
			keypadGUIObj.GetComponent<KeypadGUI>().Attach(gameObject);
			foreach (GameObject obj in keypadGUIObjs){
				obj.SetActive(true);
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
		foreach (GameObject obj in keypadGUIObjs){
			obj.SetActive(false);
		}
		GUIActive = false;
		GameObject.FindWithTag("Player").GetComponent<Movement>().ExitGUIState();
	}

}
