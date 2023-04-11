using Auxiliars;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InstructionManager : MonoBehaviour {
	
	[SerializeField]
	private TextMeshProUGUI instructionText;
	[SerializeField]
	private Image backgroundImage;
	[SerializeField]
	private Image instructionPlaceImage;

	public float readTime = 2f;
	private SpartanTimer initialInstTimer;

	private void Start() {
		this.initialInstTimer = new SpartanTimer(TimeMode.Framed);
		this.ShowInitialInstructions();
	}

	private void Update() {
		if (this.initialInstTimer.Started) {
			float currSeconds = this.initialInstTimer.GetCurrentTime(TimeScaleMode.Seconds);
			if (currSeconds >= readTime) 
				this.DismissInitialInstructions();
		}
	}

	private void ShowInitialInstructions() {
		this.instructionText.text = "Presiona el botón \"+\" y añade puntos en las esquinas de la habitación que deseas decorar. (izquierda superior, derecha superior, izquierda inferior, derecha inferior)";
		this.backgroundImage.gameObject.SetActive(true);
		this.instructionPlaceImage.gameObject.SetActive(true);
		this.initialInstTimer.Start();
	}

	private void DismissInitialInstructions() {
		this.instructionText.text = "";
		this.backgroundImage.gameObject.SetActive(false);
		this.instructionPlaceImage.gameObject.SetActive(false);
		this.initialInstTimer.Stop();
	}


}
