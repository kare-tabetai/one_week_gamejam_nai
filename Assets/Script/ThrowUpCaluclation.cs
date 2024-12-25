using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ThrowUpCalculation
{
    /// <summary>
    /// 軌道の位置をリストで返す
    /// </summary>
    /// <param name="position">投げる位置</param>
    /// <param name="way">投げる方向</param>
    /// <param name="target"></param>
    /// <param name="orbitInterval"></param>
    /// <returns></returns>
    public static List<Vector2> GetThrowUpOrbit(Vector2 position, Vector2 target, float yMax, float orbitInterval)
    {
        float gravity = -Physics.gravity.y;
        var result = new List<Vector2>();
        Vector2 way = OrbitCalculations(position, target, yMax);
        Vector2 targetVec = target - position;
        float time = 0;
        for (int i = 0; i < 50; i++)
        {
            float x = way.x * time;
            float y = way.y * time - 0.5f * gravity * time * time;
            if (Mathf.Abs(targetVec.x) < x) { break; }

            Vector2 pos = position + new Vector2(x, y);
            result.Add(pos);
            time += orbitInterval;
        }
        result.Add(target);
        return result;
    }

    /// <summary>
    /// 軌道の位置をリストで返す
    /// </summary>
    /// <param name="position">投げる位置</param>
    /// <param name="way">投げる方向</param>
    /// <param name="target"></param>
    /// <param name="orbitInterval"></param>
    /// <returns></returns>
    public static List<Vector3> GetThrowUpOrbit(Vector3 position, Vector3 way, Vector3 target, float orbitInterval)
    {
        Vector3 horizontalVec = new Vector3(way.x, 0, way.z);
        Vector2 v = new Vector2(horizontalVec.magnitude, way.y);
        float gravity = -Physics.gravity.y;

        var result = new List<Vector3>();

        float time = 0;
        for (int i = 0; i < 50; i++)
        {
            float x = v.x * time;
            float y = v.y * time - 0.5f * gravity * time * time;
            if (horizontalVec.magnitude < x) break;

            Vector3 pos = position + horizontalVec.normalized * x + new Vector3(0, y, 0);
            result.Add(pos);
            time += orbitInterval;
        }
        result.Add(target);
        return result;
    }

    /// <summary>
    /// 投げる方向と強さを返す
    /// </summary>
    /// <param name="position"></param>
    /// <param name="target"></param>
    /// <param name="yMax"></param>
    /// <returns></returns>
    public static Vector2 OrbitCalculations(Vector2 position, Vector2 target, float yMax)
    {
        float gravity = -Physics2D.gravity.y;
        Vector2 targetVec = target - position;

        if (yMax < targetVec.y)
        {
            Debug.LogWarning("ターゲットに届きません");
            return Vector3.zero;
        }

        Vector2 v;
        v.y = Mathf.Sqrt(2 * gravity * yMax);
        v.x = GetvX(targetVec.x, targetVec.y, gravity, v.y);
        return v;
    }

    public static Vector3 OrbitCalculations(Vector3 position, Vector3 target, float bombYMax)
    {
        float gravity = -Physics.gravity.y;
        Vector3 targetVec = target - position;
        Vector3 horizontalVec = new Vector3(target.x, 0, target.z) - new Vector3(position.x, 0, position.z);

        if (bombYMax < targetVec.y)
        {
            Debug.LogWarning("ターゲットに届きません");
            return Vector3.zero;
        }

        Vector2 v;
        v.y = Mathf.Sqrt(2 * gravity * bombYMax);
        v.x = GetvX(horizontalVec.magnitude, targetVec.y, gravity, v.y);
        return horizontalVec.normalized * v.x + new Vector3(0, v.y, 0);
    }

    /// <summary>
    /// x軸の速度を求める
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="g"></param>
    /// <param name="vY"></param>
    /// <returns></returns>
    private static float GetvX(float x, float y, float g, float vY)
    {
        if (x < 0)
        {
            x *= -1;
            Vector3 vec = QuadraticEquation(y, -vY * x, 0.5f * g * x * x);
            if (vec.z == 0) { return -vec.x; }
            if (y <= 0) return -Mathf.Max(vec.x, vec.y);
            else return -Mathf.Min(vec.x, vec.y);
        }
        else
        {
            Vector3 vec = QuadraticEquation(y, -vY * x, 0.5f * g * x * x);
            if (vec.z == 0) { return vec.x; }
            if (y <= 0) return Mathf.Max(vec.x, vec.y);
            else return Mathf.Min(vec.x, vec.y);
        }
    }

    /// <summary>
    /// ax^2+bx+c=0を解く
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <returns>x,yそれぞれに解がでるzが0ならaのみ答え</returns>
    private static Vector3 QuadraticEquation(float a, float b, float c)
    {
        if (a == 0)
        {
            return new Vector3(-c / b, 0, 0);
        }

        float topFormula = -b + Mathf.Sqrt(b * b - 4 * a * c);
        float bottomFormula = 2 * a;
        Vector3 vec = new Vector3(topFormula / bottomFormula, 0, -1);

        topFormula = -b - Mathf.Sqrt(b * b - 4 * a * c);
        vec.y = topFormula / bottomFormula;

        return vec;
    }
}
