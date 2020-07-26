using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ReHackToJomi", menuName = "ReHack/ExactText 2 JOMI", order = 1)]
public class ExactTextToJomiScritable : ScriptableObject
{
    public MessageKeyToJomiStroke m_data;
}
[System.Serializable]
public class MessageKeyToJomiStroke {

    [Header("What it is suppose to do")]
    public string m_actionLabel = "Unnamedaction";
    [Header("Looking for")]
    public string m_exactTextToShortTrigger = "A";
    public string m_exactTextToLongTrigger = "Press A";
    [Header("To Replace by")]
    public string m_jomiRequest = "Shift↓ A↕ Shift↑";
    public SendType m_sendType = SendType.Shortcut;
    public enum SendType
    { Command, Shortcut, Text }

    public bool SeemValide()
    {
        return
            (OneTriggerIsNotNull()
              ) &&
              HasRequestForJomi();
    }

    public bool HasRequestForJomi()
    {
        return !string.IsNullOrEmpty(m_jomiRequest);
    }

    public bool OneTriggerIsNotNull()
    {
        return !string.IsNullOrEmpty(m_exactTextToLongTrigger) ||
                      !string.IsNullOrEmpty(m_exactTextToShortTrigger);
    }
}