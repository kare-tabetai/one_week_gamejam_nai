using UnityEngine;

/// <summary>
/// 継承したクラスのStartでDontDestroyonLoad(gameObject)
/// をするとゲーム全体でのシングルトンができる．
/// しないとシーン内でのシングルトンができる．
/// 継承先ではAwake,OnDestroyは使えないので，
/// SubAwake,SubOnDestroyを使う.
/// public class 継承クラス:MonoSingleton<継承クラス>って感じで使う.
/// </summary>
/// <typeparam name="T">継承した型</typeparam>
public abstract class MonoSingleton<T> : 
    MonoBehaviour where T : MonoSingleton<T>
{
    protected static T m_isnstance;

    /// <summary>
    /// すでにインスタンスがあれば破棄
    /// 値をシーンごとにコピーする場合はこれをオーバーライド
    /// </summary>
    protected virtual void Awake()
    {
        if (m_isnstance != null)
        {
            print(typeof(T) + "was instantiated");
            Destroy(gameObject);
            return;
        }
        Instance = (T)this;
        SubAwake();
    }

    protected virtual void SubAwake()
    {

    }

    protected virtual void OnDestroy()
    {
        SubOnDestroy();
        if (m_isnstance == this)
        {
            m_isnstance = null;
        }
    }

    protected virtual void SubOnDestroy()
    {
    }

    public static bool IsInstanceNull()
    {
        return m_isnstance == null;
    }

    public static T Instance
    {
        get
        {
            if (m_isnstance == null)
            {
                //var obj = new GameObject(typeof(T).Name);
                //obj.AddComponent<T>();
                Debug.LogError(typeof(T).Name + "singleton is not instantiated");
            }
            return m_isnstance;
        }
        protected set
        {
            m_isnstance = value;
        }
    }
}
