/**
 * CyclingText.cs: Implements a cycling text widget from a text file bank.
 *
 * @author Mars Semenova
 */

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CyclingText : MonoBehaviour {
    // params
    // file
    [Header("Options")]
    [SerializeField] private float repeatRate = 15;
    [Header("Text File")]
    [SerializeField] private TextAsset textFile;
    
    // vars
    private Button txtBtn;
    private TextMeshProUGUI txt;
    private String[] lines;
    private int currInd;
    private double lastChangeTime;
    
    void Awake() {
        // get refs
        txt = GetComponent<TextMeshProUGUI>();
        txtBtn = GetComponent<Button>();
    }
    
    void Start () {
        // load facts
        LoadLines();
        UpdateLine();
        lastChangeTime = Time.unscaledTimeAsDouble;
        
        // set up btn
        if (txtBtn) {
            txtBtn.onClick.AddListener(UpdateLine);
        }
    }

    private void Update() {
        // update line
        double currentTime = Time.unscaledTimeAsDouble;
        if (currentTime - lastChangeTime >= repeatRate) {
            lastChangeTime = currentTime;
            UpdateLine();
        }
        
    }

    /**
     * Load line from passed text file.
     */
    private void LoadLines() {
        lines = textFile.text.Split('\n');
    }

    /**
     * Update displayed line.
     */
    private void UpdateLine() {
        int newInd = Random.Range(0, lines.Length);
        while (lines.Length > 1 && newInd == currInd) { 
            newInd = Random.Range(0, lines.Length); 
        }
        currInd = newInd;
        txt.text = lines[currInd];
    }
}
