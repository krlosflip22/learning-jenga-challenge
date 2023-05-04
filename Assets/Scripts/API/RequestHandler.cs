using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace LearningJenga.API
{
    public class RequestHandler : MonoBehaviour
    {
        const string URL = "https://ga1vqcu3o1.execute-api.us-east-1.amazonaws.com/Assessment/stack";

        private static RequestHandler instance;

        void Awake()
        {
            if (instance != null)
            {
                Destroy(this.gameObject);
                return;
            }

            instance = this;
        }

        public static void RequestData(Action<string> onSuccess, Action<string> onFailed)
        {
            instance.StartCoroutine(instance.RequestDataRoutine(onSuccess, onFailed));
        }

        public IEnumerator RequestDataRoutine(Action<string> onSuccess, Action<string> onFailed)
        {
            var getRequest = UnityWebRequest.Get(URL);

            yield return getRequest.SendWebRequest();

            if (getRequest.result == UnityWebRequest.Result.ConnectionError || getRequest.result == UnityWebRequest.Result.ProtocolError)
                onFailed?.Invoke(getRequest.error);
            else
                onSuccess?.Invoke(getRequest.downloadHandler.text);
        }

        void OnDestroy()
        {
            instance = null;
        }
    }
}
