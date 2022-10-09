using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Server : MonoBehaviour
{
    //状态信息
    private string stateInfo = "NULL";
    //IP地址
    private string inputIp = "172.168.2.1";
    //端口号
    private string inputPort = "6000";
    //用于监听的套接字
    private Socket socketWatch;
    //用于和客户端通信的套接字
    private Socket socketSend;

    public string inputMes = "NULL";             //发送的消息
    private int recTimes = 0;                    //接收到信息的次数
    private string recMes = "NULL";              //接收到的消息
    private bool clickConnectBtn = false;        //是否点击发送按钮
    private bool isSendData = false;             //是否点击发送按钮


    //首先根据服务器端的ip地址和设置的端口号与服务器建立链接，并创建子线程进行数据的发送和接收。
    private void ServerConnect()
    {
        try
        {
            //获取端口号
            int _port = Convert.ToInt32(inputPort);
            //获取Ip号
            string _ip = inputIp;

            Debug.Log(" ip 地址是 ：" + _ip);
            Debug.Log(" 端口号是 ：" + _port);

            clickConnectBtn = true;

            stateInfo = "ip地址是：" + _ip + "   端口号是：" + _port;

            //点击开始监听时 在服务端创建一个负责监听IP和端口号的Socket
            socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //IPAddress类提供了对IP地址的转换、处理等功能。其Parse方法可将IP地址字符串转换为IPAddress实例。
            IPAddress ip = IPAddress.Parse(_ip);
            //获取终结点的IP地址和端口号
            IPEndPoint point = new IPEndPoint(ip, _port);

            //绑定端口号
            socketWatch.Bind(point);

            Debug.Log("监听成功");
            stateInfo = "监听成功";

            //设置监听，最大同时连接10台
            socketWatch.Listen(10);

            //开启新的线程，不停的接收服务器发来的消息
            Thread r_thread = new Thread(Listen);
            //获取或设置一个值，该值指示某个线程是否为后台线程
            r_thread.IsBackground = true;
            r_thread.Start();
        }
        catch { }
    }

    //等待客户端的连接 并且创建与之通信的Socket
    private void Listen(object o)
    {
        try
        {
            Socket socketWatch = o as Socket;
            while (true)
            {
                //等待接收客户端连接
                //为新建连接创建新的 Socket
                socketSend = socketWatch.Accept();
                Debug.Log(socketSend.RemoteEndPoint.ToString() + ":" + "连接成功!");
                stateInfo = socketSend.RemoteEndPoint.ToString() + "  连接成功!";

                //开启一个新线程，执行接收消息方法
                Thread r_thread = new Thread(Received);
                r_thread.IsBackground = true;
                r_thread.Start(socketSend);

                //开启一个新线程，执行发送消息方法
                Thread s_thread = new Thread(SendMessage);
                s_thread.IsBackground = true;
                s_thread.Start(socketSend);
            }
        }
        catch { }

    }

    //服务器端不停的接收客户端发来的消息
    private void Received(object obj)
    {
        while (true)
        {
            try
            {
                //客户端连接服务器成功后，服务器接收客户端发送的消息
                byte[] buffer = new byte[1024 * 6];
                //实际接收到的有效字节数
                //Receive 方法将数据读入 buffer 参数，并返回成功读取的字节数
                int len = socketSend.Receive(buffer);
                if (len == 0)
                {
                    break;
                }

                recMes = Encoding.UTF8.GetString(buffer, 0, len);

                Debug.Log("接收到的消息：" + socketSend.RemoteEndPoint + ":" + recMes);

                recTimes++;
                stateInfo = "接收到一次数据，接收次数为 ：" + recTimes;
                Debug.Log("接收次数为：" + recTimes);
            }
            catch { }
        }
    }


    //发送消息的子线程通过点击客户端界面中的按钮进行触发，发送编辑好的信息至服务器。
    private void SendMessage(object o)
    {
        try
        {
            Socket socketSend = o as Socket;
            while (true)
            {
                //如果点击了发送按钮
                if (isSendData)
                {
                    isSendData = false;
                    string msg = inputMes;
                    byte[] buffer = new byte[1024 * 6];
                    buffer = Encoding.UTF8.GetBytes(msg);
                    socketSend.Send(buffer);
                    Debug.Log("发送的数据为 :" + inputMes);
                    Debug.Log("发送的数据字节长度 :" + buffer.Length);
                }
            }
        }
        catch { }
    }

    private void OnDisable()
    {
        Debug.Log("begin OnDisable()");

        if (clickConnectBtn)
        {
            try
            {
                socketWatch.Shutdown(SocketShutdown.Both);    //禁用Socket的发送和接收功能
                socketWatch.Close();

                socketSend.Shutdown(SocketShutdown.Both);    //禁用Socket的发送和接收功能
                socketSend.Close();                          //关闭Socket连接并释放所有相关资源
            }
            catch (Exception e)
            {
                print(e.Message);
            }
        }

        Debug.Log("end OnDisable()");
    }

    //界面
    void OnGUI()
    {
        GUI.color = Color.black;

        GUI.Label(new Rect(65, 10, 80, 20), "状态信息");

        GUI.Label(new Rect(155, 10, 80, 70), stateInfo);

        GUI.Label(new Rect(65, 80, 80, 20), "接收到消息：");

        GUI.Label(new Rect(155, 80, 80, 20), recMes);

        GUI.Label(new Rect(65, 120, 80, 20), "发送的消息：");

        inputMes = GUI.TextField(new Rect(155, 120, 100, 20), inputMes, 20);

        GUI.Label(new Rect(65, 160, 80, 20), "本机ip地址：");

        inputIp = GUI.TextField(new Rect(155, 160, 100, 20), inputIp, 20);

        GUI.Label(new Rect(65, 200, 80, 20), "本机端口号：");

        inputPort = GUI.TextField(new Rect(155, 200, 100, 20), inputPort, 20);

        if (GUI.Button(new Rect(65, 240, 60, 20), "开始监听"))
        {
            ServerConnect();
        }

        if (GUI.Button(new Rect(65, 280, 60, 20), "发送数据"))
        {
            isSendData = true;
        }
    }
}
