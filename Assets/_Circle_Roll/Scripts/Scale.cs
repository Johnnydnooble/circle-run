using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Scale : MonoBehaviour
{
    //IEnumerator Start()
    //{

    //    yield return new WaitForSeconds(1);
    //    //transform.DOScale(1.1f, 1);
    //    //transform.DOScale(0.90f, 1);
    //    Sequence s = DOTween.Sequence();
    //    s.Append(transform.DOScale(1.1f, 1));
    //    s.Append(transform.DOScale(0.8f, 1));




    //}


    void Start()
    {
        Sequence s = DOTween.Sequence();
        s.Append(transform.DOScale(1.2f, 0.3f));
        s.Append(transform.DOScale(0.8333f, 0.3f));
    }
}
