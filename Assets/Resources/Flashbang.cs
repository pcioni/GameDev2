﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Flashbang : Item {

	Transform t;
	int direction;
	Vector3 target;
	Vector3 velocity;
	bool blinding;
	List<GameObject> blinders;
	float flash_duration;
	public Sprite blam;

	public override void Activate(){
		activated = true;
		blinding = false;
		t = GetComponent<Transform> ();
		direction = holder.GetComponent<Animator> ().GetInteger ("DirectionState");
		t.position = holder.transform.position;
		flash_duration = 2f;
		blinders = new List<GameObject> ();
		rendy.enabled = true;
		switch (direction)
		{
		case 0: 
			target = t.position + new Vector3 (-3.0f, 0.0f, 0.0f);  
			velocity = new Vector3 (-0.3f, 0.0f, 0.0f);
			break; // Left
		case 1: 
			target = t.position + new Vector3 (3.0f, 0.0f, 0.0f);
			velocity = new Vector3 (0.3f, 0.0f, 0.0f);
			break; // Right
		case 2: 
			target = t.position + new Vector3 (0.0f, 3.0f, 0.0f);  
			velocity = new Vector3 (0.0f, 0.3f, 0.0f);
			break; // Up
		case 3: 
			target = t.position + new Vector3 (0.0f, -3.0f, 0.0f); 
			velocity = new Vector3 (0.0f, -0.3f, 0.0f);
			break; // Down
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (activated) {
			if (!blinding) {
				t.position += velocity;
				if ((t.position - target).magnitude <= 0.1f) {
					rendy.sprite = blam;
					GameObject[] players = GameObject.FindGameObjectsWithTag ("PlayerObject");
					foreach (GameObject player in players) {
						if ((player.transform.position - t.position).magnitude < 2f && player != holder) {
							blinding = true;
							GameObject blinder = (GameObject)Instantiate (Resources.Load ("Prefabs/Environment/Blinder"), player.transform.position, Quaternion.identity);
							blinder.layer = 9 + int.Parse (player.GetComponent<PlayerController> ().PID);
							blinders.Add (blinder);
						}
					}
					if (!blinding) {
						Destroy (gameObject);
					}
				}
			} 
			else {
				flash_duration -= Time.deltaTime;
				if (flash_duration <= 1.9f && rendy.enabled) {
					rendy.enabled = false;
				}
				if (flash_duration <= 0) {
					foreach (GameObject blinder in blinders) {
						Destroy (blinder);
					}
					Destroy (gameObject);
				}
			}
		}
	}
}
