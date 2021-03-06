﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Es.InkPainter;

public class Ball : MonoBehaviour
{
    GameManager gameManager;
    private GameObject parent;

    void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    void Update() { }

    private void OnCollisionEnter(Collision collision)
    {
        //       GameObject.FindObjectOfType<CameraMovement>().SetOffset(collision.gameObject);        
        if (collision.gameObject.name != "Background") // gameObject.name != "BallInkPaint(Clone)"
        {
            gameManager.changeColor(collision.gameObject);
            gameManager.animatePiece(collision.gameObject);
            gameManager.ProgressBar();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Gate")
        {
            //            Debug.Log("Gate " + other.name);
            gameManager.changeColor(other.gameObject);
        }

        if (other.tag == "Obstacle")
        {
            gameManager.FailLevel();
        }
        //           if (other.tag == "Plate")
        //           {
        //               Debug.Log("!!!! " + other.name);
        //               parent = other.gameObject; // GameObject.Find("ConveyourBelt(Clone)");
        //               transform.parent = parent.transform;
        //           }
    }
}