import React, { useEffect, useState } from "react";
import './styles/LobbyMenu.css';
import MainLobbyMenu from "./MainLobbyMenu";
import GameBoard from "./GameBoard";

function sendMessage(socket, type, game_state) {
    const user_id = game_state.state.user_id;
    socket.send(`${user_id} ${type}`);
}   

export default function LobbyMenu({updatePhase, game_state}) {
    const socket = new WebSocket('ws://127.0.0.1/Lobby');
    const user_id = game_state.state.user_id;

    socket.onopen = (event) => {
        sendMessage(socket, "Hello", game_state);
    }

    socket.onmessage = (event) => {
        console.log(event.data);
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