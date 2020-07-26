using JavaOpenMacroInput;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class FTL_CommandToJOMIActions : MonoBehaviour
{

    public UI_ServerDropdownJavaOMI m_target;
    public string m_lastReceived;
    public string m_lastValide;
    public void TryToExecuteFollowingCommand(string cmd) {
        m_lastReceived = cmd;
        //Move Crew to screen position
        if (WasQuickCLick(cmd)){ m_lastValide = m_lastReceived; }
        else if (WasItCrewMove(cmd)) {
            m_lastValide = m_lastReceived; }
        else if (WasMultiClickingTo(cmd)) { m_lastValide = m_lastReceived; }
        // c1 goto 50:50
        // c1 goto oxygen
        // set position oxygen 50:50

        //Attack the zone
        // w1 attack 70:20
        else if (WasItWeaponAttack(cmd)) { m_lastValide = m_lastReceived; }
        else if (WasItUpgrade(cmd)) { m_lastValide = m_lastReceived; }

        //Mouse click
        // clickto 70:20
        else if(WasMouseClickTo(cmd)) { m_lastValide = m_lastReceived; }
        // moveto 50:
        else if (WasMouseMoveTo(cmd)) { m_lastValide = m_lastReceived; }
    }

    private bool WasQuickCLick(string cmd)
    {
        Regex r = new Regex("^\\d\\d:\\d\\d$");
        cmd = cmd.Trim();
        if (r.IsMatch(cmd))
        {
            try { 
                string[] tokens = cmd.Split(':');
                float l2r, t2d;
                ConvertIntToPourcent(tokens[0], tokens[1], out l2r, out t2d);
                ClickTo(l2r, t2d);
            }
            catch (Exception e)
            {
                Debug.LogWarning("ERROR Parsing: " + e.StackTrace);
                return false;
            }
            return true;

        }
        else return false;
    }

    private bool WasItUpgrade(string cmd)
    {
        Debug.Log("CMD:" + cmd);
        bool found = false;

        cmd = cmd.ToLower().Trim();
        Debug.Log("CMD:" + cmd);
        if ("upgrade"==cmd)
        {
            Debug.Log("T:" + cmd);
            PressKey(JavaOpenMacroInput.JavaKeyEvent.VK_U);
            found = true;
        }
        else if (cmd.IndexOf("upgrade close") == 0
            || cmd.IndexOf("upgrade accept") == 0)
        {
            ClickTo(0.65f, 0.8f, true, false);
            found = true;
        }
        else if (cmd.IndexOf("upgrade undo") == 0)
        {
            ClickTo(0.3f, 0.8f, true, false);
            found = true;
        }
        else if (cmd.IndexOf("upgrade ") == 0)
        {

            string type = cmd.Substring("upgrade ".Length).Trim();
            if (type == "piloting")
                ClickTo(0.3f, 0.65f, true, false);
            else if (type == "door")
                ClickTo(0.4f, 0.65f, true, false);
            else if (type == "battery")
                ClickTo(0.45f, 0.65f, true, false);
            else if (type == "sensor" || type == "camera")
                ClickTo(0.35f, 0.65f, true, false);
            else if (type == "reactor")
                ClickTo(0.6f, 0.65f, true, false);
            else {
                int value;
                if (int.TryParse(type, out value))
                {
                    if (value == 1) ClickTo(0.31f, 0.35f, true, false);
                    if (value == 2) ClickTo(0.37f, 0.35f, true, false);
                    if (value == 3) ClickTo(0.42f, 0.35f, true, false);
                    if (value == 4) ClickTo(0.48f, 0.35f, true, false);
                    if (value == 5) ClickTo(0.52f, 0.35f, true, false);
                    if (value == 6) ClickTo(0.58f, 0.35f, true, false);
                    if (value == 7) ClickTo(0.63f, 0.35f, true, false);
                    if (value == 8) ClickTo(0.67f, 0.35f, true, false);
                }
            }
            found = true;
        }
        return found;
    }

    private void PressKey(JavaKeyEvent key)
    {
        Debug.Log("D:" + key);
        foreach (var item in m_target.GetJavaOMISelected())
        {
            item.Keyboard(key);
        }
    }

    public enum UpgradePossible { Shield, Engine, Heal, Oxygen, Weapon, Piloting,Sensor, Door,Battery }

    private bool WasMultiClickingTo(string cmd)
    {  
        //Debug.Log("-> A");
        // c1 goto 50:50
        bool found = false;
        try
        {
            string value;
            float l2r, t2d;
            if(ExtractValueAndPositionForKeyboard(cmd, " multiclick ", out value, out l2r, out t2d))
            {
                MultiClickOctagone( l2r, t2d);
                found = true;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("ERROR Parsing: " + e.StackTrace);
        }
        return found;
    }

    private bool WasItCrewMove(string cmd)
    {
        //Debug.Log("-> A");
        // c1 goto 50:50
        bool found = false;
        try {
            string value;
            float l2r, t2d; 
            Debug.Log("-> A");
            if (ExtractValueAndPositionForKeyboard(cmd, "goto ", out value, out l2r, out t2d)) {
                value = value.Trim();
                if (value.Length > 0)
                {
                    char crewIndexAsChar = value[value.Length - 1];
                    Debug.Log("-> B:"+crewIndexAsChar+" "+ cmd);
                    MoveCrew("" + crewIndexAsChar, l2r, t2d);
                    found = true;
                }
                else { 
                    MoveAllCrew(l2r, t2d);
                    found = true;
                }
            }

        } catch (Exception e) {
            Debug.LogWarning("ERROR Parsing: " + e.StackTrace);
        }
        return found;
    }

    private bool ExtractValueAndPositionForKeyboard(string cmd ,string actionSpliter, out string value, out float l2r, out float t2d)
    {
        value = "";
        t2d = l2r = 0;

        cmd = cmd.Trim().ToLower();
        int indexOfGoTo = cmd.IndexOf(actionSpliter);
        if (indexOfGoTo < 0)
            return false;
        cmd = cmd.Replace(actionSpliter, ":");

        string[] tokens = cmd.Split(':');
        if (tokens.Length != 3) return false;
        tokens[0] = tokens[0].Trim();
        tokens[1] = tokens[1].Trim();
        tokens[2] = tokens[2].Trim();
        value = tokens[0];
        if (tokens[1].Length == 0 || tokens[2].Length == 0)
            return false;

        ConvertIntToPourcent(tokens[1], tokens[2], out l2r , out t2d);
 
        return  true;
    }

    public void MoveAllCrew(float l2rpct, float t2dpct) {

        for (int i = 1; i < 9; i++)
        {
            MoveCrew(""+i, l2rpct, t2dpct);
        }
    }

    private void MoveCrew(string index, float l2rpct, float t2dpct)
    {

        SelectCrew(index);
        ClickTo(l2rpct, t2dpct, false, true);
        ClickTo(l2rpct, t2dpct, true, false);
    }

    private void Attack(int weaponIndex, float l2rpct, float t2dpct)
    {
        SelectWeapon(weaponIndex);
        ClickTo(l2rpct, t2dpct, true, false);
    }
    private void FullAttack( float l2rpct, float t2dpct)
    {
        for (int i = 1; i < 8; i++)
        {
            Attack(i, l2rpct, t2dpct);
        }
    }

    private void SelectWeapon(int weaponIndex)
    {
        if (weaponIndex > 0 && weaponIndex < 7) { 
            JavaOpenMacroInput.JavaKeyEvent key = JavaOpenMacroInput.JavaKeyEvent.VK_1;
            switch (weaponIndex)
            {
                case 1: key = JavaOpenMacroInput.JavaKeyEvent.VK_1; break;
                case 2: key = JavaOpenMacroInput.JavaKeyEvent.VK_2; break;
                case 3: key = JavaOpenMacroInput.JavaKeyEvent.VK_3; break;
                case 4: key = JavaOpenMacroInput.JavaKeyEvent.VK_4; break;
                case 5: key = JavaOpenMacroInput.JavaKeyEvent.VK_5; break;
                case 6: key = JavaOpenMacroInput.JavaKeyEvent.VK_6; break;
                case 7: key = JavaOpenMacroInput.JavaKeyEvent.VK_7; break;
                case 8: key = JavaOpenMacroInput.JavaKeyEvent.VK_8; break;
                default:
                    break;
            }
            foreach (var item in m_target.GetJavaOMISelected())
            {
                item.Keyboard(key);
            }
        }
    }

    public void MultiClickOctagone(float  l2r, float t2d, float toAddPct = 0.005f, int wave=2) {

        ClickTo(l2r, t2d, true, false);
        for (int i = 0; i < wave; i++)
        {
            ClickTo(l2r + toAddPct * wave, t2d, true, false);
            ClickTo(l2r - toAddPct * wave, t2d, true, false);
            ClickTo(l2r, t2d + toAddPct * wave, true, false);
            ClickTo(l2r, t2d - toAddPct * wave, true, false);

            ClickTo(l2r + toAddPct * wave, t2d + toAddPct * wave, true, false);
            ClickTo(l2r - toAddPct * wave, t2d - toAddPct * wave, true, false);
            ClickTo(l2r - toAddPct * wave, t2d + toAddPct * wave, true, false);
            ClickTo(l2r + toAddPct * wave, t2d - toAddPct * wave, true, false);

        }
    }


    private bool WasItWeaponAttack(string cmd)
    { //Debug.Log("-> A");
        // c1 goto 50:50
        bool found = false;
        try
        {
            string value;
            float l2r, t2d;
            if (ExtractValueAndPositionForKeyboard(cmd, "attack ", out value, out l2r, out t2d))
            {
                if (value.Length > 0)
                {
                    char crewIndexAsChar = value[value.Length - 1];
                    int l = 0;
                    if(int.TryParse(""+crewIndexAsChar, out l))
                    {
                        Attack(l, l2r, t2d);
                    }
                }
                else
                {
                    FullAttack(l2r, t2d);
                }
                found = true;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("ERROR Parsing: " + e.StackTrace);
        }
        return found;
    }

    private bool WasMouseMoveTo(string cmd)
    { //Debug.Log("-> A");
        // c1 goto 50:50
        bool found = false;
        try
        {
            string value;
            float l2r, t2d;
            if (ExtractValueAndPositionForKeyboard(cmd, "move ", out value, out l2r, out t2d))
            {
                MoveTo(l2r, t2d);
                found = true;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("ERROR Parsing: " + e.StackTrace);
        }
        return found;
    }

    private bool WasMouseClickTo(string cmd)
    { //Debug.Log("-> A");
        // c1 goto 50:50
        bool found = false;
        try
        {
            string value;
            float l2r, t2d;
            if (ExtractValueAndPositionForKeyboard(cmd, "click ", out value, out l2r, out t2d))
            {
                ClickTo(l2r, t2d);
                found = true;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("ERROR Parsing: " + e.StackTrace);
        }
        return found;
    }


    
    public void SelectCrew(string index)
    {
        int value;
        if (int.TryParse(index, out value))
            SelectCrew(value);
    
    }
        public void SelectCrew(int index) {
        if (index < 1 || index > 8) 
            return;
        JavaOpenMacroInput.JavaKeyEvent keyToPress = JavaOpenMacroInput.JavaKeyEvent.VK_F1;
        switch (index)
        {
            case 1: keyToPress = JavaOpenMacroInput.JavaKeyEvent.VK_F1; break;
            case 2: keyToPress = JavaOpenMacroInput.JavaKeyEvent.VK_F2; break;
            case 3: keyToPress = JavaOpenMacroInput.JavaKeyEvent.VK_F3; break;
            case 4: keyToPress = JavaOpenMacroInput.JavaKeyEvent.VK_F4; break;
            case 5: keyToPress = JavaOpenMacroInput.JavaKeyEvent.VK_F5; break;
            case 6: keyToPress = JavaOpenMacroInput.JavaKeyEvent.VK_F6; break;
            case 7: keyToPress = JavaOpenMacroInput.JavaKeyEvent.VK_F7; break;
            case 8: keyToPress = JavaOpenMacroInput.JavaKeyEvent.VK_F8; break;
            default:
                break;
        }
        foreach (var item in m_target.GetJavaOMISelected())
        {
            item.Keyboard(keyToPress);
        }

    }
    public void ClickTo(string pourcentInIntLeftToRight, string pctInIntTopToBot, bool left = true, bool right = true)
    {

        MoveTo(pourcentInIntLeftToRight, pctInIntTopToBot);
        Click(left, right);

    }
    public void ClickTo(float pourcentInIntLeftToRight, float pctInIntTopToBot, bool left = true, bool right = true)
    {

        MoveTo(pourcentInIntLeftToRight, pctInIntTopToBot);
        Click(left, right);

    }
    public void Click( bool left=true, bool right=true)
    {
        foreach (var item in m_target.GetJavaOMISelected())
        {
            if(left)
                item.MouseClick(JavaOpenMacroInput.JavaMouseButton.BUTTON1_DOWN_MASK);
            if (right)
                item.MouseClick(JavaOpenMacroInput.JavaMouseButton.BUTTON3_DOWN_MASK);
        }
    }
    public void MoveTo(string pourcentInIntLeftToRight, string pctInIntTopToBot)
    {
        float l2r, t2b;
        ConvertIntToPourcent(pourcentInIntLeftToRight, pctInIntTopToBot, out l2r, out t2b);
        MoveTo(l2r, t2b);

    }
    public void MoveTo(float pourcentInIntLeftToRight, float pctInIntTopToBot)
    {
       
        foreach (var item in m_target.GetJavaOMISelected())
        {
            item.MouseMoveInPourcent( pourcentInIntLeftToRight,  pctInIntTopToBot);
        }
    }
    public void ConvertIntToPourcent(string leftToRight, string topToBot, out float leftToRightPct, out float topToBotPct)
    {
        leftToRightPct = 0f;
        topToBotPct=0f;
        int l2r, t2b;
        if (int.TryParse(leftToRight, out l2r) && int.TryParse(topToBot, out t2b))
            ConvertIntToPourcent(l2r, t2b, out leftToRightPct, out topToBotPct);


    }
    public void ConvertIntToPourcent(int leftToRight, int topToBot, out float leftToRightPct, out float topToBotPct)
    {
        leftToRightPct = 0;
        topToBotPct =0;
         if (leftToRight < 99)
            leftToRightPct  = (float)leftToRight / 100f;
        else if (leftToRight < 999)
            leftToRightPct  = (float)leftToRight / 1000f;
        else if (leftToRight < 9999)
            leftToRightPct  = (float)leftToRight / 10000f;

         if (topToBot < 99)
            topToBotPct  = (float)topToBot / 100f;
        else if (topToBot < 999)
            topToBotPct  = (float)topToBot / 1000f;
        else if (topToBot < 9999)
            topToBotPct  = (float)topToBot / 10000f;
      }
}
