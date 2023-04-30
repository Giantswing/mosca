using UnityEngine;

public class EventListenerTest : MonoBehaviour
{
    public void TestAction(EventContext ctx)
    {
        print(ctx.eventString);
    }
}