using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreChange : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI point;
    [SerializeField] TextMeshProUGUI perfect;
    [SerializeField] TextMeshProUGUI great;
    [SerializeField] TextMeshProUGUI bad;
    [SerializeField] TextMeshProUGUI miss;
    [SerializeField] TextMeshProUGUI rating;


    // Start is called before the first frame update
    void Start()
    {
        point.text      = "SCORE:"  + GameManager.Instance.point.ToString("d4");
        perfect.text    = "PERFECT:"+ GameManager.Instance.perfect.ToString("d4"); 
        great.text      = "GREAT:"  + GameManager.Instance.great.ToString("d4"); 
        bad.text        = "BAD:"    + GameManager.Instance.bad.ToString("d4"); 
        miss.text       = "MISS:"   + GameManager.Instance.miss.ToString("d4");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
