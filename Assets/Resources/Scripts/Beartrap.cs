﻿using UnityEngine;
using System.Collections;

public class Beartrap : Item {

	bool primed;

	public override void Activate(){
		holder.GetComponent<PlayerController> ().held_item = null;
		activated = true;
		transform.position = holder.transform.position;
		collidy.radius = 1f;
		collidy.enabled = true;
		primed = false;
		rendy.enabled = true;
		StartCoroutine (Prime ());
	}

	void OnTriggerEnter2D(Collider2D other){
		if (activated) {
			if (primed && other.tag == "PlayerObject") {
				other.transform.position = transform.position + new Vector3(0f, 1f, 0f);
				other.GetComponent<PlayerController> ().unconscious = true;
				other.GetComponent<PlayerController> ().locked = true;
				StartCoroutine (ReleaseVictim (other.GetComponent<PlayerController> ()));
			} 
			else {
				return;
			}
		}
		else if (other.tag == "PlayerObject") {
			other.GetComponent<PlayerController> ().item_list.Add (this);
			ring.enabled = true;
		}
	}

	IEnumerator Prime(){
		yield return new WaitForSeconds (2f);
		primed = true;
	}

	IEnumerator ReleaseVictim(PlayerController victim){
		yield return new WaitForSeconds (10f);
		victim.unconscious = false;
		victim.locked = false;
		Destroy (gameObject);
	}
}
