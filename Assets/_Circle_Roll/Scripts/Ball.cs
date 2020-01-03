using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    GameManager gameManager;
    
    void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    
    void Update() { }


    private void OnCollisionEnter(Collision collision)
    {
        //       GameObject.FindObjectOfType<CameraMovement>().SetOffset(collision.gameObject);        
        if (gameObject.name != "BallInkPaint(Clone)")
        {
           gameManager.changeColor(collision.gameObject);
           gameManager.animatePiece(collision.gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {        
        if (other.tag == "Obstacle")
        {
           gameManager.FailLevel();
        }
    }
}
