using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;
using System.Linq;
using System.Collections.Generic;

public class MatlabServer : MonoBehaviour {

    public static MatlabServer instance;
    public float xMove, yMove = 0;
    public float xForce, yForce = 0;
    [ReadOnly] public bool serverRunning = false;
    [ReadOnly] public string ipAddress = "142.244.63.45"; //This comp: 142.244.63.45, Localhost: 127.0.0.1   
    [ReadOnly] public int port = 9000; 

    private Thread thread;
    private Socket newsock;
    private bool stop = false;

    // Use this for initialization
    void Awake()
    { //Always called before start() functions
      //Makes sure that there is only one instance of Matlab Server (singleton)
        if (instance == null) //If no game control found
        {
            instance = this; //Then this is the instance of the game control
        }
        else if (instance != this) //If the game object finds that instance is already on another game object, then this destroys itself as it's not needed
        {
            Destroy(gameObject);
        }
    }

    void OnApplicationQuit()
    {
        if (GameControl.instance.isRehab)
        {
            Debug.Log("Quit~!");
            thread.Abort();
            newsock.Close();
        }
    }

    public void StartThread()
    {
        thread = new Thread(new ThreadStart(ThreadMethod));
        thread.Start();
    }

    public void StopThread()
    {
        if (serverRunning)
        {
            stop = true;
        }
        else
        {
            thread.Abort();
        }
        newsock.Close();
    }

    private void ThreadMethod()
    {
        int recv;
        byte[] dataRecv = new byte[16];  //Data Received from Simulink
        byte[] dataSend = new byte[24]; //Send Collision Status, X, Y
        IEnumerable<byte> dataSendLINQ = new byte[24]; //Initialize LINQ for easy concatenation later for sending

        //Create IP End point, where I want to connect (Local IP/Port)
        IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(ipAddress), port); 
        //Create UDP Socket
        newsock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //Bind to ip. Server waits for a client at specified ip & port. 
        try
        {
            newsock.Bind(ipep);
        }
        catch (Exception e)
        {
            Debug.Log("Winsock Error: " + e.ToString());
        }
        Debug.Log("Connecting to IP: "+ ipAddress + " Port: "+ port +" Waiting for a client...");

        //Get IP of client
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint Remote = (EndPoint)(sender);

        //Receive binary Data from client
        recv = newsock.ReceiveFrom(dataRecv, ref Remote);

        //Decode data and display
        Debug.Log("Message received from " + Remote.ToString());
        serverRunning = true;

        while (true)
        {
            //Receive X Y positions
            dataRecv = new byte[16];
            recv = newsock.ReceiveFrom(dataRecv, ref Remote);


            //Convert Bytes into Doubles (This gets referenced by mainPlayer.cs to move the character) X/Y Position
            xMove = (float)BitConverter.ToDouble(dataRecv, 0); //x
            yMove = (float)BitConverter.ToDouble(dataRecv, 8); //y


            //Debug.Log("X: " +  xMove + " Y: " + yMove); //Display X/Y Position

            //Concatenate Collision Status: True, xForce, yForce. 
            dataSendLINQ = (BitConverter.GetBytes(1.00)).Concat(BitConverter.GetBytes((double)xForce)).Concat(BitConverter.GetBytes((double)yForce));
            dataSend = dataSendLINQ.ToArray(); //Convert to byte Array from IEnumerable byte Array

            //Debug.Log("X_f: " + xForce + " Y_f: " + yForce); //xForce, yForce

            //Send Info to Simulink
            newsock.SendTo(dataSend, dataSend.Length, SocketFlags.None, Remote);

            if (stop)
            {
                break;
            }
            //Debug.Log(data[0] + " " + data[1] + " " + data[2] + " " + data[3] + " " + data[4] + " " + data[5] + " " + data[6] + " " + data[7]); 
        }
        Debug.Log("Exiting Thread...");
    }
}
