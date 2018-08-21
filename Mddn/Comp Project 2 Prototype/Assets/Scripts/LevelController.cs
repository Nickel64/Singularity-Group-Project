﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour {

    public GameObject player;
    public GameObject Wormhole;

    public Image panel;
    public Image gameOverPanel;

    public Canvas gameOverUI;
    public Canvas gameUI;

    int waveCount = 0;
    float timeBetweenPortals = 30f;
    float portalCooldown = 0f;

    ArrayList portalPairs;



    // Use this for initialization
    void Start () {

        Color fadeToClear = new Color(1f, 1f, 1f, 0f);
        panel.GetComponent<FadeIn>().PanelFade(fadeToClear, 3f, false);

        
	}
	
	// Update is called once per frame
	void Update () {


        if (portalCooldown <= 0)
        {
            GetComponent<AudioSource>().Play();
            OpenPairedPortals();
        }
        else
            portalCooldown -= Time.deltaTime;


        if (player == null) {
            Debug.Log("End the Game");
            EndTheGame();
            Time.timeScale = 0.2f;
        }
		
	}

    void DestroyPortals() {
        foreach (GameObject a in portalPairs)
        {
            Destroy(a);
        }
    }

    ArrayList OpenPairedPortals() {

        portalCooldown = timeBetweenPortals;

        int numPortalPairs = Random.Range(1,3);
        portalPairs = new ArrayList();

        for (int n = 0; n < numPortalPairs; n++) {

            Vector3 locationA = new Vector3(Random.Range(0, 1000), Random.Range(0, 500), Random.Range(-300, 300));
            Vector3 locationB = new Vector3(Random.Range(0, 1000), Random.Range(0, 500), Random.Range(-300, 300));

            GameObject portalA = GameObject.Instantiate(Wormhole, locationA, Random.rotation);
            GameObject portalB = GameObject.Instantiate(Wormhole, locationB, Random.rotation);

            portalA.GetComponent<Teleport>().sisterWormhole = portalB;
            portalA.GetComponent<Teleport>().player = player;
            portalA.GetComponent<Teleport>().panel = panel;

            portalB.GetComponent<Teleport>().sisterWormhole = portalA;
            portalB.GetComponent<Teleport>().player = player;
            portalB.GetComponent<Teleport>().panel = panel;

            portalPairs.Add(portalA);
            portalPairs.Add(portalB);
        }

        return portalPairs;
    }

    void openLevel() {

    }

    void EndTheGame() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        gameOverPanel.GetComponent<FadeIn>().PanelFade(new Color(0f, 0f, 0f, 255f), 2f, false); 
        gameUI.enabled = false;
        gameOverUI.enabled = true;
        Time.timeScale = 0.2f;

    }
}
