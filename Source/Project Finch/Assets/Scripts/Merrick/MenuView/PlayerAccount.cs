using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using System.Threading;

namespace MenuView {

    public class PlayerAccount {

        public string username;
        public byte[] passwordHash;
        public float unrankedMMR;
        public float rankedMMR;
        // future getonly property to get rank name from MMR

        public int numberOfSoldiers;
        public List<OwnableItem> items = new List<OwnableItem>();
        public List<Soldier> soldiers;

        public static PlayerAccount currentPlayer = null;
        public static bool loginInProgress = false;

        /// <summary>
        /// Hashes the password, then checks user/pass with the database.
        /// If it is valid, loads all data into the STATIC currentPlayer's variables.
        /// </summary>
        public void LoginAndLoadAllData(int userID, string password) {
            if (!loginInProgress) {
                loginInProgress = true;
                //new MonoBehaviour().StartCoroutine(LoginAndLoadAllDataCoroutine(userID, password));
                Thread loginThread = new Thread(new ParameterizedThreadStart(LoginAndLoadAllData_Thread));
                loginThread.Start(new LoginInfo(userID, password));
            }
        }
        private struct LoginInfo {
            public int userID;
            public string password;
            public LoginInfo(int userID, string password) {
                this.userID = userID;
                this.password = password;
            }
        }

        public static void LoginAndLoadAllData_Thread(object loginInfo) {
            LoginInfo _loginInfo;
            try { _loginInfo = (LoginInfo)loginInfo; }
            catch (System.InvalidCastException) { Debug.Log("Invalid Cast"); return; }

            int userID = _loginInfo.userID;
            string passwordHash = Hash128.Compute(_loginInfo.password).ToString(); // Not sure if this works, can try

            bool loginSuccess = true;
            // TODO: attempt to login

            if (loginSuccess) {
                LoadDataInfo loadDataInfo = new LoadDataInfo(userID);

                Thread loadDataThread = new Thread(new ParameterizedThreadStart(LoadData_Thread));
                loadDataThread.Start(loadDataInfo);
                loadDataThread.Join();

                if (loadDataInfo.output == null) {
                    Debug.Log("Load data unsuccessful");
                    loginSuccess = false;
                }
                else {
                    currentPlayer = loadDataInfo.output;
                }
            }
            else {
                Debug.Log("Login not successful");
            }
        }

        /// <summary>
        /// Loads data of any user, for game-loading purposes or viewing profiles.
        /// </summary>
        /// <returns>The newly constructed PlayerAccount, or null if there is no user of that ID.</returns>
        public static void LoadData_Thread(object loadDataInfo) {

            LoadDataInfo _loadDataInfo;
            try { _loadDataInfo = (LoadDataInfo)loadDataInfo; }
            catch (System.InvalidCastException) { Debug.Log("Invalid Cast"); return; }

            bool loadSuccess = true;
            PlayerAccount loadedAccount = new PlayerAccount();

            // TODO: load data into loadedAccount
            

            if (loadSuccess) {
                _loadDataInfo.output = loadedAccount;
            }
            else _loadDataInfo.output = null;
            _loadDataInfo.complete = true;
        }
        public class LoadDataInfo {
            public int userID;
            public PlayerAccount output = null;
            public bool complete = false;
            public LoadDataInfo(int userID) {
                this.userID = userID;
            }
        }

        public IEnumerator getNumberOfSoldiers(int userID)
        {
            bool inProgress = true;
            RestClient.Get<UserInfo>("https://project-finch-database.firebaseio.com/User/" + userID + ".json").Then(response => 
            {
                numberOfSoldiers = response.numberOfSoldiers;
                inProgress = false;
            });
            while (inProgress) yield return new WaitForSeconds(0.25f);
        }
    }
}