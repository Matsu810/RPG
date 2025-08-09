using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        // スプライトの前面をカメラに向ける（Z軸方向をカメラに合わせる）
        transform.forward = Camera.main.transform.forward;
    }
}
