using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PF_Utils {

    public static class FirebaseParser {

        /// <summary>
        /// Takes a json returned by firebase, and splits it according to its highest level children.
        /// The returned dictionary has "" as a key for the original string, minus the removed portions.
        /// Each individual removed string has its node name as the key.
        /// All returns do NOT have surrounding braces.
        /// </summary>
        /// <param name="jsonInput">Firebase-formatted json text.</param>
        /// <returns></returns>
        public static Dictionary<string, string> SplitByBrace(string jsonInput) {
            Dictionary<string, string> r = new Dictionary<string, string>();
            r.Add("", "");
            if (jsonInput[0] == '{') jsonInput= jsonInput.Substring(1, jsonInput.Length - 2);

            bool open = false;
            bool isBrace = false;
            int openPos = 0;
            int cutPos = 0;
            int braceCount = 0;
            string lastQuote = "";

            for (int i = 0; i < jsonInput.Length; i++) {
                if (!open) {
                    if (jsonInput[i] == '"') {
                        open = true;
                        isBrace = false;
                        openPos = i + 1;
                        if (i - 1 >= 0 && jsonInput[i - 1] != ':') {
                            r[""] += jsonInput.Substring(cutPos, i - cutPos);
                            cutPos = i;
                        }
                    }
                    else if (jsonInput[i] == '{') {
                        open = true;
                        isBrace = true;
                        openPos = i + 1;
                        braceCount = 1;
                    }
                }
                else {
                    if (isBrace) {
                        if (jsonInput[i] == '{') {
                            braceCount++;
                        }
                        else if (jsonInput[i] == '}') {
                            braceCount--;
                            if (braceCount == 0) {
                                //Debug.LogWarning(jsonInput.Substring(openPos, i - openPos));
                                r.Add(lastQuote, jsonInput.Substring(openPos, i - openPos));
                                open = false;
                                if (i + 2 < jsonInput.Length && jsonInput[i + 1] == ',') cutPos = i + 2;
                                else cutPos = i + 1;
                            }
                        }
                    }
                    else { // quotes
                        if (jsonInput[i] == '\\') { // account for escape character
                            i++;
                        }
                        else if (jsonInput[i] == '"') {
                            lastQuote = jsonInput.Substring(openPos, i - openPos);
                            open = false;
                        }
                    }
                }
            }
            if (cutPos < jsonInput.Length) r[""] += jsonInput.Substring(cutPos, jsonInput.Length - cutPos);

            return r;
        }
    }
}