using UnityEngine;

public class UserInputManager : MonoBehaviour
{
    public float pitch;
    public float roll;
    public float yaw;
    public float speed;
    public bool mouthOpen;

    public float getPitch()
    {
        return pitch;
    }
    public float getRoll()
    {
        return roll;
    }
    public float getYaw()
    {
        return yaw;
    }
    public float getSpeed()
    {
        return speed;
    }
    public bool getMouthOpen()
    {
        return mouthOpen;
    }
}