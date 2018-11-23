using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using System.IO;
using System;
using Newtonsoft.Json;
using UnityEngine.EventSystems;
using System.Linq;

public class BallMove : MonoBehaviour, IPointerClickHandler
{ 
    public GameObject Ball;
    public Slider speedSlider;
    public static List<float> speedС = Enumerable.Repeat(500.0f, ballConst).ToList();       // speed coefficent of each ball

    private IEnumerator MC;                 // variable for MoveCoroutine object
    private List<Vector3> vectorDirection;  // list of Vectors3 from json
    private Vector3 temp;                   // local start position
    private int count;                      // count of points
    private int i = 1;                      // point number
    private const int ballConst = 4;        // balls count
    private bool firstStartOfBall = true;   // flag, that this ball moving was first or not
    private bool moving = false;            // flag, that this ball moving or not
    private int ballNumber;                 // number of ball

    public class Directions    // additional intermediate class using to convert from json
    {
        public List<float> x { get; set; }
        public List<float> y { get; set; }
        public List<float> z { get; set; }
    }

    public List<Vector3> LoadJson()
    {
        string[] jsonsNames = System.IO.Directory.GetFiles("Ways", "*.json");     //
        ballNumber = 0;                                                           //
        Regex pattern1 = new Regex(@"\d+");                                       //
        Match match1 = pattern1.Match(Ball.name);                                 //
        if (match1.Success)                                                       // in this blog we connect json file with neccessary ball
        {                                                                         // e.g. points from path_json1.json will be used by ball
            Regex pattern2 = new Regex(@"\(([^\)]*)\)");                          //                  path_json2.json will be used by ball(1)
            Match match2 = pattern2.Match(Ball.name);                             // 
            ballNumber = int.Parse(match2.Groups[1].Value);                       //
        }                                                                         //
        using (StreamReader sr = new StreamReader(jsonsNames[ballNumber]))        //
        {
            string json = sr.ReadToEnd();                                           // get string repr of json
            Directions direction = JsonConvert.DeserializeObject<Directions>(json); // convert string to objects of class Directions
            return ConvertToVector3(direction);
        }
    }

    /*converts Directions class objects to list of Vector3*/
    public List<Vector3> ConvertToVector3(Directions direction)                   
    {
        List<Vector3> vd = new List<Vector3>();
        for (int i = 0; i < direction.x.Count; i++)
        {
            vd.Add(new Vector3(direction.x[i], direction.y[i], direction.z[i]));
        }
        return vd;
    }

    /*initialization*/
    public void Start()                                   
    {
        vectorDirection = LoadJson();                    
        transform.position = temp = vectorDirection[0];  
        count = vectorDirection.Count;                                       
    }

    public void ReturnToTheBeginning()
    {
        try
        {
            Destroy(Ball.GetComponent<TrailRenderer>());
        }
        catch(Exception ex){}
        transform.position = vectorDirection[0];
        temp = vectorDirection[0];
    }

    public void OnOneClick()
    {
        if (!moving)
        {
            MC = MoveCoroutine();
            StartCoroutine(MC);
        }
    }

    public void OnDoubleClick()
    {
        StopCoroutine(MC);
        moving = false;
        temp = transform.position;
        ReturnToTheBeginning();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 1)
        {
            OnOneClick();
        }
        else if(eventData.clickCount == 2)
        {
            OnDoubleClick();
        }           
    }

    /*coroutine which moves clicked ball by its trajectory*/
    IEnumerator MoveCoroutine()  
    {
        moving = true;
        Vector3 vd = vectorDirection[i];
        DelTrail();
        yield return null;
        AddTrail();
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, temp + vd, speedС[ballNumber] * speedSlider.value * Mathf.Pow((Time.deltaTime), 2));
            yield return new WaitForEndOfFrame();
            if (transform.position == temp + vd)
            {
                if (i < count - 1)
                {
                    i++;
                    vd = vectorDirection[i];
                }
                else
                {
                    i = 1;
                    moving = false;
                    temp = transform.position;
                    yield break;
                }
            }
        }
    }

    /*destroys previous trail*/
    public void DelTrail()
    {
        if (!firstStartOfBall)
        {
            Destroy(Ball.GetComponent<TrailRenderer>());
        }
    }
    /*creates new component TrailRenderer*/
    public void AddTrail()  
    {
        Ball.AddComponent<TrailRenderer>();
        TrailRenderer Trail = GetComponent<TrailRenderer>();
        firstStartOfBall = false;
        Trail.material.color = GetComponent<Renderer>().material.color;
        Trail.widthMultiplier = 0.1f;
        Trail.time = 99999.0f;
    }
}
