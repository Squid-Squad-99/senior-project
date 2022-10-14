using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Relay.Payload;
using RelayClient.Payload;
using Ultility;
using UnityEngine;
using UnityEngine.Events;

namespace Relay
{
    public class RelayClient : Singleton<RelayClient>
    {
        public UnityEvent OnDisconnect;
        public UnityEvent<BasePayload> OnRecvPayload;
        public BasePayload RecvPayload => _recvPayload; 
        public bool Connected => _socket.Connected;

        [SerializeField] private string _hostNameOrAddress;
        [SerializeField] private int _port;
        private IPEndPoint _remoteEP;
        private Socket _socket;
        private byte[] _buffer;
        
        
        public IEnumerator ConnectServerAsync()
        {
            Task.Run(() =>
            {
                _socket.Connect(_remoteEP);
                Console.WriteLine("Socket connected to {0}",
                    _socket.RemoteEndPoint);
                var recvThread = Task.Run(() =>
                {
                    while (Connected)
                    {
                        _recvPayload = ReceivePayload();
                        _haveNewPayload = true;
                    }
                });
                recvThread.Wait();
            });

            StartCoroutine(ReceivePayloadMainThread());

            yield return new WaitUntil(() => Connected);
        }

        public IEnumerator JoinRoomAsync(int roomId)
        {
            SendPayload(new JoinPayload(roomId));
            yield return WaitUntilRecvType(BasePayload.Type.Status);
            if (BitConverter.ToInt32(_recvPayload.Body) != 200)
            {
                throw new Exception("join room fail");
            }

            Debug.Log("join room success");
        }

        public IEnumerator SendMsgAsync(byte[] body)
        {
            SendPayload(new MsgPayload(body));
            yield return null;
        }

        private void Awake()
        {
            IPHostEntry host = Dns.GetHostEntry(_hostNameOrAddress);
            IPAddress ipAddress = host.AddressList[0];
            _remoteEP = new IPEndPoint(ipAddress, _port);
            Socket socket = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            _socket = socket;
            _buffer = new byte[1048576]; // 2MB buffer
        }

        private void OnDestroy()
        {
            DisconnectServer();
        }

        public WaitUntil WaitUntilRecvType(BasePayload.Type type)
        {
            return new WaitUntil(() => _recvPayload != null && _recvPayload.PayloadType == (int)type);
        }

        private IEnumerator ReceivePayloadMainThread()
        {
            while (true)
            {
                yield return new WaitUntil(() => _haveNewPayload);
                OnRecvPayload?.Invoke(_recvPayload);
            }
        }

        private BasePayload _recvPayload = null;
        private bool _haveNewPayload = false;

        private BasePayload ReceivePayload()
        {
            try
            {
                int byteSize = 0;
                while (true)
                {
                    int bytesRec = _socket.Receive(_buffer, byteSize, _buffer.Length - byteSize, SocketFlags.None);
                    byteSize += bytesRec;
                    if (byteSize >= 4)
                    {
                        int lastInt = BitConverter.ToInt32(_buffer, byteSize - 4);
                        if (lastInt == BasePayload.END_FLAG)
                            break;
                    }
                }
            }
            catch (Exception)
            {
                DisconnectServer();
            }


            return BasePayload.Decode(_buffer);
        }

        private void SendPayload(BasePayload basePayload)
        {
            _socket.Send(BasePayload.Encode(basePayload));
        }

        private void DisconnectServer()
        {
            if(!Connected) return;
            SendPayload(new ClosePayload());
            OnDisconnect.Invoke();
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }
    }
}