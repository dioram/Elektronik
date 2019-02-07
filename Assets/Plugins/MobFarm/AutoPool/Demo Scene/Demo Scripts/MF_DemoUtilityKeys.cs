using UnityEngine;
using System.Collections;

[HelpURL("http://mobfarmgames.weebly.com/mf_utilitykeys.html")]
public class MF_DemoUtilityKeys : MonoBehaviour {

	[Tooltip("Pause/Unpause the game.")]
	public string pause = "p";
	[Tooltip("Plays one frame every time the key is pressed.")]
	public string frame = "f";
	[Tooltip("The frame key will play frames while held down")]
	public bool playWhileHeld;

	[Header("Time Scale:")]
	[Tooltip("Halves the current time scale.")]
	public string timeDecrease = "[";
	[Tooltip("Doubles the current time scale.")]
	public string timeIncrease = "]";
	[Tooltip("Shows the current time scale, or may be used to enter a specific time scale.")]
	[SerializeField] private float _playScale = 1f;
	public float playScale { 
		get { return _playScale; }
		set { _playScale = value;
			if ( paused == false ) { Time.timeScale = _playScale; }
		}
	}

	void OnValidate () {
		playScale = _playScale;
	}

	bool paused;
	bool oneFrame;

	void Start () {
		OnValidate(); // so validate is called when run in a build player
		AudioListener.pause = false;
	}

	void Update () {

		if ( oneFrame == true ) {
			Pause( true );
		}

		if ( Input.GetKeyDown( pause ) ) {
			Pause( !paused );
			oneFrame = false;
		}

		if ( Input.GetKeyDown( frame ) ) {
			oneFrame = true;
			Pause( false );
		}

		if ( playWhileHeld == true && Input.GetKey( frame ) ) {
			oneFrame = true;
			Pause( false );
		}

		if ( Input.GetKeyDown( timeDecrease ) ) {
			playScale *= .5f;
		}

		if ( Input.GetKeyDown( timeIncrease ) ) {
			playScale *= 2f;
		}

	}

	void Pause ( bool p ) {
		if ( p == true ) {
			Time.timeScale = 0f;
		} else {
			Time.timeScale = _playScale;
		}
		AudioListener.pause = p;
		paused = p;
	}
}
