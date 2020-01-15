using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

public class HandleScript : MonoBehaviour
{

    private float distance = 20f;

    private void Start() { }


    private void Update()
    {
    }

    void OnMouseDrag()
    {
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance); // переменной записываються координаты мыши по иксу и игрику
        Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition); // переменной - объекту присваиваеться переменная с координатами мыши

        //        transform.position = new Vector3(objPosition.x, 0f, objPosition.z); // и собственно объекту записываються координаты
//        var rb = transform.GetComponent<Rigidbody>();
//        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
        transform.position = objPosition; // и собственно объекту записываються координаты
    }
}
