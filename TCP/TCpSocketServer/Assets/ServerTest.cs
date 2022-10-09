using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ServerTest : MonoBehaviour
{
    //端口号
    private static int myPort = 60000;

    private static Socket serverSocket;

    private Thread myThread;

    //存储线程，程序结束后关闭线程
    private Dictionary<string, Thread> dicThreads = new Dictionary<string, Thread>();

    //储存byte的值
    private byte[] result = new byte[1024];

    // Start is called before the first frame update
    void Start()
    {
        //服务器IP地址  127.0.1为本机IP地址
        //IPAddress ip = IPAddress.Parse("127.0.0.1");
        IPAddress ip = IPAddress.Any;//本机地址
        //本地IP使用127.0.0.1是无法连接外网的 所以或许写IP.Any会好一些，无论是本机做测试还是远程外网的连接都能够连接的上，因为它本身可以匹配任意的目标ip

        Debug.Log(ip.ToString());

        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //IPEndPoint作用是用来封装ip和端口号，以便于指定一个唯一设备
        IPEndPoint ipEndPoint = new IPEndPoint(ip, myPort);

        //绑定IP地址，端口
        //serverSocket.Bind(new IPEndPoint(ip, myPort));
        serverSocket.Bind(ipEndPoint);

        //最多10个连接请求
        serverSocket.Listen(10);

        //Socket.LocalEndPoint:本地终结点
        Debug.Log("Create server " + serverSocket.LocalEndPoint.ToString() + " sucess");

        //服务器线程开启
        myThread = new Thread(ListenClientConnect);
        myThread.Start();

        Debug.Log("服务器启动");

    }

    //监听客户端是否连接
    private void ListenClientConnect(object obj)
    {
        while (true)
        {
            // 1.创建一个Socket 接受客户端发过来的请求消息 没有消息时堵塞
            Socket clientSocket = serverSocket.Accept();
            // 2.向客户端发送请求 连接成功 消息
            clientSocket.Send(Encoding.ASCII.GetBytes("Server Say Hello"));
            // 3.为已经连接的客户端创建一个线程 此线程用来处理客户端发送的消息
            Thread receiveThread = new Thread(ReceiveMessage);
            // 4.开启线程
            receiveThread.Start(clientSocket);

            //将创建的线程添加到字典当中
            //Socket.RemoteEndPoint:远程终结点
            string strClientIp = ((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString();
            if (!dicThreads.ContainsKey(strClientIp))
            {
                dicThreads.Add(strClientIp, receiveThread);
            }

            Debug.Log("clientSocket.LocalEndPoint:" + clientSocket.LocalEndPoint.ToString());
        }
    }

    //开启线程接受数据（将Socket传入）
    private void ReceiveMessage(object obj)
    {
        //将传入的参数进行强转
        Socket myClientSocket = obj as Socket;
        while (true)
        {
            try//接受数据
            {
                //将客户端得到的byte值写入
                int receiveNumber = myClientSocket.Receive(result);

                Debug.Log("接收字节数量：" + receiveNumber);

                if (receiveNumber > 0)
                {
                    Debug.Log("Client Say:" + Encoding.ASCII.GetString(result, 0, receiveNumber));
                }
                else
                {
                    string strAdress = ((IPEndPoint)myClientSocket.RemoteEndPoint).Address.ToString();
                    Debug.Log("Client:" + strAdress + "断开连接");
                    //清除线程
                    dicThreads[strAdress].Abort();
                }
            }
            catch (Exception ex)
            {
                //出现错误 关闭Socket
                //myClientSocket.Shutdown(SocketShutdown.Both);
                Debug.LogWarning("错误信息" + ex);
                break;
            }
        }
    }

    private void OnApplicationQuit()
    {
        //结束线程必须关闭 否则下次开启会出现错误（如果出现的话 只能重启Unity）
        myThread.Abort();

        //关闭开启的线程
        foreach(string item in dicThreads.Keys)
        {
            Debug.Log(item);
            dicThreads[item].Abort();
        }

    }
}
