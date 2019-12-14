using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    [Header("Ball")]
    [Space(3)]
    [SerializeField] GameObject ballPrefab;
    [SerializeField] Vector3 ballInitialPos = new Vector3(x: 0, y: 10, z: 0);
    [SerializeField] Color ballColor;
    [Space(3)]

    [Header("Piece")]
    [SerializeField] Material pieceMat;

    GameObject bottomLid;
    GameObject levelObject;
    private Rigidbody rb;
    private Vector3 radiusToBall;
    static int currentLevel = 1;
    Transform[] levelChildren;

    [Header("Plate")]
    [SerializeField] bool OverrideDefaultMat;
    [SerializeField] Material plateMat;   


    List<GameObject> pieceList = new List<GameObject>();

    //Materials
    Material colorMat;

    //Array of levels
    public GameObject[] levelArray;

    //Game States
    bool isPlaying;
    bool isLevelCreated;
    bool LevelFinished;

    //UI
    public GameObject winPanel;
    public GameObject losePanel;

    [SerializeField] Color newColor = Color.white;

    void Start()
    {
        //instantiate ball at initial position
        ballPrefab = Instantiate(ballPrefab, ballInitialPos, Quaternion.identity);
        //set ball's color
        ballPrefab.GetComponent<Renderer>().material.color = ballColor;
        rb = ballPrefab.GetComponent<Rigidbody>();
        levelObject = Instantiate(levelArray[currentLevel - 1], new Vector3(0, 0, 0), Quaternion.identity);
        SetupPieces();
        bottomLid = levelObject.transform.GetChild(0).gameObject;

        //Set Material
        colorMat = Resources.Load("BlueMat", typeof(Material)) as Material;
    }

    private void SetupPieces()
    {        
        foreach (Transform child in levelObject.transform.GetChild(0))
        {
            child.gameObject.AddComponent<MeshCollider>();
            if (OverrideDefaultMat)
            {
                child.gameObject.GetComponent<Renderer>().material = plateMat;
            }
            pieceList.Add(child.gameObject);
        }

        foreach (Transform child in levelObject.transform.GetChild(1))
        {
            child.gameObject.AddComponent<MeshCollider>();
            if (OverrideDefaultMat)
            {
                child.gameObject.GetComponent<Renderer>().material = plateMat;
            }
            pieceList.Add(child.gameObject);
        }


        if (levelObject.transform.childCount > 2)
        {
            foreach (Transform child in levelObject.transform.GetChild(2))
            {
                child.gameObject.AddComponent<MeshCollider>();
                if (OverrideDefaultMat)
                {
                    child.gameObject.GetComponent<Renderer>().material = plateMat;
                }
                pieceList.Add(child.gameObject);
            }
        }

        if (levelObject.transform.childCount > 3)
        {
            foreach (Transform child in levelObject.transform.GetChild(3))
            {
                child.gameObject.AddComponent<MeshCollider>();
                if (OverrideDefaultMat)
                {
                    child.gameObject.GetComponent<Renderer>().material = plateMat;
                }
                pieceList.Add(child.gameObject);
            }
        }
    }


    private void Update()
    {
        //radiusToBall = ball.transform.position - centre.transform.position;
        if (bottomLid != null)
        {
            radiusToBall = bottomLid.transform.position - ballPrefab.transform.position;
            Vector3 tangent = Vector3.Cross(Vector3.up, radiusToBall);

            if (Input.GetMouseButton(0))
            {
                Debug.Log("Force to ball applied");
                //With impulse, 0.15f is enough for relatively flat objects
                //With acceleration, that's very small
                ballPrefab.GetComponent<Rigidbody>().AddForce(tangent * 0.15f, ForceMode.Impulse);
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
            //Debug.Log(bottomLid.transform.position);
            //Debug.Log(ball.transform.position);
        }
        //Debug.Log("Bottom lid " + bottomLid);
        //Debug.Log("Ball rigidbody " + ball);
        //Debug.Log("Radius to ball is " + radiusToBall);
        //Debug.Log("Tangent is: " + tangent);
    }

    private void FixedUpdate()
    {
       // Debug.Log(pieceList.Count);
        if (pieceList.Count == 0)
        {
            //WinLevel();
            StartCoroutine(WaitForTime());
        }

        if (ballPrefab.transform.position.y < -5)
        {
            FailLevel();
        }
    }

    IEnumerator WaitForTime()
    {
        yield return new WaitForSeconds(1);
        WinLevel();
    }



    private void CheckForLevelCompletion()
    {

        
    }

    public void RestartLevel()
    {
        Time.timeScale = 1;
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        currentLevel += 1;
    }

    public void changeColor(GameObject piece)
    {
        
        //Fetch the Renderer
        Renderer rend = piece.GetComponent<Renderer>();

        pieceList.Remove(piece);


        // rend.material.DOColor(Color.green, 0.5f);

        rend.material = pieceMat;
    }

    public void animatePiece(GameObject piece)
    {

        //Sequence s = DOTween.Sequence();
        //s.Append(piece.transform.DOScale(110f, 0.25f));
        //s.Append(piece.transform.DOScale(100f, 0.25f));
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
}
