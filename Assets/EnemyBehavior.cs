﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyBehavior : MonoBehaviour {

	Transform target;
	Transform t;
	public float max_speed;
	public float slowing_radius;
	Vector3 velocity;
	bool attacking;
	int attack_timer;



	void Start () {
		target = GameObject.FindGameObjectWithTag ("PlayerObject").GetComponent<Transform> ();
		t = GetComponent<Transform> ();
		velocity = Vector3.zero;
	}

	Vector3 DynamicSeek(Vector3 target2) {
		Vector3 desired_velocity = target2 - t.position;
		float target_distance = desired_velocity.magnitude;
		desired_velocity.Normalize ();
		desired_velocity *= max_speed;

		if (target_distance < slowing_radius) {
			desired_velocity *= (target_distance / slowing_radius);
			if (target_distance < slowing_radius * 0.45f) {
				desired_velocity *= 0.5f;
				if (target_distance < slowing_radius * 0.3f) {
					desired_velocity *= 0f;
					if (!attacking) {
						attacking = true;
						attack_timer = 0;
					}
				} 
				else if (attacking) {
					attacking = false;
				}
			}

		}

		Vector3 steering = desired_velocity - velocity;
		steering = Vector3.ClampMagnitude (steering, 0.005f);

		return steering;
	}

	void OnCollisionEnter2D(Collision2D other){
		if (other.gameObject.tag == "PlayerObject") {
			Physics2D.IgnoreCollision (GetComponent<Collider2D> (), other.collider, true);
		}
	}

	void Attack(){
		attack_timer -= 1;
		if (attack_timer <= 0) {
			target.GetComponent<Consciousness> ().TakeDamage (1);
			//Debug.Log ("I have " + target.GetComponent<movement> ().health + " health left");
			attack_timer = 60;
		}
	}

	void Update () {
		if (target == null) {
			target = GameObject.FindGameObjectWithTag ("PlayerObject").GetComponent<Transform> ();
		}
		velocity += DynamicSeek (target.position);
		velocity.z = 0;
		t.position += velocity;
		if (attacking) {
			Attack ();
		}
	}

}

 