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
        public string password;
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
        public List<Soldier> soldiers = new List<Soldier>();

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
                loginInProgress = true;
                //new MonoBehaviour().StartCoroutine(LoginAndLoadAllDataCoroutine(userID, password));
                Thread loginThread = new Thread(new ParameterizedThreadStart(LoginAndLoadAllData_Thread));
                loginThread.Start(new LoginInfo(userID, password));
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
                Debug.Log(_loginInfo.complete);
                if (_loginInfo.complete) break;

                await System.Threading.Tasks.Task.Delay(1000, cancel1);
                if (cancel1.IsCancellationRequested) break;
            };

            //Debug.Log("Password:" + _loginInfo.Password);
            //Debug.Log("PasswordD:" + _loginInfo.PasswordD);

            //not using passwordHash
            if (passwordHash == _loginInfo.PasswordD)
            {
                loginSuccess = true;
            }
            

            if (loginSuccess)
            {
                Debug.Log("Login successful");
                LoadDataInfo loadDataInfo = new LoadDataInfo(userID);
                soldierList soldierList = new soldierList(userID);
                loadDataAndLoadSoldierInfo mother = new loadDataAndLoadSoldierInfo();
                mother.soldierList = soldierList;
                mother.loadDataInfo = loadDataInfo;

                Thread loadDataThread = new Thread(new ParameterizedThreadStart(LoadData_Thread));
                loadDataThread.Start(mother);
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

        public async static void getPassword_thread(object loginInfo)
        {
            Debug.Log("entered thread");
            LoginInfo _loginInfo = (LoginInfo)loginInfo;

            QueryInfo qi = new QueryInfo("https://project-finch-database.firebaseio.com/User/" + _loginInfo.userId + "/.json");
            Runner_call.Coroutines.Add(getPassword_RestClientCall(qi));
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
            //Debug.Log("set to true:" + _loginInfo.complete);
            //Debug.Log("got from database:" + _loginInfo.Password);
            
        }

        public static IEnumerator getPassword_RestClientCall(QueryInfo qi)
        {
            //Debug.Log("started coroutine");
            RestClient.Get(qi.query).Then(response =>
            {
                qi.responseText = response.Text;
                qi.inProgress = false;
            });
            yield return null;
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


            bool loadSuccess = true;
            PlayerAccount loadedAccount = new PlayerAccount();
            _mother.loadDataInfo.output = loadedAccount;

            //Debug.Log("userID:" +_loadDataInfo.userID);


            // TODO: load data into loadedAccount
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

            //Debug.Log("matchId:" + _loadDataInfo.output.matchID);
            //Debug.Log("InMatch:" + _loadDataInfo.output.InMatch);
            //Debug.Log("soldierList:" + _loadDataInfo.output.soldierList);
            //Debug.Log("userId:" + _loadDataInfo.output.userId);
            //Debug.Log("userName:" + _loadDataInfo.output.userName);
            //soldierList _soldierlist;
            //try { _soldierlist = (soldierList)soldierlist; }
            //catch (System.InvalidCastException) { Debug.Log("Invalid Cast"); return; }

            Thread getSoldierList = new Thread(new ParameterizedThreadStart(getSoldierList_thread));
            getSoldierList.Start((object)_mother.soldierList);

            System.Threading.CancellationToken cancel2 = new CancellationToken();
            for (int i = 0; i < 30; i++)
            {
                //Debug.Log(_loadDataInfo.complete);
                if (_mother.soldierList.complete) break;

                await System.Threading.Tasks.Task.Delay(1000, cancel2);
                if (cancel2.IsCancellationRequested) break;
            };

            Debug.Log("soldierList:" + _mother.soldierList.value);

            try {_mother.loadDataInfo.output.numberOfSoldiers = _mother.soldierList.value.Split(',').Length; }
            catch (Exception)
            {
                if (_mother.soldierList.value != null)
                {
                    _mother.loadDataInfo.output.numberOfSoldiers = 1;
                }
                else _mother.loadDataInfo.output.numberOfSoldiers = 0;
                    
            }

            string[] arr = new string[_mother.loadDataInfo.output.numberOfSoldiers];

            try
            {
                 arr = _mother.soldierList.value.Split(',');
            }
            catch (Exception)
            {
                if (_mother.soldierList.value != null)
                {
                    //arr = new string[1];
                    arr[0] = _mother.soldierList.value;
                }
                else
                {
                    //arr = new string[1];
                    arr[0] = null;
                }
            }
            

            foreach (string arrItem in arr)
            {
                if (arrItem != null)
                {
                    _mother.loadDataInfo.output.soldierNameList.Add(arrItem);
                }
                
            }
            
            Debug.Log("number of soldiers:"+_mother.loadDataInfo.output.soldierNameList.Count);

            foreach (string soldier_id in _mother.loadDataInfo.output.soldierNameList)
            {
                MenuView.Soldier soldier = new MenuView.Soldier();
                soldier.index = soldier_id;
                soldier.owner = _mother.loadDataInfo.output.userId;
                soldier.complete = false;
                Thread getSoldier = new Thread(new ParameterizedThreadStart(getSoldier_thread));
                getSoldier.Start(soldier);

                System.Threading.CancellationToken cancel1 = new CancellationToken();
                for (int i = 0; i < 30; i++)
                {
                    //Debug.Log(_loadDataInfo.complete);
                    if (soldier.complete) break;

                    await System.Threading.Tasks.Task.Delay(1000, cancel1);
                    if (cancel1.IsCancellationRequested) break;
                };

                //Debug.Log("soldier aim:" + soldier.aim);
                _mother.loadDataInfo.output.soldiers.Add(soldier);
                //Debug.Log("list of soldier class:"+ _loadDataInfo.output.soldiers.Count);
            }

            if (loadSuccess)
            {
                _mother.loadDataInfo.output = loadedAccount;
            }
            else _mother.loadDataInfo.output = null;

        }

        public async static void getSoldier_thread(object soldier)
        {

            MenuView.Soldier _soldier = (MenuView.Soldier)soldier;
            //Debug.Log("soldier owner:" + _soldier.owner);
            //Debug.Log("soldier index:"+_soldier.index);
            QueryInfo qi = new QueryInfo("https://project-finch-database.firebaseio.com/User/" + _soldier.owner+ "/Soldiers/Soldier"+_soldier.index+".json");
            Runner_call.Coroutines.Add(getFromDatabase_RestClientCall(qi));
            System.Threading.CancellationToken cancel = new CancellationToken();
            for (int i = 0; i < 30; i++)
            {
                if (!qi.inProgress) break;
                await System.Threading.Tasks.Task.Delay(1000, cancel);
                if (cancel.IsCancellationRequested) break;
            };
            //Debug.Log("started get");
            MenuView.Soldier _soldier1 = JsonUtility.FromJson<MenuView.Soldier>(qi.responseText);
            _soldier.aim = _soldier1.aim;
            _soldier.characterClass = _soldier1.characterClass;
            _soldier.equipments = _soldier1.equipments;
            _soldier.experience = _soldier1.experience;
            _soldier.fatigue = _soldier1.fatigue;
            _soldier.level = _soldier1.level;
            _soldier.maxHealth = _soldier1.maxHealth;
            _soldier.mobility = _soldier1.mobility;
            _soldier.mutations = _soldier1.mutations;
            //Debug.Log("soldier aim from database:" + _soldier1.aim);
            //Debug.Log("matchID:" +_loadDataInfo.output.matchID);
            //Debug.Log("soldierlist:" +_loadDataInfo.output.soldierList);
            _soldier.complete = true;
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
            //Debug.Log("started get");
            _loadDataInfo.output = JsonUtility.FromJson<PlayerAccount>(qi.responseText);
            //Debug.Log("matchID:" +_loadDataInfo.output.matchID);
            //Debug.Log("soldierlist:" +_loadDataInfo.output.soldierList);
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
            public LoadDataInfo loadDataInfo;
            public soldierList soldierList;
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
            new_account.password = password_hash;
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
                soldier.index = counter.ToString();
                soldierNameList.Add(soldier.index.ToString());
                //Debug.Log("soldiernamelist count:" + soldierNameList.Count);
                RestClient.Put("https://project-finch-database.firebaseio.com/User/" + userName + "/Soldiers/Soldier"+soldier.index+".json", soldier);
            }
            else
            {
                RestClient.Put("https://project-finch-database.firebaseio.com/User/" + userName + "/Soldiers/Soldier" + soldier.index + ".json", soldier);
            }
        }
    }
}