using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Es.InkPainter;
using Es.InkPainter.Effective;
using Es;

#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;

#endif

namespace Es.InkPainter
{
    public class TextureCreator2 : MonoBehaviour
    {
        GameObject t;
        InkCanvas inkCanvas;
        Texture tex;
        Texture2D tex2D;

        void Start()
        {
            t = gameObject.transform.Find("p_00_b(Clone)").gameObject;
            inkCanvas = t.transform.GetChild(0).gameObject.transform.GetComponent<InkCanvas>();

        }

        // Update is called once per frame
        void Update()
        {
 //           tex = inkCanvas.PaintDatas.FirstOrDefault().paintMainTexture;
 //           GetComponent<MeshRenderer>().material.mainTexture
        }
    }
}
