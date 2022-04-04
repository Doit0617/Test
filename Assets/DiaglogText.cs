using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiaglogText : Text
{
    const int VERTEX_WIDTH = 6;
    float shackScale = 1;
    float shackSpeed = 8;
    int fontCount = 0;
    int fontIdx = 0;
    List<UIVertex> output = new List<UIVertex>();
    IEnumerator StartTextAnimation()
    {
        while (true)
        {
            if (fontIdx < fontCount)
            {
                fontIdx++;
                SetVerticesDirty();
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(StartTextAnimation());
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        base.OnPopulateMesh(vh);
        vh.GetUIVertexStream(output);
        int vertexCount = output.Count;
        fontCount = vertexCount / 6;
        for (int i = 0; i < vertexCount; i += 6)
        {
            int idx = i / 6;
            if (fontIdx >= idx)
            {
                ShowText(idx);
            }
            else
            {
                HideText(idx);
            }
            if (output[i].position == output[i + 1].position)
            {
                continue;
            }
            // DoTextMove(i);
        }
        vh.Clear();
        vh.AddUIVertexTriangleStream(output);

    }

    void ShowText(int idx)
    {
        for (int i = 0; i < 6; i++)
        {
            UIVertex v = output[idx * VERTEX_WIDTH + i];
            v.color.a = byte.MaxValue;
            output[idx * 6 + i] = v;
        }
    }

    void HideText(int idx)
    {
        for (int i = 0; i < 6; i++)
        {
            UIVertex v = output[idx * VERTEX_WIDTH + i];
            v.color.a = 0;
            output[idx * VERTEX_WIDTH + i] = v;
        }
    }
    
    // 抖动
    void DoTextMove(int i, int type = 0)
    {
        UIVertex v1 = output[i];
        UIVertex v2 = output[i + 1];
        UIVertex v3 = output[i + 2];
        UIVertex v4 = output[i + 3];
        UIVertex v5 = output[i + 4];
        UIVertex v6 = output[i + 5];

        var color = output[i].color;
        Vector3 rnd = Vector3.zero;
        if (color.a == 0xff)
        {
            rnd = Random.insideUnitCircle * shackScale;
        }
        else if (color.a == 0x02)
        {
            rnd = (Mathf.Sin(i + Time.time * shackSpeed)) * Vector3.up * shackScale * 2;
        }
        else
        {
            return;
        }
        color.a = 0xFF;
        v1.position += rnd;
        v2.position += rnd;
        v3.position += rnd;
        v4.position += rnd;
        v5.position += rnd;
        v6.position += rnd;
        v1.color = v2.color = v3.color = v4.color = v5.color = v6.color = color;
        output[i] = v1;
        output[i + 1] = v2;
        output[i + 2] = v3;
        output[i + 3] = v4;
        output[i + 4] = v5;
        output[i + 5] = v6;
    }
    private void Update()
    {
        // SetVerticesDirty();
    }
}
