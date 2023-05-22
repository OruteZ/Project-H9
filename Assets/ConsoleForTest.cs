// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using System.Net;
// using TMPro;
// using Unity.VisualScripting;
// using UnityEngine;
//
// /// <summary>
// /// 각종 기능 테스트를 위한 콘솔 대용 오브젝트입니다.
// /// </summary>
// public class ConsoleForTest : MonoBehaviour
// {
//     private IEnumerator _testK;
//     void Awake()
//     {
//         _testK = EnumeratorTest();
//     }
//     
//     [Header("Get Range Inspector")]
//     public Vector3Int start1;
//     public int range;
//     public Map world;
//     [ContextMenu("Get Range")]
//     void GetRange()
//     {
//         var tiles = world.GetWalkableTiles(start1, range);
//         world.HighLightOn(tiles);
//     }
//
//     [Header("Find Path Tester")]
//     public Vector3Int startPoint;
//     public Vector3Int destPoint;
//
//     [ContextMenu("Find Path")]
//     void FindPath()
//     {
//         world.GetTile(startPoint).Highlight = true;
//         world.GetTile(destPoint).Highlight = true;
//         
//         var path = world.FindPath(startPoint, destPoint);
//         Debug.Log(path);
//         world.HighLightOn(path);
//     }
//
//     [Header("Line Draw Tester")] 
//     public Vector3Int lineStart;
//     public Vector3Int lineEnd;
//
//     void DrawLine()
//     {
//         var line = Hex.LineDraw(lineStart, lineEnd);
//         var path = line.Select(i => world.GetTile(i)).ToList();
//         world.HighLightOn(path);
//     }
//
//     void DrawLine_()
//     {
//         var line = Hex.LineDraw_(lineStart, lineEnd);
//         var path = line.Select(i => world.GetTile(i)).ToList();
//         world.HighLightOn(path);
//     }
//
//     [Header("RayCast Tester")]
//     public Vector3Int rayStart;
//     public Vector3Int rayEnd;
//
//     private void RayCastTest()
//     {
//         var result  = world.RayCast(rayStart, rayEnd);
//         var startPixel = Hex.Hex2World(rayStart);
//         var endPixel = Hex.Hex2World(rayEnd);
//
//         startPixel.y = endPixel.y = 5;
//         
//         Debug.DrawLine(startPixel, endPixel,
//             color : result ? Color.blue : Color.red, duration: 5f);
//     }
//
//     void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.A))
//         {
//             Debug.Log(Input.mousePosition);
//             
//             RaycastHit hit; 
//             var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//             if (Physics.Raycast(ray, out hit))
//             {
//                 startPoint = hit.transform.GetComponent<HexTransform>().position;
//             }
//         }
//         RaycastHit hit2; 
//         var ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
//         if (Physics.Raycast(ray2, out hit2))
//         {
//             var path = world.FindPath(startPoint, 
//                 hit2.transform.GetComponent<HexTransform>().position);
//             world.HighLightOn(path);
//         }
//
//         if (Input.GetKeyDown(KeyCode.Z))
//         {
//             RaycastHit hit; 
//             var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//             if (Physics.Raycast(ray, out hit))
//             {
//                 lineStart = hit.transform.GetComponent<HexTransform>().position;
//             }
//         }
//
//         if (Input.GetKeyDown(KeyCode.X))
//         {
//             RaycastHit hit; 
//             var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//             if (Physics.Raycast(ray, out hit))
//             {
//                 lineEnd = hit.transform.GetComponent<HexTransform>().position;
//                 DrawLine();
//             }
//         }
//         
//         if (Input.GetKeyDown(KeyCode.C))
//         {
//             RaycastHit hit; 
//             var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//             if (Physics.Raycast(ray, out hit))
//             {
//                 lineEnd = hit.transform.GetComponent<HexTransform>().position;
//                 DrawLine_();
//             }
//         }
//
//         if (Input.GetKeyDown(KeyCode.Q))
//         {
//             RaycastHit hit; 
//             var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//             if (Physics.Raycast(ray, out hit))
//             {
//                 rayStart = hit.transform.GetComponent<HexTransform>().position;
//             }
//         }
//
//         if (Input.GetKeyDown(KeyCode.W))
//         {
//             RaycastHit hit; 
//             var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//             if (Physics.Raycast(ray, out hit))
//             {
//                 rayEnd = hit.transform.GetComponent<HexTransform>().position;
//                 RayCastTest();
//             }
//         }
//
//         if (Input.GetKeyDown(KeyCode.K))
//         {
//             Debug.Log(_testK.Current);
//             _testK.MoveNext();
//         }
//     }
//
//     private IEnumerator EnumeratorTest()
//     {
//         yield return 100;
//
//         for (int i = 0; i < 100; i++)
//         {
//             yield return i;
//         }
//     }
// }
