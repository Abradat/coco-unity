import socket


class UnitySocket():
    def __init__(self):
        super().__init__()
        host = 'localhost'
        port = 8976  # initiate port no above 1024

        self.server_socket = socket.socket()  # get instance
        # look closely. The bind() function takes tuple as argument
        self.server_socket.bind((host, port))  # bind host address and port together

        # configure how many client the server can listen simultaneously
        self.server_socket.listen(1)
        self.conn, self.address = self.server_socket.accept()  # accept new connection
        print("Connection from: " + str(self.address))

    def send_data(self, data):
        self.conn.send(data.encode())
