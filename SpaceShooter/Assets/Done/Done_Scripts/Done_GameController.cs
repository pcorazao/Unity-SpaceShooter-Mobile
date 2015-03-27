using UnityEngine;
using System.Collections;
using Ligabo;

public class Done_GameController : MonoBehaviour
{
	public GameObject[] hazards;
	public Vector3 spawnValues;
	public int hazardCount;
	public float spawnWait;
	public float startWait;
	public float waveWait;
	
	public GUIText scoreText;
	public GUIText restartText;
	public GUIText gameOverText;
	
	private bool gameOver;
	private bool restart;
	private int score;

	public bool started = false;
	public bool paused = false;
	
	void Start ()
	{
		gameOver = false;
		restart = false;
		restartText.text = "";
		gameOverText.text = "";
		score = 0;
		UpdateScore ();
		Time.timeScale=0;
		//StartCoroutine (SpawnWaves ());
	}

	void OnGUI(){
		
		float butWidth=300f;
		LigaboUtils.GUIScaleBegin();
		
		if (!started && LigaboState.status!=LigaboState.Status.Running){
			if (GUI.Button(new Rect(LigaboUtils.screenWidth/2-butWidth/2, LigaboUtils.screenHeight/2-butWidth/5/2,butWidth,butWidth/5),"Start Game"))
				LigaboState.status=LigaboState.Status.Running;
		}
		
		LigaboUtils.GUIScaleEnd();
	}

	void Update ()
	{

		if (!started && LigaboState.statusTracking==LigaboState.StatusTracking.Tracking) {
			StartGame();
		}

		if (started && LigaboState.statusTracking!=LigaboState.StatusTracking.Tracking && !paused)
			PauseGame();
		
		if (started && paused && LigaboState.statusTracking==LigaboState.StatusTracking.Tracking )
			ResumeGame();


		if (restart)
		{
			if (Input.GetButton("Fire1") || Input.touchCount > 0)
			{
				Application.LoadLevel (Application.loadedLevel);
			}
		}
	}

	void StartGame(){
		StartCoroutine (SpawnWaves ());
		LigaboState.status=LigaboState.Status.Running;
		started=true;
		Time.timeScale=1;
	}

	void PauseGame(){
		Time.timeScale=0;
		paused=true;
	}
	
	void ResumeGame(){
		Time.timeScale=1;
		paused=false;
	}

	IEnumerator SpawnWaves ()
	{
		yield return new WaitForSeconds (startWait);
		while (true)
		{
			for (int i = 0; i < hazardCount; i++)
			{
				GameObject hazard = hazards [Random.Range (0, hazards.Length)];
				Vector3 spawnPosition = new Vector3 (Random.Range (-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
				Quaternion spawnRotation = Quaternion.identity;
				Instantiate (hazard, spawnPosition, spawnRotation);
				yield return new WaitForSeconds (spawnWait);
			}
			yield return new WaitForSeconds (waveWait);
			
			if (gameOver)
			{
				restartText.text = "Tap to restart";
				restart = true;
				break;
			}
		}
	}
	
	public void AddScore (int newScoreValue)
	{
		score += newScoreValue;
		UpdateScore ();
	}
	
	void UpdateScore ()
	{
		scoreText.text = "Score: " + score;
	}
	
	public void GameOver ()
	{
		gameOverText.text = "Game Over!";
		gameOver = true;
	}
}