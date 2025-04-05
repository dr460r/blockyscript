using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;


public class SyntaxHighlighter : MonoBehaviour
{
    public TMP_InputField inputData;
    public TMP_InputField inputRich;

    [Header("Control")]
    public bool enableScrollFix;

    [Header("Color")]
    public string backgroundColor = "#4c4c54";
    public Color codeDefaultColor;
    public Color codeCommandColor;
    public Color codeSpecialColor1;
    public Color codeSpecialColor2;

    void Start()
    {
        //inputData.caretBlinkRate = 0;
        inputData.caretColor = Color.white;
        inputData.customCaretColor = true;
    }

    void Update()
    {
        //Debug.Log(inputData.selectionAnchorPosition + " -> " + inputData.selectionFocusPosition);
        OnCodeChange();

        // Ukoliko je ista selektovano
        if (enableScrollFix && inputData.selectionAnchorPosition != inputData.selectionFocusPosition) 
        {
            ScrollFix();
        }

    }

    public void OnCodeChange()
    {
        // Scrollbar
        inputRich.verticalScrollbar.value = inputData.verticalScrollbar.value;
        inputData.verticalScrollbar.size = inputData.verticalScrollbar.size;

        string code = inputData.text;
        inputRich.text = ApplyStyle(code);
    }

    public void ScrollFix()
    {
        // Resenje za problem da se ne detektuje scroll kad se selektuje text i pomera view, nije sinhronizovan update
        // Resenje:
        // U TMP Input Field skripti se nalazi metoda OnScroll koja jedina updatuje poziciju Scrollbar-a
        // a tada se updatuje i scroll pozicijia za Input Rich
        // nisam siguran kako tacno radi, ali radi
        // Zato se ovde kreira "prazan" PointerEventData objekat koji izgleda okida deo u OnScroll metodi koji setuje vrednost za scroll
        // Debug.Log("ScrollFix");
        PointerEventData pd = new PointerEventData(EventSystem.current);
        pd.scrollDelta = Vector2.zero;
        inputData.OnScroll(pd);
        inputRich.OnScroll(pd);
    }

    string ApplyStyle(string code)
    {
        if (code == "") return code;
        
        //ApplyColor(ref code, code, codeDefaultColor);

        ApplyColor(ref code, "move", codeCommandColor, sufix:":");
        ApplyColor(ref code, "pick", codeCommandColor, sufix:":");
        ApplyColor(ref code, "copy", codeCommandColor, sufix:":");
        ApplyColor(ref code, "drop", codeCommandColor, sufix:":");
        ApplyColor(ref code, "jump", codeCommandColor);
        ApplyColor(ref code, "@", codeSpecialColor1);
        ApplyColor(ref code, "!", codeSpecialColor2);
        // code = code.Replace("move:", "<color=red>move</color>:");
        // code = code.Replace("pick:", "<color=red>pick</color>:");
        // code = code.Replace("drop:", "<color=red>drop</color>:");
        // code = code.Replace("jump", "<color=red>jump</color>");
        // code = code.Replace("@", "<color=yellow>@</color>");
        
        return code;
    }

    void ApplyColor(ref string code, string target, Color color, string prefix = "", string sufix = "")
    {
        code = code.Replace(prefix + target + sufix, prefix + "<color=" + ToHex(color) + ">" + target + "</color>" + sufix);
    }

    string ToHex(Color color) 
    {
        return "#" + ColorUtility.ToHtmlStringRGB(color).ToLower();
    }
}
