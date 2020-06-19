using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Paddle : MonoBehaviour
{
    [Multiline(12)]
    public string[] StageStr;
    public Sprite[] B;
    public GameObject P_Item;
    public SpriteRenderer P_ItemSr;
    public Text StageText;
    public Text ScoreText;
    public GameObject Life0;
    public GameObject Life1;
    public GameObject WinPanel;
    public GameObject GameOverPanel;
    public GameObject PausePanel;
    public AudioSource S_Break;
    public AudioSource S_Eat;
    public AudioSource S_Fail;
    public AudioSource S_Gun;
    public AudioSource S_HardBreak;
    public AudioSource S_Paddle;
    public AudioSource S_Victory;
    public Transform ItemsTr;
    public Transform BlocksTr;
    public BoxCollider2D[] BlockCol;
    public GameObject[] Ball;
    public Animator[] BallAni;
    public Transform[] BallTr;
    public SpriteRenderer[] BallSr;
    public Rigidbody2D[] BallRg;
    public GameObject[] Bullet;
    public SpriteRenderer RaddleSr;
    public BoxCollider2D PaddleCol;
    public GameObject Magnet;
    public GameObject Gun;

    bool isStart;
    public float paddleX;
    public float ballSpeed;
    float oldBallSpeed = 300;
    float paddleBorder = 2.262f;
    float paddleSize = 1.58f;
    int combo;
    int score;
    int stage;


#if (UNITY_ANDROID)
    private void Awake() { Screen.SetResolution(1080, 1920, false); }
#else
    private void Awake() { Screen.SetResolution(540, 960, false); }
#endif

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(PausePanel.activeSelf) { PausePanel.SetActive(false); Time.timeScale = 1; }
            else { PausePanel.SetActive(true); Time.timeScale = 0; }
        }

    }

    public void AllReset(int _stage)// 스테이지 초기화 (-1이면 재시작, 0이면 다음 스테지, 숫자면 숫자스테이지)
    {
        if (_stage == 0) stage++;
        else if (_stage != -1) stage = _stage;
        if (stage >= StageStr.Length) return;


        BlockGenerator();
        StartCoroutine("BallReset");
    }

    void BlockGenerator()
    {
        string currentStr = StageStr[stage].Replace("\n", "");
        currentStr = currentStr.Replace(" ", "");

        for (int i = 0; i < currentStr.Length; i++)
        {
            BlockCol[i].gameObject.SetActive(false);
            char A = currentStr[i]; string currentName = "Block"; int currentB = 0;

            if (A == '*') continue;
            else if (A == '8') { currentB = 8; currentName = "HardBlock0"; }
            else if (A == '9') { currentB = Random.Range(0, 8); }
            else currentB = int.Parse(A.ToString());

            BlockCol[i].gameObject.name = currentName;
            BlockCol[i].gameObject.GetComponent<SpriteRenderer>().sprite = B[currentB];
            BlockCol[i].gameObject.SetActive(true);
        }
    }


    IEnumerator BallReset()
    {
        BallAni[0].SetTrigger("Blink");

        StopCoroutine("InfinityLoop");
        yield return new WaitForSeconds(0.7f);
        StartCoroutine("InfinityLoop");

    }

    IEnumerator InfinityLoop()
    {
        while(true)
        {
            if(Input.GetMouseButton(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved))
            {
                paddleX = Mathf.Clamp(Camera.main.ScreenToWorldPoint(Input.GetMouseButton(0) ? Input.mousePosition : (Vector3)Input.GetTouch(0).position).x, -paddleBorder, paddleBorder);
                transform.position = new Vector2(paddleX, transform.position.y);
                if(!isStart) BallTr[0].position = new Vector2(paddleX, BallTr[0].position.y);
            }

            if (!isStart && (Input.GetMouseButtonUp(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)))
            {
                isStart = true;
                ballSpeed = oldBallSpeed;
                BallRg[0].AddForce(new Vector2(0.1f, 0.9f).normalized * ballSpeed);
            }
            yield return new WaitForSeconds(0.01f);
        }
        
        

    }
}
