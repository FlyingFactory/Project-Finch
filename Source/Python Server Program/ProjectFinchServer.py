from firebase import firebase
import time

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

        if player2move_new and player1move_new == "leftmatch":
            firebase.delete('/Match/' + str(matchid), "moveInfo")
            firebase.delete('/Match/' + str(matchid), "matchDetails")
            break


print(checkPlayerMoves(68))
