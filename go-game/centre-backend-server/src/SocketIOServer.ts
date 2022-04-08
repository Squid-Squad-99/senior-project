import express from "express";
import { createServer } from "http";
import { Server, Socket } from "socket.io";
import dotenv from "dotenv";
dotenv.config();

class SocketIOServer {
  public connectedSockets: Socket[] = [];

  private io: Server;
  private httpServer;

  constructor() {
    const app = express();
    const httpServer = createServer(app);
    const io = new Server(httpServer, {
      // options
    });
    this.io = io;
    this.httpServer = httpServer;
    // mentain list of connect  socket
    this.MentainConnectedSockets();
  }

  public StartServer() {
    // check host & port is provide
    if (process.env.HOST == undefined || process.env.PORT == undefined) {
      throw new Error(".env not set HOST, PORT");
    }
    // listening for connection
    this.httpServer.listen(process.env.PORT);
    console.log(`listening at ${process.env.PORT}...`);
  }

  public On(event: string, handler: (socket: Socket) => void) {
    this.io.on(event, handler);
  }

  private MentainConnectedSockets() {
    // connected Sockets logic
    this.io.on("connection", (socket: Socket) => {
      this.connectedSockets.push(socket);
      socket.on("disconnecting", (reason: string) => {
        // remove
        const index = this.connectedSockets.indexOf(socket);
        if (index <= -1) throw new Error("remove socket which aren't add");
        this.connectedSockets.splice(index, 1);

        console.log(`socket disconnecting, reason ${reason}`);
      });
    });
  }
}

export default SocketIOServer;
