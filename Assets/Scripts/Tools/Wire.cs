using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : Tool, IGrabbable
{
    private void Start()
    {
        isActive = false;
    }

    public override void WebGLMakeActiveTool()
    {
        base.WebGLMakeActiveTool();
    }

    public override void WebGLSwitchToDifferentTool()
    {
        base.WebGLSwitchToDifferentTool();
    }

    bool IGrabbable.CheckIfActive()
    {
        return isActive;
    }

    void IGrabbable.OnGrab()
    {
        isActive = true;
    }

    void IGrabbable.OnRelease()
    {
        if (!GamePlatformManager.IsVRMode)
        {
            return;
        }
        //toolRB.isKinematic = false;
        isActive = false;
        transform.position = defaultPosition.transform.position;
    }
}
