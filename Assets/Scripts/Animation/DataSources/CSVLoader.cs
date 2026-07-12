using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CSVLoader{ 
public (string[][] data ,Dictionary<string,int> columnIndices) loadCSV(TextAsset csvFile,bool hasHeaders){
        
        // dict for getting the index of a column from its name
        Dictionary<string,int> columnIndices = new Dictionary<string, int>();

        int rowCount;
        //sets start index for saving csvData
        int startIndex = 0;

        string[] lines = csvFile.text.Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        rowCount = lines.Length; 
    
        // Gets columns from csv if present
        if (hasHeaders){
            string[] headers = lines[0].Split(',');
            for (int i = 0; i < headers.Length; i++){columnIndices[headers[i].Trim()] = i;}
            startIndex++;
            rowCount--;
        } 

        string[][] outputData = new string[rowCount][];

        for (int i = startIndex; i < lines.Length; i++){
            string[] currentRowValues = lines[i].Split(',');

            if (currentRowValues.Length >= columnIndices.Count){

                if (hasHeaders){outputData[i-1] = currentRowValues;} 
                else {outputData[i] = currentRowValues;}
            }
        }
        
        return (outputData,columnIndices);
    }
}