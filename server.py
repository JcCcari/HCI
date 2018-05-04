import socket, select, json
import mazeMaker

class Server(object):
    # List to keep track of socket descriptors
    CONNECTION_LIST = []
    players_list = []
    viewers_list = []
    MAX_PLAYERS = 1
    RECV_BUFFER = 4096  # Advisable to keep it as an exponent of 2
    PORT = 5000

    def __init__(self):
        self.map_codes = {
            'CONNECTION': 1,
            'LOGIN': 2,
            'WAIT_PLAYERS': 3,
            'SET_GAME': 4,
            'SEND_MAP': 5,
            'POSITION': 6,
            'VALIDATION': 7,
            'START': 8,
            'UPDATE_POSITION':9,
            'FREE_SPACE':10,
            'SPACES':11,
            'NEW_MAP':12,
            'SEND_NEW_MAP':13,
            'READY_FOR_START':14,
            'START_AGAING':15,
            'WINNER':16,
            'CONGRATULATION': 17,
            'END_GAME': 18,
            'DISCONNECT':99
        }
        self.there_is_winner = False
        self.players_count = 0
        self.clients_dict = {}
        self.server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.set_up_connections()
        self.client_connect()

    def set_up_connections(self):
        # this has no effect, why ?
        print ">> Setup connections"
        self.server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        self.server_socket.bind(("0.0.0.0", self.PORT))
        self.server_socket.listen(10)  # max simultaneous connections.

        # Add server socket to the list of readable connections
        self.CONNECTION_LIST.append(self.server_socket)

    # Function to broadcast chat messages to all connected clients
    def broadcast_data(self, sock, message):
        # Do not send the message to master socket and the client who has send us the message
        for socket in self.CONNECTION_LIST:
            if socket != self.server_socket and socket != sock:
                # if not send_to_self and sock == socket: return
                try:
                    socket.send(message)
                except:
                    # broken socket connection may be, chat client pressed ctrl+c for example
                    socket.close()
                    self.CONNECTION_LIST.remove(socket)

    def broadcast_all(self, message):
        for sock, conn in self.clients_dict.iteritems():
            message_map = self.give_json(message)
            send_data_to(sock,message_map)

    def send_data_to(self, sock, message):
        try:
            sock.send(message)
        except Exception as e:
            # broken socket connection may be, chat client pressed ctrl+c for example
            print e
            socket.close()
            self.CONNECTION_LIST.remove(sock)

    def client_connect(self):
        print ">> Waiting clients"
        clients_connected_id = 0
        players_validated = 0
        while 1:
            # Get the list sockets which are ready to be read through select
            read_sockets, write_sockets, error_sockets = select.select(self.CONNECTION_LIST, [], [])

            for sock in read_sockets:
                # New connection
                if sock == self.server_socket:
                    # Handle the case in which there is a new connection recieved through server_socket
                    self.setup_connection( clients_connected_id )
                    clients_connected_id = clients_connected_id + 1
                # Some incoming message from a client
                else:
                    # Data recieved from client, process it
                    try:
                        data = sock.recv(self.RECV_BUFFER)
                        data_map = json.loads(data)
                        print data_map
                        if data:
                            #if self.players_count < MAX_PLAYERS : # waiting players
                            if data_map['option'] == self.map_codes['LOGIN']:
                                #print " LOGIN "
                                #print " Player Count ", self.players_count
                                if self.players_count < self.MAX_PLAYERS : # waiting players
                                    #print data_map
                                    #print " Player Count ", self.players_count
                                    if data_map['type'] == 1: # players
                                        print " Recibed Player", self.players_count

                                        message_map = { 'option': self.map_codes['WAIT_PLAYERS'] }
                                        #print message_map
                                        #print self.give_json(message_map)
                                        self.send_data_to( sock, self.give_json(message_map))
                                        #print message_map
                                        self.players_count += 1
                                        #print " get out login player"
                                    elif data_map['type'] == 2: # viewers
                                        print " Recibed Viewer", self.players_count
                                        pass

                                print " Player Count ", self.players_count
                                print " client dict ",self.clients_dict
                                if self.players_count == self.MAX_PLAYERS : # starting game
                                        print " Starting game!"
                                        print " Sending package set game!"
                                        for sock_dest, connect in self.clients_dict.iteritems():
                                            print "Senfing to: ", connect.address
                                            message_map = { 'option': self.map_codes['SET_GAME'], 
                                                            'player_id': connect.get_player_id() }
                                            print message_map, 1
                                            print self.give_json(message_map)
                                            self.send_data_to(sock_dest, self.give_json(message_map))
                                            print message_map, 2

                                        # FALTA GENERAR MAPA    
                                        print " Sending Map"
                                        for sock_dest, connect in self.clients_dict.iteritems():
                                            generated_map = self.generate_map(connect.player_id)
                                            print " Sending to1: ",connect.address
                                            #connect.set_map( generate_map )
                                            #connect.map = generate_map
                                            print " Sending to2: ",connect.address
                                            message_map = {'option': self.map_codes['SEND_MAP'], 
                                                            'matrix_size': len(generated_map), 
                                                            'map_data': mazeMaker.maze_to_JSON(generated_map) }
                                            print message_map
                                            print " json map ",self.give_json(message_map)
                                            self.send_data_to(sock_dest, self.give_json(message_map))
                                            print "map sended!"

                            if data_map['option'] == self.map_codes['POSITION']:
                                pos_x = data_map['matrix_pos_x']
                                pos_y = data_map['matrix_pos_y']
                                
                                #if validate_position( pos_x, pos_y):
                                message_map = {'option': self.map_codes['VALIDATION']}
                                self.send_data_to(sock, self.give_json(message_map))
                                players_validated += 1

                                if players_validated == self.MAX_PLAYERS:
                                    message_map = {'option': self.map_codes['START']}
                                    broadcast_all_clients( message_map )

                            if data_map['option'] == self.map_codes['UPDATE_POSITION']:
                                pos_x = data_map['matrix_pos_x']
                                pos_y = data_map['matrix_pos_y']
                                player_sender_id = data_map['player_id']

                                self.clients_dict[sock].position_x = pos_x
                                self.clients_dict[sock].position_y = pos_y

                                #broadcast?
                                
                            if data_map['option'] == self.map_codes['FREE_SPACE']:
                                pos_free_x = data_map['matrix_free_x']
                                pos_free_y = data_map['matrix_free_y']
                                player_sender_id = data_map['playeelf.players_count < MAX_PLAYERS : # waiting playerself.players_count < MAX_PLAYERS : # waiting playersr_id']
                                #call mazeMaker !!!!!!!!!!!!!!!!!!
                                
                                #positions_free = {}
                                #message_map = { 'option': self.map_codes['SPACES'] }
                                                #'cantidad_liberados': free_spaces, 
                                                #'liberados': positions_free}
                                #send_data_to(sock, message_map)

                            #if data_map['option'] == self.map_codes['NEW_MAP']: ## remove ? 
                            #    pass
                            #if data_map['option'] == self.map_codes['READY_FOR_START']: # delete? 
                            #    message_map  = {'option':self.map_codes['START_AGAING'] }
                            #    send_data_to( sock, message_map)
                            if data_map['option'] == self.map_codes['WINNER']: # delete? 
                                message_map_losers = {'option': self.map_codes['END_GAME']}
                                message_map_winner = {'option': self.map_codes['CONGRATULATION']}
                                send_data_to(sock, message_map_winner )
                                broadcast_all_clients_except_one( sock, message_map_losers)

                    except:
                        #self.broadcast_data(sock, "Client (%s, %s) is offline" % addr)
                        #print " [-] Client (%s, %s) is offline" % addr
                        sock.close()
                        self.CONNECTION_LIST.remove(sock)
                        continue

        self.server_socket.close()

    def setup_connection(self, id_connection):
        sockfd, addr = self.server_socket.accept()
        self.CONNECTION_LIST.append(sockfd) ## delete ?
        print " [+] Client (%s, %s) connected" % addr

        message = {'option': self.map_codes['CONNECTION'] }
        self.send_data_to(sockfd, self.give_json(message) )

        self.clients_dict.update({sockfd: Connection(addr, id_connection)})
        print " client dict ",self.clients_dict
        

    def broadcast_all_clients_except_one(self, sock, message):
        for local_soc, connection in self.clients_dict.iteritems():
            if local_soc != sock and connection.player_id is not None:
                self.send_data_to(local_soc, message)            

    def broadcast_all_clients(self, message_map):
        for soc, connection in self.clients_dict.iteritems():
            if connection.player_id is not None:
                send_data_to(soc, self.give_json(message_map) )

    def give_json(self, map_json):
        jsonstr = json.dumps(map_json)
        return str(len(jsonstr)).zfill(4) + jsonstr

    def generate_map(self, player_id):
        return mazeMaker.callmap() ## call map generator

    def validate_position(self, pos_x, pos_y):
        return True # call validate


class Connection(object):
    def __init__(self, address, id):
        self.address = address
        self.username = None #optional ?
        self.player_id = id
        self.map = None
        self.position_x = 0
        self.position_y = 0

    def get_map(self):    
        return self.map

    def set_map( self, new_map):
        self.map = new_map

    def get_player_id(self):
        return self.player_id;


if __name__ == "__main__":
    server = Server()
