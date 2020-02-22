using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {
    private int CurrentNumberOfEnemies;

	// Use this for initialization
	void Start () {
        CurrentNumberOfEnemies = 0;
    }

    // Update is called once per frame
    void Update() {
        if(CurrentNumberOfEnemies <= 0)
        {
            //spawn portal to go to to next level
        }
	}
    public void IncreaseEnemyCount()
    {
        CurrentNumberOfEnemies++;
    }
    public void DecreaseEnemyCount()
    {
        CurrentNumberOfEnemies--;
    }
}
