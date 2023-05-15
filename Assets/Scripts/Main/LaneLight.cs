using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneLight : MonoBehaviour
{
    [SerializeField] float feedSpeed = 3;
    [SerializeField] int laneNum = 0;
    [SerializeField] float alfa = 0.0f;

    bool isFlash = false;

    Renderer rend;
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isFlash)
        {
            alfa -= feedSpeed * Time.deltaTime;
            if (alfa < 0)
            {
                alfa = 0;
                isFlash = false;
            }
            rend.material.color = new Color(rend.material.color.r, rend.material.color.g, rend.material.color.b, alfa);
        }

        if (laneNum == 1)
        {
            
            if (Input.GetKeyDown(KeyCode.D)){
                FlashLane();
            }
        }
        if (laneNum == 2)
        {
            if (Input.GetKeyDown(KeyCode.F)){
                FlashLane();
            }
        }
        if (laneNum == 3)
        {
            if (Input.GetKeyDown(KeyCode.J)){
                FlashLane();
            }
        }
        if (laneNum == 4)
        {
            if (Input.GetKeyDown(KeyCode.K)){
                FlashLane();
            }
        }
        
    }

    void FlashLane()
    {
        alfa = 0.3f;
        isFlash = true;
        rend.material.color = new Color(rend.material.color.r, rend.material.color.g, rend.material.color.b, alfa);
    }
}
