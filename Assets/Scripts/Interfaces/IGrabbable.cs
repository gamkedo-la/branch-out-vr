/// <summary>
/// Interface for any object that can be grabbed and held by the VR hands
/// </summary>
public interface IGrabbable 
{
    void OnGrab();
    void OnRelease();
}
