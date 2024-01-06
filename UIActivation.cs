using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class UIActivation : MonoBehaviour
{
    
    [Header("Chat Controll")]
    [SerializeField] private GameObject userSetPanel;
    [SerializeField] private GameObject chatView;
    [SerializeField] private Transform chatPrefab;
    private TMP_InputField nameField;
    private TMP_InputField chatField;
    private ScrollRect scrollRect;
    private Transform content;

    [Header("Select Server / Client")] 
    [SerializeField] private GameObject selectPanel;
    [SerializeField] private GameObject server;
    [SerializeField] private GameObject client;
    private TMP_InputField ipField;
    private TMP_InputField portField;

    [HideInInspector]
    public static string notification = "";

    [Header("Notification Text")]
    [SerializeField] TMP_Text notificationText;

    public static string ip = "";
    public static int port = 0;

    void Start()
    {
        nameField = userSetPanel.transform.GetChild(0).GetComponent<TMP_InputField>();

        chatField = chatView.transform.GetChild(2).GetComponent<TMP_InputField>();
        scrollRect = chatView.GetComponent<ScrollRect>();
        content = chatView.transform.GetChild(0).GetChild(0);

        ipField = selectPanel.transform.GetChild(2).GetComponent<TMP_InputField>();
        portField = selectPanel.transform.GetChild(3).GetComponent<TMP_InputField>();

        StartCoroutine(Detection());
    }

    void Update()
    {
        PressKey();
    }
    private void PressKey()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //채팅창 활성화 되어 있는 상태
            if (chatField.text != "" && chatView.activeSelf == true)
            {
                ClientScript.Instance.SendMessageToServer(chatField.text);
                chatField.text = "";
                chatField.ActivateInputField();
            }
            //닉네임 필드 활성화 되어있는 상태
            else if (nameField.text != "" && userSetPanel.activeSelf == true)
            {
                ClientScript.Instance.userName = nameField.text;
                ClientScript.Instance.ConnectToServer();

                userSetPanel.SetActive(false);
                chatView.SetActive(true);
            }
        }
    }
    public void SelectServer()
    {
        if (IsFilledIP() == false)
            return;

        server.SetActive(true);
        client.SetActive(true);
        selectPanel.SetActive(false);
    }
    public void SelectClient()
    {
        if (IsFilledIP() == false)
            return;

        server.SetActive(false);
        client.SetActive(true);
        selectPanel.SetActive(false);
    }
    private bool IsFilledIP()
    {
        if (ipField.text != "" && portField.text != "")
        {
            ip = ipField.text;
            port = Convert.ToInt32(portField.text);
            return true;
        }
            
        else
        {
            notification = "IP or Port are empty";
            return false;
        } 
    }
    IEnumerator Detection()
    {
        WaitForSeconds delay = new WaitForSeconds(0.1f);
        while (true)
        {
            yield return delay;

            Notify();//알림 감지
            scrollRect.verticalNormalizedPosition = 0;//스크롤바 아래로 고정
        }
    }
    private void Notify()
    {
        if(notification != "")
        {
            notificationText.text = notification;
            notification = "";
            Invoke("ClearNoti", 5f);
        }
    }
    private void ClearNoti()
    {
        notificationText.text = "";
    }
}
