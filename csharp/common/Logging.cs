using Godot;
using Godot.Collections;
using System.Collections.Generic;

public class Logging {

    public enum Level
    {
        ERROR,
        DEBUG
    }

    static public void Log(string text)
    {
        Log(text, Level.DEBUG);
    }

    /*
    static public void Log(Dictionary text)
    {
        foreach ( object key in text.Keys )
        {
            object val;
            text.TryGetValue(key, out val);
            Log("\tkey: " + key.ToString() + " - val: " + ( val == null ? "null" : val.ToString()), Level.DEBUG);
        }
    }
    */

    static public void Log(string text, Logging.Level level)
    {
        if ( level == Level.ERROR )
        {
            GD.PrintErr(text);


        } else
        {
            GD.Print(text);

        }
    }

    //static public void Log(string text, Node gameObject, Logging.Level level)
    //{
    //    ObjectName name = GetObjectName(gameObject);
    //    if (name != null)
    //    {
    //        text = name.CountedName() + ": " + text;
    //    }

    //    Log(text, level);
    //}

    //static public void Log(string text, GameObject gameObject)
    //{
    //    Log(text, gameObject, Level.DEBUG);
    //}

    //static public ObjectName GetObjectName(GameObject gameObject)
    //{
    //    ObjectName name = gameObject.GetComponent<ObjectName>();
    //    if (name == null)
    //    {
    //        Transform nameObjectTransform = gameObject.transform.Find("ObjectName");
    //        if (nameObjectTransform != null)
    //        {
    //            name = nameObjectTransform.gameObject.GetComponent<ObjectName>();
    //        }
    //    }

    //    return name;
    //}

    //static public string GetObjectNameString(GameObject gameObject)
    //{
    //    ObjectName name = GetObjectName(gameObject);
    //    if (name != null)
    //    {
    //        return name.CountedName();
    //    }

    //    return "<ObjectName: null>";
    //}
}
