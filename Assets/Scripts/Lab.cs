using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lab : MonoBehaviour
{

    public int Size = 128;

    void Start()
    {

        //for (int j = 0; j < Size; j++)
        //{
        //    for (int i = 0; i < Size; i++)
        //    {

        //        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //        sphere.transform.position = new Vector3(i, j, 0);
        //        //sphere.GetComponent<MeshRenderer>().material.color = new Color((i * 51f + i) * 3f / 255f % 1f, (j * 51f + j) * 3f / 255f % 1f, ((i * 51f + i) * 3f / 255f * (j * 51f + j) * 3f / 255f) % 1f);
        //        //sphere.GetComponent<MeshRenderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        //        sphere.GetComponent<MeshRenderer>().material.color = new Color(Mathf.PerlinNoise(i / 10f, j / 10f), Mathf.PerlinNoise(i / 20f, j / 20f), Mathf.PerlinNoise(i / 30f, j / 30f));

        //    }
        //}

        //Speak("bababubu");

    }


    private void Speak(string words)
    {
        Debug.Log(words);
    }

}
