import express from "express";
import { createServer } from "http";
import { Server, Socket } from "socket.io";
import dotenv from 'dotenv'; dotenv.config();

const app = express();
const httpServer = createServer(app);
const io = new Server(httpServer, {
  // options
});

const connectedSockets: Socket[] = [];
const waitingJoinGameSockets: Socket[] = [];
const roomCnt = 0;

// connected Sockets logic
io.on("connection", (socket: Socket) => {   
    connectedSockets.push(socket);
}); 
io.on("disconnect", (socket: Socket) => {
    // remove
    const index = connectedSockets.indexOf(socket);
    if(index <= -1) throw new Error("remove socket which aren't add");
    connectedSockets.splice(index, 1);
})
 
// waiting join game sockets logic
io.on("connection", (socket: Socket) => {   
    waitingJoinGameSockets.push(socket);
    if(waitingJoinGameSockets.length === 2){
        // match then
        MatchPlayer(waitingJoinGameSockets[0], waitingJoinGameSockets[1]);
    }
});
io.on("disconnect", (socket: Socket) => {
    const index = waitingJoinGameSockets.indexOf(socket);
    if(index > -1){
        waitingJoinGameSockets.splice(index, 1);
    };
});


if (process.env.HOST == undefined || process.env.PORT == undefined){
    throw new Error(".env not set HOST, PORT");
};
httpServer.listen(process.env.PORT);    
console.log(`listening at ${process.env.PORT}`)


function MatchPlayer(s1: Socket, s2: Socket) {
    
}

