using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class KeypadGUI : MonoBehaviour {

	private Keypad attachedKeypad = null;
	private int currentNumber = 0;

	public void InputKeyValueGUI(){
		System.Int32.TryParse(GetComponentInChildren<InputField>().textComponent.text, out currentNumber);
		GetComponentInChildren<InputField>().textComponent.text = "0000";
		GetComponentInChildren<InputField>().MoveTextStart(false);
		attachedKeypad.InputKeyValue(currentNumber);
		Detach();
	}

	public void Attach(GameObject keypadObj){
		attachedKeypad = keypadObj.GetComponent<Keypad>();
	}
	
	public void Detach(){
		attachedKeypad = null;
	}
}
