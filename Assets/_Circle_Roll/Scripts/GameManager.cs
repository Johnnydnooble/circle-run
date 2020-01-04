using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.AI;
using UnityEngine.ProBuilder;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //    [Header("ResetLevel")]
    //    [SerializeField]public bool resetLevel;
    [Header("load Level 1 - 4")]
    [Range(1, 4)]
    [SerializeField] public int loadLevel;

    // Level
    private int _currentLevel;
    public int CurrentLevel { get { return _currentLevel; } set { _currentLevel = value; } }

    [Header("Camera Move")]
    [SerializeField] GameObject camera;
    [Range(40f, 90f)]
    [SerializeField] float rotateCameraAxisX;
    [Range(0.1f, -20f)]
    [SerializeField] float moveCameraAlongAxisZ;
    [Range(0f, 30f)]
    [SerializeField] float zoomCameraAlongAxisY;
    [SerializeField] public bool OverrideDefaultCameraMove;
//    public bool OverrideDefaultCameraMove { get; set; }

    [Header("UI")]
    [SerializeField] GameObject gamePanel;
    [SerializeField] Text textCurrent;
    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject losePanel;
    [SerializeField] GameObject confettiParticle;

    [Header("Ball")]
    [Space(5)]
    [SerializeField] GameObject ballPrefab;
    [SerializeField] GameObject ballInkPaintPrefab;
    [SerializeField] Vector3 ballInitialPos = new Vector3(x: 0, y: 10, z: 0);
    [SerializeField] Color ballColor;
    [Range(3f, 60f)]
    [SerializeField] float speedBall = 15f;
    [SerializeField] bool OverrideDefaultSpeedBall;
    [Space(5)]

    [Header("Plate")]
//    [SerializeField] GameObject plateObj;
    [SerializeField] bool OverrideDefaultMat;
    [SerializeField] Material plateMat;
    [SerializeField] Color plateColor;

    [Header("Piece")]
    [SerializeField] Material pieceMat;
    [SerializeField] Color pieceColor;

    [Header("Background")]
    [SerializeField] GameObject background;
    [SerializeField] Material backgroundMat;
    [SerializeField] Color backgroundColor;

    [Header("Array of levels")]
    [SerializeField] public GameObject[] levelArray = new GameObject[4];

    [SerializeField] Color newColor = Color.white;

    private GameObject bottomLid;
    private GameObject levelObject;
    private Rigidbody rb;
   
    private Vector3 radiusToBall;


    private List<GameObject> pieceListObst = new List<GameObject>();
    private List<GameObject> pieceList = new List<GameObject>();

    //Materials
    private Material colorMat;   

    //Game States
    bool isPlaying;
    bool isLevelCreated;
    bool LevelFinished;

    [Header("Obstacle")]
    [SerializeField] GameObject archObstaclePref;
    private GameObject archObstacle;   
    int old = 0;
    [SerializeField] bool ActivateMovingPiece;
//    [SerializeField] bool ActivateMovingBlock;
//    [SerializeField] bool ActivateMovingBall;


    private void Awake()
    {
//        if (resetLevel)
//        {
//            PlayerPrefs.SetInt("CurrentLevel", 0); // (сброс уровня)
//            resetLevel = false;
//        }

        if (loadLevel != _currentLevel && loadLevel > _currentLevel)  // 
        {
            _currentLevel = (loadLevel - 1);
            levelObject = Instantiate(levelArray[_currentLevel], new Vector3(0, 0, 0), Quaternion.identity);
            PlayerPrefs.SetInt("CurrentLevel", _currentLevel); 
        }
        else
        {
            levelObject = Instantiate(levelArray[_currentLevel], new Vector3(0, 0, 0), Quaternion.identity);
          PlayerPrefs.SetInt("CurrentLevel", _currentLevel); 
        }

        Time.timeScale = 1;

        InitSaves();
        if (_currentLevel >= levelArray.Length)
        {
            PlayerPrefs.SetInt("CurrentLevel", 0); // (сброс уровня если закончились уровни)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        SetupPieces();
        bottomLid = levelObject.transform.GetChild(0).gameObject;
    }

    void Start()
    {
        // level
        textCurrent.text = (_currentLevel + 1).ToString();
        //instantiate ball at initial position
        if (loadLevel == 4)
        {
            ballPrefab = Instantiate(ballInkPaintPrefab, ballInitialPos, Quaternion.identity);
        }
        else
        {
            ballPrefab = Instantiate(ballPrefab, ballInitialPos, Quaternion.identity);
        }
       
        rb = ballPrefab.GetComponent<Rigidbody>();

        //set color
        ballPrefab.GetComponent<Renderer>().material.color = ballColor;
        pieceMat.color = pieceColor;
        plateMat.color = plateColor;
        backgroundMat.color = backgroundColor;

        //Set Material
//        colorMat = Resources.Load("PieceMat", typeof(Material)) as Material;    

        // plate, piece
 //       levelObject = Instantiate(levelArray[_currentLevel], new Vector3(0, 0, 0), Quaternion.identity);
 //       SetupPieces();
        bottomLid = levelObject.transform.GetChild(0).gameObject;

        if (ActivateMovingPiece)
        {
            StartCoroutine(MovePiece2());
        }
          
    }

    private void SetupPieces()
    {
//        int i = 0;
        foreach (Transform child in levelObject.transform.GetChild(0))
        {
            child.gameObject.AddComponent<MeshCollider>();
            if (OverrideDefaultMat) // переопределить материал по умолчанию
            {
                child.gameObject.GetComponent<Renderer>().material = plateMat;                
            }
            pieceList.Add(child.gameObject);
           pieceListObst.Add(child.gameObject);

//--            if (i == 40 && loadLevel  == 1)
//--            {
//--                Debug.Log("Arch" + child.rotation);
//--                //                child.gameObject.AddComponent<MeshFilter>().mesh.normals
//--
//--//                var rot = Quaternion.Euler(Vector3.forward);
//--                archObstacle = Instantiate(archObstaclePref, child.position, child.rotation);
//-- //              archObstacle.transform.parent = child.gameObject.transform;
//--            }
//--            i++;           
        }

        //        foreach (Transform child in levelObject.transform.GetChild(1))
        //        {
        //            child.gameObject.AddComponent<MeshCollider>();
        //            if (OverrideDefaultMat)
        //            {
        //                child.gameObject.GetComponent<Renderer>().material = plateMat;
        //            }
        //            pieceList.Add(child.gameObject);
        //           
        //        }
        //
        //        if (levelObject.transform.childCount > 2)
        //        {
        //            foreach (Transform child in levelObject.transform.GetChild(2))
        //            {
        //                child.gameObject.AddComponent<MeshCollider>();
        //                if (OverrideDefaultMat)
        //                {
        //                    child.gameObject.GetComponent<Renderer>().material = plateMat;
        //                }
        //                pieceList.Add(child.gameObject);
        //
        //            }
        //        }
        //
        //        if (levelObject.transform.childCount > 3)
        //        {
        //            foreach (Transform child in levelObject.transform.GetChild(3))
        //            {
        //                child.gameObject.AddComponent<MeshCollider>();
        //                if (OverrideDefaultMat)
        //                {
        //                    child.gameObject.GetComponent<Renderer>().material = plateMat;
        //                }
        //                pieceList.Add(child.gameObject);
        //
        //            }
        //        }
    }


    private void FixedUpdate()
    {
        //radiusToBall = ball.transform.position - centre.transform.position;
        if (bottomLid != null)
        {
            radiusToBall = bottomLid.transform.position - ballPrefab.transform.position;
            Vector3 tangent = Vector3.Cross(Vector3.up, radiusToBall);

            if (Input.GetMouseButton(0))
            {
                Debug.Log("tangent " + tangent);
                if (OverrideDefaultSpeedBall)
                {
                    // rb.AddForce(tangent * 600f * Time.deltaTime, ForceMode.Force); // is slow
                    rb.AddForce(tangent * 20f, ForceMode.Acceleration); 
//                    rb.AddForce(tangent * .4f, ForceMode.VelocityChange); 
                }
                else
                {
                    rb.AddForce(tangent * speedBall * Time.deltaTime, ForceMode.Impulse); // is fast                  
                }                            
            }

            if (pieceList.Count > 29)
            {
                LevelFinished = true;
                foreach (GameObject piece in pieceList)
                {
                    if (piece.GetComponent<Renderer>().material.color != Color.green)
                    {
                        LevelFinished = false;
                        break;
                    }

                }
            }
        }
    }


    private void Update()
    {
//         Debug.Log(pieceList.Count);
        if (pieceList.Count == 0 && ballPrefab.name != "BallInkPaint(Clone)")
        {
            Debug.Log(levelArray[_currentLevel].name);
            StartCoroutine(WaitForTime());
        }

        if(Input.GetButtonDown("Jump"))
        {
            //  StartCoroutine(WaitStartParticle()); // если красим краской
            Debug.Log(levelArray[_currentLevel].name);
            StartCoroutine(WaitForTime());
        }

        if (ballPrefab.transform.position.y < -5)
        {
            FailLevel();
        }

        if (!OverrideDefaultCameraMove)
        {
            camera.transform.position = new Vector3(0f, zoomCameraAlongAxisY, moveCameraAlongAxisZ);
            camera.transform.rotation = Quaternion.Euler(rotateCameraAxisX, 0f, 0f);
        }        
    }

    IEnumerator WaitForTime()
    {
        yield return new WaitForSeconds(1f);
        confettiParticle.SetActive(true);
        yield return new WaitForSeconds(2f);
        WinLevel();
    }   

    public void RestartLevel()
    {
        Time.timeScale = 1;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        Time.timeScale = 1;

        _currentLevel++;
//        SaveData();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void WinLevel()
    {
        winPanel.SetActiveRecursively(true);
        Time.timeScale = 0;
    }

    public void FailLevel()
    {
        losePanel.SetActive(true);
        Time.timeScale = 0;
    }

    private void SaveData() // сохранить 
    {
        PlayerPrefs.SetInt("CurrentLevel", _currentLevel);
        Debug.Log("CurrentLevel  " + _currentLevel);
    }

    private void InitSaves() // получить 
    {
        _currentLevel = PlayerPrefs.GetInt("CurrentLevel", 0);
        SaveData();
    }

    private void CheckForLevelCompletion() { }

    public void animatePiece(GameObject piece)
    {
        Sequence s = DOTween.Sequence();
 //       s.Append(piece.transform.DOScale(1.1f, 0.25f));
        //s.Append(piece.transform.DOScale(100f, 0.25f));
    }

    public void changeColor(GameObject piece)
    {
        //Fetch the Renderer
        Renderer rend = piece.GetComponent<Renderer>();

        pieceList.Remove(piece);

        // rend.material.DOColor(Color.green, 0.5f);
        rend.material = pieceMat;
    }

    IEnumerator  MovePiece2() // 
    {
        int red = UnityEngine.Random.Range(0, pieceListObst.Count);

        yield return new WaitForSeconds(1f);

        if (pieceListObst[old].GetComponent<Renderer>().material.color == Color.green)
        {
            pieceListObst[old].GetComponent<Renderer>().material.color = plateMat.color;
        }
        else if(old != 0)
        {
            yield break;
        }     

        pieceListObst[red].GetComponent<Renderer>().material.color = Color.green;

        old = red;

        StartCoroutine(MovePiece2());
    }
}
