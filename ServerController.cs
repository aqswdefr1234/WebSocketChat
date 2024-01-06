using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;
public class ServerController : MonoBehaviour
{
    private string ipAdress = "";
    private int port = 0;

    private TcpListener server = null;
    private Thread thread;
    private List<Thread> clientThread;
    private List<TcpClient> clientList;
    private List<NetworkStream> streams;
    
    private void Start()
    {
        ipAdress = UIActivation.ip;
        port = UIActivation.port;

        clientThread = new List<Thread>();
        clientList = new List<TcpClient>();
        streams = new List<NetworkStream>();

        thread = new Thread(new ThreadStart(SetupServer));
        thread.Start();
    }
    private void SetupServer()
    {
        try
        {
            IPAddress localAddr = IPAddress.Parse(ipAdress);
            server = new TcpListener(localAddr, port);
            server.Start();

            while (true)
            {
                //Waiting new user
                Debug.Log("Waiting for connection...");
                TcpClient client = server.AcceptTcpClient();
                Debug.Log("Connected!");

                //Waiting new chat
                Thread childTread = new Thread(new ThreadStart(() => SetupClient(client)));
                childTread.Start();

                clientThread.Add(childTread);
                clientList.Add(client);
            }
        }
        catch (SocketException e)
        {
            Debug.Log("SocketException: " + e);
        }
        finally
        {
            server.Stop();
        }
    }
    private void SetupClient(TcpClient client)
    {
        byte[] buffer = new byte[1024];
        string data = null;
        NetworkStream stream = null;
        while (true)
        {
            try
            {
                int i = 0;
                string message = "";
                data = null;

                //waiting user message
                stream = client.GetStream();
                streams.Add(stream);
                while ((i = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    data = Encoding.UTF8.GetString(buffer, 0, i);
                    message = data.ToString();
                    SendMessageToClient(message);
                    Debug.Log("Server " + message);
                }
                client.Close();
            }
            catch(Exception e)
            {
                Debug.LogError("SetupClient: " + e);
                stream.Close();
            }
        }
    }

    private void OnApplicationQuit()
    {
        server.Stop();
        foreach(Thread child in clientThread)
            child.Abort();
        foreach (TcpClient client in clientList)
            client.Close();
        foreach (NetworkStream stream in streams)
            stream.Close();
        thread.Abort();
    }
    public void SendMessageToClient(string message)
    {
        byte[] msg = Encoding.UTF8.GetBytes(message);
        foreach (NetworkStream stream in streams)
            stream.Write(msg, 0, msg.Length);
    }
}
