using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class LipBlend : MonoBehaviour
{
    enum BlendShapes
    {
        JawDown = 22,
        JawLeft = 23,
        JawRight = 24,
        LowerLipDownLeft = 26,
        LowerLipDownRight = 27,
        MouthOpen = 35,
        UpperLipUpLeft = 48,
        UpperLipUpRight = 49,
    }

    int blendShapeCount;
    SkinnedMeshRenderer skinnedMeshRenderer;
    Mesh skinnedMesh;
    float blendSpeed = 1f;
    bool increasing = true;
    float globalTime = 0f;

    string clipName = "";
    string[] lipValues;
    int valuesIndex;
    bool fileloaded = false;
    bool cliploaded = false;
    WWW www, wwwc;

    void Awake()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        skinnedMesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
    }


    IEnumerator DownloadClip()
    {
        //string url = "https://www.motallebi.me/owncloud/index.php/s/ixgMXdxchb4bQq7/download";
        string url = "ftp://utis2017-teamb-linux.japaneast.cloudapp.azure.com/wav/" + "bezos" + ".wav";
        wwwc = new WWW(url);
        yield return wwwc;

        AudioClip clip = null;
        if (string.IsNullOrEmpty(wwwc.error))
        {
            clip = wwwc.GetAudioClip();
            cliploaded = true;
        }
        else
        {
            Debug.Log(wwwc.error);
        }


        //var text = File.ReadAllText(@".\Assets\" + filePath); 

        
    }


    IEnumerator DownloadFeat()
    {
        //string url = "https://www.motallebi.me/owncloud/index.php/s/ixgMXdxchb4bQq7/download";
        string url = "ftp://utis2017-teamb-linux.japaneast.cloudapp.azure.com/feats/" + "bezos" + "_save.txt";
        www = new WWW(url);
        yield return www;

        string text = "";
        if (string.IsNullOrEmpty(www.error))
        {
            text = www.text;
            fileloaded = true;
        }
        else
        {
            Debug.Log(www.error);
        }


        //var text = File.ReadAllText(@".\Assets\" + filePath); 
        string[] values_t = text.Split('\n');
        lipValues = new string[values_t.Length - 1];
        Array.Copy(values_t, 1, lipValues, 0, lipValues.Length);

        Debug.Log(lipValues.Length);
        valuesIndex = 0;

    }

    void Start()
    {
        blendShapeCount = skinnedMesh.blendShapeCount;

        //clipName = GetComponent<AudioSource>().clip.name;
        //Debug.Log(clipName);
        //GetComponent<AudioSource>().clip.
        //string filePath = clipName + "_out.txt";
        //filePath = "Assets/" + filePath;

        //TextAsset targetFile = Resources.Load<TextAsset>(filePath);
        //Debug.Log(targetFile.text);

        var w = StartCoroutine("DownloadFeat");
        w = StartCoroutine("DownloadClip");

        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = wwwc.GetAudioClip();
        audioSource.Play();
    }

    void Update()
    {

        if (!fileloaded)
        {
            if (www != null)
                Debug.Log("feat bytes " + www.bytesDownloaded);
            return;
        }

        if (!cliploaded)
        {
            Debug.Log("clip bytes " + wwwc.bytesDownloaded);
            return;
        }

        globalTime += Time.unscaledDeltaTime;

        float[] curValues = { }, values = { };
        while (true)
        {
            if (valuesIndex > lipValues.Length)
                return;
            string line = lipValues[valuesIndex++];
            string[] valuesStr = line.Split(' ');
            values = curValues;
            //curValues = Array.ConvertAll(valuesStr, float.Parse);
            curValues = new float[valuesStr.Length];
            for (int i = 0; i < valuesStr.Length; i++) curValues[i] = float.Parse(valuesStr[i]);
            if (curValues[0] > globalTime)
                break;
        }

        skinnedMeshRenderer.SetBlendShapeWeight((int)BlendShapes.MouthOpen, 2 * (values[1] + 10));
        skinnedMeshRenderer.SetBlendShapeWeight((int)BlendShapes.JawLeft, 2 * (values[2] + 10));
        skinnedMeshRenderer.SetBlendShapeWeight((int)BlendShapes.JawRight, 2 * (values[3] + 10));
        skinnedMeshRenderer.SetBlendShapeWeight((int)BlendShapes.UpperLipUpLeft, 2 * (values[4] + 10));
        skinnedMeshRenderer.SetBlendShapeWeight((int)BlendShapes.UpperLipUpRight, 2 * (values[5] + 10));
        skinnedMeshRenderer.SetBlendShapeWeight((int)BlendShapes.LowerLipDownLeft, 2 * (values[6] + 10));
        skinnedMeshRenderer.SetBlendShapeWeight((int)BlendShapes.LowerLipDownRight, 2 * (values[7] + 10));
        string[] result = new string[values.Length];
        for (int i = 0; i < values.Length; i++) result[i] = values[i].ToString();
        //string[] result = Array.ConvertAll<float,string>(values, x=>x.ToString());

        Debug.Log(string.Join("-", result));
        //Debug.Log("DT " + globalTime);

    }
}