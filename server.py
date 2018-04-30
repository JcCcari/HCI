import socket, select, json

class Server(object):
    # List to keep track of socket descriptors
    CONNECTION_LIST = []
    players_list = []
    viewers_list = []
    MAX_PLAYERS = 4
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
            'DISCONNECT':99
        }
        self.there_is_winner = False
        self.players_count = 0
        self.user_name_dict = {}
        self.server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.set_up_connections()
        self.client_connect()

    def set_up_connections(self):
        # this has no effect, why ?
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

    def send_data_to(self, sock, message):
        try:
            sock.send(message)
        except:
            # broken socket connection may be, chat client pressed ctrl+c for example
            socket.close()
            self.CONNECTION_LIST.remove(sock)

    def client_connect(self):
        print "Chat server started on port " + str(self.PORT)
        while 1:
            # Get the list sockets which are ready to be read through select
            read_sockets, write_sockets, error_sockets = select.select(self.CONNECTION_LIST, [], [])

            for sock in read_sockets:
                # New connection
                if sock == self.server_socket:
                    # Handle the case in which there is a new connection recieved through server_socket
                    self.setup_connection( self.players_count ) # players_count = player_id
                    self.players_count = self.players_count + 1
                # Some incoming message from a client
                else:
                    # Data recieved from client, process it
                    try:
                        data = sock.recv(self.RECV_BUFFER)
                        data_map = json.loads(data)
                        if data:
                            #if self.players_count < MAX_PLAYERS : # waiting players
                            if data_map['option'] == self.map_codes['LOGIN']:
                                if self.players_count < MAX_PLAYERS : # waiting players
                                    if data_map['type'] == 0: # players
                                        message_map = { 'option': self.map_codes[WAIT_PLAYERS]}
                                        self.send_data_to( sock, give_json(message_map))
                                    elif data_map['type'] == 1: # viewers
                                        pass

                                elif self.players_count == MAX_PLAYERS: # starting game
                                    if self.there_is_winner:
                                        for sock_dest, connect in self.user_name_dict.iteritems():
                                            message_map = {'option': self.map_codes[SET_GAME], 'player_id': connect.get_player_id()}
                                            send_data_to(sock_dest, self.give_json(message_map))

                                        # FALTA GENERAR MAPA    
                                        for sock_dest, connect in self.user_name_dict.iteritems():
                                            generated_map = self.generate_map(connect.player_id)
                                            connect.set_map( generate_map)
                                            message_map = {'option': self.map_codes[SEND_MAP], 'matrix_size': len(generated_map), 'map_data': generated_map }
                                            send_data_to(sock_dest, self.give_json(message_map))

                            if data_map['option'] == self.map_codes['POSITION']:
                                pos_x = data_map['matrix_pos_x']
                                pos_y = data_map['matrix_pos_y']
                                players_validated = 0
                                if validate_position( pos_x, pos_y):
                                    message_map = {'option': self.map_codes['VALIDATION']}
                                    self.send_data_to(sock, self.give_json(message_map))
                                    players_validated += 1

                                if players_validated == self.MAX_PLAYERS:
                                    message_map = {'option': self.map_codes['START']}
                                    broadcast_all_clients( message_map )

                            if data_map['option'] == self.map_codes['UPDATE_POSITION']:

                            
                        

                    except:
                        #self.broadcast_data(sock, "Client (%s, %s) is offline" % addr)
                        print " [-] Client (%s, %s) is offline" % addr
                        sock.close()
                        self.CONNECTION_LIST.remove(sock)
                        continue

        self.server_socket.close()

    def setup_connection(self, id_connection):
        sockfd, addr = self.server_socket.accept()
        self.CONNECTION_LIST.append(sockfd)
        print " [+] Client (%s, %s) connected" % addr

        message = {'option': self.map_codes['CONNECTION'] }
        self.send_data_to(sockfd, self.give_json(message) )

        self.user_name_dict.update({sockfd: Connection(addr, id_connection)})
        

    def broadcast_all_clients_except_one(self, sock, message):
        for local_soc, connection in self.user_name_dict.iteritems():
            if local_soc != sock and connection.player_id is not None:
                self.send_data_to(local_soc, message)

    def broadcast_all_clients(self, message_map)
        for soc, connection in self.user_name_dict.iteritems():
            if connection.player_id is not None:
                send_data_to(soc, self.give_json(message_map) )

    def give_json(self, map_json):
        jsonstr = json.dumps(map_json)
        return str(len(jsonstr)).zfill(4) + jsonstr

    def generate_map(self, player_id):
        return [] ## call map generator

    def validate_position(self, pos_x, pos_y):
        return True # call validate


class Connection(object):
    def __init__(self, address, id):
        self.address = address
        self.username = None #optional ?
        self.player_id = None
        self.map = []

    def get_map(self):    
        return self.map

    def set_map( self, new_map):
        self.map = new_map

    def.get_player_id(self):
        return self.player_id;


if __name__ == "__main__":
    server = Server()
