//extern alias CoreCompat;

using UnityEngine;
using System;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Data.Matlab;
using System.Collections;

public class CIFAR10 : MonoBehaviour
{
    private GameObject[,,,] objs;
    // private GameObject[,,,,] nCore;
    private string[] labelName;
    private string[] layerName;
    private GameObject[] labelText;
    private Matrix<float>[,] lastData;
    private int[] dataSize;
    private System.Random ran = new System.Random();
    private GameObject[,] lines;
    private GameObject[] trainState;
    private bool isUpdate;

    GameObject GenerateText(string val, Vector3 position, Color color, int fontSize)
    {
        GameObject text = new GameObject();
        text.transform.position = position;
        text.AddComponent<TextMesh>();
        var style = text.GetComponent<TextMesh>();
        style.text = val;
        style.fontSize = fontSize;
        style.anchor = TextAnchor.MiddleCenter;
        style.color = color;
        return text;
    }

    float GetRandom(System.Random ran, float minn, float maxx)
    {
        return (float)(ran.NextDouble() * (maxx - minn) + minn);
    }

    bool WhetherSame(Matrix<float> a, Matrix<float> b)
    {
        int numRow = a.RowCount;
        int numCol = a.ColumnCount;
        for (int i = 0; i < numRow; i++)
        {
            for (int j = 0; j < numCol; j++)
            {
                if (Math.Abs(a[i, j] - b[i, j]) > 1e-8) return false;
            }
        }
        return true;
    }

    void CreateImage()
    {
        String path = "Assets/Resources/Textures/CIFAR10/cifar10_matrix_data.mat";
        if (!System.IO.File.Exists(path)) { return; }
        try
        {
            Matrix<float> m = MatlabReader.Read<float>(path, "image_R");
            CreateCubes(m, 0, 0, 1);
        }
        catch
        {

        }

    }

    void UpdateImage()
    {
        String path = "Assets/Resources/Textures/CIFAR10/cifar10_matrix_data.mat";
        if (!System.IO.File.Exists(path)) { return; }
        try
        {
            Matrix<float> r = MatlabReader.Read<float>(path, "image_R");
            isUpdate = lastData[0, 0] == null || !WhetherSame(lastData[0, 0], r);
            if (isUpdate)
            {
                Matrix<float> g = MatlabReader.Read<float>(path, "image_G");
                Matrix<float> b = MatlabReader.Read<float>(path, "image_B");
                UpdateCubes(r, g, b, 0, 0, 1);
            }
            
        }
        catch
        {

        }
    }

    GameObject CreateCube(Vector3 position, Vector3 scale)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.transform.position = position;
        obj.transform.localScale = scale;
        obj.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Material/cubeRender2");
        obj.GetComponent<MeshRenderer>().material.shader = Shader.Find("Unlit/cubeShader2");
        return obj;
    }

    void CreateCubes(Matrix<float> data, int layer, int number, int cubesNum)
    {
        int row = data.RowCount;
        int col = data.ColumnCount;
        int edge = (int)Math.Sqrt(cubesNum);
        int numRow = number / edge;
        int numCol = number % edge;
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                Vector3 position = new Vector3(2 * (row - i - 1) - row + 1 + (2 * numRow - edge + 1) * (row + 20),
                                                    2 * (col - j - 1) - col + 1 + (2 * numCol - edge + 1) * (col + 20), layer * 100);
                Vector3 scale = new Vector3(2f, 2f, 2f);
                objs[layer, number, i, j] = CreateCube(position, scale);
            }
        }
        lastData[layer, number] = data;
    }

    void UpdateCubes(Matrix<float> data, int layer, int number, int cubesNum)
    {
        //if(lastData[layer, number] != null && WhetherSame(lastData[layer, number], data)){
        //    return;
        //}
        int row = data.RowCount;
        int col = data.ColumnCount;
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                objs[layer, number, i, j].GetComponent<MeshRenderer>().material.color = new Color(data[j, col - i - 1], data[j, col - i - 1], data[j, col - i - 1], 0.5f);
            }
        }
        lastData[layer, number] = data;
    }

    void UpdateCubes(Matrix<float> R, Matrix<float> G, Matrix<float> B, int layer, int number, int cubesNum)
    {
        int row = R.RowCount;
        int col = R.ColumnCount;
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                objs[layer, number, i, j].GetComponent<MeshRenderer>().material.color = new Color(R[j, col - i - 1], G[j, col - i - 1], B[j, col - i - 1], 0.5f);
            }
        }
        lastData[layer, number] = R;
    }

    //void CreateCores(Matrix<float> data, int layer, int lastNum, int nextNum)
    //{
    //    int row = data.RowCount;
    //    int col = data.ColumnCount;
    //    Vector3 lastPosition = objs[layer, lastNum, lastData[layer, 0].RowCount / 2 - 1, lastData[layer, 0].ColumnCount / 2 - 1].transform.position;
    //    Vector3 nextPosition = objs[layer + 1, nextNum, lastData[layer + 1, 0].RowCount / 2 - 1, lastData[layer + 1, 0].ColumnCount / 2 - 1].transform.position;
    //    for (int i = 0; i < row; i++)
    //    {
    //        for (int j = 0; j < col; j++)
    //        {
    //            Vector3 position = new Vector3((lastPosition.x + nextPosition.x) / 2 + 2 * (i - row / 2.0f), (lastPosition.y + nextPosition.y) / 2 + 2 * (j - col / 2.0f), layer * 100 + 50);
    //            Vector3 scale = new Vector3(1.5f, 1.5f, 1.5f);
    //            nCore[layer, lastNum, nextNum, i, j] = CreateCube(position, scale);

    //            // nCore每个像素点要跟前后两层对应的featureMap相连

    //            //// 上一层的连线
    //            //for (int k = 0;k < lastData[layer, 0].RowCount;k ++)
    //            //{
    //            //    for (int l = 0;l < lastData[layer, 0].ColumnCount;l ++)
    //            //    {
    //            //        GameObject tmp = new GameObject();
    //            //        tmp.AddComponent<LineRenderer>();
    //            //        tmp.GetComponent<LineRenderer>().material = Resources.Load<Material>("Material/lineRender");
    //            //        tmp.GetComponent<LineRenderer>().material.shader = Shader.Find("Unlit/lineShader");
    //            //        tmp.GetComponent<LineRenderer>().SetWidth(0.05f, 0.05f);
    //            //        tmp.GetComponent<LineRenderer>().SetPosition(0, nCore[layer, lastNum, nextNum, i, j].transform.position);
    //            //        tmp.GetComponent<LineRenderer>().SetPosition(1, objs[layer, lastNum, k, l].transform.position);
    //            //    }
    //            //}


    //        }
    //    }
    //}

    //void UpdateCores(Matrix<float> data, int layer, int lastNum, int nextNum)
    //{
    //    int row = data.RowCount;
    //    int col = data.ColumnCount;
    //    for (int i = 0; i < row; i++)
    //    {
    //        for (int j = 0; j < col; j++)
    //        {
    //            nCore[layer, lastNum, nextNum, i, j].GetComponent<MeshRenderer>().material.color = new Color(data[j, col - i - 1], data[j, col - i - 1], data[j, col - i - 1]);
    //        }
    //    }
    //}

    [Obsolete]
    void CreateNeural(int[] data)
    {
        int deltaX = 200;

        GenerateText(layerName[0], new Vector3(deltaX, 0, 0), new Color(1f, 1f, 1f), 200);

        int layer_nums = data.GetLength(0);
        String path = "Assets/Resources/Textures/CIFAR10/cifar10_matrix_data.mat";

        if (!System.IO.File.Exists(path)) { return; }

        for (int i = 0; i < layer_nums; i++)
        {
            GenerateText(layerName[i + 1], new Vector3(deltaX, 0, (i + 1) * 100f), new Color(1f, 1f, 1f), 200);

            int matrix_nums = data[i];

            for (int j = 0; j < matrix_nums; j++)
            {
                // Matrix<float> m = MatlabReader.Read<float>(string.Format("Assets/Resources/Textures/MNIST/matrix{0:D3}_{0:D3}.mat", i, j));
                //CreateCube(i, j, matrix_nums, m);
                try
                {
                    Matrix<float> m = MatlabReader.Read<float>(path, string.Format("{0:D3}", i) + string.Format("_{0:D3}", j));
                    CreateCubes(m, i + 1, j, matrix_nums);
                }
                catch
                {

                }
            }
        }

        GenerateText("分类结果", new Vector3(deltaX, 0f, (dataSize.Length + 2) * 100f + 50f), new Color(1f, 1f, 1f), 200);
        for (int i = 0; i < 10; i++)
        {
            Vector3 position = new Vector3(20 * (i - 4.5f), 200f, (dataSize.Length + 2) * 100f + 50f);
            Vector3 scale = new Vector3(10f, 10f, 10f);
            objs[dataSize.Length + 2, i, 0, 0] = CreateCube(position, scale);

            var textPosition = new Vector3(20 * (i - 4.5f), 215f, (dataSize.Length + 2) * 100f + 50f);

            GenerateText(labelName[i], textPosition, new Color(0f, 1f, 0f), 120);
        }

        GenerateText("全连接层", new Vector3(deltaX, 0f, (dataSize.Length + 1) * 100f), new Color(1f, 1f, 1f), 200);
        try
        {
            Matrix<float> m = MatlabReader.Read<float>(path, string.Format("{0:D3}", dataSize.Length) + string.Format("_{0:D3}", 0));
            int edge = 32;
            for (int i = 0; i < edge; i++)
            {
                for (int j = 0; j < edge; j++)
                {
                    Vector3 position = new Vector3(GetRandom(ran, -150, 150), GetRandom(ran, -150, 150), GetRandom(ran, 480, 520));
                    Vector3 scale = new Vector3(1f, 1f, 1f);
                    objs[dataSize.Length + 1, 0, i, j] = CreateCube(position, scale);
                    for (int k = 0; k < 10; k++)
                    {
                        lines[i * 20 + j, k] = new GameObject();
                        lines[i * 20 + j, k].AddComponent<LineRenderer>();
                        lines[i * 20 + j, k].GetComponent<LineRenderer>().material = Resources.Load<Material>("Material/lineRender");
                        lines[i * 20 + j, k].GetComponent<LineRenderer>().material.shader = Shader.Find("Unlit/lineShader");
                        lines[i * 20 + j, k].GetComponent<LineRenderer>().SetWidth(0.05f, 0.05f);
                        lines[i * 20 + j, k].GetComponent<LineRenderer>().SetPosition(0, objs[dataSize.Length + 1, 0, i, j].transform.position);
                        lines[i * 20 + j, k].GetComponent<LineRenderer>().SetPosition(1, objs[dataSize.Length + 2, k, 0, 0].transform.position);
                        // lines[i * 20 + j, k].GetComponent<LineRenderer>().material.color = new Color(2.5f, 0.5f, 0.5f);
                        //lines[i * 20 + j, k].GetComponent<LineRenderer>().material = new Material(Shader.Find("Particles/Additive"));

                        // lines[i * 20 + j, k].GetComponent<LineRenderer>().SetCoors(new Color(0f, 0f, 0f), new Color(0f, 0f, 0f));
                        //lines[i * 20 + j, k].GetComponent<LineRenderer>().material.color = new Color(1f, 1f, 1f);
                    }

                }
            }
        }
        catch
        {

        }

        //// 生成featureMap之间的连线
        //try
        //{
        //    ArrayList newSize = new ArrayList();
        //    newSize.Add(1);
        //    for (int i = 0; i < dataSize.Length; i++)
        //    {
        //        newSize.Add(dataSize[i]);
        //    }
        //    // Debug.Log(newSize.Count);
        //    for (int i = 0; i + 1 < newSize.Count; i++) // 层
        //    {
        //        for (int j = 0; j < int.Parse(newSize[i].ToString()); j++) // 前一层
        //        {
        //            for (int k = 0; k < int.Parse(newSize[i + 1].ToString()); k++) // 后一层
        //            {
        //                //for (int l = 0;l <  lastData[i+1, 0].RowCount;l ++) // 后一层的row
        //                //{
        //                //    for (int m = 0;m < lastData[i+1, 0].ColumnCount;m ++) // 后一层的col
        //                //    {
        //                // 后一层的中心点 
        //                int l = lastData[i + 1, 0].RowCount / 2;
        //                int m = lastData[i + 1, 0].ColumnCount / 2;
        //                // int nCoreEdgeSize = 5;
        //                for (int n = 0; n < lastData[i, 0].RowCount; n++)
        //                {
        //                    for (int o = 0; o < lastData[i, 0].ColumnCount; o++)
        //                    {
        //                        if ((n * o) % 10 < 9) { continue; }

        //                        GameObject tmp = new GameObject();
        //                        tmp.AddComponent<LineRenderer>();
        //                        tmp.GetComponent<LineRenderer>().material = Resources.Load<Material>("Material/lineRender");
        //                        tmp.GetComponent<LineRenderer>().material.shader = Shader.Find("Unlit/lineShader");
        //                        tmp.GetComponent<LineRenderer>().SetWidth(0.05f, 0.05f);
        //                        tmp.GetComponent<LineRenderer>().SetPosition(0, objs[i + 1, k, l, m].transform.position);
        //                        tmp.GetComponent<LineRenderer>().SetPosition(1, objs[i, j, n, o].transform.position);
        //                    }
        //                }
        //                //    }
        //                //}
        //            }
        //        }
        //    }
        //}
        //catch
        //{

        //}

        //// 生成reshape(-1)操作的连线
        //try
        //{
        //    int layerNum = dataSize.Length;
        //    int pos = 0;
        //    for (int i = 0; i < dataSize[layerNum - 1]; i++)
        //    {
        //        for (int j = 0; j < lastData[layerNum, 0].RowCount; j++)
        //        {
        //            for (int k = 0; k < lastData[layerNum, 0].ColumnCount; k++)
        //            {
        //                if (!((j + k) % 4 == 0)) { continue; }
        //                GameObject tmp = new GameObject();
        //                tmp.AddComponent<LineRenderer>();
        //                tmp.GetComponent<LineRenderer>().material = Resources.Load<Material>("Material/lineRender");
        //                tmp.GetComponent<LineRenderer>().material.shader = Shader.Find("Unlit/lineShader");
        //                tmp.GetComponent<LineRenderer>().SetWidth(0.05f, 0.05f);
        //                tmp.GetComponent<LineRenderer>().SetPosition(0, objs[layerNum, i, j, k].transform.position);
        //                tmp.GetComponent<LineRenderer>().SetPosition(1, objs[layerNum + 1, 0, pos / 20, pos % 20].transform.position);
        //                pos++;
        //                Debug.Log(pos);
        //            }
        //        }
        //    }
        //}
        //catch
        //{

        //}

        //// 生成nCore
        //try
        //{
        //    ArrayList nCoreSize = new ArrayList();
        //    nCoreSize.Add(1);
        //    for (int i = 0; i < dataSize.Length; i++)
        //    {
        //        nCoreSize.Add(dataSize[i]);
        //    }
        //    //Debug.Log(nCoreSize);
        //    for (int i = 0; i + 1 < nCoreSize.Count; i++)
        //    {
        //        //Debug.Log(int.Parse(nCoreSize[i].ToString()));
        //        //Debug.Log(int.Parse(nCoreSize[i+1].ToString()));
        //        for (int j = 0; j < int.Parse(nCoreSize[i].ToString()); j++)
        //        {
        //            for (int k = 0; k < int.Parse(nCoreSize[i + 1].ToString()); k++)
        //            {
        //                // Debug.Log(string.Format("nCore_{0:D3}", i) + string.Format("_{0:D3}", j) + string.Format("_{0:D3}", k));
        //                Matrix<float> m = MatlabReader.Read<float>(path,
        //                    string.Format("nCore_{0:D3}", i) + string.Format("_{0:D3}", j) + string.Format("_{0:D3}", k));
        //                //Debug.Log("OK");
        //                CreateCores(m, i, j, k);
        //            }
        //        }

        //    }
        //}
        //catch
        //{

        //}

    }

    void CreateTrainState()
    {
        trainState[0] = GenerateText("", new Vector3(-200f, 0f, 0f), new Color(255/255f, 165/255f, 79/255f), 100);
        for (int i = 0; i < dataSize.Length; i++)
        {
            trainState[i + 1] = GenerateText("", new Vector3(-200f, 0f, (i + 1) * 100f), new Color(255 / 255f, 165 / 255f, 79 / 255f), 100);
        }
        trainState[dataSize.Length + 1] = GenerateText("", new Vector3(-200f, 0f, (dataSize.Length + 1) * 100f), new Color(255 / 255f, 165 / 255f, 79 / 255f), 100);
        trainState[dataSize.Length + 2] = GenerateText("", new Vector3(-200f, 0f, (dataSize.Length + 2) * 100f + 50f), new Color(255 / 255f, 165 / 255f, 79 / 255f), 100);
    }

    void UpdateTrainState()
    {
        if (!isUpdate) { return;  }
        String path = "Assets/Resources/Textures/CIFAR10/cifar10_matrix_data.txt";
        try
        {
            if (System.IO.File.Exists(path))
            {
                string line, allVal = "";
                System.IO.StreamReader file = new System.IO.StreamReader(path);
                while((line = file.ReadLine()) != null)
                {
                    allVal += line;
                    allVal += "\n";
                }
                // GenerateText(allVal, new Vector3(-150, 20, 0), new Color(1f, 1f, 1f), 100);
                for (int i = 0;i <= dataSize.Length + 2;i ++)
                {
                    var style = trainState[i].GetComponent<TextMesh>();
                    style.text = allVal;
                }
            }
        }
        catch
        {

        }
    }

    [Obsolete]
    void UpdateNeural(int[] data)
    {
        int layer_nums = data.GetLength(0);
        String path = "Assets/Resources/Textures/CIFAR10/cifar10_matrix_data.mat";
        if (!System.IO.File.Exists(path)) { return; }

        // 更新featureMap的颜色
        try
        {
            if (!isUpdate)
            {

            }
            else
            {
                for (int i = 0; i < layer_nums; i++)
                {
                    int matrix_nums = data[i];
                    for (int j = 0; j < matrix_nums; j++)
                    {

                        //String path = string.Format("Assets/Resources/Textures/MNIST/matrix{0:D3}_", i);
                        //path += string.Format("{0:D3}.mat", j);
                        if (System.IO.File.Exists(path))
                        {
                            //Matrix<float> m = MatlabReader.Read<float>(path);
                            //Matrix<float> m = MatlabReader.Read<float>("Assets/Resources/Textures/MNIST/matrix_data.mat", string.Format("{0:D3}_{0:D3}", i, j));

                            try
                            {
                                Matrix<float> m = MatlabReader.Read<float>(path, string.Format("{0:D3}", i) + string.Format("_{0:D3}", j));
                                UpdateCubes(m, i + 1, j, matrix_nums);
                            }
                            catch
                            {

                            }
                        }
                    }
                }
            }
        }
        catch
        {

        }


        // 更新最后0-9正立方体颜色
        if (!System.IO.File.Exists(path)) { return; }
        try
        {
            if (!isUpdate)
            {

            }
            else
            {
                Matrix<float> m = MatlabReader.Read<float>(path, "output");

                for (int i = 0; i < 10; i++)
                {
                    if (i == (int)(m[0, 0] + 1e-10))
                    {
                        objs[dataSize.Length + 2, i, 0, 0].GetComponent<MeshRenderer>().material.color = new Color(1f, 0f, 0f);
                    }
                    else
                    {
                        objs[dataSize.Length + 2, i, 0, 0].GetComponent<MeshRenderer>().material.color = new Color(205/255f, 205/255f, 193/255f);

                    }
                }


            }
        }
        catch
        {

        }

        // 更新全连接层小立方体颜色
        try
        {
            if (!isUpdate)
            {

            }
            else
            {
                Matrix<float> m = MatlabReader.Read<float>(path, string.Format("{0:D3}", dataSize.Length) + string.Format("_{0:D3}", 0));
                int edge = 20;
                for (int i = 0; i < edge; i++)
                {
                    for (int j = 0; j < edge; j++)
                    {
                        if (Math.Abs(m[i, j]) > 0.5f)
                        {
                            objs[dataSize.Length + 1, 0, i, j].GetComponent<MeshRenderer>().material.color = new Color(1f, 0f, 0f, 0.5f);
                        }
                        else
                        {
                            objs[dataSize.Length + 1, 0, i, j].GetComponent<MeshRenderer>().material.color = new Color(205 / 255f, 205 / 255f, 193 / 255f);
                        }
                    }
                }
            }
        }
        catch
        {

        }

        // 更新全连接层线的颜色
        try
        {
            if (!isUpdate)
            {

            }
            else
            {
                int edge = 32;
                for (int i = 0; i < edge; i++)
                {
                    for (int j = 0; j < edge; j++)
                    {
                        for (int k = 0; k < 10; k++)
                        {
                            //Debug.Log(objs[dataSize.Length + 1, 0, i, j].GetComponent<MeshRenderer>().material.color.g);

                            if ((int)(objs[dataSize.Length + 1, 0, i, j].GetComponent<MeshRenderer>().material.color.r + 1e-6) == 1f
                                && (int)(objs[dataSize.Length + 2, k, 0, 0].GetComponent<MeshRenderer>().material.color.r + 1e-6) == 1f)
                            {
                                lines[(i * edge) + j, k].GetComponent<LineRenderer>().material.color = new Color(1f, 0f, 0f);
                                //lines[(i * 20) + j, k].GetComponent<LineRenderer>().material.SetColor("Color", new Color(1f, 0f, 0f, 1f));
                            }
                            else
                            {
                                lines[(i * edge) + j, k].GetComponent<LineRenderer>().material.color = new Color(205 / 255f, 205 / 255f, 193 / 255f);
                                //lines[(i * 20) + j, k].GetComponent<LineRenderer>().material.SetColor("Color", new Color(1f, 0f, 0f, 1f));
                            }

                        }

                    }
                }
            }
        }
        catch
        {

        }

        //// 更新nCore的颜色
        //try
        //{
        //    if (!isUpdate)
        //    {

        //    }
        //    else
        //    {
        //        ArrayList nCoreSize = new ArrayList();
        //        nCoreSize.Add(1);
        //        for (int i = 0; i < dataSize.Length; i++)
        //        {
        //            nCoreSize.Add(dataSize[i]);
        //        }
        //        for (int i = 0; i + 1 < nCoreSize.Count; i++)
        //        {
        //            for (int j = 0; j < int.Parse(nCoreSize[i].ToString()); j++)
        //            {
        //                for (int k = 0; k < int.Parse(nCoreSize[i + 1].ToString()); k++)
        //                {
        //                    Matrix<float> m = MatlabReader.Read<float>(path,
        //                        string.Format("nCore_{0:D3}", i) + string.Format("_{0:D3}", j) + string.Format("_{0:D3}", k));
        //                    //Debug.Log("OK");
        //                    //CreateCores(m, i, j, k);
        //                    UpdateCores(m, i, j, k);
        //                }
        //            }

        //        }
        //    }
        //}
        //catch
        //{

        //}

    }

    //void CreateCube(int layer, int matrix, int totMatrix, Matrix<float> data)
    //{
    //    int sz1 = data.RowCount;
    //    int sz2 = data.ColumnCount;

    //    GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Plane);
    //    obj.transform.localScale = new Vector3(sz2 / 10.0f, 1.0f, sz1 / 10.0f);
    //    Vector3 rotate = new Vector3(-90.0f, 0.0f, 0.0f);
    //    obj.transform.localRotation = Quaternion.Euler(rotate);

    //    obj.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/cubeRender");
    //    obj.GetComponent<MeshRenderer>().material.shader = Shader.Find("Custom/cubeRender");
    //    float off = 1.2f * sz1;
    //    obj.transform.position = new Vector3(matrix * off + off / 2.0f - off / 2.0f * totMatrix, 0f, (layer + 1) * 50f);

    //    //objs[layer, matrix] = obj;
    //}

    //void UpdateCube(int layer, int matrix, int totMatrix, Matrix<float> data)
    //{

    //    int sz1 = data.RowCount;
    //    int sz2 = data.ColumnCount;

    //    Texture2D texture = new Texture2D(sz2, sz1);
    //    //Texture2D texture = (Texture2D)objs[layer, matrix].GetComponent<MeshRenderer>().material.GetTexture("_MainTex");

    //    for (int i = 0; i < sz2; i++)
    //    {
    //        for (int j = 0; j < sz1; j++)
    //        { 
    //            texture.SetPixel(sz2 - 1 - i, sz1 - 1 - j, new Color(data[sz1 - 1 - j, i], data[sz1 - 1 - j, i], data[sz1 - 1 - j, i], 0.9f));
    //        }
    //    }
    //    texture.Apply();
    //    texture.filterMode = FilterMode.Point;
    //    texture.wrapMode = TextureWrapMode.Clamp;

    //    //objs[layer, matrix].GetComponent<MeshRenderer>().material.SetTexture("_MainTex", texture);
    //}

    // Start is called before the first frame update
    [Obsolete]
    void Start()
    {
        dataSize = new int[] { 4, 4, 16, 16 };
        objs = new GameObject[20, 400, 50, 50];
        // nCore = new GameObject[20, 20, 20, 10, 10];
        lastData = new Matrix<float>[20, 400];
        lines = new GameObject[1500, 10];
        labelName = new string[] { "飞机", "汽车", "鸟", "猫", "鹿", "狗", "青蛙", "马", "船", "货车" };
        labelText = new GameObject[10];
        layerName = new string[] { "输入图片", "第一层卷积", "第二层卷积", "第三层卷积", "第四层卷积", "第五层卷积",
                                   "第六层卷积", "第七层卷积", "第八层卷积", "第九层卷积" };
        trainState = new GameObject[10];
        isUpdate = true;
        CreateImage();
        CreateNeural(dataSize);
        //CreateNeural(new int[] { 6, 16 });

        CreateTrainState();
    }

    // Update is called once per frame
    [Obsolete]
    void Update()
    {
        UpdateImage();
        UpdateNeural(dataSize);
        UpdateTrainState();
    }

}