
from firebase import firebase
import time

firebase = firebase.FirebaseApplication('https://project-finch-database.firebaseio.com/')


def matchmaking():
    num_of_matches = 0
    while True:
        playersInQueue = firebase.get('/queuingForMatch/', None)
        print(playersInQueue)
        if playersInQueue != None:
            listOfPlayers = list(playersInQueue.keys())
            print(len(listOfPlayers))
            if len(listOfPlayers) >= 2:
                print(listOfPlayers[0], listOfPlayers[1])
                num_of_matches += 1
                firebase.put('/queuingForMatch/'+listOfPlayers[0], "userId", None)
                firebase.put('/queuingForMatch/'+listOfPlayers[1], "userId", None)
                firebase.put('/User/'+listOfPlayers[0], "InMatch", True)
                firebase.put('/User/'+listOfPlayers[1], "InMatch", True)
                create_match(1, listOfPlayers[0], listOfPlayers[1])


def create_match(matchId, player1, player2):
    firebase.put('/Match/'+str(matchId), 'matchDetails', {'matchID': matchId, 'map':'default',
                                                          'matchInProgress': True, 'matchedPlayer1': player1, 'matchedPlayer2':player2})

    firebase.put('/Match/'+str(matchId)+'/moveInfo/moveHistory', 'a_playersMoves', {0:""})
    firebase.put('/Match/'+str(matchId)+'/moveInfo/', 'p1move', {"value":""})
    firebase.put('/Match/'+str(matchId)+'/moveInfo/', 'p2move', {"value":""})


matchmaking()