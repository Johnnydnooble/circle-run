using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public enum CameraMode { Fixed, Dynamic, OnlyUp, SlowOnMoveDown };
    public CameraMode cameraMode;
    //we only care about height distance to the camera
    //because the movement on the plate is not just vertical, we need to augment by more than just 1xY
    float cameraOffsetY;
    public float currentYBall;
    public float smoothSpeed = 1000.0f;
    public bool offsetDetermined;
    public float initialYBall;

    //scale by some factor of the initil offset
    float initialOffset;
    float persistentYDistanceOfBallFromInitialY;

    //for 3:4, distance of around 18 in y is good from the ball.

    void Start()
    {
        
        
    }

    void FixedUpdate()
    {
        //Camera Movement Logic Here
        if (cameraOffsetY != 0)
        {
            if (cameraMode == CameraMode.Dynamic)
            {
                currentYBall = GameObject.FindObjectOfType<Ball>().transform.position.y;
                float yDistanceOfBallFromInitialY = currentYBall - initialYBall;
                Debug.Log("Gap is: " + yDistanceOfBallFromInitialY);

                float desiredPositionY = currentYBall + cameraOffsetY + yDistanceOfBallFromInitialY * 1.5f;
                float desiredPositionYSmooth = Mathf.Lerp(transform.position.y, desiredPositionY, 1.0f);


                Vector3 newTransformPosition = new Vector3(x: transform.position.x, y: desiredPositionYSmooth, z: transform.position.z);
                transform.position = newTransformPosition;
            }

            if (cameraMode == CameraMode.OnlyUp)
            {
                currentYBall = GameObject.FindObjectOfType<Ball>().transform.position.y;

                float yDistanceOfBallFromInitialY = currentYBall - initialYBall;
                Debug.Log(yDistanceOfBallFromInitialY);
                if (yDistanceOfBallFromInitialY < persistentYDistanceOfBallFromInitialY)
                {
                    
                } else
                {                    
                    persistentYDistanceOfBallFromInitialY = yDistanceOfBallFromInitialY;

                    float desiredPositionY = currentYBall + cameraOffsetY + yDistanceOfBallFromInitialY * 1.3f;
                    float higherYCameraPosition = Mathf.Max(yDistanceOfBallFromInitialY, persistentYDistanceOfBallFromInitialY);
                    
                    Vector3 newTransformPosition = new Vector3(x: transform.position.x, y: desiredPositionY, z: transform.position.z);
                    transform.position = newTransformPosition;
                }
            }

            if (cameraMode == CameraMode.SlowOnMoveDown)
            {
                currentYBall = GameObject.FindObjectOfType<Ball>().transform.position.y;

                float yDistanceOfBallFromInitialY = currentYBall - initialYBall;
                Debug.Log(yDistanceOfBallFromInitialY);
                if (yDistanceOfBallFromInitialY < persistentYDistanceOfBallFromInitialY)
                {
                    float desiredPositionY = currentYBall + cameraOffsetY + yDistanceOfBallFromInitialY*1.3f;
                    Vector3 newTransformPosition = new Vector3(x: transform.position.x, y: desiredPositionY, z: transform.position.z);
                    transform.position = newTransformPosition;
                }
                else
                {
                    persistentYDistanceOfBallFromInitialY = yDistanceOfBallFromInitialY;

                    float desiredPositionY = currentYBall + cameraOffsetY + yDistanceOfBallFromInitialY * 1.3f;
                    

                    Vector3 newTransformPosition = new Vector3(x: transform.position.x, y: desiredPositionY, z: transform.position.z);
                    transform.position = newTransformPosition;
                }
            }

            if (cameraMode == CameraMode.Fixed)
            {
               
            }
        }
    }

    public void SetOffset(GameObject piece)
    {
        if (!offsetDetermined)
        {
            initialYBall = GameObject.FindObjectOfType<Ball>().transform.position.y;
            cameraOffsetY = transform.position.y - piece.transform.position.y;
            initialOffset = cameraOffsetY;
            offsetDetermined = true;            
        }
    }
}
