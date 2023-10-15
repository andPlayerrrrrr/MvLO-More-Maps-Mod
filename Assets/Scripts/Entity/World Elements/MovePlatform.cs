using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector2 initialPosition;
    public float timeX = 1.0f;
    public float timeY = 0f;
    public bool riders = false;
    private List<GameObject> ride = new List<GameObject>(); //床に乗ってるオブジェクト
    public void Start()
    {
            initialPosition = transform.position;
    }
    public void FixedUpdate()
    {
        transform.position = new Vector2(Mathf.Sin(Time.time) * timeX + initialPosition.x, Mathf.Sin(Time.time) * timeY + initialPosition.y);
        if(riders == true)
        {
            foreach (GameObject g in ride)
            {
                Vector2 v = g.transform.position;
                g.transform.position = new Vector3(Mathf.Sin(Time.time) * timeX + v.x, Mathf.Sin(Time.time) * timeY + v.y);   //yの移動は不要////////////
            }
        }
    }
}
