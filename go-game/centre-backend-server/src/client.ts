import { io } from "socket.io-client";
import dotenv from 'dotenv'; dotenv.config();
import PacketType, { PlayerDataPck, RequestMatchPck, ResponsePck, TicketPck } from "./PacketType";

if (process.env.HOST == undefined || process.env.PORT == undefined) {
    throw new Error(".env not set HOST, PORT");
};
const url = `http://${process.env.HOST}:${process.env.PORT}`;
const socket = io(url);
const playerId = process.argv[process.argv.length - 1];
test();

function testConnection() {
    socket.on(PacketType.Connection, () => console.log("Connect to server"));
}
function testPlayerData() {
    console.log(`send player data`);
    const playerData: PlayerDataPck = { id: playerId };
    socket.emit(PacketType.PlayerData, playerData);
}

async function testMatch() {
    console.log("request match");
    const requestMatchPck: RequestMatchPck = {};
    socket.emit(PacketType.RequestMatch, requestMatchPck);
    socket.on(PacketType.Ticket, (tickerPck: TicketPck)=>{
        console.log(`get ticket: ${tickerPck.MethodSpecificData}, ${tickerPck.p2pConnectMethod}`);
        if(playerId == "1") socket.emit(PacketType.GameData, "Hello from another player");
        else socket.on(PacketType.GameData, (data)=>console.log(`get game data: ${data}`));
    })
}

function test() {
    testConnection();
    testPlayerData();
    testMatch();
}
