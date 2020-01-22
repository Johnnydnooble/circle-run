﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.AI;
using UnityEngine.ProBuilder;
using UnityEngine.UI;
using System.Linq;
using Es.InkPainter.Effective;
using Es;
using Es.InkPainter;


public class GameManager : MonoBehaviour
{
    [Header("ResetLevel")]
    private bool resetLevel;
    //        [Header("load Level 1 - 30")]
    //        [Range(1, 30)]
    //       private int loadLevel;
    private bool newGamePlay = false;

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
    [SerializeField] GameObject _ballPrefab;
    public GameObject BallPrefab { get { return _ballPrefab; } set { _ballPrefab = value; } }
    [SerializeField] GameObject ballInkPaintPrefab;
    [SerializeField] Vector3 ballInitialPos = new Vector3(x: 0, y: 10, z: 0);
    [SerializeField] Color ballColor;
    [Range(3f, 60f)]
    [SerializeField] float speedBall = 15f;
    [SerializeField] bool forceImpulse;
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
    [SerializeField] GameObject _background;
    public GameObject Background { get { return _background; } set { _background = value; } }

    [SerializeField] Material backgroundMat;  
    [SerializeField] Color backgroundColor;

    [Header("Array of levels")]
    [SerializeField] public GameObject[] levelArray = new GameObject[7];

    [SerializeField] Color newColor = Color.white;

    private GameObject bottomLid;
    private GameObject levelObject;
    private Rigidbody rb;
    private Rigidbody rb2;
    private Rigidbody rb3;
    private Rigidbody rbLevel;

    private Vector3 radiusToBall;


    private List<GameObject> pieceListObst = new List<GameObject>();
    private List<GameObject> pieceList = new List<GameObject>();

    //Materials
    private Material colorMat;

    //Game States
    private bool isPlaying;
    private bool isLevelCreated;
    private bool LevelFinished;

    [Header("Obstacle")]
    [SerializeField] GameObject archObstaclePref;
    private GameObject archObstacle;
    private int old = 0;
    [SerializeField] bool ActivateMovingPiece;
    //    [SerializeField] bool ActivateMovingBlock;
    //    [SerializeField] bool ActivateMovingBall;


    private float angle = 0f;
    private float speed = 5f;
    private float radius = 0f;
    private Vector3 startPlateCenter;
    private bool inOut = true;

    private Mesh mesh;
    private MeshCollider meshCol;

    private InkCanvas inkCanvas;


    [Range(2, 64)]
    public int resolution = 32;
    private Texture2D texture2D;
    //       private RenderTexture sourceTex;
    private Coroutine coroutinePixelColor;

    private void Awake()
    {

        InitSaves();
        PlayerPrefs.SetInt("CurrentLevel", _currentLevel);
        if ((_currentLevel) >= levelArray.Length)
        {
            Reset();
        }
        else
        {
            // plate, piece
            levelObject = Instantiate(levelArray[_currentLevel], new Vector3(0, 0, 0), Quaternion.identity);
            Time.timeScale = 1;
            SetupPieces();

            bottomLid = levelObject.transform.GetChild(1).gameObject;
        }


        //=            }


    }

    void Start()
    {
        // level
        textCurrent.text = (_currentLevel + 1).ToString();

        //instantiate ball at initial position
        _ballPrefab = Instantiate(_ballPrefab, ballInitialPos, Quaternion.identity); // 1 ball
        rb = _ballPrefab.GetComponent<Rigidbody>();

        //inkPaint            ballPrefab = Instantiate(ballInkPaintPrefab, ballInitialPos, Quaternion.identity); // 1 ball
        //inkPaint            rb = ballPrefab.GetComponent<Rigidbody>();
        //inkPaint            //           StartCoroutine(InstantiateBallInkPaintPrefab()); // 3 balls

        //inkPaint       //     inkCanvas = levelObject.transform.GetComponent<InkCanvas>(); // скрипт закраски кистью без детей
        //inkPaint              inkCanvas = levelObject.transform.GetChild(0).GetComponent<InkCanvas>(); // скрипт закраски кистью

        rbLevel = levelObject.GetComponent<Rigidbody>();

        //set color
        _ballPrefab.GetComponent<Renderer>().material.color = ballColor;
        pieceMat.color = pieceColor;
        plateMat.color = plateColor;
        backgroundMat.color = backgroundColor;

        bottomLid = levelObject.transform.GetChild(0).gameObject;

        if (_currentLevel == 5) // движущиеся клетки // ActivateMovingPiece
        {
            StartCoroutine(MovePiece2());
        }

       

        //Analytics here

        TinySauce.OnGameStarted(levelNumber: CurrentLevel.ToString());

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

                 
        }

        
    }


    private void FixedUpdate()
    {
        //radiusToBall = ball.transform.position - centre.transform.position;
        //        Debug.Log(bottomLid);
        if (bottomLid != null)
        {
            radiusToBall = bottomLid.transform.position - _ballPrefab.transform.position;
            Vector3 tangent = Vector3.Cross(Vector3.up, radiusToBall).normalized;
            //            Vector3 tangent = Vector3.Cross(Vector3.up, radiusToBall);

            if (Input.GetMouseButton(0))
            {
                if (!forceImpulse)
                {
                    if (OverrideDefaultSpeedBall)
                    { 
                        rb.AddForce(tangent * 35f * Time.fixedDeltaTime, ForceMode.VelocityChange);
                    }
                    else
                    {      
                        rb.AddForce(tangent * speedBall * Time.fixedDeltaTime, ForceMode.VelocityChange);
                    }
                }
                else
                {
                    if (OverrideDefaultSpeedBall)
                    {                       
                        rb.AddForce(tangent * 35f * Time.fixedDeltaTime, ForceMode.Impulse);
                    }
                    else
                    {
                        rb.AddForce(tangent * speedBall * Time.fixedDeltaTime, ForceMode.Impulse);
                    }
                }                
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


    //
    private void Update()
    {
        //            Debug.Log(pieceList.Count);
        if (pieceList.Count == 0) // Paint Face // && ballPrefab.name != "BallInkPaint(Clone)"
        {
            StartCoroutine(WaitForTime());
        }

        if (_ballPrefab.transform.position.y < -5)
        {
            FailLevel();
        }

        if (!OverrideDefaultCameraMove) // MovingCamera
        {
            camera.transform.position = new Vector3(0f, zoomCameraAlongAxisY, moveCameraAlongAxisZ);
            camera.transform.rotation = Quaternion.Euler(rotateCameraAxisX, 0f, 0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bonus")
        {
            _ballPrefab.GetComponent<CollisionPainter>().brush.splatScale = 0.2f;
        }
    }

    IEnumerator PixelColor() // inkPaint
    {
        yield return new WaitForSeconds(2f);
        RenderTexture sourceTex = inkCanvas.PaintDatas.FirstOrDefault().paintMainTexture; // получить текстуру из памяти
                                                                                          //              RenderTexture sourceTex = levelObject.transform.GetChild(0).GetComponent<RenderTexture>(); // 2 вариант доступа к текстуре - доделать
                                                                                          //           var sourceTex = inkCanvas.PaintDatas.FirstOrDefault().material.mainTexture as Texture2D; // 3 вар получить текстуру из памяти - доделать

        //           GetComponent<MeshRenderer>().material.mainTexture = sourceTex; // посмотреть текстуру

        texture2D = new Texture2D(resolution, resolution, TextureFormat.RGB24, false); // создаем текстуру2D
        texture2D.filterMode = FilterMode.Point;

        RenderTexture scaleTex = new RenderTexture(resolution, resolution, 0); // Создаем новую  RenderTexture с меньшим разрешением
        Graphics.Blit(sourceTex, scaleTex); // копируем настройки и шейдер
        RenderTexture.active = scaleTex;      // загружаем текстуру в рендер

        texture2D.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0); // из рендера  RenderTexture - загружаем в texture2D
        texture2D.Apply();

        var pixels = texture2D.GetPixels();

        int countPixel = 0;
        for (int i = 0; i < pixels.Length; i++)
        {
            //                Debug.Log(i + "Pixels " + pixels[i]);
            //                if (pixels[i].r < 0.200 && pixels[i].g < 0.200 && pixels[i].b > 0.800) // синий
            if (pixels[i].r > 0.8 && pixels[i].g > 0.7 && pixels[i].b < 0.3) // желтый
            {
                countPixel++;
                if (countPixel > 1000)
                {
                    StartCoroutine(WaitForTime());
                }
            }
        }

        GetComponent<MeshRenderer>().material.mainTexture = texture2D; // посмотреть текстуру

        RenderTexture.active = null; // очистить времменую  RenderTexture

        StartCoroutine(PixelColor());
        coroutinePixelColor = null; // удалить карутину
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

    public void NewGamePlayOnOff()
    {
        newGamePlay = newGamePlay ? false : true;
    }

    public void Reset()
    {
        PlayerPrefs.SetInt("CurrentLevel", 0); // (сброс прогресса игры)
                                               //                resetLevel = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        Time.timeScale = 1;

        _currentLevel++;

        SaveData();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void WinLevel()
    {
        winPanel.SetActiveRecursively(true);
        Time.timeScale = 0;
        TinySauce.OnGameFinished(levelNumber: CurrentLevel.ToString(), true, 1);
    }

    public void FailLevel()
    {
        losePanel.SetActive(true);
        Time.timeScale = 0;
        TinySauce.OnGameFinished(levelNumber: CurrentLevel.ToString(), false, 1);
    }

    private void SaveData() // сохранить 
    {
        PlayerPrefs.SetInt("CurrentLevel", _currentLevel);
        //            Debug.Log("CurrentLevel  " + _currentLevel);
    }

    private void InitSaves() // получить 
    {
        _currentLevel = PlayerPrefs.GetInt("CurrentLevel", 0);
        SaveData();
    }

    private void CheckForLevelCompletion() { }

   

    public void changeColor(GameObject piece)
    {
        //Fetch the Renderer
        Renderer rend = piece.GetComponent<Renderer>();

        pieceList.Remove(piece);

        // rend.material.DOColor(Color.green, 0.5f);
        rend.material = pieceMat;
    }

    IEnumerator MovePiece2() // 
    {
        int red = UnityEngine.Random.Range(0, pieceListObst.Count);

        yield return new WaitForSeconds(1f);

        if (pieceListObst[old].GetComponent<Renderer>().material.color == Color.green)
        {
            pieceListObst[old].GetComponent<Renderer>().material.color = plateMat.color;
        }
        else if (old != 0)
        {
            yield break;
        }

        pieceListObst[red].GetComponent<Renderer>().material.color = Color.green;

        old = red;

        StartCoroutine(MovePiece2());
    }
}
