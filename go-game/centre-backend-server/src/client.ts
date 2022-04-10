import {io} from "socket.io-client";
import dotenv from 'dotenv'; dotenv.config();
import PacketType, {PlayerDataPck, RequestMatchPck} from "./PacketType";

if (process.env.HOST == undefined || process.env.PORT == undefined){
    throw new Error(".env not set HOST, PORT");
};
const url = `http://${process.env.HOST}:${process.env.PORT}`;
const socket = io(url);
// test();

const testConnection =() =>{
    socket.on(PacketType.Connection, ()=>console.log("Connect to server"));
}
const testPlayerData  = () => {
    console.log(`send player data`);
    const playerData: PlayerDataPck = {id: "testId1"};
    socket.emit(PacketType.PlayerData, playerData);
}

const testMatch = () => {
    console.log("request match");
    const requestMatchPck: RequestMatchPck = {};
    socket.emit(PacketType.RequestMatch, requestMatchPck);
}

const test = () => {
    testConnection();
    testPlayerData();
    testMatch();
}
