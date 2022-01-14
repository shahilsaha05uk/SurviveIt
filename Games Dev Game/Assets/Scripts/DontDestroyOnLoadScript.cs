using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyOnLoadScript : MonoBehaviour
{
    static DontDestroyOnLoadScript instance;


    private void Awake()
    {
        /*        if(instance!= null)
                {
                    Destroy(gameObject);
                }
                else
                {
                    instance = this;
                    DontDestroyOnLoad(this.gameObject);
                }*/

    }




}
