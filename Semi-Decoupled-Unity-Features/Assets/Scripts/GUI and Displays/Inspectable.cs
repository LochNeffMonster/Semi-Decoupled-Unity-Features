using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inspectable : MonoBehaviour
{
    //public InteractionType useItemType = InteractionType.None;
    public GUIStyle closeTextStyle;
    private string closeDocumentText = "Press [INTERACT] to close";

    public string inspectableName = null;
    public List<Texture2D> inspectableImages = new List<Texture2D>();

    private Texture2D _leftArrowEnabled;
    private Texture2D _rightArrowEnabled;
    public int arrowPadding = 20;

    private bool _inspecting = false;
    private int _currentImage = 0;
    private bool _collected;
    private bool _canExitInspection = false;
    //private Player _currentPlayer = null;

    void Awake()
    {
        closeTextStyle.font = (Font)Resources.Load("Menu/chinese rocks rg");
        closeTextStyle.fontSize = ScreenUtil.getPixels(36);
        closeTextStyle.alignment = TextAnchor.MiddleCenter;
        closeTextStyle.normal.textColor = Color.white;

        _leftArrowEnabled = Resources.Load<Texture2D>("Textures/Documents/ArrowHardlineLeft");
        _rightArrowEnabled = Resources.Load<Texture2D>("Textures/Documents/ArrowHardlineRight");
    }

    /* Update takes in player mouse clicks to navigate through documents/inspectables
     *
     */
    void Update()
    {
        bool exitingInspection = false;
        //if (_currentPlayer != null)
        //{
            if (InputManager.getKeyDown(InputAction.INTERACT))
            {
                exitingInspection = true;
            }
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                // if clicked on top area, exit inspection
                if (Input.mousePosition.y > Screen.height - Screen.height / 5)
                {
                    exitingInspection = true;
                }
                else if (Input.mousePosition.x > (Screen.width - ScreenUtil.getPixels(inspectableImages[_currentImage].width)) / 2)
                {
                    // if clicked on document or on right side of the screen
                    if (_currentImage != inspectableImages.Count - 1)
                    {
                        // move to the next page
                        _currentImage++;
                    }
                }
                else
                { // if clicked on left side of the screen
                    if (_currentImage != 0)
                    {
                        // move to the previous page
                        _currentImage--;
                    }
                }
            }
            if (exitingInspection && _canExitInspection)
            {
                //_currentPlayer.toggleInspection();
                _currentImage = 0;
                _inspecting = false;
                //_currentPlayer = null;
            }
        //}
        _canExitInspection = true;
    }

    public void OnGUI()
    {
        if (_inspecting)
        {
            float startingX = (Screen.width - ScreenUtil.getPixels(inspectableImages[_currentImage].width)) / 2;
            float startingY = (Screen.height - ScreenUtil.getPixels(inspectableImages[_currentImage].height) + ScreenUtil.getPixels(50)) / 2;
            float docWidth = ScreenUtil.getPixels(inspectableImages[_currentImage].width);
            float docHeight = ScreenUtil.getPixels(inspectableImages[_currentImage].height);
            GUI.DrawTexture(new Rect(startingX, startingY, docWidth, docHeight), inspectableImages[_currentImage], ScaleMode.StretchToFill, true);

            // If there is a previous page, show left arrow
            if (_currentImage > 0)
            {
                float startingLeftArrowX = startingX - ScreenUtil.getPixels(arrowPadding) - ScreenUtil.getPixels(_leftArrowEnabled.width);
                float startingLeftArrowY = (Screen.height - ScreenUtil.getPixels(_leftArrowEnabled.height)) / 2;
                GUI.DrawTexture(new Rect(startingLeftArrowX, startingLeftArrowY, ScreenUtil.getPixels(_leftArrowEnabled.width),
                    ScreenUtil.getPixels(_leftArrowEnabled.height)), _leftArrowEnabled, ScaleMode.StretchToFill, true);
            }

            // If there is a next page, show right arrow
            if (_currentImage < inspectableImages.Count - 1)
            {
                float startingRightArrowX = Screen.width / 2 + ScreenUtil.getPixels(inspectableImages[_currentImage].width) / 2 + ScreenUtil.getPixels(arrowPadding);
                float startingRightArrowY = (Screen.height - ScreenUtil.getPixels(_rightArrowEnabled.height)) / 2;
                GUI.DrawTexture(new Rect(startingRightArrowX, startingRightArrowY, ScreenUtil.getPixels(_rightArrowEnabled.width),
                    ScreenUtil.getPixels(_rightArrowEnabled.height)), _rightArrowEnabled, ScaleMode.StretchToFill, true);
            }

            GUI.Label(new Rect(0, ScreenUtil.getPixels(15), Screen.width, closeTextStyle.fontSize), InputManager.parseString(closeDocumentText), closeTextStyle);
        }
    }

    public bool isCollected()
    {
        return _collected;
    }

    public void setAsCollected()
    {
        _collected = true;
    }
}