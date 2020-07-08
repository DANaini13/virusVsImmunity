using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------
// Singleton class
// Note: if type T has a parent class which happens to be singleton, 
// they will be treated as entirely different classes. So no using child 
// instance as parent singleton, and vice versa. This is usually the desired behavior. 
// It allows strong typed instance when parent already is a singleton.
//----------------------------------------------------------------------
public class USingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static T _instance;

    public static T Inst
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<T>();

                if(_instance != null && _instance.GetType() != typeof(T))
                {
                    // this happens when _instance is a subclass of T.
                    _instance = null;
                }

                if (GameObject.FindObjectsOfType<T>().Length > 1)
                {
                    Debug.LogError("[Singleton] Something went really wrong " +
                        " - there should never be more than 1 singleton!" +
                        " Reopening the scene might fix it.");
                    return _instance;
                }

                if (_instance == null)
                {
                    GameObject singleton = new GameObject();
                    _instance = singleton.AddComponent<T>();
                    singleton.name = "(singleton) " + typeof(T).ToString();

                    GameObject.DontDestroyOnLoad(singleton);

                    Debug.Log("[Singleton] An instance of " + typeof(T) +
                        " is needed in the scene, so '" + singleton +
                        "' was created with DontDestroyOnLoad.");
                }
                else
                {
                    //Debug.Log("[Singleton] Using instance already created: " + _instance.gameObject.name);
                }
            }

            return _instance;
        }
    }

    public static T InstDoNotAutoCreate()
    {
        if (_instance == null)
        {
            _instance = GameObject.FindObjectOfType<T>();

            if(_instance != null && _instance.GetType() != typeof(T))
            {
                // this happens when _instance is a subclass of T.
                _instance = null;
            }

            if (GameObject.FindObjectsOfType<T>().Length > 1)
            {
                Debug.LogError("[Singleton] Something went really wrong " +
                    " - there should never be more than 1 singleton!" +
                    " Reopening the scene might fix it.");
                return _instance;
            }
        }

        return _instance;
    }
}
