using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendulum : MonoBehaviour
{
    public float speed = .5f; //скорость туда-сюда
    public float amp = 180; //величина размаха

    void Update()
    {
 //       transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Sin(Time.time * speed) * amp));
        transform.eulerAngles = new Vector3(0, 0, Mathf.PingPong(Time.time * 60, 90) - 45);
 
//        transform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(-35f, 35f, Time.time));
       
    }
}
