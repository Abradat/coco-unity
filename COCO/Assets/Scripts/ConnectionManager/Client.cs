using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;

// Client Class for Socket Connection
public class Client : MonoBehaviour
{
    // region private members     
    private TcpClient socketConnection;
    private Thread clientReceiveThread;
    private readonly string host = "localhost";
    private readonly int port = 8976;
    private string serverMessage;
    // endregion
    // Use this for initialization  
    void Start()
    {
        Debug.Log("Starting Socket");
        ConnectToTcpServer();
    }
    // Update is called once per frame
    void Update()
    {
        // Check whether a message is received from the server or not
        // If a message is received from the server, an event is propagated to the listeners
        switch (serverMessage)
        {
            case "0":
                ServiceLocator.Instance.eventManager.Propagate(new ZeroDetected());
                break;
            case "1":
                ServiceLocator.Instance.eventManager.Propagate(new OneDetected());
                break;
            case "2":
                ServiceLocator.Instance.eventManager.Propagate(new TwoDetected());
                break;
            case "3":
                ServiceLocator.Instance.eventManager.Propagate(new ThreeDetected());
                break;
            case "4":
                ServiceLocator.Instance.eventManager.Propagate(new FourDetected());
                break;
            case "5":
                ServiceLocator.Instance.eventManager.Propagate(new FiveDetected());
                break;
        }
    }
    // Connect to the server and listen for data from the server
    // The ListenForData function runs as a thread in the application in order to avoid delay in the main process
    private void ConnectToTcpServer()
    {
        try
        {
            clientReceiveThread = new Thread(new ThreadStart(ListenForData));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
        }
        catch (Exception e)
        {Debug.Log("On client connect exception " + e);}
    }

    // This function opens a socket connection to the server and reads the incoming data
    private void ListenForData()
    {
        try
        {
            socketConnection = new TcpClient(host, port); // Opens Socket 
            Byte[] bytes = new Byte[10];
            while (true)
            {
                // Get a stream object for reading              
                using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;
                    // Read incomming stream into byte arrary.                  
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);
                        // Convert byte array to string message.                        
                        serverMessage = Encoding.ASCII.GetString(incommingData);
                        Debug.Log("server message received as: " + serverMessage);
                    }
                }
            }
        }
        catch (SocketException socketException)
        {Debug.Log("Socket exception: " + socketException);}
    }
}
