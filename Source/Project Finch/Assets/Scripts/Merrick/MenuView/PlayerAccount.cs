using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using System.Threading;

namespace MenuView
{

    public class PlayerAccount
    {

        public string userName;
        public byte[] passwordHash;
        public float unrankedMMR;
        public float rankedMMR;
        // future getonly property to get rank name from MMR

        public Soldier soldier;
        public bool InMatch;
        public volatile int matchID;
        public string soldierList;
        public string userId;
        public int numberOfSoldiers;
        public List<OwnableItem> items = new List<OwnableItem>();
        public List<Soldier> soldiers;

        public static PlayerAccount currentPlayer = null;
        public static bool loginInProgress = false;


        /// <summary>
        /// Hashes the password, then checks user/pass with the database.
        /// If it is valid, loads all data into the STATIC currentPlayer's variables.
        /// </summary>
        public void LoginAndLoadAllData(int userID, string password)
        {
            if (!loginInProgress)
            {
                loginInProgress = true;
                //new MonoBehaviour().StartCoroutine(LoginAndLoadAllDataCoroutine(userID, password));
                Thread loginThread = new Thread(new ParameterizedThreadStart(LoginAndLoadAllData_Thread));
                loginThread.Start(new LoginInfo(userID, password));
            }
        }
        private struct LoginInfo
        {
            public int userID;
            public string password;
            public LoginInfo(int userID, string password)
            {
                this.userID = userID;
                this.password = password;
            }
        }

        public static void LoginAndLoadAllData_Thread(object loginInfo)
        {
            LoginInfo _loginInfo;
            try { _loginInfo = (LoginInfo)loginInfo; }
            catch (System.InvalidCastException) { Debug.Log("Invalid Cast"); return; }

            int userID = _loginInfo.userID;
            string passwordHash = Hash128.Compute(_loginInfo.password).ToString(); // Not sure if this works, can try

            bool loginSuccess = true;
            // TODO: attempt to login

            if (loginSuccess)
            {
                LoadDataInfo loadDataInfo = new LoadDataInfo(userID);

                Thread loadDataThread = new Thread(new ParameterizedThreadStart(LoadData_Thread));
                loadDataThread.Start(loadDataInfo);
                loadDataThread.Join();

                if (loadDataInfo.output == null)
                {
                    Debug.Log("Load data unsuccessful");
                    loginSuccess = false;
                }
                else
                {
                    currentPlayer = loadDataInfo.output;
                }
            }
            else
            {
                Debug.Log("Login not successful");
            }
        }

        /// <summary>
        /// Loads data of any user, for game-loading purposes or viewing profiles.
        /// </summary>
        /// <returns>The newly constructed PlayerAccount, or null if there is no user of that ID.</returns>
        public async static void LoadData_Thread(object loadDataInfo)
        {

            LoadDataInfo _loadDataInfo;
            try { _loadDataInfo = (LoadDataInfo)loadDataInfo; }
            catch (System.InvalidCastException) { Debug.Log("Invalid Cast"); return; }

            bool loadSuccess = true;
            PlayerAccount loadedAccount = new PlayerAccount();
            _loadDataInfo.output = loadedAccount;

            //Debug.Log("userID:" +_loadDataInfo.userID);


            // TODO: load data into loadedAccount
            Thread getDatabase = new Thread(new ParameterizedThreadStart(getFromDatabase_thread));
            getDatabase.Start((object)_loadDataInfo);

            System.Threading.CancellationToken cancel = new CancellationToken();
            for (int i = 0; i < 30; i++)
            {
                Debug.Log(_loadDataInfo.complete);
                if (_loadDataInfo.complete) break;

                await System.Threading.Tasks.Task.Delay(1000, cancel);
                if (cancel.IsCancellationRequested) break;
            };

            //Debug.Log("thread complete");
            //Debug.Log("test");
            //Debug.Log("yo:" + _loadDataInfo.output.matchID);
            //Debug.Log("test1");


            for (int i = 1; i < loadedAccount.numberOfSoldiers + 1; i++)
            {
                RestClient.Get<Soldier>("https://project-finch-database.firebaseio.com/User/" + _loadDataInfo.userID + "/Soldiers/Soldier" + i + "/.json").Then(response =>
                    {
                        Soldier soldier = new Soldier();
                        soldier = response;
                        loadedAccount.soldiers.Add(soldier);
                    });
            }

            if (loadSuccess)
            {
                _loadDataInfo.output = loadedAccount;
            }
            else _loadDataInfo.output = null;

        }

        public async static void getFromDatabase_thread(object loadDataInfo)
        {
            Debug.Log("entered Thread");

            LoadDataInfo _loadDataInfo = (LoadDataInfo)loadDataInfo;

            QueryInfo qi = new QueryInfo("https://project-finch-database.firebaseio.com/User/" + _loadDataInfo.userID + "/.json");
            Runner_call.Coroutines.Add(getFromDatabase_RestClientCall(qi));
            System.Threading.CancellationToken cancel = new CancellationToken();
            for (int i = 0; i < 30; i++)
            {
                if (!qi.inProgress) break;
                await System.Threading.Tasks.Task.Delay(1000, cancel);
                if (cancel.IsCancellationRequested) break;
            };
            //Debug.Log("started get");
            _loadDataInfo.output = JsonUtility.FromJson<PlayerAccount>(qi.responseText);
            //Debug.Log("matchID:" +_loadDataInfo.output.matchID);
            //Debug.Log("soldierlist:" +_loadDataInfo.output.soldierList);
            _loadDataInfo.complete = true;
        }
        public class LoadDataInfo
        {
            public int userID;
            public PlayerAccount output = null;
            public bool complete = false;
            public LoadDataInfo(int userID)
            {
                this.userID = userID;
            }
        }
        public static IEnumerator getFromDatabase_RestClientCall(QueryInfo qi)
        {
            Debug.Log("started coroutine");
            RestClient.Get(qi.query).Then(response =>
            {
                qi.responseText = response.Text;
                qi.inProgress = false;
            });
            yield return null;
        }

        public class QueryInfo
        {
            public bool inProgress = true;
            public string query;
            public string responseText = "";
            public QueryInfo(string query)
            {
                this.query = query;
            }
        }
    }
}