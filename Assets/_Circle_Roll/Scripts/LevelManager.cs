﻿using System.Collections;
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
        //        _gameManager.Background.GetComponent<Renderer>().material.color = levelsItem[_gameManager.CurrentLevel].backgroundColor;
        //        _gameManager.BallPrefab.GetComponent<Renderer>().material.color = levelsItem[_gameManager.CurrentLevel].ballColor;
        //        _gameManager.BallPrefab.GetComponent<Renderer>().material.color = levelsItem[_gameManager.CurrentLevel].ballColor;

        
        ballMat.color = levelsItem[_gameManager.CurrentLevel].ballColor;
        plateMat.color = levelsItem[_gameManager.CurrentLevel].plateColor;
        pieceMat.color = levelsItem[_gameManager.CurrentLevel].plateColored;
        background.color = levelsItem[_gameManager.CurrentLevel].backgroundColor;
    }
}
