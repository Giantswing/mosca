using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICustomTeleport
{
    void CustomTeleport(Transform teleporterTransform, Transform originalTeleportTransform);

    GameObject ReturnGameobject();
}