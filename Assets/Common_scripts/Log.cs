using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class Log {

    private static FieldInfo[] LogSettingsFields;
    private static FieldInfo DefaultField;

    public static void LinkFields()
    {
        LogSettingsFields = Links.LSettings.GetType().GetFields();
        DefaultField = Links.LSettings.GetType().GetField("Default");
    }

    public static void Notice(string ScriptName, object LogMessage, Object Context = null)
    {
        FieldInfo _f = FindField(ScriptName);
        if (_f == DefaultField) //Тип не найден, логируем по умолчанию
        {
            Debug.Log("Default logging: " + ScriptName + " : " + LogMessage.ToString(), Context);
        }
        else // Тип найден
        {
            if (bool.Parse(_f.GetValue(Links.LSettings).ToString()))
                Debug.Log("'" + ScriptName + "' : " + LogMessage.ToString(), Context);
        }
    }
    public static void Notice(string ScriptName, Resource Res)
    {
        Notice(ScriptName, Res.Name + " -- logging resource");
        Notice(ScriptName, Res.Type + " -- type number");
        Notice(ScriptName, Res.Material + " -- material");
        Notice(ScriptName, Res.MachineToCreate + " -- machine to create");
        if (Res.Details != null)
        {
            Notice(ScriptName, "Details");
            foreach (ResourceDetail _d in Res.Details)
                Notice(ScriptName, "Detail id: " + _d.Type + ", Detail count :" + _d.Count);
        }
        else
            Notice(ScriptName, "Recoucre has no details for creation");
        if (Res.SubTypes != null)
        {
            Notice(ScriptName, "SubTypes");
            foreach (ResourceSubType _s in Res.SubTypes)
                Notice(ScriptName, "Subtype id: " + _s.Type + ", Subtype material :" + _s.MaterialName);
        }
        else
            Notice(ScriptName, "Recoucre has no subtypes");
        Notice(ScriptName, Res.TimeToCreate + " -- time to create");
        Notice(ScriptName, Res.TimeToMine + " -- time to mine");
        Notice(ScriptName, Res.Volume + " -- volume");
    }
    public static void Notice(string ScriptName, BuildingRequirements Requirements)
    {
        Log.Notice(ScriptName, "Required resources to build " + Requirements.TypeName + " are:");
        foreach (ResourceEnumerator _e in Requirements.ResourceRequirements)
        {
            Log.Notice(ScriptName, _e.Resource.Name + " - " + _e.Count);
        }
    }
    
    public static void Warning(string ScriptName, object LogMessage, Object Context = null)
    {
        //FieldInfo _f = FindField(ScriptName);
        //if (_f == DefaultField) //Тип не найден, логируем по умолчанию
        //{
        //    if (bool.Parse(_f.GetValue(Links.LSettings).ToString()))
        //        Debug.LogWarning("Default logging: " + ScriptName + " : " + LogMessage.ToString(), Context);
        //}
        //else // Тип найден
        //{
            //if (bool.Parse(_f.GetValue(Links.LSettings).ToString()))
        Debug.LogWarningFormat(ScriptName + ": " + LogMessage.ToString(), Context);
        //}
    }

    public static bool CheckScriptLogging(string ScriptName)
    {
        FieldInfo _f = FindField(ScriptName);
        return bool.Parse(_f.GetValue(Links.LSettings).ToString());
    }

    private static FieldInfo FindField(string FieldName)
    {
        foreach (FieldInfo _f in LogSettingsFields)
        {
            if (_f.Name == FieldName)
                return _f;
        }
        return DefaultField;
    }
}