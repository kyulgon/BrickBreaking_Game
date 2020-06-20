using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
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
    public SpriteRenderer PaddleSr;
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

        Clear();
        BlockGenerator();
        StartCoroutine("BallReset");

        StageText.text = stage.ToString();
        score = 0;
        ScoreText.text = "0";

        PaddleSr.enabled = true;
        Life0.SetActive(true);
        Life1.SetActive(true);
        WinPanel.SetActive(false);
        GameOverPanel.SetActive(false);
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
        isStart = false;
        combo = 0;
        Ball[0].SetActive(true);
        Ball[1].SetActive(false);
        Ball[2].SetActive(false);

        BallAni[0].SetTrigger("Blink");
        BallTr[0].position = new Vector2(paddleX, -3.55f);

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

    public IEnumerator BallCollisionEnter2D(Transform ThisBallTr, Rigidbody2D ThisBallRg, Ball ThisBallCs, GameObject Col, Transform ColTr, SpriteRenderer ColSr, Animator ColAni)
    {
        if (!isStart) yield break;

        switch (Col.name)
        {
            case "Paddle":
                ThisBallRg.velocity = Vector2.zero;
                ThisBallRg.AddForce((ThisBallTr.position - transform.position).normalized * ballSpeed);
                S_Paddle.Play();
                combo = 0;
                break;

            case "DeathZone":
                ThisBallTr.gameObject.SetActive(false);
                BallCheck();
                break;

            case "HardBlock0":
                Col.name = "HardBlock1";
                ColSr.sprite = B[9];
                S_HardBreak.Play();
                break;

            case "HardBlock1":
                Col.name = "HardBlock2";
                ColSr.sprite = B[10];
                S_HardBreak.Play();
                break;

            case "HardBlock2":

            case "Block":
                BlockBreak(Col, ColTr, ColAni);
                break;

        }

    }

    void BlockBreak(GameObject Col, Transform ColTr, Animator ColAni)
    {
        // 아이템 생성

        // 스코어 증가, 콤보당 1점, 3콤도이상은 3점
        score += (++combo > 3) ? 3 : combo;
        ScoreText.text = score.ToString();

        // 벽돌 부서지는 애니메이션
        ColAni.SetTrigger("Break");
        S_Break.Play();
        StartCoroutine(ActiveFalse(Col));

        // 블럭 체크
        StopCoroutine("BlockCheck");
        StartCoroutine("BlockCheck");
    }

    IEnumerator ActiveFalse(GameObject Col)
    {
        yield return new WaitForSeconds(0.2f);
        Col.SetActive(false);
    }

    void BallCheck()
    {
        int ballCount = 0;
        foreach (GameObject OneBall in GameObject.FindGameObjectsWithTag("Ball")) ballCount++;

        if (ballCount == 0)
        {
            if(Life1.activeSelf)
            {
                Life1.SetActive(false);
                StartCoroutine("BallReset");
                S_Fail.Play();
            }
            else if (Life0.activeSelf)
            {
                Life0.SetActive(false);
                StartCoroutine("BallReset");
                S_Fail.Play();
            }
            else
            {
                GameOverPanel.SetActive(true);
                S_Fail.Play();
                Clear();
            }
        }
    }

    IEnumerator BlockCheck()
    {
        yield return new WaitForSeconds(0.5f);
        int blockCount = 0;
        for (int i = 0; i < BlocksTr.childCount; i++)
        {
            if (BlocksTr.GetChild(i).gameObject.activeSelf) blockCount++;
        }

        // 승리
        if(blockCount == 0)
        {
            WinPanel.SetActive(true);
            S_Victory.Play();
            Clear();
        }
    }


    void Clear()
    {
        for (int i = 0; i < 3; i++)
        {
            Ball[i].SetActive(false);
        }
        PaddleSr.enabled = false;
    }

}
