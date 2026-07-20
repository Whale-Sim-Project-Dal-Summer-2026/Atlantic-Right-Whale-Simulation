using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CyclingText : MonoBehaviour {
    // refs
    public TextAsset funFactsFile;
    private Button funFactsBtn;
    private TextMeshProUGUI funFactsTxt;
    
    // vars
    private String[] funFacts;
    private int currFactInd;
    public float repeatRate;
    
    void Awake() {
        // get refs
        funFactsTxt = GetComponent<TextMeshProUGUI>();
        funFactsBtn = GetComponent<Button>();
    }
    
    void Start () {
        // load facts
        LoadFunFacts();
        // dispatch update events
        InvokeRepeating(nameof(UpdateFunFact), 0.01f, repeatRate);
        if (funFactsBtn) {
            funFactsBtn.onClick.AddListener(UpdateFunFact);
        }
    }

    private void LoadFunFacts() {
        funFacts = funFactsFile.text.Split('\n');
    }

    private void UpdateFunFact() {
        int newFactInd = 0;
        while (funFacts.Length > 1 && newFactInd == currFactInd) {
            newFactInd = Random.Range(0, funFacts.Length); // TODO: rand
        }
        currFactInd = newFactInd;
        funFactsTxt.text = funFacts[currFactInd];
    }
}
