using UnityEngine;
using KeepCoding;
using NotKanji;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json.Linq;

public class NotKanjiModule : ModuleScript
{
    [SerializeField]
    GameObject Underline;
    [SerializeField]
    KMSelectable[] Keys;
    [SerializeField]
    TextMesh[] KeysText;
    [SerializeField]
    TextMesh ScreenText;

    Encoder encoder;
    List<int> input;

    bool isStrikeAnimation = false;

    private void Start()
    {
        var gameInfo = Get<KMBombInfo>();
        input = new List<int>();

        encoder = new Encoder(JObject.Parse(gameInfo.QueryWidgets(KMBombInfo.QUERYKEY_GET_SERIAL_NUMBER, null).First())["serial"].ToString(), this);

        ScreenText.text = encoder.Encode();

        for (int i = 0; i < 4; i++)
        {
            int j = i;
            KeysText[i].text = encoder.Kanjis[i].Kanji;
            Keys[j].OnInteract += () =>
            {
                KeyInteractHandler(j);
                return false;
            };
        }

    }

    private void KeyInteractHandler(int key)
    {
        if (isStrikeAnimation || input.Contains(key) || IsSolved) return;

        Keys[key].AddInteractionPunch(.2f);
        Keys[key].transform.localPosition += Vector3.down * 0.004f;

        input.Add(key);
        if(input.Count >= 4)
        {
            if(encoder.Check(input))
            {
                HandleSolve();
                return;
            }
            HandleStrike();
        } else
        {
            PlaySound("TapSound");
        }
        return;
    }

    private void ResetModule()
    {
        input = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            var position = Keys[i].transform.localPosition;
            position.y = 0.0134f;
            Keys[i].transform.localPosition = position;
        }

    }
    private void HandleStrike()
    {
        isStrikeAnimation = true;
        StartCoroutine(StrikeAnimation());
    }

    private IEnumerator StrikeAnimation()
    {
        PlaySound("StrikeSound");
        ScreenText.color = redText;
        KeysText[0].color = redText;
        yield return new WaitForSeconds(0.60f);
        KeysText[1].color = redText;
        yield return new WaitForSeconds(0.2f);
        KeysText[2].color = redText;
        yield return new WaitForSeconds(0.47f);
        KeysText[3].color = redText;
        yield return new WaitForSeconds(0.70f);
        KeysText[0].color = Color.white;
        yield return new WaitForSeconds(0.3f);
        KeysText[1].color = Color.white;
        yield return new WaitForSeconds(0.3f);
        KeysText[2].color = Color.white;
        yield return new WaitForSeconds(0.3f);
        KeysText[3].color = Color.white;
        yield return new WaitForSeconds(0.2f);
        ScreenText.color = Color.black;
        Strike($"Received {string.Join("", input.Select(i => i.ToString()).ToArray())}, expected {string.Join("", encoder.Answers.Select(i => i.ToString()).ToArray())}");
        isStrikeAnimation = false;
        ResetModule();
        yield return null;
    }

    private void HandleSolve()
    {
        StartCoroutine(SolveAnimation());
        Solve("Correct answer. Module solved!");
    }

    IEnumerator SolveAnimation()
    {
        PlaySound("SolveSound");
        ScreenText.color = greenText;
        KeysText[0].color = greenText;
        yield return new WaitForSeconds(0.25f);
        ScreenText.color = Color.black;
        KeysText[1].color = greenText;
        yield return new WaitForSeconds(0.25f);
        ScreenText.color = greenText;
        KeysText[2].color = greenText;
        yield return new WaitForSeconds(0.25f);
        ScreenText.color = Color.black;
        KeysText[3].color = greenText;
        yield return new WaitForSeconds(0.25f);
        ScreenText.color = greenText;
        KeysText[0].color = Color.white;
        yield return new WaitForSeconds(0.25f);
        ScreenText.color = Color.black;
        KeysText[1].color = Color.white;
        yield return new WaitForSeconds(0.25f);
        ScreenText.color = greenText;
        KeysText[2].color = Color.white;
        yield return new WaitForSeconds(0.25f);
        ScreenText.color = Color.black;
        KeysText[3].color = Color.white;
        yield return new WaitForSeconds(0.25f);
        ScreenText.color = greenText;
        KeysText[0].color = greenText;
        yield return new WaitForSeconds(0.25f);
        ScreenText.color = Color.black;
        KeysText[1].color = greenText;
        yield return new WaitForSeconds(0.25f);
        ScreenText.color = greenText;
        KeysText[2].color = greenText;
        yield return new WaitForSeconds(0.25f);
        ScreenText.color = Color.black;
        KeysText[3].color = greenText;
        yield return new WaitForSeconds(0.25f);
        ScreenText.color = greenText;
        int m1 = 0;
        int m2 = 0;
        int m3 = 0;
        int m4 = 0;
        while (m1 != 30)
        {
            yield return new WaitForSeconds(0.015f);
            Keys[0].transform.localPosition = Keys[0].transform.localPosition + Vector3.up * -0.00021f;
            m1++;
        }
        while (m2 != 30)
        {
            yield return new WaitForSeconds(0.015f);
            Keys[1].transform.localPosition = Keys[1].transform.localPosition + Vector3.up * -0.00021f;
            m2++;
        }
        while (m3 != 30)
        {
            yield return new WaitForSeconds(0.015f);
            Keys[2].transform.localPosition = Keys[2].transform.localPosition + Vector3.up * -0.00021f;
            m3++;
        }
        while (m4 != 30)
        {
            yield return new WaitForSeconds(0.015f);
            Keys[3].transform.localPosition = Keys[3].transform.localPosition + Vector3.up * -0.00021f;
            m4++;
        }
        ScreenText.text = "";
        Underline.transform.localPosition = Underline.transform.localPosition + Vector3.back * -500f;
        GetComponent<KMBombModule>().HandlePass();
        yield return null;
    }

    static Color greenText = new Color(0.15f, 1f, 0f);
    static Color redText = new Color(1f, 0.15f, 0);
}