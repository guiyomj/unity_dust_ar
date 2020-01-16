using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateUI2 : MonoBehaviour
{
    public Text valText1;
    public Text valText2;
    public Text valText3;
    public Text valText4;
    public Text valText5;
    public GameObject Object;
    public GameObject Arrow1;
    public GameObject Arrow2;
    public GameObject Arrow3;
    public GameObject Arrow4;
    public static float magneticHeading;
    public static float trueHeading;
    private int[] n;
    private float[] result;


    LocationInfo myGPSLocation;
    private static double MyLatitude, MyLongtitude;  // 내 위치
    private static double[] TargetLatitude, TargetLongtitude;  // 수신기 위치
    private double DistanceToMeter;
    DistUnit unit;


    IEnumerator Start()
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
        TargetLatitude = new double[6] { 37.481227, 37.481431, 37.481431, 37.481431, 37.481431, 37.481431 };  // 수신기 위도 1~6
        TargetLongtitude = new double[6] { 126.879913, 126.880204 , 126.880204 , 126.880204 , 126.880204 , 126.880204 };  // 수신기 경도 1~6

        Vector3 pos;
        pos = Object.transform.localPosition;

        while (true)
        {
            //헤딩 값 가져오기
            if (Input.compass.headingAccuracy == 0 || Input.compass.headingAccuracy > 0)
            {
                List<Dictionary<string, object>> data = CSVReader.Read("env");
                Input.location.Start(); //위치 서비스 시작
                Input.compass.enabled = true; //나침반 활성화
                magneticHeading = Input.compass.magneticHeading;
                trueHeading = Input.compass.trueHeading;

                myGPSLocation = Input.location.lastData;
                MyLongtitude = Math.Round(myGPSLocation.longitude, 6);
                MyLatitude = Math.Round(myGPSLocation.latitude, 6);
                //두 점간의 거리
                for (var i = 0; i < 5; i++)
                {
                    DistanceToMeter = Math.Round(distance(MyLatitude, MyLongtitude, TargetLatitude[i], TargetLongtitude[i], DistUnit.kilometer), 6);  // 해당 각도 범위에 위치한 수신기와의 거리
                }
                Debug.Log("수신기와의 거리: "+ DistanceToMeter+"(km)\t 각도: "+trueHeading+"\t 내 위치: "+ MyLatitude+",  "+ MyLongtitude);

                if (trueHeading > 0 && trueHeading <= 60 && DistanceToMeter < 2)  // 수신기 1의 범위각도
                {
                    valText1.text = data[0]["Dust"] + "";
                    valText2.text = data[0]["No2"] + "";
                    valText3.text = data[0]["O3"] + "";
                    valText4.text = data[0]["CO"] + "";
                    valText5.text = data[0]["SO2"] + "";
                    n[0] = (int)data[0]["No2"];
                    n[1] = (int)data[0]["O3"];
                    n[2] = (int)data[0]["CO"];
                    n[3] = (int)data[0]["SO2"];
                    n[4] = (int)data[0]["Dust"];
                }
                else if (trueHeading > 60 && trueHeading <= 120 && DistanceToMeter < 2)
                {
                    valText1.text = data[1]["Dust"] + "";
                    valText2.text = data[1]["No2"] + "";
                    valText3.text = data[1]["O3"] + "";
                    valText4.text = data[1]["CO"] + "";
                    valText5.text = data[1]["SO2"] + "";
                    n[0] = (int)data[1]["No2"];
                    n[1] = (int)data[1]["O3"];
                    n[2] = (int)data[1]["CO"];
                    n[3] = (int)data[1]["SO2"];
                    n[4] = (int)data[1]["Dust"];
                }
                else if (trueHeading > 120 && trueHeading <= 180 && DistanceToMeter < 2)
                {
                    valText1.text = data[2]["Dust"] + "";
                    valText2.text = data[2]["No2"] + "";
                    valText3.text = data[2]["O3"] + "";
                    valText4.text = data[2]["CO"] + "";
                    valText4.text = data[2]["SO2"] + "";
                    n[0] = (int)data[2]["No2"];
                    n[1] = (int)data[2]["O3"];
                    n[2] = (int)data[2]["CO"];
                    n[3] = (int)data[2]["SO2"];
                    n[4] = (int)data[2]["Dust"];
                }
                else if (trueHeading > 180 && trueHeading <= 240 && DistanceToMeter < 2)
                {
                    valText1.text = data[3]["Dust"] + "";
                    valText2.text = data[3]["No2"] + "";
                    valText3.text = data[3]["O3"] + "";
                    valText4.text = data[3]["CO"] + "";
                    valText5.text = data[3]["SO2"] + "";
                    n[0] = (int)data[3]["No2"];
                    n[1] = (int)data[3]["O3"];
                    n[2] = (int)data[3]["CO"];
                    n[3] = (int)data[3]["SO2"];
                    n[4] = (int)data[3]["Dust"];
                }
                else if (trueHeading > 240 && trueHeading <= 300 && DistanceToMeter < 2)
                {
                    valText1.text = data[4]["Dust"] + "";
                    valText2.text = data[4]["No2"] + "";
                    valText3.text = data[4]["O3"] + "";
                    valText4.text = data[4]["CO"] + "";
                    valText5.text = data[4]["SO2"] + "";
                    n[0] = (int)data[4]["No2"];
                    n[1] = (int)data[4]["O3"];
                    n[2] = (int)data[4]["CO"];
                    n[3] = (int)data[4]["SO2"];
                    n[4] = (int)data[4]["Dust"];
                }
                else if (trueHeading > 300 && trueHeading <= 360 && DistanceToMeter < 2)
                {
                    valText1.text = data[5]["Dust"] + "";
                    valText2.text = data[5]["No2"] + "";
                    valText3.text = data[5]["O3"] + "";
                    valText4.text = data[0]["CO"] + "";
                    valText5.text = data[5]["SO2"] + "";
                    n[0] = (int)data[5]["No2"];
                    n[1] = (int)data[5]["O3"];
                    n[2] = (int)data[5]["CO"];
                    n[3] = (int)data[5]["SO2"];
                    n[4] = (int)data[5]["Dust"];
                }
                
                result[0] = 90f - (n[0] * (9f / 5f));
                result[1] = 90f - (n[1] * (9f / 5f));
                result[2] = 90f - (n[2] * (9f / 5f));
                result[3] = 90f - (n[3] * (9f / 5f));
                Arrow1.transform.localRotation = Quaternion.Euler(0f, 0f, result[0]);
                Arrow2.transform.localRotation = Quaternion.Euler(0f, 0f, result[1]);
                Arrow3.transform.localRotation = Quaternion.Euler(0f, 0f, result[2]);
                Arrow4.transform.localRotation = Quaternion.Euler(0f, 0f, result[3]);
                pos.x = (n[4] / 100f * 224f) - 657f;
                Object.transform.localPosition = pos;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    //여기서부터 거리

    // This function converts decimal degrees to radians
    static double deg2rad(double deg)
    {
        return (deg * Math.PI / 180.0);
    }

    // This function converts radians to decimal degrees
    static double rad2deg(double rad)
    {
        return (rad * 180 / Math.PI);
    }

    static double distance(double lat1, double lon1, double lat2, double lon2, DistUnit unit)
    {

        double theta = lon1 - lon2;
        double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));

        dist = Math.Acos(dist);
        dist = rad2deg(dist);
        dist = dist * 60 * 1.1515;

        if (unit == DistUnit.kilometer)
        {
            dist = dist * 1.609344;
        }
        else if (unit == DistUnit.meter)
        {
            dist = dist * 1609.344;
        }

        return (dist);
    }

}

enum DistUnit
{
    kilometer,
    meter
}