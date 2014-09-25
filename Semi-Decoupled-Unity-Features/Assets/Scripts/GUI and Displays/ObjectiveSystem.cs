using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class ObjectiveSystem : MonoBehaviour
{
    private const string UPDATE_TEXT = "Your partner discovered this objective";
    // the order of the Objective in the case that there is a specific order
    private static int _currentOrder = -1;
    // the actual text to display the objective
    private static string _objectiveText = "Find the documentary crew";


    // variables to display text and background
    private int _padding;
    private bool _noObjective = false;
    private bool _partnerUpdate = false;
    private int _backgroundWidth;
    private Rect _backgroundRect;
    private Rect _partnerRect;
    private Texture2D _backgroundTexture;
    public GUIStyle objectiveStyle;
    public GUIStyle updateStyle;
    [Range(0, 500)]
    public int objectiveWidth = 100;

    // variables to control fading
    private float _startingAlpha;
    private float _startingTime;
    private float _currentAlpha = 0.0f;
    private float _timeElapsed = 0.0f;
    public float fadeTime = 10f;
    public float holdTime = 1f;
    private bool _holding = false;
    private float _timeWaited = 0.0f;
    private bool _active = false;
    private bool _fadeIn = false;
    private bool _fadeOut = false;

    private static ObjectiveSystem _sInstance;
    public static ObjectiveSystem sInstance
    {
        get
        {
            if (_sInstance == null)
            {
                _sInstance = (ObjectiveSystem)FindObjectOfType(typeof(ObjectiveSystem));
            }
            return _sInstance;
        }
    }

    public void Awake()
    {
        if (ObjectiveSystem.sInstance.GetInstanceID() != this.GetInstanceID())
        {
            Debug.LogWarning("There were multiple instances of " + GetType() + ", please fix that.");
            Destroy(this);
        }
        _backgroundTexture = (Texture2D)Resources.Load<Texture2D>("Textures/GUI/ObjectSystemplank1");
        _partnerRect = new Rect(_padding * 3 / 8, _padding / 8, ScreenUtil.getPixels(100), ScreenUtil.getPixels(20));
    }

    /* Set up objective system as a singleton
     */
    void Start()
    {
        objectiveStyle.fontSize = ScreenUtil.getPixels(objectiveStyle.fontSize);
        updateStyle.fontSize = ScreenUtil.getPixels(updateStyle.fontSize);

        _padding = ScreenUtil.getPixels(40);
        _backgroundWidth = (objectiveStyle.fontSize / 2) * _objectiveText.Length + ScreenUtil.getPixels(100);
        _backgroundRect = new Rect(0, ScreenUtil.getPixels(30), _backgroundWidth, objectiveStyle.fontSize * 2);
    }

    void Update()
    {

    }

    /* Sets the objective that the player is supposed to accomplish
     * @param objective The objective text that is being set
     *
     * @param order The order of the objective. This is to stop minor objectives from becoming
     *              the primary objective. For example: "find lighter" is order 1, "set statue
     *              on fire" is order 2. In other words, the player is not told to find the lighter
     *              is they have already been told to set the statue on fire.
     */
    [RPC]
    public void setObjective(string objective, int order)
    {
        if (_objectiveText == "" || (order > _currentOrder))
        {
            _objectiveText = objective;
            _currentOrder = order;
            _noObjective = false;
            _fadeIn = true;
            _active = true;
            _startingTime = Time.time;
            _startingAlpha = _currentAlpha;
        }
        _backgroundWidth = (objectiveStyle.fontSize / 2) * _objectiveText.Length + ScreenUtil.getPixels(100);
        _backgroundRect = new Rect(0, ScreenUtil.getPixels(30), _backgroundWidth, objectiveStyle.fontSize * 2);
    }

    /* Resets the objective and order to the default values
     */
    [RPC]
    public void resetObjective()
    {
        _objectiveText = "";
        _currentOrder = 0;
        _noObjective = true;
    }

    /* When the player presses the objective button, this function is called
     * depending on the state of the objective gui, the function will respond
     * differently
     */
    [RPC]
    public void showObjective()
    {
        if (_noObjective)
        {
            return;
        }
        if (_holding)
        {
            _startingTime = Time.time;
        }
        else if (_fadeOut)
        {
            _startingTime = Time.time;
            _startingAlpha = _currentAlpha;
            _fadeOut = false;
            _fadeIn = true;
        }
        else if (!_active && _currentAlpha < 1)
        {
            _fadeIn = true;
            _active = true;
            _startingTime = Time.time;
            _startingAlpha = _currentAlpha;
        }
    }

    /* When a player updates the objective, this function is called for the other player
     * Which shows the objective displayed in a different location with an additional 
     * message of "Your partner discovered the objective ..."
     */
    [RPC]
    public void showPartnerUpdate()
    {
        _partnerUpdate = true;
        showObjective();
    }

    public void OnGUI()
    {
        if ((!_active && !_holding) || _noObjective)
        {
            return;
        }

        Color guiColor = GUI.color;
        if (_fadeIn)
        {
            //Fading in
            _timeElapsed += Time.deltaTime;
            float alphaOffset = (Time.time - _startingTime) / fadeTime;
            if (alphaOffset >= 1.0f)
            {
                alphaOffset = 1.0f;
                _timeElapsed = 0.0f;
                _fadeIn = false;
                _holding = true;
                _startingTime = Time.time;
            }
            guiColor.a = _startingAlpha + alphaOffset;
            GUI.color = guiColor;
        }
        else if (_holding)
        {
            //Pausing to show the player it for a temporary time
            _timeWaited = Time.time - _startingTime;
            if (_timeWaited >= holdTime)
            {
                _holding = false;
                _fadeOut = true;
                _startingTime = Time.time;
            }
            guiColor.a = 1.0f;
            GUI.color = guiColor;
        }
        else if (_fadeOut)
        {
            //Fading out
            _timeElapsed += Time.deltaTime;
            float alphaOffset = (Time.time - _startingTime) / fadeTime;
            if (guiColor.a <= 0.0f)
            {
                _timeElapsed = 0.0f;
                _fadeOut = false;
                _active = false;
                _partnerUpdate = false;
            }
            if (alphaOffset > 1.0f)
            {
                alphaOffset = 1.0f;
            }
            guiColor.a = 1 - alphaOffset;
            GUI.color = guiColor;
        }
        _currentAlpha = GUI.color.a;

        if (_partnerUpdate)
        {
            GUI.Label(_partnerRect, UPDATE_TEXT, updateStyle);
        }
        GUI.DrawTexture(_backgroundRect, _backgroundTexture, ScaleMode.StretchToFill, true);

        //Text
        GUI.Label(new Rect(_backgroundRect.x, ScreenUtil.getPixels(45),
            ScreenUtil.getPixels(50), _padding / 2), _objectiveText, objectiveStyle);
    }

    /* Gets the current objective for the player to display
     */
    public string getObjective()
    {
        return _objectiveText;
    }
}
