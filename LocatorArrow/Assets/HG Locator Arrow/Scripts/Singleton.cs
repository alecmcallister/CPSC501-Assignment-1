using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	static bool m_ShuttingDown;
	static object m_Lock = new object();
	static T m_Instance;

	public static T Instance
	{
		get
		{
			if (m_ShuttingDown)
				return null;

			lock (m_Lock)
			{
				if (m_Instance == null)
				{
					m_Instance = (T)FindObjectOfType(typeof(T));

					if (m_Instance == null)
					{
						GameObject singletonObject = new GameObject(typeof(T).ToString() + " (Singleton)");
						m_Instance = singletonObject.AddComponent<T>();

						DontDestroyOnLoad(singletonObject);
					}
				}

				return m_Instance;
			}
		}
	}

	void OnApplicationQuit()
	{
		m_ShuttingDown = true;
	}

	void OnDestroy()
	{
		m_ShuttingDown = true;
	}
}