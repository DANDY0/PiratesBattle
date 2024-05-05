using System.Collections;
using UnityEngine;

namespace Behaviours
{
    public class CoroutineRunner : MonoBehaviour, ICoroutineRunner
    {
        private void Awake() => DontDestroyOnLoad(gameObject);
    }

    public interface ICoroutineRunner
    {
        Coroutine StartCoroutine(IEnumerator coroutine);
    }
}