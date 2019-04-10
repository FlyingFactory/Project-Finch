using UnityEngine;
using System.Collections;

namespace Proyecto26
{
    public static class StaticCoroutine
    {
        public class CoroutineHolder : MonoBehaviour { }

        private static CoroutineHolder _runner;
        public static CoroutineHolder runner
        {
            get
            {
                if (_runner == null)
                {
                    _runner = new GameObject("Static Coroutine RestClient").AddComponent<CoroutineHolder>();
                    Object.DontDestroyOnLoad(_runner);
                }
                return _runner;
            }
        }

        public static void StartCoroutine(IEnumerator coroutine)
        {
            //return runner.StartCoroutine(coroutine);
            Runner_call.Coroutines.Add(coroutine);
        }
    }
}
