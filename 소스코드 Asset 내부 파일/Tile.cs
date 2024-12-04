using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool IsBuildTower { set; get; }

    // Start is called before the first frame update
    void Awake()
    {
        IsBuildTower = false;
        
    }

    // Update is called once per frame

}
