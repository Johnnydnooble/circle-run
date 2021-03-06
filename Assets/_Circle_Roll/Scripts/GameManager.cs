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

//    [SerializeField] Text _textScore;
//    [SerializeField] int scoreCount = 0;
    [SerializeField] Text[] prevLevelTexts;
    [SerializeField] Text[] nextLevelTexts;
    [SerializeField] GameObject _progressBarPanel;
    [SerializeField] Image _progressBarImage;
    [SerializeField] Image _backgroundProgressBar;
//    [SerializeField] float scoreCountToFloat;
    [SerializeField] int _progressBarCountBag = 0;
    private float pieceListLength;
    private float temp;
    [SerializeField] Text loseCompleteProgressText;   

    [Header("Ball")]
    [Space(5)]
    [SerializeField] GameObject _ballPrefab;
    public GameObject BallPrefab { get { return _ballPrefab; } set { _ballPrefab = value; } }
    [SerializeField] GameObject ballInkPaintPrefab;
    [SerializeField] Vector3 ballInitialPos = new Vector3(x: 0, y: 10, z: 0);
    [SerializeField] Color ballColor;
    [Range(3f, 90f)]
    [SerializeField] float speedBall = 85f;
    private float speedBallLevels = 85f;
    [SerializeField] bool forceImpulse;
    [SerializeField] bool OverrideDefaultSpeedBall;
    private bool startMoveBall = false;

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
    [SerializeField] public GameObject[] levelArray = new GameObject[20];

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

    [Header("Tutorial")]
    [SerializeField] GameObject _tutorialParent;

    private void Awake()
    {
        #region  CurrentOld
        //            if (resetLevel)
        //            {
        //                PlayerPrefs.SetInt("CurrentLevel", 0); // (сброс прогресса игры)
        //                resetLevel = false;
        //            }

        //=           if (loadLevel != _currentLevel && loadLevel > _currentLevel)  // 
        //=           {
        //=               _currentLevel = (loadLevel);
        //=               levelObject = Instantiate(levelArray[_currentLevel], new Vector3(0, 0, 0), Quaternion.identity);
        //=               PlayerPrefs.SetInt("CurrentLevel", _currentLevel);
        //=           }
        //=           else
        //=           {
        #endregion CurrentOld
     
        InitSaves();



        PlayerPrefs.SetInt("CurrentLevel", _currentLevel);
        
        if (_currentLevel > 29)
        {
            int randomIntLevel = UnityEngine.Random.Range(10, 20);
            levelObject = Instantiate(levelArray[randomIntLevel], new Vector3(0, 0, 0), Quaternion.identity);

            Time.timeScale = 1;
            SetupPieces();

            bottomLid = levelObject.transform.GetChild(1).gameObject;
        }

        else
        {
            levelObject = Instantiate(levelArray[_currentLevel], new Vector3(0, 0, 0), Quaternion.identity);

            Time.timeScale = 1;
            SetupPieces();

            bottomLid = levelObject.transform.GetChild(1).gameObject;
        }



        
        // } //CurrentOld
    }

    void Start()
    {
        Physics.gravity = new Vector3(0, -17f, 0);

        TinySauce.OnGameStarted(levelNumber: _currentLevel.ToString()); // Аналитика

        if (!PlayerPrefs.HasKey("NotNew"))
        {
            StartCoroutine(ActiveTutorial(true));
        }


        //if (_currentLevel == 0) // Tutorial 
        //{
        //   StartCoroutine(ActiveTutorial(true));
        //}

        // level
        textCurrent.text = (_currentLevel + 1).ToString();

        // progress bar
        for (int i = 0; i < prevLevelTexts.Length; i++)
        {
            prevLevelTexts[i].text = _currentLevel + 1 + ""; // меняем номер текущего уровня в ProgressBar
            nextLevelTexts[i].text = _currentLevel + 2 + ""; // меняем номер следущего уровня в ProgressBar      
        }
        
       
        temp = pieceListLength = pieceList.Count; 

        //instantiate ball at initial position
        _ballPrefab = Instantiate(_ballPrefab, ballInitialPos, Quaternion.identity); // 1 ball
        rb = _ballPrefab.GetComponent<Rigidbody>();
        rbLevel = levelObject.GetComponent<Rigidbody>();

        StartCoroutine(StartMoveBall());

        #region old1
        //inkPaint            ballPrefab = Instantiate(ballInkPaintPrefab, ballInitialPos, Quaternion.identity); // 1 ball
        //inkPaint            rb = ballPrefab.GetComponent<Rigidbody>();
        //inkPaint            //           StartCoroutine(InstantiateBallInkPaintPrefab()); // 3 balls

        //inkPaint       //     inkCanvas = levelObject.transform.GetComponent<InkCanvas>(); // скрипт закраски кистью без детей
        //inkPaint              inkCanvas = levelObject.transform.GetChild(0).GetComponent<InkCanvas>(); // скрипт закраски кистью



        //set color
        //===        _ballPrefab.GetComponent<Renderer>().material.color = ballColor;
        //===        pieceMat.color = pieceColor;
        //===        plateMat.color = plateColor;
        //===        backgroundMat.color = backgroundColor;

        //Set Material
        //        colorMat = Resources.Load("PieceMat", typeof(Material)) as Material;    

        //            bottomLid = levelObject.transform.GetChild(0).gameObject;

        //       if (_currentLevel == 5) // движущиеся клетки // ActivateMovingPiece
        //       {
        //           StartCoroutine(MovePiece2());
        //       }

        //inkPaint            if (coroutinePixelColor == null)
        //inkPaint            {
        //inkPaint                coroutinePixelColor = StartCoroutine(PixelColor());
        //inkPaint            }
        #endregion old1
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
        if (_currentLevel < 2) //  Уменьшаем скорость если 1-2 уровни.
        {
            speedBallLevels = 85f;
        }
        else if(_currentLevel < 10)
        {
            speedBallLevels = 85f;
        }
        else
        {
            speedBallLevels = 130f;
        }

        if (bottomLid != null)
        {
            radiusToBall = bottomLid.transform.position - _ballPrefab.transform.position;
            Vector3 tangent = Vector3.Cross(Vector3.up, radiusToBall).normalized;
            //            Vector3 tangent = Vector3.Cross(Vector3.up, radiusToBall);

            if (Input.GetMouseButton(0) && startMoveBall)
            {
                StartCoroutine(ActiveTutorial(false));

                if (!forceImpulse)
                {
                    if (OverrideDefaultSpeedBall)
                    { 
                        rb.AddForce(tangent * 40f * Time.fixedDeltaTime, ForceMode.VelocityChange);
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
                        rb.AddForce(tangent * speedBallLevels * Time.fixedDeltaTime, ForceMode.Impulse);
                    }
                    else
                    {
                        rb.AddForce(tangent * speedBall * Time.fixedDeltaTime, ForceMode.Impulse);
                    }
                }                
            }
            #region old2
            //            if (Input.GetMouseButton(0))
            //            {
            //                if (inOut)
            //                {
            //                    radius = 0f;
            //                    inOut = false;
            //                }
            //                radius = Mathf.Lerp(radius, 1f, Time.deltaTime * 3f);
            //                angle += Time.deltaTime;
            //                var x = Mathf.Cos(angle * speed) * radius;
            //                var z = Mathf.Sin(angle * speed) * radius;
            //                levelObject.transform.position = new Vector3(x, 0f, z) + startPlateCenter;
            //            }
            //            else
            //            {
            //                radius = Mathf.Lerp(radius, 0f, Time.deltaTime * 3f);
            //                angle += Time.deltaTime;
            //                var x = Mathf.Cos(angle * speed) * radius;
            //                var z = Mathf.Sin(angle * speed) * radius;
            //                levelObject.transform.position = new Vector3(x, 0f, z) + startPlateCenter;
            //
            //                inOut = true;
            //            }
            #endregion old2
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

    IEnumerator StartMoveBall()
    {
        yield return new WaitForSeconds(2f);
        startMoveBall = true;
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
        yield return new WaitForSeconds(.7f);
        confettiParticle.SetActive(true);
        yield return new WaitForSeconds(1.3f);
        WinLevel();
    }

    IEnumerator InstantiateBallInkPaintPrefab() // 3 ball
    {
        _ballPrefab = Instantiate(ballInkPaintPrefab, ballInitialPos, Quaternion.identity);
        rb = _ballPrefab.GetComponent<Rigidbody>();
        yield return new WaitForSeconds(1f);
        _ballPrefab = Instantiate(ballInkPaintPrefab, new Vector3(ballInitialPos.x + 1f, ballInitialPos.y, ballInitialPos.z + 1f), Quaternion.identity);
        rb2 = _ballPrefab.GetComponent<Rigidbody>();
        yield return new WaitForSeconds(1f);
        _ballPrefab = Instantiate(ballInkPaintPrefab, new Vector3(ballInitialPos.x - 1f, ballInitialPos.y, ballInitialPos.z - 1f), Quaternion.identity);
        rb3 = _ballPrefab.GetComponent<Rigidbody>();
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
        TinySauce.OnGameFinished(levelNumber: _currentLevel.ToString(), true, 100); // Аналитика - выйграл

        winPanel.SetActiveRecursively(true);
        Time.timeScale = 0;
    }

    public void FailLevel()
    {
        TinySauce.OnGameFinished(levelNumber: _currentLevel.ToString(), false, 100); // Аналитика - проиграл

        losePanel.SetActive(true);
        loseCompleteProgressText.text = (int)(_progressBarImage.fillAmount * 100) + "% completed";
        Time.timeScale = 0;
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

    public void animatePiece(GameObject piece)
    {
        //            Sequence s = DOTween.Sequence();
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

    public void ProgressBar()
    {      
        if (temp > pieceList.Count)
        {
            _progressBarImage.fillAmount += 1 / pieceListLength; //   
            temp = pieceList.Count;
        }
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

    IEnumerator ActiveTutorial(bool active)
    {
        PlayerPrefs.SetInt("NotNew", 1);
        if (active == false)
        {
            _tutorialParent.SetActive(active);
        }
        else
        {
            yield return new WaitForSeconds(1.8f);
            _tutorialParent.SetActive(active);
        }       
    }
}
