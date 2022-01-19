using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public int scene = 0;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
