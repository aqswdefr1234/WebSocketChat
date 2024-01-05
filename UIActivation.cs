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
    private ScrollRect scrollRect;//��ũ�ѹٸ� �ڵ����� �Ʒ������� �����ϱ� ����
    private Transform content;

    [Header("Select Server / Client")] 
    [SerializeField] private GameObject selectPanel;
    [SerializeField] private GameObject server;
    [SerializeField] private GameObject client;

    
    void Start()
    {
        nameField = userSetPanel.transform.GetChild(0).GetComponent<TMP_InputField>();
        chatField = chatView.transform.GetChild(2).GetComponent<TMP_InputField>();
        scrollRect = chatView.GetComponent<ScrollRect>();
        content = chatView.transform.GetChild(0).GetChild(0);
        StartCoroutine(ScrollbarBottomFocus());
    }

    void Update()
    {
        PressKey();
    }
    private void PressKey()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //ä��â Ȱ��ȭ �Ǿ� �ִ� ����
            if (chatField.text != "" && chatView.activeSelf == true)
            {
                ClientScript.Instance.SendMessageToServer(chatField.text);
                chatField.text = "";
                chatField.ActivateInputField();
            }
            //�г��� �ʵ� Ȱ��ȭ �Ǿ��ִ� ����
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
        server.SetActive(true);
        client.SetActive(true);
        selectPanel.SetActive(false);
    }
    public void SelectClient()
    {
        server.SetActive(false);
        client.SetActive(true);
        selectPanel.SetActive(false);
    }
    IEnumerator ScrollbarBottomFocus()
    {
        WaitForSeconds delay = new WaitForSeconds(0.1f);
        while (true)
        {
            yield return delay;
            scrollRect.verticalNormalizedPosition = 0;
        }
    }
}
