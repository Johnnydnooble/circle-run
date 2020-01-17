using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleCube : MonoBehaviour
{
  
    int direction = 0;

    void Start()
    {
        StartCoroutine(ScaleCub());
    }

    // Update is called once per frame
    void Update()
    {
        if (direction == 1)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(transform.localScale.x, 1f, transform.localScale.z), 2f * Time.deltaTime);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(transform.localScale.x, .001f, transform.localScale.z), Time.deltaTime);
        }
    }

    IEnumerator ScaleCub()
    {
        yield return new WaitForSeconds(6f);
    
        direction = direction == 1 ? -1 : 1;

        StartCoroutine(ScaleCub());
    }
}
