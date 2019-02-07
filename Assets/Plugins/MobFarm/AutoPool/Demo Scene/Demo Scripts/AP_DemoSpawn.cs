using UnityEngine;
using System.Collections;

public class AP_DemoSpawn : MonoBehaviour {

	public GameObject spawnPrefab;
	public bool randomChild;
	public int addToPool;
	public int minPool;
	public float spawnInterval;
	public float spawnVelocity;
	public float spawnAngleError;

	float nextSpawn;
	Rigidbody myRigidbody;

	void Awake () {
		myRigidbody = gameObject.GetComponent<Rigidbody>();
		MF_AutoPool.InitializeSpawn( spawnPrefab, addToPool, minPool );
	}

	void Update () {
		if ( Time.time >= nextSpawn ) {

			Vector2 errorV2 = Random.insideUnitCircle * spawnAngleError;
			Quaternion spawnAngle = Quaternion.Euler( errorV2.x, errorV2.y, 0 );

			GameObject obj = null;
			if ( randomChild == true ) {
				obj = MF_AutoPool.Spawn( spawnPrefab, Random.Range(0,3), transform.position + (spawnAngle * transform.forward), transform.rotation * spawnAngle );
			} else {
				obj = MF_AutoPool.Spawn( spawnPrefab, transform.position + (spawnAngle * transform.forward), transform.rotation * spawnAngle );
			}

			// add some force and some random direction
			Rigidbody rb = null;
			if ( obj ) { rb = obj.GetComponent<Rigidbody>(); }
			if ( rb ) {
				Vector3 myVelocity = myRigidbody ? myRigidbody.velocity : Vector3.zero;
				rb.velocity = myVelocity + ( obj.transform.forward * spawnVelocity );
			}

			nextSpawn = Time.time + spawnInterval;
		}
	}
}
