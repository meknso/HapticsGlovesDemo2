using UnityEngine;
using System.Net;
using System.Net.Sockets;

public class SendToSimulink : MonoBehaviour
{
    UdpClient client;
    IPEndPoint endPoint;

    AudioSource audioSource;

    byte lastUnityObj = 0;        // valor enviado anterior (MATLAB usa saltos)
    float lastX = 0f;             // última posición en X
    public float step = 0.02f;    // sensibilidad: cada 0.02 cuenta como un cambio
    public int maxLevel = 255;    // límite superior (por si te mueves mucho)
    public int minLevel = 0;      // límite inferior

    byte level = 127;             // nivel inicial en el medio (para poder subir o bajar)

    void Start()
    {
        client = new UdpClient();
        endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 25000);
        audioSource = GetComponent<AudioSource>();

        lastX = transform.position.x;
    }

    void Update()
    {
        float x = transform.position.x;

        float diff = x - lastX;

        // Si el movimiento es mayor o igual a 0.02 → cuenta como un cambio
        if (Mathf.Abs(diff) >= step)
        {
            if (diff > 0)
                level++;
            else
                level--;

            // Mantener dentro del rango
            level = (byte)Mathf.Clamp(level, minLevel, maxLevel);

            lastX = x;
        }

        byte unity_obj = level;

        // Reproducir audio al cambio (opcional)
        if (unity_obj != lastUnityObj)
        {
            audioSource.Play();
            lastUnityObj = unity_obj;
        }

        // Enviar UDP
        client.Send(new byte[] { unity_obj }, 1, endPoint);
    }

    void OnApplicationQuit()
    {
        client.Close();
    }
}
