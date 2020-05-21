using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCPReportTM
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        //创建 1个客户端套接字 和1个负责监听服务端请求的线程  
        Socket socketClient = null;
        Thread threadClient = null;
        string IPAdd = "192.168.196.209";
        string RePort = "8885";

        private void BtnBeginListen_Click(object sender, EventArgs e)
        {
            if (txtUser.Text != "")
            {
                bool IsContain = false;
                int[] num = new int[10] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                int i = 0;
                if (IPAdd.Contains(num[i].ToString()))
                {
                    //定义一个套字节监听  包含3个参数(IP4寻址协议,流式连接,TCP协议)
                    socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    //需要获取文本框中的IP地址
                    IPAddress ipaddress = IPAddress.Parse(IPAdd.Trim());
                    //将获取的ip地址和端口号绑定到网络节点endpoint上
                    IPEndPoint endpoint = new IPEndPoint(ipaddress, int.Parse(RePort.Trim()));
                    //这里客户端套接字连接到网络节点(服务端)用的方法是Connect 而不是Bind
                    socketClient.Connect(endpoint);
                    //创建一个线程 用于监听服务端发来的消息
                    threadClient = new Thread(RecMsg);
                    //将窗体线程设置为与后台同步
                    threadClient.IsBackground = true;
                    //启动线程
                    threadClient.Start();
                }
                else
                {
                    //定义一个套字节监听  包含3个参数(IP4寻址协议,流式连接,TCP协议)
                    socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    //需要获取文本框中的IP地址                
                    IPAddress[] ips = Dns.GetHostAddresses(IPAdd.Trim());
                    string ic = Convert.ToString(ips[1]);
                    IPAddress ipaddress = IPAddress.Parse(ic);
                    //将获取的ip地址和端口号绑定到网络节点endpoint上
                    IPEndPoint endpoint = new IPEndPoint(ipaddress, int.Parse(RePort.Trim()));
                    //这里客户端套接字连接到网络节点(服务端)用的方法是Connect 而不是Bind
                    socketClient.Connect(endpoint);
                    //创建一个线程 用于监听服务端发来的消息
                    threadClient = new Thread(RecMsg);
                    //将窗体线程设置为与后台同步
                    threadClient.IsBackground = true;
                    //启动线程
                    threadClient.Start();
                }
                btnSend.Enabled = true;
                btnConnServer.Enabled = false;
                txtUser.ReadOnly = true;
            }
            else
            {
                MessageBox.Show("昵称不能为空","提示");
            }
        }
        /// <summary>
        /// 接收服务端发来信息的方法
        /// </summary>
        private void RecMsg()
        {
            while (true) //持续监听服务端发来的消息
            {
                //定义一个1M的内存缓冲区 用于临时性存储接收到的信息
                byte[] arrRecMsg = new byte[1024 * 1024];
                //将客户端套接字接收到的数据存入内存缓冲区, 并获取其长度
                int length = socketClient.Receive(arrRecMsg);
                //将套接字获取到的字节数组转换为人可以看懂的字符串
                string strRecMsg = Encoding.UTF8.GetString(arrRecMsg, 0, length);
                arrRecMsg = null;
                //将发送的信息追加到聊天内容文本框中
                txtMsg.AppendText(strRecMsg + Environment.NewLine);
            }
        }

        /// <summary>
        /// 发送字符串信息到服务端的方法
        /// </summary>
        /// <param name="sendMsg">发送的字符串信息</param>
        private void ClientSendMsg(string sendMsg)
        {
            //将输入的内容字符串转换为机器可以识别的字节数组
            byte[] arrClientSendMsg = Encoding.UTF8.GetBytes(GetCurrentTime() + " " + txtUser.Text + "：" + sendMsg + Environment.NewLine);
            //调用客户端套接字发送字节数组
            socketClient.Send(arrClientSendMsg);
            //将发送的信息追加到聊天内容文本框中
            txtMsg.AppendText(GetCurrentTime() + " " + txtUser.Text + "：" + sendMsg + Environment.NewLine);
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            //调用ClientSendMsg方法 将文本框中输入的信息发送给服务端
            ClientSendMsg(txtSendMessage.Text.Trim());
            txtSendMessage.Text = "";

        }

        private void TxtCMsg_KeyDown(object sender, KeyEventArgs e)
        {
            //当光标位于文本框时 如果用户按下了键盘上的Enter键 
            if (e.KeyCode == Keys.Enter)
            {
                //则调用客户端向服务端发送信息的方法
                ClientSendMsg(txtSendMessage.Text.Trim());    
            }
            txtSendMessage.Text = "";
        }
        /// <summary>
        /// 获取当前系统时间的方法
        /// </summary>
        /// <returns>当前时间</returns>
        private DateTime GetCurrentTime()
        {
            DateTime currentTime = new DateTime();
            currentTime = DateTime.Now;
            return currentTime;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
