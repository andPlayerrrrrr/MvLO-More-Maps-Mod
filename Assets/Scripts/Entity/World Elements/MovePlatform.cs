using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector2 initialPosition;
    public float timeX = 1.0f;
    public float timeY = 0f;
    private List<GameObject> ride = new List<GameObject>(); //床に乗ってるオブジェクト
    public void Start()
    {
            initialPosition = transform.position;
    }
    public void FixedUpdate()
    {
        transform.position = new Vector2(Mathf.Sin(Time.time) * timeX + initialPosition.x, Mathf.Sin(Time.time) * timeY + initialPosition.y);
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // 触れたobjの親を移動床にする
            other.transform.SetParent(transform);
        }
    }
    /// <summary>
    /// 移動床のコライダーがobjから離れた時の処理
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // 触れたobjの親をなくす
            other.transform.SetParent(null);
        }
    }
}
