﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIAnimation : MonoBehaviour {

    private Image img;
    private SpriteRenderer rend;
    private Animator anim;

    private string name;

	// Use this for initialization
	void Start () {

        img  = GetComponent<Image>();
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        name = "";
	}
	
	// Update is called once per frame
	void Update () {

        if (rend.sprite != null)
        {
            if (!name.Equals(rend.sprite.name) || true)
            {
                img.sprite = rend.sprite;
                name = rend.sprite.name;
            }
        }

        

	}
}