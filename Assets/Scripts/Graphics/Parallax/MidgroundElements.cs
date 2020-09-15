using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[System.Serializable]
public class MidgroundElements {
    
    [System.Serializable]
    public struct rowData {
        public Sprite[] sprites;
    }

    public rowData[] midgroundPair = new rowData[2];
}
