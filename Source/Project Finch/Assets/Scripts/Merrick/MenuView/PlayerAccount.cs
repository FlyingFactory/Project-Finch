using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MenuView {

    public class PlayerAccount {

        public string username;
        public byte[] passwordHash;
        public float unrankedMMR;
        public float rankedMMR;
        // future getonly property to get rank name from MMR
        public List<OwnableItem> items = new List<OwnableItem>();
        public List<Soldier> soldiers;

        public static PlayerAccount currentPlayer = null;

        /// <summary>
        /// Hashes the password, then checks user/pass with the database.
        /// If it is valid, loads all data into the STATIC currentPlayer's variables.
        /// </summary>
        /// <returns>true if the login was successful.</returns>
        public static bool LoginAndLoadAllData(int userID, string password) { // Not sure if you want int or string for ID, just go ahead and change if needed
            bool loginSuccess = true;
            string passwordHash = Hash128.Compute(password).ToString(); // Not sure if this works, can try
            // TODO: attempt to login
            if (loginSuccess) {
                currentPlayer = LoadData(userID);
                if (currentPlayer == null) {
                    Debug.Log("Load data unsuccessful");
                    loginSuccess = false;
                }
            }
            else {
                Debug.Log("Login not successful");
            }
            return loginSuccess;
        }

        /// <summary>
        /// Loads data of any user, for game-loading purposes or viewing profiles.
        /// </summary>
        /// <returns>The newly constructed PlayerAccount, or null if there is no user of that ID.</returns>
        public static PlayerAccount LoadData(int userID) {
            bool loadSuccess = true;
            PlayerAccount loadedAccount = new PlayerAccount();

            // TODO: load data

            if (loadSuccess) {
                return loadedAccount;
            }
            else return null;
        }
    }
}