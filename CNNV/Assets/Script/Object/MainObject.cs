//extern alias CoreCompat;

using UnityEngine;
using System;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Data.Matlab;

public class MainObject : MonoBehaviour
{
    private GameObject[,] objs;

    void CreateNeural(int[] data)
    {
        int layer_nums = data.GetLength(0);
        int maxx = -1;
        for (int i = 0; i < layer_nums; i++)
        {
            maxx = Math.Max(maxx, data[i]);
        }
        objs = new GameObject[layer_nums, maxx];

        Matrix<float> m;

        for (int i = 0; i < layer_nums; i++)
        {
            int matrix_nums = data[i];
            for (int j = 0; j < matrix_nums; j++)
            {
                m = MatlabReader.Read<float>(string.Format("Assets/Resources/Textures/MNIST/matrix{0:D3}_{0:D3}.mat", i, j));
                CreateCube(i, j, matrix_nums, m);
            }
        }
    }

    void CreateCube(int layer, int matrix, int totMatrix, Matrix<float> data)
    {
        int sz1 = data.RowCount;
        int sz2 = data.ColumnCount;

        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Plane);
        obj.transform.localScale = new Vector3(sz2 / 10.0f, 1.0f, sz1 / 10.0f);
        Vector3 rotate = new Vector3(-90.0f, 0.0f, 0.0f);
        obj.transform.localRotation = Quaternion.Euler(rotate);

        obj.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/cubeRender");
        obj.GetComponent<MeshRenderer>().material.shader = Shader.Find("Custom/cubeRender");
        float off = 1.2f * sz1;
        obj.transform.position = new Vector3(matrix * off + off / 2.0f - off / 2.0f * totMatrix, 0f, (layer + 1) * 50f);

        objs[layer, matrix] = obj;
    }

    void UpdateNeural(int[] data)
    {
        int layer_nums = data.GetLength(0);
        for (int i = 0; i < layer_nums; i++)
        {
            int matrix_nums = data[i];
            for (int j = 0; j < matrix_nums; j++)
            {
                String path = string.Format("Assets/Resources/Textures/matrix{0:D3}_", i);
                path += string.Format("{0:D3}.mat", j);
                if(System.IO.File.Exists(path))
                {
                    Matrix<float> m = MatlabReader.Read<float>(path);
                    UpdateCube(i, j, matrix_nums, m);
                }
            }
        }
    }

    void UpdateCube(int layer, int matrix, int totMatrix, Matrix<float> data)
    {

        int sz1 = data.RowCount;
        int sz2 = data.ColumnCount;

        Texture2D texture = new Texture2D(sz2, sz1);
        //Texture2D texture = (Texture2D)objs[layer, matrix].GetComponent<MeshRenderer>().material.GetTexture("_MainTex");

        for (int i = 0; i < sz2; i++)
        {
            for (int j = 0; j < sz1; j++)
            {
                texture.SetPixel(sz2 - 1 - i, sz1 - 1 - j, new Color(data[sz1 - 1 - j, i], data[sz1 - 1 - j, i], data[sz1 - 1 - j, i], 0.9f));
            }
        }
        texture.Apply();
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        objs[layer, matrix].GetComponent<MeshRenderer>().material.SetTexture("_MainTex", texture);
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateNeural(new int[] { 6, 16 });
    }

    // Update is called once per frame
    void Update()
    {
        UpdateNeural(new int[] { 6, 16 });
    }

}