using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class Turn_Manager : MonoBehaviour
{
    public Button button;
    public GameObject circularImageObject;
    public GameObject bg;
    public GameObject Jang;
    public GameObject RedJang;
    public GameObject RedDoor0;
    public GameObject RedDoor1;
    private SpriteRenderer bgSpriter;
    public Sprite[] BackGroundSprite;
    public Image circularImage;
    private float duration = 1f;  
    public GameObject[] Ghosts;
    RectTransform rt;
    private int currentnum = 1;
    private int currentday = 0;


    private EventManager eventManager;

    // public List<GameObject> panels;
    private List<int> currentDayList;
    private List<int> day0 = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 0, 0, 0, 0 };
    private List<int> day1 = new List<int> { 0, 0, 0,0,0,0,0,0 ,0,1,2,0,0,0,0};
    private List<int> day2 = new List<int> { 6, 6, 4,5,6,6,6,3,6,6,4,5,6,6,6,6,6,6,6 };
    private List<int> day3 = new List<int> { 7,7,7,7,7,7 };
    private Camera camera;
    private Vector3 originalPosition;

    float shakeDuration = 0.3f;   // ��鸲 ���� �ð�
    float shakeIntensity = 0.3f;  // ��鸲 ����

    Tuple<int, int> dayAndTurn;

    void Awake()
    {
        eventManager = FindObjectOfType<EventManager>();
        bgSpriter = bg.GetComponent<SpriteRenderer>();
        camera = Camera.main;
        originalPosition = camera.transform.position;
    }

    void Start(){
        dayAndTurn = SaveManager.getDayAndTurn();
        currentday = dayAndTurn.Item1;
        currentnum = dayAndTurn.Item2;
        Debug.Log("현재 일차-현재 턴: " + currentday + "-" + currentnum);

        rt = Ghosts[0].GetComponent<RectTransform>();
        ShowPanel();
    }
    public void ShowPanel()
    {
        triggerEvent();
        SaveManager.setDayAndTurn(currentday, currentnum);
        if (currentday == 0)
        {
            currentDayList = day0;
        }
        else if (currentday == 1)
        {
            currentDayList = day1;
        }
        else if (currentday == 2)
        {
            currentDayList = day2;
        }
        else if (currentday == 3) {
            currentDayList = day3;
        }
        else
        {
            Debug.LogError("Invalid currentday value.");
            return;
        }

        if (currentnum == 0 && currentDayList != day0)
        {
            RedDoor1.SetActive(false);
            RedDoor0.SetActive(true);
            Jang.SetActive(false);
            RedJang.SetActive(false);

        }
        else if (currentDayList == day0 && currentnum == 1) {
            RedDoor1.SetActive(false);
            RedDoor0.SetActive(true);
            Jang.SetActive(false);

        }
        else if (currentnum == currentDayList.Count - 1 || (currentnum == 0 && currentDayList == day0))
        {
            RedDoor0.SetActive(false);
            RedDoor1.SetActive(true);

            if (currentDayList == day2)
            {
                RedJang.SetActive(true);
            }
            else
            {
                Jang.SetActive(true);
            }
        }
        else
        {
            RedDoor0.SetActive(false);
            RedDoor1.SetActive(false);
            Jang.SetActive(false);
            RedJang.SetActive(false);
        }

        if (currentDayList[currentnum] == 0)  //�⺻
        {
            bgSpriter.sprite = bgSpriter.sprite == BackGroundSprite[0] ? BackGroundSprite[1] : BackGroundSprite[0];
        }
        else if (currentDayList[currentnum] == 1) //���α� 1
        {
            bgSpriter.sprite = BackGroundSprite[2];
        }
        else if (currentDayList[currentnum] == 2) //���α� 2
        {
            bgSpriter.sprite = BackGroundSprite[3];
        }
        else if (currentDayList[currentnum] == 3) //����
        {
            bgSpriter.sprite = BackGroundSprite[4];
        }
        else if (currentDayList[currentnum] == 4) //2���� ���α� 1
        {
            bgSpriter.sprite = BackGroundSprite[5];
        }
        else if (currentDayList[currentnum] == 5) //2���� ���α� 2
        {
            bgSpriter.sprite = BackGroundSprite[6];
        }
        else if (currentDayList[currentnum] == 6)
        { //2���� �⺻
            if (bgSpriter.sprite == BackGroundSprite[7])
            {
                bgSpriter.sprite = BackGroundSprite[8];
            }
            else
            {
                bgSpriter.sprite = BackGroundSprite[7];
            }
        }
        else if (currentDayList[currentnum] == 7) { //�������� �⺻1,2
            if (bgSpriter.sprite == BackGroundSprite[9])
            {
                bgSpriter.sprite = BackGroundSprite[10];
            }
            else
            {
                bgSpriter.sprite = BackGroundSprite[9];
            }
        }

        //���� �⺻1,2 , ���α� 1,2, ���� ,2���� ���α� 1,2, 2���� �⺻ 1,2, �������� �⺻1,2
        currentnum++;
        


        if (currentnum >= currentDayList.Count)
        {
            currentnum = 0;
            currentday++;
        }

        circularImageObject.SetActive(true);
        circularImage.fillAmount = 1f;

        // StopCoroutine(FillAmountLerp());
        StartCoroutine(FillAmountLerp());

        button.enabled = false;

        Invoke("createButton", duration);

    }

    public void createButton()
    {
        button.enabled = true;
    }

    IEnumerator FillAmountLerp()
    {
        float elapsedTime = 0f;
        float startValue = 1f;
        float endValue = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newValue = Mathf.Lerp(startValue, endValue, elapsedTime / duration);
            circularImage.fillAmount = newValue;
            yield return null;
        }

        // ���� ���� ��Ȯ�ϰ� ������
        circularImage.fillAmount = endValue;
    }

    public IEnumerator CameraShake()
    {

         // ī�޶� ���� ��ġ ����

        for (float elapsed = 0; elapsed < shakeDuration; elapsed += Time.deltaTime)
        {
            
            float offsetY = -Mathf.Sin(elapsed * 15) * shakeIntensity;  
            camera.transform.position = originalPosition + new Vector3(0, offsetY, 0);

            yield return null; 
        }

        // ��鸲�� ���� �� ī�޶� ��ġ�� ���� ��ġ�� ����
        camera.transform.position = originalPosition;
    }

    public void cameraWalking()
    {
        StartCoroutine(CameraShake());
        AudioManager.Instance.PlaySFX("PlayerWalking");
    }

    void triggerEvent() {

        int day = currentday;
        int turn = currentnum;

        if (day == 0 && turn == 0)
        {
            eventManager.Event0_0();
        }
        else if (day == 0 && turn == 3) {
            eventManager.Event0_3();
        }
        else if (day == 0  && turn == 7)
        {
            rt.localScale = new Vector3(1f, 1f, 1f);
            Ghosts[0].SetActive(true);
        }

        else if (day == 0 && turn == 8)
        {
            rt.localScale = new Vector3(2f, 2f, 1f);
        }

        else if (day == 0 && turn == 9)
        {
            Ghosts[0].SetActive(false);
        }

        else if (day == 0 && turn == 10)
        {
            eventManager.Event0_10();
            AudioManager.Instance.PlaySFX("WallGhostAppear");
        }

        else if (day == 0 && turn == 11)
        {
            StartCoroutine(eventManager.Event0_11());
        }

        if(day == 0 && turn == 13)
        {
            eventManager.Event0_13();
        }

        if (day == 0 && turn == 14)
        {
            eventManager.Event0_14();
        }

        if (day == 0 && turn == 15){
            eventManager.Event0_15();
            AudioManager.Instance.PlaySFX("crow");

        }

        else if (day == 1 && turn == 2)
        {
            StartCoroutine(eventManager.Event1_2());
            AudioManager.Instance.PlaySFX("NeckGhostHorror");
        }
        /*
        else if (day == 1 && turn == 4)
        {
            eventManager.Event1_4();

        }*/
        else if (day == 1 && turn == 6)
        {
            StartCoroutine(eventManager.Event1_6());
        }

        else if (day == 1 && turn == 9)
        {
            eventManager.Event1_9();
        }

        else if (day == 1 && turn == 10)
        {
            eventManager.Event1_10();
        }

        else if (day == 1 && turn == 11)
        {
            eventManager.Event1_11();
        }

        else if (day == 1 && turn == 12)
        {
            eventManager.Event1_12();
        }

        else if (day == 1 && turn == 13)
        {
            eventManager.Event1_13();
        }

        else if (day == 1 && turn == 14)
        {
            eventManager.Event1_14();
        }

        else if (day == 2 && turn == 1)
        {
            AudioManager.Instance.PlaySFX("KnockDoor");
        }

        else if (day == 2 && turn == 5)
        {
            eventManager.Event2_05();
        }

        else if (day == 2 && turn == 7){
            StartCoroutine(eventManager.Event2_7());
        }

        else if (day == 2 && turn == 8){
            StartCoroutine(eventManager.Event2_8());
        }

        else if (day == 2 && turn == 9){
            eventManager.Event2_9();
        }

        else if (day == 2 && turn == 12){
            eventManager.Event2_12();
        }

        else if (day == 2 && turn == 13){
            eventManager.Event2_13();
        }

        else if (day == 2 && turn == 14){
            StartCoroutine(eventManager.Event2_14());
            
        }
        else if (day == 2 && turn == 16)
        {
            eventManager.Event2_16();
        }
        else if (day == 2 && turn == 17)
        {
            eventManager.Event2_17();
        }

        else if (day == 2 && turn == 18)
        {
            AudioManager.Instance.PlaySFX("Scream");
            AudioManager.Instance.PlayMusic("3bgm");
        }
        else if (day == 3 && turn == 4)
        {
            eventManager.Event3_1();
        }
    }
    
}




