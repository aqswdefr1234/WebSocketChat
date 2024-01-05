using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Debug = UnityEngine.Debug;
using TMPro;
using System.Collections.Generic;
using System.Collections;
public class ClientScript : MonoBehaviour
{
    [HideInInspector]
    public string userName = "";

    //Receive Chat
    private List<string> chatList = new List<string>();
    [SerializeField] private Transform chatPrefab;
    [SerializeField] private Transform content;

    //Instance
    public static ClientScript instance = null;
    public static ClientScript Instance
    {
        get
        {
            if(instance == null)
                return null;
            return instance;
        }
    }
    //Set Server
    public string serverIP = "172.30.1.98"; // Set this to your server's IP address.
    public int serverPort = 7777;             // Set this to your server's port.
    private TcpClient client;
    private NetworkStream stream;
    private Thread clientReceiveThread;

    void Awake()
    {
        if(instance == null)
            instance = this;

    }
    IEnumerator ReceiveChat()
    {
        WaitForSeconds delay = new WaitForSeconds(0.2f);
        int preCount = 0;
        int newCount = 0;
        while(true)
        {
            newCount = chatList.Count;
            if (preCount != newCount)
            {
                for(int i = 0; i < newCount - preCount; i++)
                {
                    Instantiate(chatPrefab, content).GetComponent<TMP_Text>().text = chatList[newCount - i - 1];
                }
                preCount = chatList.Count;
            }
            yield return delay;
        }
    }
    internal void ConnectToServer()
    {
        try
        {
            client = new TcpClient(serverIP, serverPort);
            stream = client.GetStream();
            Debug.Log($"{userName} : Connected to server.");
            
            StartCoroutine(ReceiveChat());
            clientReceiveThread = new Thread(new ThreadStart(ListenForData));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
        }
        catch (SocketException e)
        {
            Debug.LogError("SocketException: " + e.ToString());
        }
    }

    private void ListenForData()
    {
        try
        {
            byte[] bytes = new byte[1024];
            while (true)
            {
                // Check if there's any data available on the network stream
                if (stream.DataAvailable)
                {
                    int length;
                    // Read incoming stream into byte array.
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incomingData = new byte[length];
                        Array.Copy(bytes, 0, incomingData, 0, length);
                        // Convert byte array to string message.
                        string serverMessage = Encoding.UTF8.GetString(incomingData);
                        Debug.Log("Server message received: " + serverMessage);
                        chatList.Add(serverMessage);
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    public void SendMessageToServer(string message)
    {
        if (client == null || !client.Connected)
        {
            Debug.LogError("Client not connected to server.");
            return;
        }
        string sendMessage = $"{userName} : {message}";
        byte[] data = Encoding.UTF8.GetBytes(sendMessage);
        stream.Write(data, 0, data.Length);
    }

    void OnApplicationQuit()
    {
        if (stream != null)
            stream.Close();
        if (client != null)
            client.Close();
        if (clientReceiveThread != null)
            clientReceiveThread.Abort();
    }
}
