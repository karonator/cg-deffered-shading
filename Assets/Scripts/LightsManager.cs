using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsManager : MonoBehaviour
{
    const int LIGHT_COUNT = 32;
    const float ANIM_SPEED = 0.005f;

    private Vector3[] positions = new Vector3[LIGHT_COUNT];
    private Vector3[] directions = new Vector3[LIGHT_COUNT];
    private Vector4[] colors = new Vector4[LIGHT_COUNT];

    private GameObject[] lightSpheres = new GameObject[LIGHT_COUNT];

    void Start()
    {
        Shader shader = Shader.Find("Unlit/Color");
        Material material = new Material(shader);

        for (int i = 0; i < LIGHT_COUNT; i++)
        {
            positions[i] = new Vector3(Random.Range(-4.5f, 4.5f), Random.Range(.5f, 9.5f), Random.Range(-4.5f, 4.5f));
            colors[i] = new Vector4(Random.Range(0, 1.0f), Random.Range(0, 1.0f), Random.Range(0, 1.0f), 0);

            directions[i] = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            directions[i].Normalize();
            /*
            lightSpheres[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            lightSpheres[i].gameObject.layer = 8;
            lightSpheres[i].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            lightSpheres[i].transform.position = positions[i];

            lightSpheres[i].GetComponent<Renderer>().material = material;
            lightSpheres[i].GetComponent<Renderer>().material.color = colors[i];
            */
        }
    }

    void Update()
    {
        // граничные значения для каждой компоненты
        Vector2[] bounds = { new Vector2(-5f, 5f), new Vector2(0, 10f), new Vector2(-5f, 5f) };

        for (int i = 0; i < LIGHT_COUNT; i++)
        {
            positions[i] += directions[i] * ANIM_SPEED;

            // цикл по трём компонентам (x, y, z)
            for (int comp = 0; comp < 3; comp++)
            {
                // если вышли за границу то вернуть значение к граничному и отразить вектор скорости
                if (Mathf.Clamp(positions[i][comp], bounds[comp].x, bounds[comp].y) != positions[i][comp])
                {
                    positions[i][comp] = Mathf.Clamp(positions[i][comp], bounds[comp].x, bounds[comp].y);
                    directions[i][comp] = -directions[i][comp];
                }
            }
            directions[i].Normalize();

            // lightSpheres[i].transform.position = positions[i];
        }
    }

    void OnRemove()
    {
        for (int i = 0; i < LIGHT_COUNT; i++)
        {
            DestroyImmediate(lightSpheres[i]);
        }
    }

    public Vector4[] lightsPositions()
    {
        Vector4[] out_positions = new Vector4[LIGHT_COUNT];
        for (int i = 0; i < LIGHT_COUNT; i++)
        {
            // заодно конвертируем Vector3 в Vector4 для дальнейшей передачи в шейдер
            out_positions[i] = positions[i];
        }
        return out_positions;
    }

    public Vector4[] lightsColors()
    {
        return colors;
    }
}
