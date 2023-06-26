using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
public class Global : MonoBehaviour
{

    [SerializeField]
    int sizeGrid = 8;

    [SerializeField]
    Vector3 sizeTill;

    [SerializeField]
    string ip = "10.0.46.114";

    [SerializeField]
    int port = 8000;

    [SerializeField]
    GameObject world;

    [SerializeField]
    GameObject house;

    [SerializeField]
    GameObject office;

    [SerializeField]
    GameObject emptySpace;

    [SerializeField]
    GameObject people;

    
    private List<GameObject> houses = new List<GameObject>();
    private List<GameObject> offices = new List<GameObject>();
    private List<GameObject> emptySpaces = new List<GameObject>();
    private Dictionary<int, GameObject> peopleList = new Dictionary<int, GameObject>();


    private TcpClient socketConnection;
    private Thread clientReceiveThread;

    private WorldJSONInfo infoWorld = null;

    // Start is called before the first frame update
    void Start()
    {
        world.transform.localScale = new Vector3(sizeTill.x * sizeGrid, 1f, sizeTill.z * sizeGrid);
        int id = 0;
        for (int i = 0; i < sizeGrid; i++)
        {
            for (int j = 0; j < sizeGrid; j++)
            {
                Vector3 pos = new Vector3(
                  sizeTill.x * i,
                  0f,
                  sizeTill.z * j
                  );
                GameObject instantiatedES = Instantiate(emptySpace);
                instantiatedES.transform.position = pos;
                instantiatedES.SetActive(false);
                emptySpaces.Add(instantiatedES);


                GameObject instantiatedH = Instantiate(house);
                instantiatedH.transform.position = pos;
                instantiatedH.SetActive(false);
                houses.Add(instantiatedH);


                GameObject childES = instantiatedES.transform.GetChild(0).gameObject;
                ToHouse h = childES.GetComponent<ToHouse>();
                h.id = id;
                h.global = this;


                GameObject instantiatedO = Instantiate(office);
                instantiatedO.transform.position = pos;
                instantiatedO.SetActive(false);

                offices.Add(instantiatedO);

                GameObject childH = instantiatedH.transform.GetChild(0).gameObject;

                ToOffice o = childH.GetComponent<ToOffice>();
                o.id = id;
                o.global = this;


                GameObject childO = instantiatedO.transform.GetChild(0).gameObject;

                ToEmptySpace es = childO.GetComponent<ToEmptySpace>();
                es.id = id;
                es.global = this;

                id++;



            }
        }
     

        EstablishConnection();



    }

    public void EstablishConnection()
    {
        ConnectToTcpServer();
        if (infoWorld != null && infoWorld.building != null)
        {
            for (int i = 0; i < infoWorld.building.Count; i++)
            {

                int v = infoWorld.building[i];
                emptySpaces[i].SetActive(false);
                houses[i].SetActive(false);
                offices[i].SetActive(false);
                if (v == 0)
                {
                    emptySpaces[i].SetActive(true);
                }
                else if (v == 1)
                {
                    houses[i].SetActive(true);
                }
                else
                {
                    offices[i].SetActive(true);
                }
            }
        }

    }

    public void setConnection(string ip_, string port_)
    {
        ip = ip_;
        port = int.Parse(port_);
    }

    void Update()
    {
        if (infoWorld != null)
        {


            for (int i = 0; i < infoWorld.building.Count; i++)
            {

                int v = infoWorld.building[i];
                emptySpaces[i].SetActive(false);
                houses[i].SetActive(false);
                offices[i].SetActive(false);
                if (v == 0)
                {
                    emptySpaces[i].SetActive(true);
                }
                else if (v == 1)
                {
                    houses[i].SetActive(true);
                }
                else
                {
                    offices[i].SetActive(true);
                }
            }


            foreach (GameObject obj in peopleList.Values)
            {
                obj.SetActive(false);

            }
            List<GameObject> ps = new List<GameObject>();
            foreach (PeopleInfo pi in infoWorld.people)
            {
                int id = pi.d[0];
                GameObject obj = null;
                if (!peopleList.ContainsKey(id))
                {
                    obj = Instantiate(people);
                    peopleList.Add(id, obj);

                }
                else
                {
                    obj = peopleList[id];
                }
                ps.Add(obj);
                obj.SetActive(true);
                obj.transform.GetChild(0).gameObject.SetActive(true);
                obj.transform.GetChild(1).gameObject.SetActive(true);
                obj.transform.GetChild(2).gameObject.SetActive(true);
                obj.transform.position = new Vector3(pi.d[1], 0, pi.d[2]);
                int rot = 90 - pi.d[3];
                obj.transform.rotation = Quaternion.AngleAxis(rot, Vector3.up);



            }
            List<int> ids = new List<int>(peopleList.Keys);
            foreach (int id in ids)
            {
                GameObject obj = peopleList[id];
                if (!obj.activeSelf)
                {
                    obj.transform.position = new Vector3(0, -100, 0);
                    peopleList.Remove(id);
                    GameObject.Destroy(obj);
                }

            }
        }

    }

    public void sendModificationMessage(int id, int type)
    {
        ModificationJSONInfo info = new ModificationJSONInfo();
        info.id = id;
        info.type = type;
        SendMessageToServer(ModificationJSONInfo.ToJSON(info) + "\n");

    }
    private void SendMessageToServer(string clientMessage)
    {
        if (socketConnection == null)
        {
            return;
        }
        try
        {
            // Get a stream object for writing. 			
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {
                // Convert string message to byte array.                 
                byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);

                // Write byte array to socketConnection stream.                 
                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
                Debug.Log("Client sent his message - should be received by server");
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    private void ListenForData()
    {
        try
        {
            socketConnection = new TcpClient(ip, port);

            SendMessageToServer("connected\n");
            Byte[] bytes = new Byte[1024];
            string fullMessage = "";
            while (true)
            {
                using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;
                    // Read incomming stream into byte arrary. 					

                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {

                        var incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);
                        // Convert byte array to string message. 						
                        string serverMessage = Encoding.ASCII.GetString(incommingData);
                        fullMessage += serverMessage;

                        if (fullMessage.Contains("\n"))
                        {
                            string[] messages = fullMessage.Split("\n");
                            infoWorld = WorldJSONInfo.CreateFromJSON(messages[0]);
                            fullMessage = messages.Length > 1 ? messages[1] : "";
                        }
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }


    private void ConnectToTcpServer()
    {
        try
        {
            clientReceiveThread = new Thread(new ThreadStart(ListenForData));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);
        }
    }
}
