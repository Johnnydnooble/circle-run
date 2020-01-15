using UnityEngine;
using System.Collections;

public class ObjectControl : MonoBehaviour
{
    public string[] tags; // массив тегов, объекты которых можно двигать
    public Camera _camera; // основная камера сцены
    private Transform curObj;
    private float mass;

    void Start()
    {
        //        GameObject camTemp = Camera.Find("Main Camera");
        _camera = Camera.FindObjectOfType<Camera>();        
    }

    bool GetTag(string curTag)
    {
        bool result = false;
        foreach (string t in tags)
        {
            if (t == curTag) result = true;
        }
        return result;
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButton(0)) // Удерживать левую кнопку мыши
        {
            RaycastHit hit;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (GetTag(hit.transform.tag) && hit.rigidbody && !curObj)
                {
                    curObj = hit.transform;
//                    mass = curObj.GetComponent<Rigidbody>().mass; // запоминаем массу объекта
 //                   curObj.GetComponent<Rigidbody>().mass = 0.0001f; // убираем массу, чтобы не сбивать другие объекты
                    curObj.GetComponent<Rigidbody>().useGravity = false; // убираем гравитацию
                    curObj.GetComponent<Rigidbody>().freezeRotation = true; // заморозка вращения
 //                   curObj.position += new Vector3(0, 0.5f, 0); // немного приподымаем выбранный объект
                }
            }

            if (curObj)
            {
                Vector3 mousePosition = _camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _camera.transform.position.y));
                //                curObj.GetComponent<Rigidbody>().MovePosition(new Vector3(mousePosition.x, curObj.position.y, mousePosition.z) * 20f * Time.fixedDeltaTime);
//                curObj.GetComponent<Rigidbody>().MovePosition(new Vector3(mousePosition.x * 20f * Time.fixedDeltaTime, curObj.position.y, mousePosition.z * 20f * Time.fixedDeltaTime));
                curObj.GetComponent<Rigidbody>().MovePosition(new Vector3((Mathf.Clamp(mousePosition.x * 20f * Time.fixedDeltaTime, -1.5f, 1.5f)), curObj.position.y, (Mathf.Clamp(mousePosition.z * 20f * Time.fixedDeltaTime, -1.5f, 1.5f))));

                //               curObj.transform.position = new Vector3(mousePosition.x, curObj.position.y, mousePosition.z);
//                curObj.transform.position = new Vector3((Mathf.Clamp(mousePosition.x * 20f * Time.fixedDeltaTime, -1.5f, 1.5f)), curObj.position.y, (Mathf.Clamp(mousePosition.z * 20f * Time.fixedDeltaTime, -1.5f, 1.5f)));
            }
        }
        else if (curObj)
        {
            curObj.GetComponent<Rigidbody>().freezeRotation = false;
            curObj.GetComponent<Rigidbody>().useGravity = true;
            curObj.GetComponent<Rigidbody>().mass = mass;

            curObj = null;
        }
    }
}
