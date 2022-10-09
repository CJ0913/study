using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ClientTest : MonoBehaviour
{
    public Button sendBtn;
    public InputField inputField;

    private Socket clientSocket;

    private static byte[] result = new byte[1024];

    // Start is called before the first frame update
    void Start()
    {
        sendBtn.onClick.AddListener(SendMessage);

        //要连接的服务器IP地址
        //IPAddress ip = IPAddress.Any;
        IPAddress ip = IPAddress.Parse("192.168.31.22");

        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            //配置服务器IP与端口，并且尝试连接
            clientSocket.Connect(new IPEndPoint(ip, 60000));
            Debug.Log("Connect Success");
        }
        catch (Exception ex)
        {
            Debug.Log("Connect Fair" + ex);
            return;
        }

        //int receiveLen = clientSocket.Receive(result);
        //Debug.Log("开始" + receiveLen);

        Thread receiveThread = new Thread(ReceiveMessage);
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    private void ReceiveMessage(object obj)
    {
        while (true)
        {
            try
            {
                //Receive 方法将数据读入 buffer 参数，并返回成功读取的字节数
                int len = clientSocket.Receive(result);
                if (len > 0)
                {
                    string recMes = Encoding.ASCII.GetString(result, 0, len);
                    Debug.Log("Server Say：" + recMes);
                }
                else
                {
                    break;
                }

            }
            catch (Exception ex)
            {
                //出现错误 关闭Socket
                //clientSocket.Shutdown(SocketShutdown.Both);
                Debug.LogWarning("错误信息" + ex);
                break;
            }
        }
    }

    private void SendMessage()
    {
        if (!inputField.text.Equals(string.Empty))
        {
            try
            {
                string strMessage = inputField.text;
                Debug.Log("发送消息" + strMessage);
                // 传送信息
                clientSocket.Send(Encoding.ASCII.GetBytes(strMessage));
            }
            catch (Exception ex)
            {
                Debug.Log("发送失败:" + ex);
                //出现错误 关闭Socket
                //clientSocket.Shutdown(SocketShutdown.Both);
            }
        }
    }
}
