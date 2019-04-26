
from firebase import firebase
import random
import sys
import time
import threading

firebase = firebase.FirebaseApplication('https://project-finch-database.firebaseio.com/')


def checkPlayerMoves(matchid):
    counter = 0
    endturn_counter = 0
    player1move_old = ''
    player2move_old = ''
    #for constantly checking database

    while True:

        player1move_new = firebase.get('/Match/'+str(matchid)+'/moveInfo/p1move', 'value')
        player2move_new = firebase.get('/Match/' + str(matchid) + '/moveInfo/p2move', 'value')

        if player1move_new != player1move_old and player1move_new != '' and player1move_new is not None:
            print(player1move_new)
            if endturn_counter % 2 == 0:
                firebase.put('/Match/' + str(matchid) + '/moveInfo/moveHistory/a_playersMoves', str(counter), player1move_new)
                counter += 1
                if 'endturn' in player1move_new:
                    endturn_counter += 1
            else:
                print("Invalid move. It is not player1's turn")

        if player2move_new != player2move_old and player2move_new != '' and player2move_new is not None:
            print(player2move_new)
            if endturn_counter % 2 == 1:

                firebase.put('/Match/' + str(matchid) + '/moveInfo/moveHistory/a_playersMoves', str(counter), player2move_new)
                counter += 1
                if 'endturn' in player2move_new:
                    endturn_counter += 1
            else:
                print("Invalid move. It is not player2's turn")

        player1move_old = player1move_new
        player2move_old = player2move_new
        time.sleep(0.1)

        if player2move_new == "defeat" and player1move_new == "victory":
            game_winner = firebase.get('/Match/' + str(matchid)+ '/matchDetails', 'matchedPlayer1')
            game_loser = firebase.get('/Match/' + str(matchid)+ '/matchDetails', 'matchedPlayer2')
            creditRewards(game_winner, game_loser)
            firebase.delete('/Match/' + str(matchid), "moveInfo")
            firebase.delete('/Match/' + str(matchid), "matchDetails")
            sys.exit()

        if player2move_new == "victory" and player1move_new == "defeat":
            game_loser = firebase.get('/Match/' + str(matchid) + '/matchDetails', 'matchedPlayer1')
            game_winner = firebase.get('/Match/' + str(matchid) + '/matchDetails', 'matchedPlayer2')
            creditRewards(game_winner, game_loser)
            firebase.delete('/Match/' + str(matchid), "moveInfo")
            firebase.delete('/Match/' + str(matchid), "matchDetails")
            sys.exit()


def creditRewards(winner, loser):
    #Creditting winner
    winner_current_currency = firebase.get('/User/' + winner, 'currency')
    winner_current_currency += 500
    firebase.put('/User/' + winner, 'currency', winner_current_currency)
    firebase.put('/User/' + winner, 'InMatch', False)
    firebase.put('/User/'+ winner, 'matchID', -1)

    #Creditting loser
    loser_current_currency = firebase.get('/User/' + loser, 'currency')
    loser_current_currency += 100
    firebase.put('/User/' + loser, 'currency', loser_current_currency)
    firebase.put('/User/' + loser, 'InMatch', False)
    firebase.put('/User/' + loser, 'matchID', -1)


def matchmaking(initial_matchId):
    while True:
        playersInQueue = firebase.get('/queuingForMatch/', None)
        print(playersInQueue)
        if playersInQueue != None:
            listOfPlayers = []
            for keys in playersInQueue.keys():
                listOfPlayers.append(keys)
            print("length of list: " +str(len(listOfPlayers)))
            if len(listOfPlayers) >= 2:
                print(listOfPlayers[0], listOfPlayers[1])
                initial_matchId += 1
                firebase.put('/queuingForMatch/'+listOfPlayers[0], "UserID", None)
                firebase.put('/queuingForMatch/'+listOfPlayers[1], "UserID", None)
                firebase.put('/User/' + listOfPlayers[0], "matchID", initial_matchId)
                firebase.put('/User/'+listOfPlayers[0], "InMatch", True)
                firebase.put('/User/' + listOfPlayers[1], "matchID", initial_matchId)
                firebase.put('/User/'+listOfPlayers[1], "InMatch", True)

                create_match(initial_matchId, listOfPlayers[0], listOfPlayers[1])
                threading.Thread(target= checkPlayerMoves, args=(initial_matchId,) , daemon=True).start()

                #can try testing later. start the thread, stop the main program see if the thread still looks out for game moves.


def create_match(matchId, player1, player2):
    mapSeed = random.randint(0,2147483647)

    randint = bool(random.getrandbits(1))
    if randint:
        firebase.put('/Match/' + str(matchId), 'matchDetails', {'matchID': matchId, 'map': 'default',
                                                                'matchInProgress': True, 'matchedPlayer1': player1,
                                                                'matchedPlayer2': player2, 'mapSeed': mapSeed})
    else:
        firebase.put('/Match/' + str(matchId), 'matchDetails', {'matchID': matchId, 'map': 'default',
                                                                'matchInProgress': True, 'matchedPlayer1': player2,
                                                                'matchedPlayer2': player1, 'mapSeed': mapSeed})
    time.sleep(5)

    firebase.put('/Match/'+str(matchId)+'/moveInfo/moveHistory', 'a_playersMoves', {0: ""})
    firebase.put('/Match/'+str(matchId)+'/moveInfo/', 'p1move', {"value":""})
    firebase.put('/Match/'+str(matchId)+'/moveInfo/', 'p2move', {"value":""})


initial_matchId = random.randint(0,2147483647)
matchmaking(initial_matchId)

