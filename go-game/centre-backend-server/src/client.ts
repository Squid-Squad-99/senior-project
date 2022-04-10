import { io } from "socket.io-client";
import dotenv from 'dotenv'; dotenv.config();
import PacketType, { PlayerDataPck, RequestMatchPck, ResponsePck } from "./PacketType";

if (process.env.HOST == undefined || process.env.PORT == undefined) {
    throw new Error(".env not set HOST, PORT");
};
const url = `http://${process.env.HOST}:${process.env.PORT}`;
const socket = io(url);
test();

function testConnection() {
    socket.on(PacketType.Connection, () => console.log("Connect to server"));
}
function testPlayerData() {
    console.log(`send player data`);
    const playerData: PlayerDataPck = { id: "testId1" };
    socket.emit(PacketType.PlayerData, playerData);
}

async function testMatch() {
    console.log("request match");
    const requestMatchPck: RequestMatchPck = {};
    socket.emit(PacketType.RequestMatch, requestMatchPck);

}

function test() {
    testConnection();
    testPlayerData();
    testMatch();
}
