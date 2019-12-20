using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CamMove : MonoBehaviour
{

 //   public GameObject Player;

    private Vector3 offset;
    public float smooth = 5.0f;

    public Transform m_Player; // Reference to the player's transform.



    void Start()
    {  
        StartCoroutine(FindPlayer());
    }

    void LateUpdate()
    {
        if (m_Player !=  null && PlayerPrefs.GetInt("CurrentLevel", 0) == 1)
        {
            Debug.Log("222222222222222");
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, m_Player.position.y + ((Mathf.Abs(m_Player.position.x) + Mathf.Abs(m_Player.position.z)) * 2) + offset.y , transform.position.z), Time.deltaTime * smooth);
        }
        else if (m_Player != null && PlayerPrefs.GetInt("CurrentLevel", 0) == 0)
        {
            Debug.Log("11111111111");
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, m_Player.position.y + offset.y, transform.position.z), Time.deltaTime * smooth);
        }
     
    }

    IEnumerator FindPlayer()
    {
        yield return new WaitForSeconds(1f);
        m_Player = GameObject.FindGameObjectWithTag("Player").transform;
        offset = transform.position - m_Player.transform.position;
    }
}





/*




bool moveRight, moveLeft; // просто значения влево/вправо - [b]необязательны[/b]
public Transform selfTransform; //сохраняем трансформ нашего объекта и камеры
public Transform mainCamTransform;
[SerializeField]
Camera cam;    //вешаем сюда нашу камеру
Vector3 wantedPosition;

void Start()
{
    mainCamTransform = cam.transform;
    selfTransform = transform;
    StartCoroutine(coUpdate());
}

IEnumerator coUpdate()
{

    while (true)
    {

        if (moveRight)
        {
            wantedPosition = new Vector3(selfTransform.position.x, mainCamTransform.position.y + 100, mainCamTransform.position.z);
        }
        if (moveLeft)
        {
            wantedPosition = new Vector3(selfTransform.position.x, mainCamTransform.position.y - 100, mainCamTransform.position.z);
        }
        mainCamTransform.position = Vector3.Lerp(mainCamTransform.position, wantedPosition, Time.deltaTime * 5.0f); //плавно сдвигает камеру. В нашем случае по X

        yield return 0;
    }




}
}












public float xMargin = 1f; // Distance in the x axis the player can move before the camera follows.
    public float yMargin = 1f; // Distance in the y axis the player can move before the camera follows.
    public float xSmooth = 8f; // How smoothly the camera catches up with it's target movement in the x axis.
    public float ySmooth = 8f; // How smoothly the camera catches up with it's target movement in the y axis.
    public Vector2 maxXAndY; // The maximum x and y coordinates the camera can have.
    public Vector2 minXAndY; // The minimum x and y coordinates the camera can have.

    public Transform m_Player; // Reference to the player's transform.

    public void Start()
    {
        // Setting up the reference.
        m_Player = GameObject.FindGameObjectWithTag("Player").transform;

        m_Player = GameObject.Find("Ball(Clone)").transform;

    }

    private bool CheckYMargin()
    {
        // Returns true if the distance between the camera and the player in the y axis is greater than the y margin.
        return Mathf.Abs(transform.position.y - m_Player.position.y) > yMargin;
    }


    private void Update()
    {
        Debug.Log(m_Player);
        TrackPlayer();

    }


    private void TrackPlayer()
    {
        // By default the target x and y coordinates of the camera are it's current x and y coordinates
        float targetY = transform.position.y;

        // If the player has moved beyond the y margin...
        if (CheckYMargin())
        {
            // ... the target y coordinate should be a Lerp between the camera's current y position and the player's current y position.
            targetY = Mathf.Lerp(transform.position.y, m_Player.position.y, ySmooth * Time.deltaTime);
        }

        // The target x and y coordinates should not be larger than the maximum or smaller than the minimum.
//            targetX = Mathf.Clamp(targetX, minXAndY.x, maxXAndY.x);
        targetY = Mathf.Clamp(targetY, minXAndY.y, maxXAndY.y);

        // Set the camera's position to the target position with the same z component.
        transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
    }
}
*/
