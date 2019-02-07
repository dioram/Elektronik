using UnityEngine;
using System.Collections;

public class AP_DemoDeathTimer : MonoBehaviour {

	[Header("Begin State:")]
	public GameObject enabledObject;
	public GameObject disabledObject;
	[Header("Versions:")]
	public GameObject[] version;
	[Header("Swap Visuals:")]
	public float deathTime = 10f;
	[Header("Despawn:")]
	public float despawnTime = 1f;

	Rigidbody myRigidbody;
	AP_Reference refScript;
	bool didDeath;
	float startTime;

	void Awake () {
		myRigidbody = gameObject.GetComponent<Rigidbody>();
		refScript = gameObject.GetComponent<AP_Reference>();
		if ( refScript == null ) { Debug.Log( gameObject.name + ": No AP_Reference script found. Object won't despawn."); }
	}

	void OnEnable () {
		// reset object
		startTime = Time.time;
		didDeath = false;
		if ( myRigidbody ) {
			myRigidbody.isKinematic = false;
			myRigidbody.velocity = Vector3.zero;
			myRigidbody.angularVelocity = Vector3.zero;
		}
		// set begining state to hide blast and show object
		if ( enabledObject ) { enabledObject.SetActive( true ); }
		if ( disabledObject ) { disabledObject.SetActive( false ); }
		for ( int i=0; i < version.Length; i++ ) {
			if ( version[i] ) { version[i].SetActive( false ); }
		}
	}

	void Update () {
		// check if time to die
		if ( didDeath == false && Time.time >= startTime + deathTime ) {
			if ( myRigidbody ) { myRigidbody.isKinematic = true; }
			// swap states of objects to show blast and hide object
			if ( enabledObject ) { enabledObject.SetActive( false ); }
			if ( disabledObject ) { disabledObject.SetActive( true ); }
			for ( int i=0; i < version.Length; i++ ) {
				if ( version[i] ) { version[i].SetActive( false ); }
			}
			didDeath = true;
		}
		// check if time to despawn
		if ( Time.time >= startTime + deathTime + despawnTime ) {
			if ( refScript ) {
				if ( MF_AutoPool.Despawn( refScript ) == false ) {
					// if object couldn't be despawned, then destroy it
					Destroy( gameObject );
				}
				// Alternately, we could use:

				// MF_AutoPool.Despawn( gameObject );

				// and not have to bother with initializing AP_Reference at all,
				// but using gameObject is a little slower, since internally it uses GetComponent<MF_AutoPoolreference>() to find the script
			} else {
				// no refScript found
				Destroy( gameObject );
			}
		}
	}
}
