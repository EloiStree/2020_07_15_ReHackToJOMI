using RestreamChatHacking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static MessageKeyToJomiStroke;

public class RestreamToJomiKindaCSVDemo : MonoBehaviour
{

    public TextAsset m_csvInput;

    public UI_ServerDropdownJavaOMI m_server;
    public string m_startWith = "#rj ";
    public string m_antiSpam = "aaa";
    public int m_descriptionIndex = 0;
    public int m_shortTriggerIndex = 2;
    public int m_longTriggerIndex = 3;
    public int m_jomiCommandIndex = 4;
    public int m_jomiCommandStyleIndex = 5;
    public char m_spliter = '胞';
    public List<MessageKeyToJomiStroke> m_actionsPossible = new List<MessageKeyToJomiStroke>();

    public StringCommandFound m_executed;
    public StringCommandFound m_failToExecute;


    public void Start()
    {
        LoadCSV();
    }

    private void LoadCSV()
    {
        m_actionsPossible.Clear();
        if (m_csvInput != null)
        {
            string txt = m_csvInput.text;
            string[] lines = txt.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                MessageKeyToJomiStroke input = new MessageKeyToJomiStroke();
                string[] tokens = lines[i].Split(m_spliter);
                if (tokens.Length > m_descriptionIndex)
                {
                    input.m_actionLabel = tokens[m_descriptionIndex];
                }
                if (tokens.Length > m_shortTriggerIndex)
                {
                    input.m_exactTextToShortTrigger = tokens[m_shortTriggerIndex];
                }
                if (tokens.Length > m_longTriggerIndex)
                {
                    input.m_exactTextToLongTrigger = tokens[m_longTriggerIndex];
                }
                if (tokens.Length > m_jomiCommandIndex)
                {
                    input.m_jomiRequest = tokens[m_jomiCommandIndex];
                }
                if (tokens.Length > m_jomiCommandStyleIndex)
                {
                    string styleTxt = tokens[m_jomiCommandStyleIndex].ToLower();
                    switch (styleTxt)
                    {
                        case "cmd":
                        case "command":
                            input.m_sendType = MessageKeyToJomiStroke.SendType.Command;
                            break;
                        case "past":
                        case "pasttext":
                            input.m_sendType = MessageKeyToJomiStroke.SendType.Text;
                            break;
                        default:
                            input.m_sendType = MessageKeyToJomiStroke.SendType.Shortcut;
                            break;
                    }
                }
                if(input.SeemValide())
                m_actionsPossible.Add(input);
            }
        
        }
    }
    public string m_lastReceived;
    public void TryToTranslateAsKeyStroke(RestreamChatMessage message)
    {
        bool executed=false;
        string msg = message.Message;
        if (msg.ToLower().StartsWith(m_startWith))
        {
            string toTranslate = msg.Substring(0 + m_startWith.Length);
            int antispam = toTranslate.LastIndexOf(m_antiSpam);
            if (antispam > -1)
            {
                toTranslate = toTranslate.Substring(0, antispam);
            }
            m_lastReceived = toTranslate;
            for (int i = 0; i < m_actionsPossible.Count; i++)
            {
                if (toTranslate.Trim().ToLower()
                    == m_actionsPossible[i].m_exactTextToShortTrigger.ToLower().Trim()
                    || toTranslate.Trim().ToLower()
                    == m_actionsPossible[i].m_exactTextToLongTrigger.ToLower().Trim())
                {
                    executed = true;
                    foreach (var item in m_server.GetJavaOMISelected())
                    {
                        if (m_actionsPossible[i].m_sendType == SendType.Shortcut)
                            item.SendShortcutCommands(m_actionsPossible[i].m_jomiRequest);
                        if (m_actionsPossible[i].m_sendType == SendType.Command)
                            item.SendRawCommand(m_actionsPossible[i].m_jomiRequest);
                        if (m_actionsPossible[i].m_sendType == SendType.Text)
                            item.PastText(m_actionsPossible[i].m_jomiRequest);
                    }
                }

            }

            if (executed)
                m_executed.Invoke(toTranslate);
            else
                m_failToExecute.Invoke(toTranslate);
        }
    }
}
[System.Serializable]
public class StringCommandFound : UnityEvent<string>
{

}