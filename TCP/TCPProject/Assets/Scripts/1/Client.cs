using UnityEngine;
using System.Net;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Text;


/*
 * 使用同步TCP编写客户端程序的一般步骤为：
 * 创建一个包含传输过程中采用的网络类型、数据传输类型和协议类型的Socket对象。
 * 与远程服务器建立连接。
 * 与服务器进行数据传输。
 * 完成工作后，向服务器发送关闭信息，并关闭与服务器的连接。
 */
public class Client : MonoBehaviour
{
    //状态信息
    private string stateInfo = "NULL";
    //IP地址
    private string inputIp = "172.168.2.1";
    //端口号
    private string inputPort = "6000";
    //客户端套接字，用来链接远端服务器
    private Socket socketSend;

    public string inputMes = "NULL";             //发送的消息
    private int recTimes = 0;                    //接收到信息的次数
    private string recMes = "NULL";              //接收到的消息
    private bool clickSend = false;              //是否点击发送按钮


    //首先根据服务器端的ip地址和设置的端口号与服务器建立链接，并创建子线程进行数据的发送和接收。
    private void ClientConnect()
    {
        try
        {
            //获取端口号
            int _port = Convert.ToInt32(inputPort);
            //获取Ip号
            string _ip = inputIp;

            //创建客户端Socket，获得远程IP和端口号
            //new Socket(网络类型,套接字类型,使用的协议)
            socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //IPAddress类提供了对IP地址的转换、处理等功能。其Parse方法可将IP地址字符串转换为IPAddress实例。
            IPAddress ip = IPAddress.Parse(_ip);
            //获取终结点的IP地址和端口号
            IPEndPoint point = new IPEndPoint(ip, _port);

            socketSend.Connect(point);
            Debug.Log("连接成功，" + "IP = " + ip + "port = " + _port);
            stateInfo = ip + ":" + _port + "连接成功";

            //开启新的线程，不停的接收服务器发来的消息
            Thread r_thread = new Thread(Received);
            //获取或设置一个值，该值指示某个线程是否为后台线程
            r_thread.IsBackground = true;
            r_thread.Start();

            //开启新的线程，不停的给服务器发送消息
            Thread s_thread = new Thread(SendMessage);
            s_thread.IsBackground = true;
            s_thread.Start();

        }
        catch (Exception)
        {
            Debug.Log("IP或者端口号错误......");
            stateInfo = "IP或者端口号错误......";
        }
    }

    //接收消息,将接收到的字节数组转换为字符串并进行显示
    private void Received()
    {
        while (true)
        {
            try
            {
                byte[] buffer = new byte[1024 * 6];
                //实际接收到的有效字节数
                //Receive 方法将数据读入 buffer 参数，并返回成功读取的字节数
                int len = socketSend.Receive(buffer);
                if (len == 0)
                {
                    break;
                }

                recMes = Encoding.UTF8.GetString(buffer, 0, len);

                Debug.Log("客户端接收到的数据 ： " + recMes);

                recTimes++;
                stateInfo = "接收到一次数据，接收次数为 ：" + recTimes;
                Debug.Log("接收次数为：" + recTimes);
            }
            catch { }
        }
    }


    //发送消息的子线程通过点击客户端界面中的按钮进行触发，发送编辑好的信息至服务器。
    private void SendMessage()
    {
        try
        {
            while (true)
            {
                //如果点击了发送按钮
                if (clickSend)
                {
                    clickSend = false;
                    string msg = inputMes;
                    byte[] buffer = new byte[1024 * 6];
                    buffer = Encoding.UTF8.GetBytes(msg);
                    socketSend.Send(buffer);
                }
            }
        }
        catch { }
    }

    private void OnDisable()
    {
        Debug.Log("begin OnDisable()");

        if (socketSend.Connected)
        {
            try
            {
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

    //用户界面
    void OnGUI()
    {
        GUI.color = Color.black;

        GUI.Label(new Rect(65, 10, 60, 20), "状态信息");

        GUI.Label(new Rect(135, 10, 80, 60), stateInfo);

        GUI.Label(new Rect(65, 70, 50, 20), "服务器ip地址");

        inputIp = GUI.TextField(new Rect(125, 70, 100, 20), inputIp, 20);

        GUI.Label(new Rect(65, 110, 50, 20), "服务器端口");

        inputPort = GUI.TextField(new Rect(125, 110, 100, 20), inputPort, 20);

        GUI.Label(new Rect(65, 150, 80, 20), "接收到消息：");

        GUI.Label(new Rect(155, 150, 80, 20), recMes);

        GUI.Label(new Rect(65, 190, 80, 20), "发送的消息：");

        inputMes = GUI.TextField(new Rect(155, 190, 100, 20), inputMes, 20);

        if (GUI.Button(new Rect(65, 230, 60, 20), "开始连接"))
        {
            ClientConnect();
        }

        if (GUI.Button(new Rect(65, 270, 60, 20), "发送信息"))
        {
            clickSend = true;
        }
    }
}
