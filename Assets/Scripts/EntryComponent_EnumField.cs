﻿using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[Serializable]
public class EntryComponent_EnumField : EntryComponent_SelectTypeBase
{
    private static readonly string[] EnumTypeName = new string[]{ "Popup", "Flags" };

    [SerializeField] private long _valueAsLong;
    [SerializeField] bool  _isFlags;

    public override object Value
    {
        get
        {
            if(SelectedType != null)
                return (Enum)  Enum.Parse(SelectedType, _valueAsLong.ToString());
            return null;

        }
        set
        {
            _valueAsLong = Convert.ToInt64((Enum)value);
              
        }
    }


    
    public override List<Type> GetAvailableTypes()
    {
        return (from t in AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
            where (t.IsPublic || t.IsNestedPublic) && !(t.IsAbstract && t.IsSealed) && !t.IsGenericType && t.IsEnum && !t.IsClass
            select t).ToList();
    }

    public override object Clone()
    {
        var item = base.Clone() as EntryComponent_EnumField;
        item._valueAsLong = _valueAsLong;
        item._isFlags = _isFlags;
        return item;

    }

#if UNITY_EDITOR
    
    protected override void DrawObjectField(ref Rect pos)
    {
        if (SelectedType != null)
        {
            if(Value == null|| SelectedType != Value.GetType())
                Value = Activator.CreateInstance(SelectedType);
        }

        if (!_isFlags)
        {
            bool found = false;
            foreach (var value in Enum.GetValues(SelectedType))
                if (value.Equals(Value))
                {
                    found = true;
                    break;
                }
            if (!found)
                Value = Activator.CreateInstance(SelectedType);

            Value = EditorGUI.EnumPopup(pos, FieldName, (Enum)Value);
        }
        else  Value = EditorGUI.EnumFlagsField(pos, FieldName, (Enum) Value);
    }

    protected override void DrawEdit(ref Rect pos)
    {
        base.DrawEdit(ref pos);

        if (SelectedType != null)
        {
            pos.height = SingleLineHeight;
            _isFlags = EditorGUI.Popup(pos, "Field View Type", _isFlags ? 1 : 0, EnumTypeName) == 1;
            pos.y += pos.height;
        }
    }

    public override float GetPropertyHeight()
    {
        return base.GetPropertyHeight() + (SelectedType!=null? SingleLineHeight:0) ;
    }
#endif


}