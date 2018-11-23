using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCamera : MonoBehaviour
{
    [SerializeField]
    private Camera[] cameraList;    // list of cameras to each ball
    private int curent;             // camera number

    void Start () {       
        curent = 0;
        for (int i = 0; i < cameraList.Length; i++)
        {
            cameraList[i].gameObject.SetActive(false); // setting all cameras non-active
        }
        if (cameraList.Length > 0)
        {
            cameraList[0].gameObject.SetActive(true);  // setting main camera active
        }
    }

    /*changes number of camera like 0->3->2->1->0
      and also stops previous ball*/
    public void OnClickLeftButton()  
    {
        cameraList[curent].gameObject.SetActive(false);
        BallMove.speedС[curent] = 0.0f;                            
        curent = (curent-1 < 0) ? cameraList.Length-1 : curent-1;
        cameraList[curent].gameObject.SetActive(true);
        BallMove.speedС[curent] = 250.0f;                          
    }

    /* changes number of camera like 0->1->2->3->0 
       and also stops previous ball*/
    public void OnClickRightButton()                               
    {
        cameraList[curent].gameObject.SetActive(false);
        BallMove.speedС[curent] = 0.0f;                            
        curent = (curent+1) % cameraList.Length;
        cameraList[curent].gameObject.SetActive(true);
        BallMove.speedС[curent] = 250.0f;                          
    }
}
