using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour {

	public GameObject explosion;
	public GameObject playerExplosion;

	// Update is called once per frame
	void OnTriggerEnter(Collider other) {

		if (other.tag == "Boundary") 
			return;

		if (other.tag == "Player") 
			Instantiate (playerExplosion, other.transform.position, other.transform.rotation); //create a new game object

		Instantiate (explosion, transform.position, transform.rotation); //create a new game object

//			Debug.Log (other.name);
			Destroy(other.gameObject);
			Destroy (gameObject);

	}

}
