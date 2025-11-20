using UnityEngine;
using System.Net;
using System.Net.Sockets;

public class SendToSimulink : MonoBehaviour
{
    UdpClient client;
    IPEndPoint endPoint;

    AudioSource audioSource;   // referencia al audio

    byte lastUnityObj = 0;     // almacena el último valor enviado

    void Start()
    {
        client = new UdpClient();
        endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 25000);

        audioSource = GetComponent<AudioSource>(); // obtiene audio del objeto
    }

    void Update()
    {
        float xPos = transform.position.x;
        byte unity_obj;

        if (xPos < -1f)
            unity_obj = 1;
        else if (xPos < 1f)
            unity_obj = 2;
        else
            unity_obj = 3;

        // Solo reproducir audio si cambia entre zonas
        if (unity_obj != lastUnityObj)
        {
            audioSource.Play();     // 🔊 Reproduce el .wav
            lastUnityObj = unity_obj;
        }

        // Envía UDP a MATLAB
        client.Send(new byte[] { unity_obj }, 1, endPoint);
    }

    void OnApplicationQuit()
    {
        client.Close();
    }
}
