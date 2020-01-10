using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Es.InkPainter;

public class TextureCreator1 : MonoBehaviour
{
    [Range(2, 512)]
    public int resolution = 256;

    private Texture2D texture;

    private void OnEnable()
    {
        if (texture == null)
        {
                       texture = new Texture2D(resolution, resolution, TextureFormat.RGB24, true);
                       texture.name = "Procedural Texture";
                       texture.wrapMode = TextureWrapMode.Clamp;
                       texture.filterMode = FilterMode.Trilinear; // FilterMode.Point;
                       texture.anisoLevel = 9;
//            PaintDatas.FirstOrDefault().paintMainTexture
            GetComponent<MeshRenderer>().material.mainTexture = texture;
        }
        FillTexture();
    }

    public void FillTexture()
    {
        if (texture.width != resolution)
        {
            texture.Resize(resolution, resolution);
        }

        Vector3 point00 = new Vector3(-0.5f, -0.5f);
        Vector3 point10 = new Vector3(0.5f, -0.5f);
        Vector3 point01 = new Vector3(-0.5f, 0.5f);
        Vector3 point11 = new Vector3(0.5f, 0.5f);

        float stepSize = 1f / resolution;
        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                texture.SetPixel(x, y, new Color((x + 0.5f) * stepSize % 0.1f, (y + 0.5f) * stepSize % 0.1f, 0f) * 10f);
            }
        }
        texture.Apply();
    }
}
