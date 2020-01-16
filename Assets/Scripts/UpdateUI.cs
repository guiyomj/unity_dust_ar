using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Threading;

public class UpdateUI : MonoBehaviour
{
    public Text valText1;
    public Text valText2;
    public Text valText3;
    public Text valText4;
    public Text valText5;
    public GameObject Object;
    public GameObject Arrow1, Arrow2, Arrow3, Arrow4;
    public GameObject sender1, sender2, sender3, sender4, sender5, sender6;  //수신기 선언
    public static float magneticHeading;
    public static float trueHeading;
    private int[] n;
    private float[] result;
    private int[,] result2;
    private int exitCountValue;
    AndroidJavaObject UnityActivity, UnityInstance;

    void Awake()
    {
        StartCoroutine("DBConnect");
        StartCoroutine("DBUpdate");
    }

    void Start()
    {
        AndroidJavaClass ajc = new AndroidJavaClass("com.inglobetechnologies.ARMediaSDKUnityExample");
        UnityActivity = ajc.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass ajc2 = new AndroidJavaClass("com.example.plugin");
        UnityInstance = ajc2.CallStatic<AndroidJavaClass>("instance");
        UnityInstance.Call("setContext", UnityActivity);
    }
    
    void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                exitCountValue++;
                Thread.Sleep(300);
                Debug.Log("count="+exitCountValue);

                if (exitCountValue == 1)
                {
                    if (!IsInvoking("disable_DubleClick"))//invoke 함수가 실행되고있는지 파악해주는 함수
                        Invoke("disable_DubleClick", 1f); //실행시키고자 하는 함수 딜레이시간
                    ShowToast("'뒤로'버튼을 한번 더 누르면 종료됩니다.",false);
                }

                if (exitCountValue == 2) {
                    Application.Quit();
                }
            }
        }
    }

    IEnumerator DBUpdate()
    {
        valText1 = GameObject.Find("Value0").GetComponent<Text>();
        valText2 = GameObject.Find("Value1").GetComponent<Text>();
        valText3 = GameObject.Find("Value2").GetComponent<Text>();
        valText4 = GameObject.Find("Value3").GetComponent<Text>();
        valText5 = GameObject.Find("Value4").GetComponent<Text>();
        Object = GameObject.Find("Object");
        Arrow1 = GameObject.Find("Arrow1");
        Arrow2 = GameObject.Find("Arrow2");
        Arrow3 = GameObject.Find("Arrow3");
        Arrow4 = GameObject.Find("Arrow4");
        n = new int[5];
        result = new float[5];
        Vector3 pos;
        pos = Object.transform.localPosition;
        result2 = new int[6, 6];

        sender1 = GameObject.Find("ARMediaLocation2");  // 1번째 수신기 예시 (해당 수신기의 마커 이미지는 화면에서 invisible 처리)
        sender2 = GameObject.Find("ARMediaLocation3");  // 2번째 수신기 예시
        sender3 = GameObject.Find("ARMediaLocation7");  // 3번째 수신기 예시
        sender4 = GameObject.Find("ARMediaLocation8");  // 4번째 수신기 예시
        sender5 = GameObject.Find("ARMediaLocation9");  // 5번째 수신기 예시
        sender6 = GameObject.Find("ARMediaLocation10");  // 6번째 수신기 예시


        while (true)
        {
            //헤딩 값 가져오기
            if (Input.compass.headingAccuracy == 0 || Input.compass.headingAccuracy > 0)
            {
                Input.location.Start(); //위치 서비스 시작
                Input.compass.enabled = true; //나침반 활성화
                magneticHeading = Input.compass.magneticHeading;
                trueHeading = Input.compass.trueHeading;

                if(CheckObjectIsInCamera(sender1)==true)
                //if (trueHeading >= 0 && trueHeading <= 60)
                {
                    StartCoroutine("DBConnect");
                    valText1.text = result2[0, 0]+"";
                    valText2.text = result2[0, 1]+"";
                    valText3.text = result2[0, 2]+"";
                    valText4.text = result2[0, 3]+"";
                    valText5.text = result2[0, 4]+"";
                    n[0] = result2[0, 0];
                    n[1] = result2[0, 1];
                    n[2] = result2[0, 2];
                    n[3] = result2[0, 3];
                    n[4] = result2[0, 4];
                }
                else if (CheckObjectIsInCamera(sender2) == true)
                //else if (trueHeading > 60 && trueHeading <= 120)
                {
                    StartCoroutine("DBConnect");
                    valText1.text = result2[1, 0] + "";
                    valText2.text = result2[1, 1] + "";
                    valText3.text = result2[1, 2] + "";
                    valText4.text = result2[1, 3] + "";
                    valText5.text = result2[1, 4] + "";
                    n[0] = result2[1, 0];
                    n[1] = result2[1, 1];
                    n[2] = result2[1, 2];
                    n[3] = result2[1, 3];
                    n[4] = result2[1, 4];
                }
                else if (CheckObjectIsInCamera(sender3) == true)
                //else if (trueHeading > 120 && trueHeading <= 180)
                {
                    StartCoroutine("DBConnect");
                    valText1.text = result2[2, 0] + "";
                    valText2.text = result2[2, 1] + "";
                    valText3.text = result2[2, 2] + "";
                    valText4.text = result2[2, 3] + "";
                    valText5.text = result2[2, 4] + "";
                    n[0] = result2[2, 0];
                    n[1] = result2[2, 1];
                    n[2] = result2[2, 2];
                    n[3] = result2[2, 3];
                    n[4] = result2[2, 4];
                }
                else if (CheckObjectIsInCamera(sender4) == true)
                //else if (trueHeading > 180 && trueHeading <= 240)
                {
                    StartCoroutine("DBConnect");
                    valText1.text = result2[3, 0] + "";
                    valText2.text = result2[3, 1] + "";
                    valText3.text = result2[3, 2] + "";
                    valText4.text = result2[3, 3] + "";
                    valText5.text = result2[3, 4] + "";
                    n[0] = result2[3, 0];
                    n[1] = result2[3, 1];
                    n[2] = result2[3, 2];
                    n[3] = result2[3, 3];
                    n[4] = result2[3, 4];
                }
                else if (CheckObjectIsInCamera(sender5) == true)
                //else if (trueHeading > 240 && trueHeading <= 300)
                {
                    StartCoroutine("DBConnect");
                    valText1.text = result2[4, 0] + "";
                    valText2.text = result2[4, 1] + "";
                    valText3.text = result2[4, 2] + "";
                    valText4.text = result2[4, 3] + "";
                    valText5.text = result2[4, 4] + "";
                    n[0] = result2[4, 0];
                    n[1] = result2[4, 1];
                    n[2] = result2[4, 2];
                    n[3] = result2[4, 3];
                    n[4] = result2[4, 4];
                }
                else if (CheckObjectIsInCamera(sender6) == true)
                //else
                {
                    StartCoroutine("DBConnect");
                    valText1.text = result2[5, 0] + "";
                    valText2.text = result2[5, 1] + "";
                    valText3.text = result2[5, 2] + "";
                    valText4.text = result2[5, 3] + "";
                    valText5.text = result2[5, 4] + "";
                    n[0] = result2[5, 0];
                    n[1] = result2[5, 1];
                    n[2] = result2[5, 2];
                    n[3] = result2[5, 3];
                    n[4] = result2[5, 4];
                }
                
                result[1] = 90f - (n[1] * (9f / 5f));
                result[2] = 90f - (n[2] * (9f / 5f));
                result[3] = 90f - (n[3] * (9f / 5f));
                result[4] = 90f - (n[4] * (9f / 5f));
                Arrow1.transform.localRotation = Quaternion.Euler(0f, 0f, result[1]);
                Arrow2.transform.localRotation = Quaternion.Euler(0f, 0f, result[2]);
                Arrow3.transform.localRotation = Quaternion.Euler(0f, 0f, result[3]);
                Arrow4.transform.localRotation = Quaternion.Euler(0f, 0f, result[4]);
                pos.x = (n[0] / 100f * 224f) - 657f;
                Object.transform.localPosition = pos;

            }

            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator DBConnect()
    {
        print("connect");
        string url = "http://13.209.72.30/glgl.php";
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string db = www.downloadHandler.text.Replace("<body>", "").Replace("</body>", "").Replace("<html>", "").Replace("</html>", "").Replace(" ", "").Replace("\n", "");
                int n1 = db.IndexOf("[[") + 2, n2 = db.IndexOf("]]");
                Debug.Log(db);
                db = db.Substring(n1, n2 - n1);
                string[] res = db.Split(new string[] { "],[" }, System.StringSplitOptions.RemoveEmptyEntries),tmp;
                for (var i = 0; i < res.Length; i++)
                {
                    tmp = res[i].Replace("]", "").Replace("[", "").Split(new char[] { ',' });
                    for (var j = 0; j < 5; j++)
                    {
                        result2[i, j] = int.Parse(tmp[j]);
                        Debug.Log(result2[i, j]);
                    }
                }
            }
        }
    }

    public bool CheckObjectIsInCamera(GameObject _target)
    {
        Camera selectedCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        Vector3 screenPoint = selectedCamera.WorldToViewportPoint(_target.transform.position);
        bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        return onScreen;
    }

    void disable_DubleClick()
    {
        exitCountValue = 0;
        Debug.Log("count=" + exitCountValue);
    }
    
    public void ShowToast(string msg, bool isLong)
    {
        UnityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
         {
             UnityInstance.Call("ShowToast", msg, 0);
             {
                 if (isLong == false)
                 {
                     UnityInstance.Call("ShowToast", msg, 0);
                 }
                 else
                 {
                     UnityInstance.Call("ShowToast", msg, 1);
                 }
             }
         }));
    } 
}
