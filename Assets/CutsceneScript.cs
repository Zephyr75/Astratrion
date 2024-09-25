using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CutsceneScript : MonoBehaviour
{
    public Transform chain, zephyr, sky;
    public GameObject place, village, lake, swordLake, swordSplash, king, queen, witch;
    private GameObject[] triggers;
    public RuntimeAnimatorController peopleIdle, womenTalk, menTalk;
    public string triggerName;
    public int waitTime;
    private List<string> dialogues = new List<string>();
    public TextMeshProUGUI dialoguesText;
    private float t = 0f;
    private bool changingTime = false, inCutscene = false;
    private Color actualColor, newColor;
    private float[,] colors;
    private int i = 0;
    public GameObject fight;

    private void Awake()
    {
        triggers = new GameObject[transform.childCount];
        int i = 0;
        foreach (Collider trigger in transform.GetComponentsInChildren<Collider>())
        {
            triggers[i] = trigger.gameObject;
            i++;
        }
        //triggers[1].SetActive(false);
        //triggers[2].SetActive(false);
        //triggers[3].SetActive(false);
        //triggers[4].SetActive(false);
        //triggers[5].SetActive(false);
        colors  = new float[,] { { 0.2392157f, 0.772549f, 0.8823529f }, { 0.1922392f, 0.2287884f, 0.3018868f } };
        actualColor = new Color(colors[0,0], colors[0, 1], colors[0, 2], 1);
        RenderSettings.skybox.SetColor("_EmissionColor", actualColor);
    }

    private void FixedUpdate()
    {
        foreach (Collider trigger in transform.GetComponentsInChildren<Collider>())
        {
            //Debug.Log(trigger.gameObject.name);
            if (trigger.GetComponent<TriggerScript>().isActive)
            {
                triggerName = trigger.name;
                waitTime = trigger.GetComponent<TriggerScript>().cutsceneTime;
            }
        }

        if (changingTime)
        {
            SetSkybox();
        }

        if (!inCutscene)
        {
            switch (triggerName)
            {
                case "TriggerReynal": AddDialogues("[E] Parler"); break;
                case "TriggerReynal2": AddDialogues("[E] Parler"); break;
                case "TriggerReynal3": AddDialogues("[E] Parler"); break;
                case "TriggerRope": AddDialogues("[E] Activer"); break;
                case "TriggerClimb": AddDialogues("[E] Grimper"); break;
                case "TriggerPancarte": AddDialogues("[E] Lire"); break;
                case "TriggerKing": StartCoroutine(PlayMusic(fight, 5, 114)); break;
            }
            if (triggerName != "" && Input.GetKey("e"))
            {
                inCutscene = true;
                PlayCutscene(triggerName);
                zephyr.GetComponent<Zeph>().PlayCutscene(triggerName, waitTime);
                zephyr.GetComponent<MainCamera>().PlayCutscene(triggerName, waitTime);
                StartCoroutine(ActivateObject(waitTime, triggers[i], false));
                StartCoroutine(ActivateObject(waitTime, triggers[i + 1], true));
                StartCoroutine(SetInCutscene(waitTime));
            }
        }
    }

    private IEnumerator SetInCutscene(float time)
    {
        yield return new WaitForSeconds(time);
        inCutscene = false;
        triggerName = "";
        i++;
    }

    private void PlayCutscene(string triggerName)
    {
        switch (triggerName)
        {
            case "TriggerReynal":
                StartCoroutine(ActivateObject(30f, place, true)); StartCoroutine(ActivateObject(30f, village, false));
                AddDialogues("Bienvenue en Astratrion Terriens!",
                "Nous vous attendions depuis des années.", "Il est temps de mettre fin au règne de Kaon.",
                "Laissez-nous organiser un grand banquet en votre honneur.", "Ce sera l'occasion pour vous de récupérer l'arme des maîtres de l'air.");
                StartCoroutine(ActivateSkybox(18f)); newColor = new Color(colors[1, 0], colors[1, 1], colors[1, 2], 1);
                StartCoroutine(ChangeController(0f, queen, peopleIdle));
                break;
            case "TriggerReynal2":
                AddDialogues("Profitez de la fête, vous êtes nos invités ce soir!",
                "Puis rendez-vous au bord du lac pour recevoir l'épée de légende.");
                StartCoroutine(ActivateObject(waitTime, lake, true));
                break;
            case "TriggerReynal3":
                AddDialogues("Il est temps pour toi, Zéphyr, descendant des maîtres de l'air", "De recevoir l'arme légendaire de tes aïeux", " ",
                "Ô toi, fier enfant, \n Qui contrôle les Vents", "Viens nous libérer \n De ta sainte Épée.",
                "Oh toi, Fils des Cieux \n Oh toi, Élu des Dieux", "Fais virevolter ta lame \n Pour dissiper les flammes.");
                StartCoroutine(ChangeController(6f, king, peopleIdle));
                StartCoroutine(ChangeController(0f, witch, peopleIdle));
                StartCoroutine(ChangeController(6f, witch, womenTalk));
                StartCoroutine(ChangeController(18f, witch, peopleIdle));
                StartCoroutine(EnableAnimator(18f, swordLake, true));
                StartCoroutine(ActivateObject(21f, swordSplash, true));
                StartCoroutine(ActivateObject(28f, swordLake, false));
                StartCoroutine(ChangeController(45f, king, menTalk));
                StartCoroutine(ChangeController(45f, witch, womenTalk));
                break;
            case "TriggerRope": Rope();
                break;
            case "TriggerPancarte": AddDialogues("< Eolie \n Reynal > \n < Lysael");
                break;

        }
    }

    private void AddDialogues(params string[] lines)
    {
        dialogues = new List<string>();
        for (int i = 0; i < lines.Length; i++)
        {
            dialogues.Add(lines[i]);
        }
        StartCoroutine(ReadDialogue());
    }

    private IEnumerator ReadDialogue()
    {
        for (int i = 0; i < dialogues.Count; i++)
        {
            dialoguesText.text = dialogues[i];
            yield return new WaitForSeconds(3f);
        }
        dialoguesText.text = "";
    }

    private IEnumerator ActivateObject(float time, GameObject obj, bool value)
    {
        yield return new WaitForSeconds(time);
        obj.SetActive(value);
    }

    private IEnumerator EnableAnimator(float time, GameObject obj, bool value)
    {
        yield return new WaitForSeconds(time);
        obj.GetComponent<Animator>().enabled = value;
    }

    private IEnumerator ChangeController(float time, GameObject obj, RuntimeAnimatorController controller)
    {
        yield return new WaitForSeconds(time);
        obj.GetComponent<Animator>().runtimeAnimatorController = controller;
    }

    private IEnumerator PlayMusic(GameObject music, float start, float wait)
    {
        yield return new WaitForSeconds(start);
        music.SetActive(true);
        yield return new WaitForSeconds(wait);
        music.SetActive(false);
    }

    private IEnumerator ActivateSkybox(float time)
    {
        yield return new WaitForSeconds(time);
        changingTime = true;
    }

    private void SetSkybox()
    {

        t += 0.05f * Time.deltaTime;
        RenderSettings.skybox.SetColor("_EmissionColor", Color.Lerp(actualColor, newColor, t));
        if (t > 1f)
        {
            t = 0f;
            changingTime = false;
            actualColor = newColor;
            for (int i = 0; i < sky.childCount; i++)
            {
                StartCoroutine(ActivateObject(Random.Range(1, 20), sky.GetChild(i).gameObject, true));
            }
        }
    }

    private void Rope()
    {
        float time = 17f;
        for (int i = chain.childCount - 1; i >= 0; i--)
        {
            StartCoroutine(ActivateObject(time, chain.GetChild(i).gameObject, true));
            time += .1f;
        }
    }
    
}