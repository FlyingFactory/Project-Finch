using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using System.Threading;
using System;

namespace MenuView
{

    public class PlayerAccount
    {

        public string userName;
        public byte[] passwordHash;
        public string Password;
        public float unrankedMMR;
        public float rankedMMR;
        // future getonly property to get rank name from MMR

        public int counter = 0;
        public Soldier soldier;
        public bool InMatch;
        public volatile int matchID;
        public string userId;
        public int numberOfSoldiers;
        public List<string> soldierNameList;
        public List<OwnableItem> items = new List<OwnableItem>();
        public Dictionary<string, Soldier> soldiers = new Dictionary<string, Soldier>();
        public LoginInfo loginInfo;
        public bool dataLoaded = false;
        public int currency = 0;

        public static PlayerAccount currentPlayer = null;
        public static bool loginInProgress = false;


        /// <summary>
        /// Hashes the password, then checks user/pass with the database.
        /// If it is valid, loads all data into the STATIC currentPlayer's variables.
        /// </summary>
        public void LoginAndLoadAllData(string userID, string password)
        {
            if (!loginInProgress)
            {
                Debug.Log("logging in..");
                loginInProgress = true;
                //new MonoBehaviour().StartCoroutine(LoginAndLoadAllDataCoroutine(userID, password));
                LoginInfo _loginInfo = new LoginInfo(userID, password);
                Thread loginThread = new Thread(new ParameterizedThreadStart(LoginAndLoadAllData_Thread));
                loginThread.Start(_loginInfo);
            }
        }

        public class LoginInfo
        {
            public volatile string userId;
            public volatile string Password;
            public volatile string PasswordD;
            public bool complete = false;
            public LoginInfo(string userID, string password)
            {
                this.userId = userID;
                this.Password = password;
            }
        }

        public async static void LoginAndLoadAllData_Thread(object loginInfo)
        {
            LoginInfo _loginInfo;
            try { _loginInfo = (LoginInfo)loginInfo; }
            catch (System.InvalidCastException) { Debug.Log("Invalid Cast"); return; }

            string userID = _loginInfo.userId;
            string passwordHash = Hash128.Compute(_loginInfo.Password).ToString(); // Not sure if this works, can try
            //Debug.Log("passwordHash: " + passwordHash);
            bool loginSuccess = false;
            
            // TODO: attempt to login, Fixed

            Thread getPassword = new Thread(new ParameterizedThreadStart(getPassword_thread));
            getPassword.Start((object) _loginInfo);

            System.Threading.CancellationToken cancel1 = new CancellationToken();
            for (int i = 0; i < 10; i++)
            {

                if (_loginInfo.complete) break;

                await System.Threading.Tasks.Task.Delay(1000, cancel1);
                if (cancel1.IsCancellationRequested) break;
            };

            //not using passwordHash
            if (passwordHash == _loginInfo.PasswordD)
            {
                loginSuccess = true;
            }
            

            if (loginSuccess)
            {
                LoadDataInfo loadDataInfo = new LoadDataInfo(userID);
                soldierList soldierList = new soldierList(userID);
                loadDataAndLoadSoldierInfo mother = new loadDataAndLoadSoldierInfo();
                mother.soldierList = soldierList;
                mother.loadDataInfo = loadDataInfo;

                Thread loadDataThread = new Thread(new ParameterizedThreadStart(LoadData_Thread));
                loadDataThread.Start(mother);

                System.Threading.CancellationToken cancel3 = new CancellationToken();
                for (int i = 0; i < 10; i++)
                {
                    //Debug.Log(_loginInfo.complete);
                    if (mother.complete) break;

                    await System.Threading.Tasks.Task.Delay(1000, cancel3);
                    if (cancel1.IsCancellationRequested) break;
                };
                if (loadDataInfo.output == null)
                {
                    Debug.Log("Load data unsuccessful");
                    loginSuccess = false;
                }
                else
                {
                    currentPlayer = loadDataInfo.output;
                    currentPlayer = mother.loadDataInfo.output;
                    currentPlayer.dataLoaded = true;
                }
            }
            else
            {
                Debug.Log("Login not successful");
            }
        }

        public async static void getPassword_thread(object loginInfo)
        {
            LoginInfo _loginInfo = (LoginInfo)loginInfo;

            QueryInfo qi = new QueryInfo("https://project-finch-database.firebaseio.com/User/" + _loginInfo.userId + "/.json");
            Runner_call.Coroutines.Add(getFromDatabase_RestClientCall(qi));
            System.Threading.CancellationToken cancel = new CancellationToken();
            for (int i = 0; i < 30; i++)
            {
                if (!qi.inProgress) break;
                await System.Threading.Tasks.Task.Delay(1000, cancel);
                if (cancel.IsCancellationRequested) break;
            };
            
            LoginInfo _loginInfo1 = JsonUtility.FromJson<LoginInfo>(qi.responseText);
            _loginInfo.PasswordD = _loginInfo1.Password;
            _loginInfo.complete = true;
        }


        public async static void checkForMatch(PlayerAccount player)
        {
            LoadDataInfo loadDataInfo = new LoadDataInfo(player.userId);
            Thread check_match = new Thread(new ParameterizedThreadStart(getFromDatabase_thread));
            check_match.Start(loadDataInfo);

            System.Threading.CancellationToken cancel = new CancellationToken();
            for (int i = 0; i < 30; i++)
            {
                //Debug.Log(_loadDataInfo.complete);
                if (loadDataInfo.complete) break;

                await System.Threading.Tasks.Task.Delay(1000, cancel);
                if (cancel.IsCancellationRequested) break;
            };
            player.matchID = loadDataInfo.output.matchID;
            //Debug.Log("matchID:" + player.matchID);
        }


        /// <summary>
        /// Loads data of any user, for game-loading purposes or viewing profiles.
        /// </summary>
        /// <returns>The newly constructed PlayerAccount, or null if there is no user of that ID.</returns>
        public async static void LoadData_Thread(object mother)
        {
            loadDataAndLoadSoldierInfo _mother;
            try { _mother = (loadDataAndLoadSoldierInfo)mother; }
            catch (System.InvalidCastException) { Debug.Log("Invalid Cast"); return; }

            Thread getDatabase = new Thread(new ParameterizedThreadStart(getFromDatabase_thread));
            getDatabase.Start((object)_mother.loadDataInfo);

            System.Threading.CancellationToken cancel = new CancellationToken();
            for (int i = 0; i < 30; i++)
            {
                //Debug.Log(_loadDataInfo.complete);
                if (_mother.loadDataInfo.complete) break;

                await System.Threading.Tasks.Task.Delay(1000, cancel);
                if (cancel.IsCancellationRequested) break;
            };

            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            //TO DO: DO THE STRING FORMATTING HERE.
            //USING _mother.loadDataInfo.output_string <- this is the full string that we pulled from database.
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            Dictionary<string,string> d = PF_Utils.FirebaseParser.SplitByBrace(_mother.loadDataInfo.output_string);
            _mother.loadDataInfo.output =  JsonUtility.FromJson<PlayerAccount>(d[""]);

            Dictionary<string, string> d_soldiers = PF_Utils.FirebaseParser.SplitByBrace(d["Soldiers"]);
            foreach (KeyValuePair<string,string> kvp in d_soldiers)
            {
                if (kvp.Key != "")
                {
                    _mother.loadDataInfo.output.soldiers[kvp.Key] = JsonUtility.FromJson<Soldier>(kvp.Value);
                    _mother.loadDataInfo.output.soldiers[kvp.Key].uniqueId = kvp.Key;
                }
            }
            soldierListClass soldierList = JsonUtility.FromJson<soldierListClass>(d["soldierList"]);

            foreach(string arrItem in soldierList.value.Split(','))
            {
                _mother.loadDataInfo.output.soldierNameList.Add(arrItem);
            }
            _mother.complete = true;
        }


        public async static void getFromDatabase_thread(object loadDataInfo)
        {

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
            _loadDataInfo.output_string = qi.responseText;
            _loadDataInfo.output = JsonUtility.FromJson<PlayerAccount>(qi.responseText);
            _loadDataInfo.complete = true;
        }

        public async static void getSoldierList_thread(object soldier_list)
        {
            soldierList _soldierList = (soldierList)soldier_list;
            QueryInfo qi = new QueryInfo("https://project-finch-database.firebaseio.com/User/" + _soldierList.userID + "/soldierList.json");
            Runner_call.Coroutines.Add(getFromDatabase_RestClientCall(qi));
            System.Threading.CancellationToken cancel = new CancellationToken();
            for (int i = 0; i < 30; i++)
            {
                if (!qi.inProgress) break;
                await System.Threading.Tasks.Task.Delay(1000, cancel);
                if (cancel.IsCancellationRequested) break;
            };
            soldierList buffer = JsonUtility.FromJson<soldierList>(qi.responseText);
            _soldierList.value = buffer.value;
            Debug.Log("soldierList:" + _soldierList.value);
        }

        public class LoadDataInfo
        {
            public string userID;
            public string output_string;
            public PlayerAccount output = null;
            public bool complete = false;
            public LoadDataInfo(string userID)
            {
                this.userID = userID;
            }
        }

        public class soldierList
        {
            public string value;
            public string userID;
            public bool complete = false;
            public soldierList(string userID)              
            {
                this.userID = userID;
                this.value = "";
            }
        }
        public class loadDataAndLoadSoldierInfo
        {
            public LoadDataInfo loadDataInfo = new LoadDataInfo("default");
            public soldierList soldierList = new soldierList("default");
            public bool complete;
        }

        public static IEnumerator getFromDatabase_RestClientCall(QueryInfo qi)
        {
            //Debug.Log("started coroutine");
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

        public void createNewAccount(string userName, string password)
        {
            string password_hash = Hash128.Compute(password).ToString();
            PlayerAccount new_account = new PlayerAccount();
            new_account.Password = password_hash;
            new_account.userName = userName;
            new_account.InMatch = false;
            new_account.matchID = -1;
            RestClient.Put("https://project-finch-database.firebaseio.com/User/" + userName + ".json", new_account);
        }

        public void putSoldier(Soldier soldier, bool itIsNew)
        {
            if (itIsNew)
            {
                //Debug.Log("putting new soldier..");
                counter += 1;
                //Debug.Log("counter:" + counter);
                //soldier.uniqueId = counter.ToString();
                soldierNameList.Add(soldier.uniqueId.ToString());
                //Debug.Log("soldiernamelist count:" + soldierNameList.Count);
                RestClient.Put("https://project-finch-database.firebaseio.com/User/" + userName + "/Soldiers/Soldier"+soldier.uniqueId+".json", soldier);
            }
            else
            {
                RestClient.Put("https://project-finch-database.firebaseio.com/User/" + userName + "/Soldiers/Soldier" + soldier.uniqueId + ".json", soldier);
            }
        }

        public static void stringToClass(string json)
        {
            int start_index_soldiers = json.IndexOf("\"Soldiers\":");
            int end_index_soldiers = json.LastIndexOf("}},");
            string filtered_first = json.Substring(0, start_index_soldiers);

            string second_sub_string = json.Substring(end_index_soldiers + 3, json.Length - (end_index_soldiers + 3));
            int start_index_soldierList = second_sub_string.IndexOf("\"soldierList\":");
            int end_index_soldierList = second_sub_string.LastIndexOf("},");

            string filtered_second = second_sub_string.Substring(0, start_index_soldierList) +second_sub_string.Substring(end_index_soldierList+2, second_sub_string.Length - (end_index_soldierList+2));
            string top_dict_data = filtered_first + filtered_second;

            string list_of_soldier_class = json.Substring(start_index_soldiers, end_index_soldiers - start_index_soldiers + 2);

            //soldierListClass list_of_soldiers = JsonUtility.FromJson<soldierListClass>(list_of_soldier_class);

            //Debug.Log(list_of_soldiers.list_of_soldier_class.Count);
            Debug.Log(list_of_soldier_class);
            currentPlayer = JsonUtility.FromJson<MenuView.PlayerAccount>(top_dict_data);
            //currentPlayer.soldiers = JsonUtility.FromJson<Object>()
            //Debug.Log(currentPlayer.matchID);
            //Debug.Log(currentPlayer.soldiers.Count);
        }

        public class soldierListClass
        {
            public string value;
        }

        public async static void getMatchDetails_thread(object matchDetails)
        {
            MatchDetails _matchDetails = (MatchDetails)matchDetails;
            QueryInfo qi = new QueryInfo("https://project-finch-database.firebaseio.com/Match/" + _matchDetails.matchID + "/matchDetails.json");
            Runner_call.Coroutines.Add(getFromDatabase_RestClientCall(qi));
            System.Threading.CancellationToken cancel = new CancellationToken();
            for (int i = 0; i < 30; i++)
            {
                if (!qi.inProgress) break;
                await System.Threading.Tasks.Task.Delay(1000, cancel);
                if (cancel.IsCancellationRequested) break;
            };
            MatchDetails buffer = JsonUtility.FromJson<MatchDetails>(qi.responseText);
            _matchDetails.mapSeed = buffer.mapSeed;
            _matchDetails.matchedPlayer1 = buffer.matchedPlayer1;
            _matchDetails.complete = true;
        }
    }
}