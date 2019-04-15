

from firebase import firebase

firebase = firebase.FirebaseApplication('https://project-finch-database.firebaseio.com/')
result = firebase.get('/queuingForMatch', '1293')

result1 = firebase.get('/User/1293/Soldiers/Soldier1/CharacterClass', None)
print(result1)

put = firebase.put('/queuingForMatch/1298', 'UserID', '1298')

def checkPlayerMoves(matchid):
    counter = 0
    player1move_old = ''
    player2move_old = ''
    #for constantly checking database
    while True:
        player1move_new = firebase.get('/Match/'+str(matchid)+'/moveInfo', 'u_player1Move')
        if player1move_new != player1move_old:
            counter += 1
            player1move_old = player1move_new
            firebase.put('/Match/' + str(matchid) + '/moveInfo/a_playersMoves', counter, player1move_new)
        player2move_new = firebase.get('/Match/'+str(matchid)+'/moveInfo', 'u_player2Move')
        if player2move_new != player2move_old:
            counter += 1
            player2move_old = player2move_new
            firebase.put('/Match/' + str(matchid) + '/moveInfo/a_playersMoves', counter, player2move_new)


print(checkPlayerMoves(68))
