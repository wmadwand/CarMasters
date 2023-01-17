using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IController
{
    IEnumerator Init();
}

public class TrackSpawner : MonoBehaviour, IController
{
    public IEnumerator Init()
    {
        yield return null;
    }
}
