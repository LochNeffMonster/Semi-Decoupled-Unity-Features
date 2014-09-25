using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum InputAction
{
    UP,
    DOWN,
    RIGHT,
    LEFT,
    INTERACT,
    SPRINT,
    SNEAK,
    VIEW_OBJECTIVE,
    FLASHLIGHT,
    FLASHLIGHT_ALT,
    ESCAPE_MENU
};

public static class InputActionExtension
{
    public static bool canBeRemapped(this InputAction action)
    {
        return action != InputAction.ESCAPE_MENU;
    }
}

public class InputManager
{
    private static Dictionary<InputAction, KeyCode> _keycodeMap = new Dictionary<InputAction, KeyCode>();
    private static bool _canRecieveInput = true;
    private static bool _playerPrefsLoaded = false;

    public static bool canRecieveInput
    {
        get
        {
            return _canRecieveInput;
        }
        set
        {
            _canRecieveInput = value;
        }
    }

    /* Initializes the _keycodeMap with the default keyset
     */
    static InputManager()
    {
        loadDefaults(false);
    }

    public static void loadDefaults(bool saveToPlayerPrefs = true)
    {
        _keycodeMap[InputAction.UP] = KeyCode.W;
        _keycodeMap[InputAction.DOWN] = KeyCode.S;
        _keycodeMap[InputAction.RIGHT] = KeyCode.D;
        _keycodeMap[InputAction.LEFT] = KeyCode.A;
        _keycodeMap[InputAction.INTERACT] = KeyCode.E;
        _keycodeMap[InputAction.SPRINT] = KeyCode.LeftShift;
        _keycodeMap[InputAction.SNEAK] = KeyCode.LeftControl;
        _keycodeMap[InputAction.VIEW_OBJECTIVE] = KeyCode.Tab;
        _keycodeMap[InputAction.ESCAPE_MENU] = KeyCode.Escape;
        _keycodeMap[InputAction.FLASHLIGHT] = KeyCode.F;
        _keycodeMap[InputAction.FLASHLIGHT_ALT] = KeyCode.Mouse2;
        if (saveToPlayerPrefs)
        {
            foreach (var pair in _keycodeMap)
            {
                string key = pair.Key.ToString();
                int value = (int)pair.Value;
                PlayerPrefs.SetInt(key, value);
            }
        }
    }

    public static string parseString(string inString)
    {
        foreach (InputAction inputAction in (InputAction[])Enum.GetValues(typeof(InputAction)))
        {
            KeyCode code = _keycodeMap[inputAction];
            string actionName = Enum.GetName(typeof(InputAction), inputAction);
            string codeName = Enum.GetName(typeof(KeyCode), code);
            if (codeName.Contains("Arrow"))
            {
                codeName = codeName.Replace("Arrow", "");
            }
            else
            {
                codeName = codeName.Replace("Right", "");
                codeName = codeName.Replace("Left", "");
            }
            inString = inString.Replace("[" + actionName + "]", codeName);
        }
        return inString;
    }

    /* Loads the stored PlayerPrefs into the key mapping.  This needs to be
     * called from the main Unity thread or else complaining will happen.
     */
    public static void loadPlayerPrefs()
    {
        loadDefaults(false);
        foreach (InputAction inputAction in (InputAction[])Enum.GetValues(typeof(InputAction)))
        {
            if (inputAction.canBeRemapped())
            {
                KeyCode code = (KeyCode)PlayerPrefs.GetInt(inputAction.ToString(), (int)_keycodeMap[inputAction]);
                _keycodeMap[inputAction] = code;
            }
        }
        _playerPrefsLoaded = true;
    }

    /**Rebinds the given key name to be connected to the key
     * that is currently being pressed.  If no key is pressed,
     * this method still returns imidiately.
     * 
     * @return      true if the rebind was a succes
     *              false if the key could not be rebound
     */
    public static bool rebindKey(InputAction key)
    {
        if (!_playerPrefsLoaded)
        {
            loadPlayerPrefs();
        }
        System.Array codes = System.Enum.GetValues(typeof(KeyCode));
        foreach (int i in codes)
        {
            KeyCode code = (KeyCode)i;
            if (Input.GetKeyUp(code))
            {
                _keycodeMap[key] = code;
                PlayerPrefs.SetInt(key.ToString(), (int)code);
                return true;
            }
        }
        return false;
    }

    public static KeyCode getKeyCode(InputAction key)
    {
        if (!_playerPrefsLoaded)
        {
            loadPlayerPrefs();
        }
        return _keycodeMap[key];
    }

    /*Returns whether or not the given key is being pressed 
     * right now.
     * 
     * @return      true if the key is being held down
     *              false if the key is not being held down
     */
    public static bool getKey(InputAction key)
    {
        if (!_playerPrefsLoaded)
        {
            loadPlayerPrefs();
        }
        if (!_canRecieveInput)
        {
            return false;
        }
        KeyCode code = _keycodeMap[key];
        return Input.GetKey(code);
    }

    /*Returns whether or not the given key was just pressed
     * on this exact frame.
     * 
     * @return      true if the key was just pressed on this frame
     *              false otherwise
     */
    public static bool getKeyDown(InputAction key)
    {
        if (!_playerPrefsLoaded)
        {
            loadPlayerPrefs();
        }
        if (!_canRecieveInput)
        {
            return false;
        }
        KeyCode code = _keycodeMap[key];
        return Input.GetKeyDown(code);
    }

    /*Returns whether or not the given key was just released
     * on this exact frame
     * 
     * @return      true if the key was just released on this frame
     *              false otherwise
     */
    public static bool getKeyUp(InputAction key)
    {
        if (!_playerPrefsLoaded)
        {
            loadPlayerPrefs();
        }
        if (!_canRecieveInput)
        {
            return false;
        }
        KeyCode code = _keycodeMap[key];
        return Input.GetKeyUp(code);
    }

}
