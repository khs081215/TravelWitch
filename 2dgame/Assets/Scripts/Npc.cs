using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/**
 * NPC와의 대화 및 코인 20개의 리워드를 제공합니다.
 */
public class Npc : MonoBehaviour
{
    private bool talkOn = false;
    private bool onDialog = false;
    private GameObject[] player;
    private GameObject[] uicanvas;
    private int dialogFlag = 0;
    private Text dialogText;
    private float delayTime = 0.0f;
    private bool isCrown = false;
    
    private void Start()
    {
        player= GameObject.FindGameObjectsWithTag("Player");
        uicanvas = GameObject.FindGameObjectsWithTag("UI");
    }

    private void Update()
    {
        if(talkOn&&!onDialog)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                player = GameObject.FindGameObjectsWithTag("Player");
                player[0].GetComponent<MovementController>().isTalking = true;
                transform.GetChild(0).gameObject.SetActive(false);
                onDialog = true;
                if(player[0].GetComponent<Player>().inventory.items[0].quantity>19) isCrown = true;
            }
        }

        if (onDialog)
        {

            uicanvas[0].transform.GetChild(0).gameObject.SetActive(true);
            uicanvas[0].transform.GetChild(1).gameObject.SetActive(true);
            uicanvas[0].transform.GetChild(2).gameObject.SetActive(true);
            uicanvas[0].transform.GetChild(3).gameObject.SetActive(true);
            uicanvas[0].transform.GetChild(4).gameObject.SetActive(false);
            uicanvas[0].transform.GetChild(5).gameObject.SetActive(false);
            uicanvas[0].transform.GetChild(6).gameObject.SetActive(false);
            uicanvas[0].transform.GetChild(7).gameObject.SetActive(false);
            dialogText = uicanvas[0].transform.GetChild(2).gameObject.GetComponent<Text>();

            if (!isCrown)
            {
                if (dialogFlag == 0) dialogText.text = "안녕? 혹시 나 좀 도와줄래?";
                if (Input.GetKeyDown(KeyCode.Space)) dialogFlag += 1;
                if (dialogFlag == 1) dialogText.text = "몬스터가 내 금화를 전부 훔쳐갔어.";
                if (dialogFlag == 2) dialogText.text = "금화를 모두 가져다주면 특별한 아이템을 줄게";
                if (dialogFlag == 3) dialogText.text = "내 금화를 가져간 몬스터는 오른쪽 포탈 너머에 있어.";
                if (dialogFlag == 4) dialogText.text = "금화 20개만 가져다줘. 그리고 금화를 노리는 사람이 많으니 조심해";
                if (dialogFlag > 4)
                {
                    delayTime += Time.deltaTime;
                }
            }
            else
            {
                if (dialogFlag == 0) dialogText.text = "정말 금화 20개를 모아왔구나!";
                if (Input.GetKeyDown(KeyCode.Space)) dialogFlag += 1;
                if (dialogFlag == 1) dialogText.text = "정말 고마워!";
                if (dialogFlag == 2) dialogText.text = "약속대로 특별한 아이템인..";
                if (dialogFlag == 3) dialogText.text = "이 왕관을 줄게!";
                if (dialogFlag == 4) dialogText.text = "다음에 또 보자!";
                if (dialogFlag == 5)
                {
                    player[0].transform.GetChild(5).gameObject.SetActive(true);
                }

                if (dialogFlag > 4)
                {
                    delayTime += Time.deltaTime;
                }
            }

            if (delayTime > 0.5f)
            {
                uicanvas[0].transform.GetChild(0).gameObject.SetActive(false);
                uicanvas[0].transform.GetChild(1).gameObject.SetActive(false);
                uicanvas[0].transform.GetChild(2).gameObject.SetActive(false);
                uicanvas[0].transform.GetChild(3).gameObject.SetActive(false);
                uicanvas[0].transform.GetChild(4).gameObject.SetActive(true);
                uicanvas[0].transform.GetChild(5).gameObject.SetActive(true);
                if (player[0].GetComponent<Player>().isSkillCoolTime)
                {
                    uicanvas[0].transform.GetChild(6).gameObject.SetActive(true);
                    uicanvas[0].transform.GetChild(7).gameObject.SetActive(true);
                }

                onDialog = false;
                dialogFlag = 0;
                transform.GetChild(0).gameObject.SetActive(true);
                player[0].GetComponent<MovementController>().isTalking = false;
                delayTime = 0.0f;
            }
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            talkOn = true;
            
            transform.GetChild(0).gameObject.SetActive(true);
        }

    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            talkOn = false;
            transform.GetChild(0).gameObject.SetActive(false);
        }

    }



}
