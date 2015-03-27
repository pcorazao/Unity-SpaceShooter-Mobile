using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour {

	public GameObject explosion;
	public GameObject playerExplosion;
	private GameController gameController;
	public int scoreValue;

	void Start()
	{
		var gameControllerObject = GameObject.FindGameObjectWithTag ("GameController");
		if (gameControllerObject != null)
			gameController = gameControllerObject.GetComponent<GameController> ();

		if(gameController == null)
			Debug.Log("can't find gameController");
	}

	// Update is called once per frame
	void OnTriggerEnter(Collider other) {

		if (other.tag == "Boundary") 
			return;

		if (other.tag == "Player") 
			Instantiate (playerExplosion, other.transform.position, other.transform.rotation); //create a new game object



		Instantiate (explosion, transform.position, transform.rotation); //create a new game object


		gameController.AddScore (scoreValue);

//			Debug.Log (other.name);
			Destroy(other.gameObject);
			Destroy (gameObject);

	}

}
