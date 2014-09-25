using UnityEngine;
using System.Collections;

public class ScreenUtil
{
    public const int DEFAULT_WIDTH = 1920;
    public const int DEFAULT_HEIGHT = 1080;

    /**
     * This will return the specified pixels based on the 1920/1080 resolution.
     * 
     * Not totally optimized for resolutions other than 16:9 right now, but works
     * for the most part.
     */
    public static int getPixels(int pixels)
    {
        float newPixels;

        newPixels = pixels * ((float)Screen.width / (float)DEFAULT_WIDTH);

        return (int)newPixels;
    }

    //Overloaded for floats
    public static float getPixels(float pixels)
    {
        float newPixels;

        //Debug.Log(Screen.width+", "+DEFAULT_WIDTH);
        newPixels = pixels * ((float)Screen.width / (float)DEFAULT_WIDTH);

        return newPixels;
    }

    public static Rect scaleRect(Rect r)
    {
        r.x = getPixels(r.x);
        r.y = getPixels(r.y);
        r.width = getPixels(r.width);
        r.height = getPixels(r.height);
        return r;
    }
}
