//using RestreamChatHacking;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using static MessageKeyToJomiStroke;

//public class RestreamToJomiScriptableDemo : MonoBehaviour
//{
//    public UI_ServerDropdownJavaOMI m_server;
//    public string m_startWith = "#rj ";
//    public ExactTextToJomiScritable[] m_actionsPossible = new ExactTextToJomiScritable[1];
   

//    public void TryToTranslateAsKeyStroke(RestreamChatMessage message)
//    {
//        string msg = message.Message;
//        if (msg.ToLower().StartsWith(m_startWith))
//        {
//            string toTranslate = msg.Substring(0+ m_startWith.Length );
//            Debug.Log("Test:"+toTranslate);
//            for (int i = 0; i < m_actionsPossible.Length; i++)
//            {
//                if (toTranslate.Trim().ToLower() == m_actionsPossible[i].m_data.m_exactTextToTrigger.ToLower().Trim()) {
//                    foreach (var item in m_server.GetJavaOMISelected())
//                    {
//                        if (m_actionsPossible[i].m_data.m_sendType == SendType.Shortcut)
//                            item.SendShortcutCommands(m_actionsPossible[i].m_data.m_jomiRequest);
//                        if (m_actionsPossible[i].m_data.m_sendType == SendType.Command)
//                            item.SendRawCommand(m_actionsPossible[i].m_data.m_jomiRequest);
//                        if (m_actionsPossible[i].m_data.m_sendType == SendType.Text)
//                            item.PastText(m_actionsPossible[i].m_data.m_jomiRequest);
//                    }
//                }

//            }
            
//        }
//    }
//    //public void TryToTranslateAsRawJOMI(RestreamChatMessage message)
//    //{
//    //    string msg = message.Message;
//    //    if (msg.ToLower().StartsWith("#jomi "))
//    //    {
//    //        string cmd = msg.Substring(0, msg.Length - 6);
//    //        foreach (var item in m_server.GetJavaOMISelected())
//    //        {
//    //            item.SendRawCommand(cmd);
//    //        }
//    //    }
//    //}

//    // Update is called once per frame
//    void Update()
//    {
        
//    }
//}
