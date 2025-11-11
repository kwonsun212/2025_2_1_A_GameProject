using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CityGenerator : EditorWindow
{
    private int gridSizeX = 50;             //도시 가로 크기
    private int gridSizeZ = 50;             //고시 세로 크기
    private float buildingSpacing = 15;     //도로 사이 간격
    private float roadWidth = 5f;           //도로 폭
    private bool makeStatic = true;         //생성되는 오브젝트를 Static으로 만들지 여부

    [MenuItem("Tools/City Generator")]      //Unity 상단 메뉴에 버튼 추가

    public static void ShoWindow()
    {
        GetWindow<CityGenerator>("City Generator");     //에디터 창 열기
    }

    private void OnGUI()
    {
        GUILayout.Label("Simple City Generator", EditorStyles.boldLabel);

        gridSizeX = EditorGUILayout.IntField("Grid Size X", gridSizeX);     //X크기 입력
        gridSizeX = EditorGUILayout.IntField("Grid Size Z", gridSizeZ);     //Z크기 입력
        buildingSpacing = EditorGUILayout.FloatField("Building Spacing", buildingSpacing);
        roadWidth = EditorGUILayout.FloatField("Road Width", roadWidth);    //도로 폭 입력
        makeStatic = EditorGUILayout.Toggle("Make Static", makeStatic);     //Static 설정

        GUILayout.Space(10);

        if (GUILayout.Button("Generate City"))   //도시 생성 버튼
        {
            GenerateCity();
        }

        if (GUILayout.Button("Clear City"))   //도시 삭제 버튼
        {
            ClearCity();
        }
    }

    private void CreateBuilding(Vector3 position, Transform parent)
    {
        GameObject building = GameObject.CreatePrimitive(PrimitiveType.Cube);
        building.name = "Building";         //오브젝트 이름 설정

        float height = Random.Range(5.0f, 20.0f);       //랜덤 건물 높이
        building.transform.position = position + Vector3.up * height / 2.0f;
        building.transform.localScale = new Vector3(buildingSpacing - roadWidth - 1f, height, buildingSpacing - roadWidth - 1f);
        building.transform.SetParent(parent);           //Buildings 그룹 아래로 넣기

        Renderer renderer = building.GetComponent<Renderer>();      //색 변경을 위한 Renderer
        renderer.material.color = new Color(Random.Range(0.5f, 0.8f), Random.Range(0.5f, 0.8f), Random.Range(0.5f, 0.8f));

        if (makeStatic)
        {
            building.isStatic = true;
        }
    }

    private void CreateRoad(Vector3 position, Transform parent)
    {
        GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);

        road.transform.position = position + Vector3.up * 0.1f;     //살짝 바닥 위에 두기
        road.transform.localScale = new Vector3(buildingSpacing, 0.2f, buildingSpacing);
        road.transform.SetParent(parent);       //Roads 그룹 아래로 넣기

        Renderer renderer = road.GetComponent<Renderer>();      //색 변경을 위한 Renderer
        renderer.material.color = new Color(0.3f, 0.3f, 0.3f);  //회색 색상

        if (makeStatic)              //Static 설장
        {
            road.isStatic = true;   //도로 Static 처리
        }

    }

    private void ClearCity()            //도시 삭제 함수
    {
        GameObject city = GameObject.Find("City");          //City 오브젝트 찾기
        if(city != null)
        {
            DestroyImmediate(city);             //에디터에서 즉시 삭제
            Debug.Log("City cleard");
        }
        else
        {
            Debug.Log("도시가 없습니다");
        }

    }
    private void GenerateCity()             //도시 생성 함수
    {
        GameObject cityParent = new GameObject("City");         //전테 도시를 담을 부모 오브젝트

        GameObject buildingParent = new GameObject("Buildings");    //건물 묶은 부모
        buildingParent.transform.SetParent(cityParent.transform, false);

        GameObject roadsParent = new GameObject("Roads");           //도로 묶은 부모
        roadsParent.transform.SetParent(cityParent.transform, false);

        for (int x = 0; x < gridSizeX; x++)     //X 방향 반복
        {
            for (int z = 0; z < gridSizeZ; z++)     //Z 방향 반복    
            {
                Vector3 position = new Vector3(x * buildingSpacing, 0, z * buildingSpacing);    //각 위치 계산

                if(x % 2 == 0 || z % 2 == 0)        //짝수 중에는 도로 생성
                {
                    CreateRoad(position, roadsParent.transform);
                }
                else
                {
                    CreateBuilding(position, buildingParent.transform);
                }
            }
        }

    }

}
