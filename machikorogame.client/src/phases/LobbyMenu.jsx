import React, { useEffect, useState } from "react";
import './styles/LobbyMenu.css';
import MainLobbyMenu from "./MainLobbyMenu";
import GameBoard from "./GameBoard";

function sendMessage(socket, type, message, game_state) {
    const user_id = game_state.state.user_id;
    var msg = {
        "id": user_id, 
        "type": type,
        "message": message
    }
    console.log(JSON.stringify(msg));
    socket.send(JSON.stringify(msg));
}

export default function LobbyMenu({updatePhase, game_state}) {
    const socket = new WebSocket('ws://127.0.0.1/Lobby');
    const user_id = game_state.state.user_id;

    socket.onopen = (event) => {
        console.log("connected");
    }

    socket.onmessage = (event) => {
        console.log(`Got message: ${event.data}`);
        var msg = JSON.parse(event.data);
        if (msg['type'] == 'bad') {
            LeaveLobby();
            return;
        } else if (msg['type'] == 'hello') {
            sendMessage(socket, "hello", "", game_state);
        }
    }

    function LeaveLobby() {
        socket.close(1000, "Player has left lobby");
        updatePhase(MainLobbyMenu);
    }

    function StartGame() {  
        socket.close(1000, "Game started");
        updatePhase(GameBoard);
    }

    return (
        <div id="LobbyMenu">
            <div className="goodstuff">Название лобби</div>
            <div id="buttons">
                <button id="StartGameBtn" className="LobbyBtn" onClick={StartGame}>Запустить игру</button>
                <div></div>
                <button className="LobbyBtn" onClick={LeaveLobby}>Покинуть лобби</button>
            </div>
            <div id="Players">

            </div>
        </div>
    )
}