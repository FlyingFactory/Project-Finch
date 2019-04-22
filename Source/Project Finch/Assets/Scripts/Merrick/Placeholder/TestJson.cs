using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestJson : MonoBehaviour {

    private string TestString = "{\"InMatch\":true,\"Password\":\"0a543f6269d88d66824e3305226bf311\",\"Soldiers\":{\"Soldier1\":{\"aim\":65,\"characterClass\":0,\"complete\":false,\"experience\":0.0,\"fatigue\":0.0,\"index\":\"1\",\"level\":1,\"maxHealth\":6,\"mobility\":6,\"name\":\"undefined\",\"owner\":\"\"},\"Soldier2\":{\"aim\":65,\"characterClass\":0,\"complete\":false,\"experience\":0.0,\"fatigue\":0.0,\"index\":\"2\",\"level\":1,\"maxHealth\":6,\"mobility\":6,\"name\":\"undefined\",\"owner\":\"\"},\"Soldier3\":{\"aim\":65,\"characterClass\":0,\"complete\":false,\"experience\":0.0,\"fatigue\":0.0,\"index\":\"3\",\"level\":1,\"maxHealth\":6,\"mobility\":6,\"name\":\"undefined\",\"owner\":\"\"}},\"matchID\":-1,\"numberOfSoldiers\":3,\"rankedMMR\":0.0,\"soldierList\":{\"value\":\"1,2,3\"},\"unrankedMMR\":0.0,\"userId\":\"\",\"userName\":\"user001\"}";

    private void Start() {
        Dictionary<string, string> d = PF_Utils.FirebaseParser.SplitByBrace(TestString);
        foreach (KeyValuePair<string, string> kvp in d) {
            Debug.Log("key: " + kvp.Key + " --- value: " + kvp.Value);
        }

        Dictionary<string, string> d2 = PF_Utils.FirebaseParser.SplitByBrace(d["Soldiers"]);
        foreach (KeyValuePair<string, string> kvp in d2) {
            Debug.Log("soldier: " + kvp.Key + " --- value: " + kvp.Value);
        }
    }
}
