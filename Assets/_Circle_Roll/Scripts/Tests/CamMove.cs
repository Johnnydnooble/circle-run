using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Es.InkPainter
{
    public class CamMove : MonoBehaviour
    {

        //   public GameObject Player;

        private Vector3 offset;
        [SerializeField] private float smooth = 0.5f;

        [SerializeField] private Transform player; // Reference to the player's transform.
        [SerializeField] private GameObject plate;
        [SerializeField] private GameObject targetCamera;

        private GameManager gameManager;


        void Start()
        {
            gameManager = GameObject.FindObjectOfType<GameManager>();
            StartCoroutine(FindPlayer());
            plate = gameManager.levelArray[gameManager.CurrentLevel];
        }

        private void Update()
        {
            if (player != null && plate != null && gameManager.OverrideDefaultCameraMove)
            {
                if (gameManager.CurrentLevel >= 9)
                {
                    //                 Debug.Log(plate.name);
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetCamera.transform.rotation, Time.deltaTime * 0.5f);
                    transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, player.position.y + ((Mathf.Abs(player.position.x) + Mathf.Abs(player.position.z)) * .4f) + offset.y, targetCamera.transform.position.z), Time.deltaTime * smooth);
                }
                else //if (m_Player != null && PlayerPrefs.GetInt("CurrentLevel", 0) == 2)
                {
//                    Debug.Log("It's not a Plate_9_02(Clone)");
                    transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, player.position.y + offset.y, transform.position.z), Time.deltaTime * smooth);
                }
            }
        }

        IEnumerator FindPlayer()
        {
            yield return new WaitForSeconds(1f);
            player = GameObject.FindGameObjectWithTag("Player").transform;
//            plate = GameObject.FindGameObjectWithTag("Plate");
            offset = transform.position - player.transform.position;
            offset += new Vector3(0, 2f, 0);
        }
    }
}




/*




bool moveRight, moveLeft; 
public Transform selfTransform; 
public Transform mainCamTransform;
[SerializeField]
Camera cam;   
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
