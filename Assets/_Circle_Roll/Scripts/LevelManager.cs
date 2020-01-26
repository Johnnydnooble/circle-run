using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] GameManager _gameManager;

    [SerializeField] LevelsItem[] levelsItem;

    //   [SerializeField] GameObject ball;
    [SerializeField] Material ballMat;
    [SerializeField] Material plateMat;
    [SerializeField] Material pieceMat;
    [SerializeField] Material background;
    

    private void Start()
    {
            ballMat.color = levelsItem[_gameManager.RealLevel].ballColor;
            plateMat.color = levelsItem[_gameManager.RealLevel].plateColor;
            pieceMat.color = levelsItem[_gameManager.RealLevel].plateColored;
            background.color = levelsItem[_gameManager.RealLevel].backgroundColor;      
    }
}
