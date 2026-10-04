using UnityEngine;
public static class InputManager
{
    private const KeyCode RunKey = KeyCode.LeftShift; 
    private const KeyCode ReloadKey = KeyCode.R;        
    private const KeyCode DropKey = KeyCode.G;         
    private const int FireButton = 0;

    public static float GetHorizontal()
    {
        return Input.GetAxis("Horizontal");
    }

    public static float GetVertical()
    {
        return Input.GetAxis("Vertical");
    }

    public static Vector2 GetLookDelta()
    {
        return Input.mousePositionDelta;
    }

    public static bool GetRunDown()
    {
        return Input.GetKeyDown(RunKey);
    }

    public static bool GetRunUp()
    {
        return Input.GetKeyUp(RunKey);
    }

    public static bool GetFireDown()
    {
        return Input.GetMouseButtonDown(FireButton); // Ateþ tuþuna bu karede basýldý mý (tek atýþ)
    }

    public static bool GetFireHeld()
    {
        return Input.GetMouseButton(FireButton);   // Ateþ tuþu basýlý tutuluyor mu (otomatik atýþ)
    }

    public static bool GetReloadDown()
    {
        return Input.GetKeyDown(ReloadKey);
    }

    public static bool GetDropDown()
    {
        return Input.GetKeyDown(DropKey);
    }
}