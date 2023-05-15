using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollObject : MonoBehaviour
{
    public float speed = 1f;
    public float startPosition;
    public float endPosition;

    void Update()
    {
        //毎フレームx座標を少し移動させる
        transform.Translate(-1 * speed * Time.deltaTime, 0f, 0f);

        // スクロールが目標ポイントに達したかのチェック
        if (transform.position.x <= endPosition)
        {
            ScrollEnd();
        }
    }

    void ScrollEnd()
    {
        // 通り過ぎた分を加味してposition設定
        float diff = transform.position.x - endPosition;
        Vector3 restartPosition = transform.position;
        restartPosition.x = startPosition + diff;
        transform.position = restartPosition;

        SendMessage("OnScrollEnd", SendMessageOptions.DontRequireReceiver);
    }
}
