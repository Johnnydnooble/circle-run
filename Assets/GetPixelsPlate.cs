using UnityEngine;
using System.Collections;

public class GetPixelsPlate : MonoBehaviour
{
    public Texture2D sourceTex;
    void Start()
    {
        Color32[] pix = sourceTex.GetPixels32();
        System.Array.Reverse(pix);
        Texture2D destTex = new Texture2D(sourceTex.width, sourceTex.height);
        destTex.SetPixels32(pix);
        destTex.Apply();
        GetComponent<Renderer>().material.mainTexture = destTex;
    }
}
