using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepFactory : MonoBehaviour
{

    public GameObject Sheep;

    static private SheepFactory instance;

    private void Awake()
    {
        instance = this;
    }

    static public GameObject NewSheep(Vector3 position, Barn barn)
    {
        GameObject sheep = NewSheep(position);
        sheep.GetComponent<Sheep>().SetBarn(barn);
        return sheep;
    }

    static public GameObject NewSheep(Vector3 position)
    {
        return Instantiate(instance.Sheep, position, Quaternion.identity);
    }

}
