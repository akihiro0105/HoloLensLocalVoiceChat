using System;
using System.Collections;
using System.Collections.Generic;
using HoloLensModule.Environment;
using HoloLensModule.Network;
using SaveWAVFile;
using UnityEngine;

namespace LocalVoiceChat
{
    /// <summary>
    /// ローカルネットワーク内でUDPブロードキャストによるボイスチャットを行う
    /// </summary>
    public class LocalVoiceChatManager : MonoBehaviour
    {
        /// <summary>
        /// 受信音声出力用
        /// </summary>
        [SerializeField] private AudioSource audioSource;

        /// <summary>
        /// Voice周波数
        /// </summary>
        [SerializeField] private int frequency = 16000;

        /// <summary>
        /// UDP通信用ポート
        /// </summary>
        [SerializeField] private int port = 8001;

        /// <summary>
        /// UDPネットワーク処理
        /// </summary>
        private UDPListenerManager listener;
        private UDPSenderManager sender;

        private WAVStreamSender voiceSender;

        // Use this for initialization
        void Start()
        {
            // Voice受信処理
            var voiceListener = new WAVStreamListener(audioSource, frequency);
            listener = new UDPListenerManager(port);
            listener.ListenerByteEvent += (data, address) =>
            {
                if (SystemInfomation.IPAddress.Equals(address)) return;
                voiceListener.SetDataList(data);
            };
           
            // Voice送信処理
            sender = new UDPSenderManager(SystemInfomation.DirectedBroadcastAddress, port);
            voiceSender = new WAVStreamSender();
            StartCoroutine(voiceSender.StartStreamRecordData(data => sender.SendMessage(data), frequency));
        }

        void OnDestroy()
        {
            voiceSender.StopStreamRecordData();
            listener.DisConnectListener();
            sender.DisConnectSender();
        }
    }
}
