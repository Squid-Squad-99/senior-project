import {io} from "socket.io-client";
import dotenv from 'dotenv'; dotenv.config();

if (process.env.HOST == undefined || process.env.PORT == undefined){
    throw new Error(".env not set HOST, PORT");
};
const url = `http://${process.env.HOST}:${process.env.PORT}`;
const socket = io(url);

socket.on("connect", () =>  {console.log("socket connect")});