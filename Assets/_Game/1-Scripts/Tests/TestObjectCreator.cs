using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TestType
{
    Update,
    Subscribe
}

public class TestObjectCreator : MonoBehaviour
{
    public TestType testType;
    public int numberOfObjects = 1000;

    private void Start()
    {
        for (var i = 0; i < numberOfObjects; i++)
        {
            GameObject go = new GameObject($"TestObject{i}");
            go.transform.SetParent(transform);
            switch (testType)
            {
                case TestType.Update:
                    go.AddComponent<TestObjectUpdate>();
                    break;
                case TestType.Subscribe:
                    go.AddComponent<TestObjectSubscribe>();
                    break;
            }
        }
    }
}